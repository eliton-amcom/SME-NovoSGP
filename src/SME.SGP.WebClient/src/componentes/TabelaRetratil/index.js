import React, { useState, useCallback, useEffect } from 'react';
import t from 'prop-types';
import shortid from 'shortid';

// Ant
import { Tooltip } from 'antd';

// Componentes
import Cabecalho from './componentes/Cabecalho';

// Estilos
import { TabelaEstilo, Tabela, DetalhesAluno, LinhaTabela } from './style';

function TabelaRetratil({
  alunos,
  children,
  onChangeAlunoSelecionado,
  permiteOnChangeAluno,
  codigoAlunoSelecionado,
  exibirProcessoConcluido,
  tituloCabecalho,
  pularDesabilitados,
}) {
  const [retraido, setRetraido] = useState(false);
  const [alunoSelecionado, setAlunoSelecionado] = useState(null);

  useEffect(() => {
    if (codigoAlunoSelecionado) {
      const alunoSelecionar = alunos.find(
        item => item.codigoEOL == codigoAlunoSelecionado
      );

      if (alunoSelecionar) {
        setAlunoSelecionado(alunoSelecionar);
      }
    }

    if (!(alunos && alunos.length)) {
      setAlunoSelecionado(null);
    }
  }, [alunos, codigoAlunoSelecionado]);

  const permiteSelecionarAluno = useCallback(async () => {
    if (permiteOnChangeAluno) {
      const permite = await permiteOnChangeAluno();
      return permite;
    }
    return true;
  }, [permiteOnChangeAluno]);

  const isAlunoSelecionado = aluno => {
    return alunoSelecionado && aluno.codigoEOL === alunoSelecionado.codigoEOL;
  };

  const proximoAlunoHabilitado = aluno => {
    const indexProximo = alunos.indexOf(aluno) + 1;
    if (indexProximo !== alunos.length) {
      const proximoAluno = alunos[indexProximo];
      if (proximoAluno.desabilitado) {
        return proximoAlunoHabilitado(proximoAluno);
      }
      return proximoAluno;
    }
    return false;
  };

  const proximoAlunoHandler = useCallback(async () => {
    const permite = await permiteSelecionarAluno();
    if (permite) {
      if (alunos.indexOf(alunoSelecionado) === alunos.length - 1) return;
      const aluno = pularDesabilitados
        ? proximoAlunoHabilitado(alunoSelecionado)
        : alunos[alunos.indexOf(alunoSelecionado) + 1];
      if (aluno) {
        setAlunoSelecionado(aluno);
        onChangeAlunoSelecionado(aluno);
      }
    }
  }, [
    alunoSelecionado,
    alunos,
    onChangeAlunoSelecionado,
    permiteSelecionarAluno,
  ]);

  const anteriorAlunoHabilitado = aluno => {
    const indexAnterior = alunos.indexOf(aluno) - 1;
    if (indexAnterior >= 0) {
      const alunoAnterior = alunos[indexAnterior];
      if (alunoAnterior.desabilitado) {
        return anteriorAlunoHabilitado(alunoAnterior);
      }
      return alunoAnterior;
    }
    return false;
  };

  const anteriorAlunoHandler = useCallback(async () => {
    const permite = await permiteSelecionarAluno();
    if (permite) {
      if (alunos.indexOf(alunoSelecionado) === 0) return;
      const aluno = pularDesabilitados
        ? anteriorAlunoHabilitado(alunoSelecionado)
        : alunos[alunos.indexOf(alunoSelecionado) - 1];
      if (aluno) {
        setAlunoSelecionado(aluno);
        onChangeAlunoSelecionado(aluno);
      }
    }
  }, [
    alunoSelecionado,
    alunos,
    onChangeAlunoSelecionado,
    permiteSelecionarAluno,
  ]);

  const desabilitarAnterior = () => {
    return (
      alunos.length === 0 ||
      alunos.indexOf(alunoSelecionado) === 0 ||
      !alunoSelecionado
    );
  };

  const desabilitarProximo = () => {
    return (
      alunos.length === 0 ||
      alunos.indexOf(alunoSelecionado) === alunos.length - 1 ||
      !alunoSelecionado
    );
  };

  const onClickLinhaAluno = async aluno => {
    const permite = await permiteSelecionarAluno();
    if (permite) {
      setAlunoSelecionado(aluno);
      onChangeAlunoSelecionado(aluno);
    }
  };

  return (
    <TabelaEstilo>
      <div className="tabelaCollapse">
        <Tabela className={retraido && `retraido`}>
          <thead>
            <tr>
              <th>Nº</th>
              <th>Nome</th>
            </tr>
          </thead>
          <tbody>
            {alunos.map(item => (
              <LinhaTabela
                className={isAlunoSelecionado(item) && `selecionado`}
                key={shortid.generate()}
                ativo={!item.desabilitado}
                onClick={() => onClickLinhaAluno(item)}
                processoConcluido={item.processoConcluido}
              >
                <td>
                  {item.numeroChamada}
                  {item.marcador && item.marcador.descricao ? (
                    <Tooltip title={item.marcador.descricao}>
                      <span className="iconeSituacao" />
                    </Tooltip>
                  ) : (
                    ''
                  )}
                </td>
                <td>
                  <div
                    style={{
                      marginLeft: '-9px',
                    }}
                  >
                    {exibirProcessoConcluido ? (
                      <i className="icone-concluido fa fa-check-circle" />
                    ) : (
                      ''
                    )}
                    {item.nome}
                  </div>
                </td>
              </LinhaTabela>
            ))}
          </tbody>
        </Tabela>
      </div>
      <DetalhesAluno>
        <Cabecalho
          titulo={tituloCabecalho}
          retraido={retraido}
          onClickCollapse={() => setRetraido(!retraido)}
          onClickAnterior={anteriorAlunoHandler}
          onClickProximo={proximoAlunoHandler}
          desabilitarAnterior={desabilitarAnterior()}
          desabilitarProximo={desabilitarProximo()}
        />
        {children}
      </DetalhesAluno>
    </TabelaEstilo>
  );
}

TabelaRetratil.propTypes = {
  alunos: t.oneOfType([t.array]),
  children: t.oneOfType([t.element, t.func]),
  onChangeAlunoSelecionado: t.func,
  permiteOnChangeAluno: t.func,
  codigoAlunoSelecionado: t.oneOfType([t.any]),
  exibirProcessoConcluido: t.bool,
  tituloCabecalho: t.string,
  pularDesabilitados: t.bool,
};

TabelaRetratil.defaultProps = {
  alunos: [],
  children: () => {},
  onChangeAlunoSelecionado: () => {},
  permiteOnChangeAluno: () => true,
  codigoAlunoSelecionado: null,
  exibirProcessoConcluido: false,
  tituloCabecalho: 'Detalhes do estudante',
  pularDesabilitados: false,
};

export default TabelaRetratil;

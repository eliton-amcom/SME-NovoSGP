import PropTypes from 'prop-types';
import React, { useCallback, useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Loader } from '~/componentes';
import { setDadosListasNotasConceitos } from '~/redux/modulos/conselhoClasse/actions';
import { erros } from '~/servicos/alertas';
import ServicoConselhoClasse from '~/servicos/Paginas/ConselhoClasse/ServicoConselhoClasse';
import ListasCarregar from './listasCarregar';

const ListasNotasConceitos = props => {
  const { bimestreSelecionado } = props;

  const dispatch = useDispatch();

  const listaTiposConceitos = useSelector(
    store => store.conselhoClasse.listaTiposConceitos
  );

  const dadosPrincipaisConselhoClasse = useSelector(
    store => store.conselhoClasse.dadosPrincipaisConselhoClasse
  );
  const {
    fechamentoTurmaId,
    conselhoClasseId,
    alunoCodigo,
    turmaCodigo,
    tipoNota,
    media,
    alunoDesabilitado,
  } = dadosPrincipaisConselhoClasse;

  const [exibir, setExibir] = useState(false);
  const [carregando, setCarregando] = useState(false);

  const obterDadosLista = useCallback(async () => {
    setCarregando(true);
    const resultado = await ServicoConselhoClasse.obterNotasConceitosConselhoClasse(
      conselhoClasseId,
      fechamentoTurmaId,
      alunoCodigo
    ).catch(e => erros(e));

    if (resultado && resultado.data) {
      dispatch(setDadosListasNotasConceitos(resultado.data));
      setExibir(true);
    } else {
      setExibir(false);
    }
    setCarregando(false);
  }, [alunoCodigo, conselhoClasseId, dispatch, fechamentoTurmaId]);

  useEffect(() => {
    const bimestre = bimestreSelecionado.valor;

    if (bimestre && turmaCodigo && fechamentoTurmaId && alunoCodigo) {
      obterDadosLista();
    }
  }, [
    turmaCodigo,
    alunoCodigo,
    bimestreSelecionado.valor,
    fechamentoTurmaId,
    obterDadosLista,
  ]);

  return (
    <Loader
      className={carregando ? 'text-center' : ''}
      loading={carregando}
      tip="Carregando lista(s) notas e conceitos"
    >
      {exibir && bimestreSelecionado.valor ? (
        <ListasCarregar
          ehFinal={bimestreSelecionado.valor === 'final'}
          tipoNota={tipoNota}
          listaTiposConceitos={listaTiposConceitos}
          mediaAprovacao={media}
          alunoDesabilitado={alunoDesabilitado}
        />
      ) : (
        ''
      )}
    </Loader>
  );
};

ListasNotasConceitos.propTypes = {
  bimestreSelecionado: PropTypes.oneOfType([PropTypes.object]),
};

ListasNotasConceitos.defaultProps = {
  bimestreSelecionado: {},
};

export default ListasNotasConceitos;

import React, { useState, useEffect } from 'react';

// Redux
import { useSelector } from 'react-redux';

// Servicos
import history from '~/servicos/history';
import RotasDto from '~/dtos/rotasDto';
import AtribuicaoCJServico from '~/servicos/Paginas/AtribuicaoCJ';
import { verificaSomenteConsulta } from '~/servicos/servico-navegacao';
import { erros } from '~/servicos/alertas';

// Componentes SGP
import { Cabecalho } from '~/componentes-sgp';

// Componentes
import { Card, DataTable, ButtonGroup, Loader } from '~/componentes';
import Filtro from './componentes/Filtro';

// Styles
import { PilulaEstilo } from './styles';

function AtribuicaoCJLista() {
  const [itensSelecionados, setItensSelecionados] = useState([]);
  const [itens, setItens] = useState([]);
  const [filtro, setFiltro] = useState({});
  const [somenteConsulta, setSomenteConsulta] = useState(false);
  const permissoesTela = useSelector(store => store.usuario.permissoes);

  const colunas = [
    {
      title: 'Modalidade',
      dataIndex: 'modalidade',
      key: 'modalidade',
    },
    {
      title: 'Turma',
      dataIndex: 'turma',
      key: 'turma',
    },
    {
      title: 'Disciplinas',
      dataIndex: 'disciplinas',
      key: 'disciplinas',
      width: '80%',
      render: linha => {
        return linha.map(item => <PilulaEstilo>{item}</PilulaEstilo>);
      },
    },
  ];

  const onClickVoltar = () => history.push('/');

  const onClickBotaoPrincipal = () =>
    history.push(
      `/gestao/atribuicao-cjs/novo?dreId=${filtro.DreId}&ueId=${filtro.UeId}`
    );

  const onSelecionarItems = items => {
    setItensSelecionados(items);
  };

  const onClickEditar = item => {
    history.push(
      `/gestao/atribuicao-cjs/editar?modalidadeId=${item.modalidadeId}&turmaId=${item.turmaId}&dreId=${filtro.DreId}&ueId=${filtro.UeId}`
    );
  };

  const onChangeFiltro = valoresFiltro => {
    setFiltro({
      AnoLetivo: '2019',
      DreId: valoresFiltro.dreId,
      UeId: valoresFiltro.ueId,
      UsuarioRF: valoresFiltro.professorRf,
    });
  };

  const validarFiltro = React.useCallback(() => {
    return !!filtro.DreId && !!filtro.UeId && !!filtro.UsuarioRF;
  }, [filtro]);

  useEffect(() => {
    setSomenteConsulta(verificaSomenteConsulta(permissoesTela));
  }, [permissoesTela]);

  useEffect(() => {
    async function buscaItens() {
      try {
        const { data, status } = await AtribuicaoCJServico.buscarLista(filtro);
        if (status === 200 && data) {
          setItens(data);
        }
      } catch (error) {
        erros(error);
      }
    }
    if (validarFiltro()) {
      setItens([]);
      buscaItens();
    }
  }, [filtro]);

  return (
    <>
      <Cabecalho pagina="Atribuição de CJ" />
      <Loader loading={false}>
        <Card mx="mx-0">
          <ButtonGroup
            somenteConsulta={somenteConsulta}
            permissoesTela={permissoesTela[RotasDto.ATRIBUICAO_CJ_LISTA]}
            temItemSelecionado={
              itensSelecionados && itensSelecionados.length >= 1
            }
            onClickVoltar={onClickVoltar}
            onClickBotaoPrincipal={onClickBotaoPrincipal}
            labelBotaoPrincipal="Novo"
            desabilitarBotaoPrincipal={
              !!filtro.DreId === false && !!filtro.UeId === false
            }
          />
          <Filtro onFiltrar={onChangeFiltro} />
          <div className="col-md-12 pt-2 py-0 px-0">
            <DataTable
              id="lista-atribuicoes-cj"
              idLinha="modalidadeId"
              colunaChave="id"
              columns={colunas}
              dataSource={itens}
              onClickRow={onClickEditar}
              multiSelecao
              selecionarItems={onSelecionarItems}
            />
          </div>
        </Card>
      </Loader>
    </>
  );
}

export default AtribuicaoCJLista;

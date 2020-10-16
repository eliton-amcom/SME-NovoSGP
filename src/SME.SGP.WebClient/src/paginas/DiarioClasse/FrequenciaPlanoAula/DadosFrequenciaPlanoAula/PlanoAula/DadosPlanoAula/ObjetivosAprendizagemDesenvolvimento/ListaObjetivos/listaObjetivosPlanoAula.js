import PropTypes from 'prop-types';
import React, { useCallback, useEffect, useState } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import TransferenciaLista from '~/componentes-sgp/TranferenciaLista/transferenciaLista';
import {
  setDadosParaSalvarPlanoAula,
  setExibirLoaderFrequenciaPlanoAula,
  setModoEdicaoPlanoAula,
} from '~/redux/modulos/frequenciaPlanoAula/actions';
import { erros } from '~/servicos/alertas';
import ServicoPlanoAula from '~/servicos/Paginas/DiarioClasse/ServicoPlanoAula';
import { ContainerListaObjetivos } from './listaObjetivosPlanoAula.css';

const ListaObjetivosPlanoAula = React.memo(props => {
  const { tabAtualComponenteCurricular } = props;

  const dispatch = useDispatch();

  const temPeriodoAberto = useSelector(
    state => state.frequenciaPlanoAula.listaDadosFrequencia?.temPeriodoAberto
  );

  const dadosParaSalvarPlanoAula = useSelector(
    state => state.frequenciaPlanoAula.dadosParaSalvarPlanoAula
  );

  const componenteCurricular = useSelector(
    state => state.frequenciaPlanoAula.componenteCurricular
  );

  const [dadosEsquerda, setDadosEsquerda] = useState([]);
  const [dadosDireita, setDadosDireita] = useState([]);
  const [idsSelecionadsEsquerda, setIdsSelecionadsEsquerda] = useState([]);
  const [idsSelecionadsDireita, setIdsSelecionadsDireita] = useState([]);

  const obterObjetivosPlanoAnualComponenteAtual = useCallback(
    listaObjetivos => {
      return listaObjetivos.find(
        item =>
          String(item.componenteCurricularId) ===
          String(tabAtualComponenteCurricular.codigoComponenteCurricular)
      );
    },
    [tabAtualComponenteCurricular]
  );

  const obterObjetivosPlanoAulaComponenteAtual = useCallback(() => {
    return dadosParaSalvarPlanoAula?.objetivosAprendizagemComponente.find(
      item =>
        String(item.componenteCurricularId) ===
        String(tabAtualComponenteCurricular.codigoComponenteCurricular)
    );
  }, [dadosParaSalvarPlanoAula, tabAtualComponenteCurricular]);

  const montarDadosListasObjetivos = useCallback(
    listaObjetivos => {
      const objetivosPlanoAula = obterObjetivosPlanoAulaComponenteAtual();
      const objetivosPlanoAnual = obterObjetivosPlanoAnualComponenteAtual(
        listaObjetivos
      );

      let listaEsquerda = [];
      let listaDireita = [];
      if (objetivosPlanoAula && objetivosPlanoAnual) {
        objetivosPlanoAnual.objetivosAprendizagem.forEach(objetivo => {
          if (
            !objetivosPlanoAula.objetivosAprendizagem.find(
              item => item.id === objetivo.id
            )
          ) {
            listaEsquerda.push(objetivo);
          }
        });
      } else if (objetivosPlanoAnual) {
        listaEsquerda = objetivosPlanoAnual.objetivosAprendizagem;
      }

      if (objetivosPlanoAula) {
        listaDireita = objetivosPlanoAula.objetivosAprendizagem;
      }

      if (listaEsquerda.length) {
        setDadosEsquerda(listaEsquerda);
      }

      if (listaDireita.length) {
        setDadosDireita(listaDireita);
      }
    },
    [
      obterObjetivosPlanoAulaComponenteAtual,
      obterObjetivosPlanoAnualComponenteAtual,
    ]
  );

  const obterObjetivosPorAnoEComponenteCurricular = useCallback(() => {
    if (
      componenteCurricular &&
      componenteCurricular.codigoComponenteCurricular
    ) {
      dispatch(setExibirLoaderFrequenciaPlanoAula(true));
      ServicoPlanoAula.obterListaObjetivosPorAnoEComponenteCurricular()
        .then(listaObjetivos => {
          montarDadosListasObjetivos(listaObjetivos);
        })
        .catch(e => {
          erros(e);
        })
        .finally(() => {
          dispatch(setExibirLoaderFrequenciaPlanoAula(false));
        });
    }
  }, [dispatch, componenteCurricular, montarDadosListasObjetivos]);

  useEffect(() => {
    if (tabAtualComponenteCurricular) {
      obterObjetivosPorAnoEComponenteCurricular();
    }
  }, [tabAtualComponenteCurricular, obterObjetivosPorAnoEComponenteCurricular]);

  const parametrosListaEsquerda = {
    title: 'Objetivos de aprendizagem',
    columns: [
      {
        title: 'Código',
        dataIndex: 'codigo',
        className: 'desc-codigo',
        width: '90px',
      },
      {
        title: 'Descrição',
        dataIndex: 'descricao',
        className: 'desc-descricao',
      },
    ],
    dataSource: dadosEsquerda,
    onSelectRow: setIdsSelecionadsEsquerda,
    selectedRowKeys: idsSelecionadsEsquerda,
    selectMultipleRows: temPeriodoAberto,
  };

  const parametrosListaDireita = {
    title:
      dadosDireita && dadosDireita.length
        ? 'Objetivos de aprendizagem selecionados para este componente curricular'
        : 'Você precisa selecionar ao menos um objetivo para poder inserir a descrição do plano',
    columns: [
      {
        title: 'Código',
        dataIndex: 'codigo',
        className: 'desc-codigo fundo-codigo-lista-direita',
        width: '90px',
      },
      {
        title: 'Descrição',
        dataIndex: 'descricao',
        className: 'desc-descricao',
      },
    ],
    dataSource: dadosDireita,
    onSelectRow: setIdsSelecionadsDireita,
    selectedRowKeys: idsSelecionadsDireita,
    selectMultipleRows: temPeriodoAberto,
  };

  const obterListaComIdsSelecionados = (list, ids) => {
    return list.filter(item => ids.find(id => String(id) === String(item.id)));
  };

  const obterListaSemIdsSelecionados = (list, ids) => {
    return list.filter(item => !ids.find(id => String(id) === String(item.id)));
  };

  const obterIndexComponenteJaInseridoPlanoAula = () => {
    // Pegar a lista de objetivos  já inseridos no Plano de Aula pelo componente curricular selecionado na tab!
    // Pega o index desse componente e remove o componente na lista de objetivos já inseridos no Plano de Aula pelo componente curricular selecionado na tab!
    const componenteParaRemover = dadosParaSalvarPlanoAula.objetivosAprendizagemComponente.find(
      item =>
        item.componenteCurricularId ==
        tabAtualComponenteCurricular.codigoComponenteCurricular
    );
    const indexComponente = dadosParaSalvarPlanoAula.objetivosAprendizagemComponente.indexOf(
      componenteParaRemover
    );

    return indexComponente;
  };

  const adicionarRemoverItensReduxListaDireita = novaLista => {
    if (
      dadosParaSalvarPlanoAula.objetivosAprendizagemComponente &&
      dadosParaSalvarPlanoAula.objetivosAprendizagemComponente.length &&
      novaLista &&
      novaLista.length
    ) {
      const index = obterIndexComponenteJaInseridoPlanoAula();

      if (index > -1) {
        dadosParaSalvarPlanoAula.objetivosAprendizagemComponente[
          index
        ].objetivosAprendizagem = novaLista;
      } else {
        // Quando tiver um novo componente selecionado na tab que não tem objetivos inseridos ainda vai criar um novo!
        const novoValor = {
          componenteCurricularId:
            tabAtualComponenteCurricular.codigoComponenteCurricular,
          objetivosAprendizagem: novaLista,
        };
        dadosParaSalvarPlanoAula.objetivosAprendizagemComponente.push(
          novoValor
        );
      }
    } else if (novaLista && novaLista.length) {
      // Quando for o primeiro registro na lista da direita!
      dadosParaSalvarPlanoAula.objetivosAprendizagemComponente = [
        {
          componenteCurricularId:
            tabAtualComponenteCurricular.codigoComponenteCurricular,
          objetivosAprendizagem: novaLista,
        },
      ];
    } else {
      // Remove os objetivos da direita (inseridos do plano aula) para adicionar na lista da esquerda( que vem do plano anual )
      const index = obterIndexComponenteJaInseridoPlanoAula();
      if (index > -1) {
        dadosParaSalvarPlanoAula.objetivosAprendizagemComponente.splice(
          index,
          1
        );
        debugger;
      }
    }

    dispatch(setDadosParaSalvarPlanoAula(dadosParaSalvarPlanoAula));
  };

  const onClickAdicionar = () => {
    if (idsSelecionadsEsquerda && idsSelecionadsEsquerda.length) {
      const novaListaDireita = obterListaComIdsSelecionados(
        dadosEsquerda,
        idsSelecionadsEsquerda
      );

      const novaListaEsquerda = obterListaSemIdsSelecionados(
        dadosEsquerda,
        idsSelecionadsEsquerda
      );

      setDadosEsquerda([...novaListaEsquerda]);
      setDadosDireita([...novaListaDireita, ...dadosDireita]);

      const novaLista = [...novaListaDireita, ...dadosDireita];
      adicionarRemoverItensReduxListaDireita(novaLista);

      dispatch(setModoEdicaoPlanoAula(true));
      setIdsSelecionadsEsquerda([]);
    }
  };

  const onClickRemover = async () => {
    if (idsSelecionadsDireita && idsSelecionadsDireita.length) {
      const novaListaEsquerda = obterListaComIdsSelecionados(
        dadosDireita,
        idsSelecionadsDireita
      );

      const novaListaDireita = obterListaSemIdsSelecionados(
        dadosDireita,
        idsSelecionadsDireita
      );

      setDadosEsquerda([...novaListaEsquerda, ...dadosEsquerda]);
      setDadosDireita([...novaListaDireita]);

      const novaLista = [...novaListaDireita];
      adicionarRemoverItensReduxListaDireita(novaLista);

      dispatch(setModoEdicaoPlanoAula(true));
      setIdsSelecionadsDireita([]);
    }
  };

  return (
    <ContainerListaObjetivos>
      <TransferenciaLista
        listaEsquerda={parametrosListaEsquerda}
        listaDireita={parametrosListaDireita}
        onClickAdicionar={onClickAdicionar}
        onClickRemover={onClickRemover}
      />
    </ContainerListaObjetivos>
  );
});

ListaObjetivosPlanoAula.propTypes = {
  tabAtualComponenteCurricular: PropTypes.oneOfType([PropTypes.object]),
};

ListaObjetivosPlanoAula.defaultProps = {
  tabAtualComponenteCurricular: {},
};

export default ListaObjetivosPlanoAula;

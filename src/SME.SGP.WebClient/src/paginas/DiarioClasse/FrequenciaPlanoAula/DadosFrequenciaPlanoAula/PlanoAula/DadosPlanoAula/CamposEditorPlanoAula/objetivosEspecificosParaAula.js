import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import CardCollapse from '~/componentes/cardCollapse';
import Editor from '~/componentes/editor/editor';
import { setModoEdicaoPlanoAula } from '~/redux/modulos/frequenciaPlanoAula/actions';
import ServicoPlanoAula from '~/servicos/Paginas/DiarioClasse/ServicoPlanoAula';

const ObjetivosEspecificosParaAula = () => {
  const dispatch = useDispatch();

  const desabilitarCamposPlanoAula = useSelector(
    state => state.frequenciaPlanoAula.desabilitarCamposPlanoAula
  );

  const dadosPlanoAula = useSelector(
    state => state.frequenciaPlanoAula.dadosPlanoAula
  );

  const objetivosAprendizagemComponente = useSelector(
    state =>
      state.frequenciaPlanoAula.dadosPlanoAula?.objetivosAprendizagemComponente
  );

  const temPeriodoAberto = useSelector(
    state => state.frequenciaPlanoAula.temPeriodoAberto
  );

  const configCabecalho = {
    altura: '44px',
    corBorda: '#4072d6',
  };

  const temPeloMenosUmObjetivoSelecionado = () => {
    if (
      objetivosAprendizagemComponente &&
      objetivosAprendizagemComponente.length
    ) {
      const algumaTabTemObjetivoSelecionado = objetivosAprendizagemComponente.find(
        item => item.objetivosAprendizagem && item.objetivosAprendizagem.length
      );
      if (algumaTabTemObjetivoSelecionado) {
        return true;
      }
    }
    return false;
  };

  const onChangeObjetivosEspecificosParaAula = valor => {
    ServicoPlanoAula.atualizarDadosPlanoAula('descricao', valor);
    dispatch(setModoEdicaoPlanoAula(true));
  };

  return (
    <>
      <CardCollapse
        key="objetivos-especificos-para-aula"
        titulo="Objetivos específicos para a aula"
        indice="objetivos-especificos-para-aula"
        configCabecalho={configCabecalho}
        show
      >
        <fieldset className="mt-3">
          <Editor
            desabilitar={
              desabilitarCamposPlanoAula ||
              !temPeriodoAberto ||
              !temPeloMenosUmObjetivoSelecionado()
            }
            onChange={onChangeObjetivosEspecificosParaAula}
            inicial={dadosPlanoAula.descricao}
          />
        </fieldset>
      </CardCollapse>
    </>
  );
};

export default ObjetivosEspecificosParaAula;

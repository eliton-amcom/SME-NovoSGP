import produce from 'immer';

const inicial = {
  bimestresPlanoAnual: [],
  listaComponentesCurricularesPlanejamento: [],
  planoAnualEmEdicao: false,
  componenteCurricular: undefined,
  tabAtualComponenteCurricular: [],
  dadosBimestresPlanoAnual: [],
  dadosEditadosBimestresPlanoAnual: [],
  listaObjetivosAprendizagemPorComponente: [],
  errosPlanoAnual: [],
  exibirModalErrosPlanoAnual: false,
  exibirLoaderPlanoAnual: false,
};

export default function planoAnual(state = inicial, action) {
  return produce(state, draft => {
    switch (action.type) {
      case '@planoAnual/setBimestresPlanoAnual': {
        return {
          ...draft,
          bimestresPlanoAnual: action.payload,
        };
      }
      case '@planoAnual/setListaComponentesCurricularesPlanejamento': {
        return {
          ...draft,
          listaComponentesCurricularesPlanejamento: action.payload,
        };
      }
      case '@planoAnual/setPlanoAnualEmEdicao': {
        return {
          ...draft,
          planoAnualEmEdicao: action.payload,
        };
      }
      case '@planoAnual/setComponenteCurricularPlanoAnual': {
        return {
          ...draft,
          componenteCurricular: action.payload,
        };
      }
      case '@planoAnual/limparDadosPlanoAnual': {
        return {
          ...draft,
          listaComponentesCurricularesPlanejamento: [],
          bimestresPlanoAnual: [],
          planoAnualEmEdicao: false,
          // componenteCurricular: undefined,
          tabAtualComponenteCurricular: [],
          dadosBimestresPlanoAnual: [],
          dadosEditadosBimestresPlanoAnual: [],
          listaObjetivosAprendizagemPorComponente: [],
          errosPlanoAnual: [],
          exibirModalErrosPlanoAnual: false,
          exibirLoaderPlanoAnual: false,
        };
      }
      case '@planoAnual/setTabAtualComponenteCurricular': {
        const dados = state.tabAtualComponenteCurricular;
        dados[action.payload.bimestre] = action.payload.componente;
        return {
          ...draft,
          tabAtualComponenteCurricular: dados,
        };
      }
      case '@planoAnual/setDadosBimestresPlanoAnual': {
        const dados = state.dadosBimestresPlanoAnual;
        dados[action.payload.bimestre] = action.payload;
        return {
          ...draft,
          dadosBimestresPlanoAnual: dados,
        };
      }
      case '@planoAnual/setListaObjetivosAprendizagemPorComponente': {
        const dados = state.listaObjetivosAprendizagemPorComponente;
        dados[action.payload.codigoComponenteCurricular] =
          action.payload.objetivos;
        return {
          ...draft,
          listaObjetivosAprendizagemPorComponente: dados,
        };
      }
      case '@planoAnual/setErrosPlanoAnual': {
        return {
          ...draft,
          errosPlanoAnual: action.payload,
        };
      }
      case '@planoAnual/setExibirModalErrosPlanoAnual': {
        return {
          ...draft,
          exibirModalErrosPlanoAnual: action.payload,
        };
      }
      case '@planoAnual/setExibirLoaderPlanoAnual': {
        return {
          ...draft,
          exibirLoaderPlanoAnual: action.payload,
        };
      }

      default:
        return draft;
    }
  });
}

import produce from 'immer';

const inicial = {
  exibirModalAviso: false,
  dadosSecoesPorEtapaDeEncaminhamentoAEE: [],
  exibirLoaderEncaminhamentoAEE: false,
  dadosEncaminhamento: null,
  exibirModalErrosEncaminhamento: false,
  exibirModalEncerramentoEncaminhamentoAEE: false,
  desabilitarCamposEncaminhamentoAEE: false,
};

export default function EncaminhamentoAEE(state = inicial, action) {
  return produce(state, draft => {
    switch (action.type) {
      case '@encaminhamentoAEE/setExibirModalAviso': {
        return {
          ...draft,
          exibirModalAviso: action.payload,
        };
      }
      case '@encaminhamentoAEE/setDadosModalAviso': {
        return {
          ...draft,
          dadosModalAviso: action.payload,
        };
      }

      case '@encaminhamentoAEE/setDadosSecoesPorEtapaDeEncaminhamentoAEE': {
        return {
          ...draft,
          dadosSecoesPorEtapaDeEncaminhamentoAEE: action.payload,
        };
      }
      case '@encaminhamentoAEE/setExibirLoaderEncaminhamentoAEE': {
        return {
          ...draft,
          exibirLoaderEncaminhamentoAEE: action.payload,
        };
      }
      case '@encaminhamentoAEE/setDadosEncaminhamento': {
        return {
          ...draft,
          dadosEncaminhamento: action.payload,
        };
      }
      case '@encaminhamentoAEE/setExibirModalErrosEncaminhamento': {
        return {
          ...draft,
          exibirModalErrosEncaminhamento: action.payload,
        };
      }
      case '@encaminhamentoAEE/setExibirModalEncerramentoEncaminhamentoAEE': {
        return {
          ...draft,
          exibirModalEncerramentoEncaminhamentoAEE: action.payload,
        };
      }
      case '@encaminhamentoAEE/setDesabilitarCamposEncaminhamentoAEE': {
        return {
          ...draft,
          desabilitarCamposEncaminhamentoAEE: action.payload,
        };
      }
      case '@encaminhamentoAEE/setLimparDadosEncaminhamento': {
        return {
          ...draft,
          dadosSecoesPorEtapaDeEncaminhamentoAEE: [],
          exibirLoaderEncaminhamentoAEE: false,
          dadosEncaminhamento: null,
          exibirModalErrosEncaminhamento: false,
          exibirModalEncerramentoEncaminhamentoAEE: false,
        };
      }

      default:
        return draft;
    }
  });
}

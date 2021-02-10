/**
 * @enum {Number}
 */
const situacaoAEE = {
  /** Em digitação */
  Rascunho: 1,
  /** Aguardando validação da coordenação */
  Encaminhado: 2,
  /** Aguardando análise do AEE */
  Analise: 3,
  /** Finalizado */
  Finalizado: 4,
  /** Encerrado */
  Encerrado: 5,
  /** Aguardando atribuição de responsável */
  AtribuicaoResponsavel: 6,
};

export default situacaoAEE;

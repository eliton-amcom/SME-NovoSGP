import PropTypes from 'prop-types';
import React from 'react';
import { useSelector } from 'react-redux';
import QuestionarioDinamico from '~/componentes-sgp/QuestionarioDinamico/questionarioDinamico';
import ServicoPlanoAEE from '~/servicos/Paginas/Relatorios/AEE/ServicoPlanoAEE';

const MontarDadosPorSecao = props => {
  const { dados, dadosQuestionarioAtual, match } = props;

  const dadosCollapseLocalizarEstudante = useSelector(
    store => store.collapseLocalizarEstudante.dadosCollapseLocalizarEstudante
  );

  // const validaSeDesabilitarCampo = () => {
  //   const planoId = match?.params?.id;

  //   return (
  //     desabilitarCamposPlanoAEE ||
  //     (planoId && !planoAEEDados.podeEditar) ||
  //     planoAEEDados?.situacao === situacaoPlanoAEE.Cancelado ||
  //     planoAEEDados?.situacao === situacaoPlanoAEE.Encerrado
  //   );
  // };

  return dadosQuestionarioAtual?.length ? (
    <QuestionarioDinamico
      codigoAluno={dadosCollapseLocalizarEstudante?.codigoAluno}
      codigoTurma={dadosCollapseLocalizarEstudante?.codigoTurma}
      anoLetivo={dadosCollapseLocalizarEstudante?.anoLetivo}
      dados={dados}
      dadosQuestionarioAtual={dadosQuestionarioAtual}
      // desabilitarCampos={validaSeDesabilitarCampo()}
      funcaoRemoverArquivoCampoUpload={ServicoPlanoAEE.removerArquivo}
      urlUpload="v1/plano-aee/upload"
    />
  ) : (
    ''
  );
};

MontarDadosPorSecao.propTypes = {
  dados: PropTypes.oneOfType([PropTypes.object]),
  match: PropTypes.oneOfType([PropTypes.object]),
  dadosQuestionarioAtual: PropTypes.oneOfType([PropTypes.object]),
};

MontarDadosPorSecao.defaultProps = {
  dados: {},
  match: {},
  dadosQuestionarioAtual: [],
};

export default MontarDadosPorSecao;

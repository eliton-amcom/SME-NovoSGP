import QuestionarioDinamicoFuncoes from '~/componentes-sgp/QuestionarioDinamico/Funcoes/QuestionarioDinamicoFuncoes';
import tipoQuestao from '~/dtos/tipoQuestao';
import { store } from '~/redux';
import {
  setDadosCollapseAtribuicaoResponsavel,
  setLimparDadosAtribuicaoResponsavel,
} from '~/redux/modulos/collapseAtribuicaoResponsavel/actions';
import {
  setDadosCollapseLocalizarEstudante,
  setLimparDadosLocalizarEstudante,
} from '~/redux/modulos/collapseLocalizarEstudante/actions';
import {
  setDadosEncaminhamento,
  setDadosModalAviso,
  setExibirLoaderEncaminhamentoAEE,
  setExibirModalAviso,
  setExibirModalErrosEncaminhamento,
  setLimparDadosEncaminhamento,
} from '~/redux/modulos/encaminhamentoAEE/actions';
import { setDadosObjectCardEstudante } from '~/redux/modulos/objectCardEstudante/actions';
import { erros } from '~/servicos/alertas';
import api from '~/servicos/api';

const urlPadrao = 'v1/encaminhamento-aee';

class ServicoEncaminhamentoAEE {
  obterSituacoes = () => {
    return api.get(`${urlPadrao}/situacoes`);
  };

  obterAlunoSituacaoEncaminhamentoAEE = codigoAluno => {
    return api.get(`${urlPadrao}/estudante/${codigoAluno}/situacao`);
  };

  obterAvisoModal = async () => {
    const { dispatch } = store;

    const state = store.getState();
    const { encaminhamentoAEE } = state;

    const { dadosModalAviso } = encaminhamentoAEE;

    if (!dadosModalAviso) {
      const retorno = await api.get(`${urlPadrao}/instrucoes-modal`);
      if (retorno?.data) {
        dispatch(setDadosModalAviso(retorno.data));
        dispatch(setExibirModalAviso(true));
      } else {
        dispatch(setDadosModalAviso());
        dispatch(setExibirModalAviso(false));
      }
    } else {
      dispatch(setExibirModalAviso(true));
    }
  };

  obterSecoesPorEtapaDeEncaminhamentoAEE = encaminhamentoAeeId => {
    let url = `${urlPadrao}/secoes`;
    if (encaminhamentoAeeId) {
      url += `?encaminhamentoAeeId=${encaminhamentoAeeId}`;
    }
    return api.get(url);
  };

  obterQuestionario = (
    questionarioId,
    encaminhamentoId,
    codigoAluno,
    codigoTurma
  ) => {
    let url = `${urlPadrao}/questionario?questionarioId=${questionarioId}&codigoAluno=${codigoAluno}&codigoTurma=${codigoTurma}`;
    if (encaminhamentoId) {
      url = `${url}&encaminhamentoId=${encaminhamentoId}`;
    }
    return api.get(url);
  };

  obterEncaminhamentoPorId = async encaminhamentoId => {
    const { dispatch } = store;

    dispatch(setExibirLoaderEncaminhamentoAEE(true));

    const resultado = await api
      .get(`${urlPadrao}/${encaminhamentoId}`)
      .catch(e => erros(e))
      .finally(() => dispatch(setExibirLoaderEncaminhamentoAEE(false)));

    if (resultado?.data) {
      const { aluno, turma, responsavelEncaminhamentoAEE } = resultado?.data;

      const dadosObjectCard = {
        nome: aluno.nome,
        numeroChamada: aluno.numeroAlunoChamada,
        dataNascimento: aluno.dataNascimento,
        codigoEOL: aluno.codigoAluno,
        situacao: aluno.situacao,
        dataSituacao: aluno.dataSituacao,
      };
      dispatch(setDadosObjectCardEstudante(dadosObjectCard));

      const dadosCollapseLocalizarEstudante = {
        anoLetivo: turma.anoLetivo,
        codigoAluno: aluno.codigoAluno,
        codigoTurma: turma.codigo,
        turmaId: turma.id,
      };
      dispatch(
        setDadosCollapseLocalizarEstudante(dadosCollapseLocalizarEstudante)
      );

      const dadosResponsavel = {
        codigoRF: responsavelEncaminhamentoAEE?.rf,
        nomeServidor: responsavelEncaminhamentoAEE?.nome,
        id: responsavelEncaminhamentoAEE?.id,
      };
      dispatch(setDadosCollapseAtribuicaoResponsavel(dadosResponsavel));

      dispatch(setDadosEncaminhamento(resultado?.data));
    } else {
      dispatch(setLimparDadosAtribuicaoResponsavel());
      dispatch(setLimparDadosLocalizarEstudante());
      dispatch(setLimparDadosEncaminhamento());
    }
  };

  // TODO
  // secaoEstaConcluida = secaoId => {
  //   const state = store.getState();
  //   const { encaminhamentoAEE } = state;
  //   const { formsSecoesEncaminhamentoAEE } = encaminhamentoAEE;

  //   if (formsSecoesEncaminhamentoAEE?.length) {
  //     const form = formsSecoesEncaminhamentoAEE.find(
  //       d => d.secaoId === secaoId
  //     );
  //     if (form && form()) {
  //       return form().getFormikContext().isValid;
  //     }
  //   }

  //   return false;
  // };

  salvarEncaminhamento = async (
    encaminhamentoId,
    situacao,
    enviarEncaminhamento
  ) => {
    const { dispatch } = store;

    const state = store.getState();
    const { questionarioDinamico, collapseLocalizarEstudante } = state;
    const { formsQuestionarioDinamico } = questionarioDinamico;

    const { dadosCollapseLocalizarEstudante } = collapseLocalizarEstudante;

    let contadorFormsValidos = 0;

    const validaAntesDoSubmit = refForm => {
      let arrayCampos = [];

      const camposValidar = refForm?.state?.values;
      if (camposValidar && Object.keys(camposValidar)?.length) {
        arrayCampos = Object.keys(camposValidar);
      }

      arrayCampos.forEach(campo => {
        refForm.setFieldTouched(campo, true, true);
      });
      return refForm.validateForm().then(() => {
        if (
          refForm.getFormikContext().isValid ||
          Object.keys(refForm.getFormikContext().errors).length === 0
        ) {
          contadorFormsValidos += 1;
        }
      });
    };

    if (formsQuestionarioDinamico?.length) {
      let todosOsFormsEstaoValidos = !enviarEncaminhamento;

      if (enviarEncaminhamento) {
        const promises = formsQuestionarioDinamico.map(async item =>
          validaAntesDoSubmit(item.form())
        );

        await Promise.all(promises);

        todosOsFormsEstaoValidos =
          contadorFormsValidos ===
          formsQuestionarioDinamico?.filter(a => a)?.length;
      }

      if (todosOsFormsEstaoValidos) {
        const valoresParaSalvar = {
          id: encaminhamentoId || 0,
          turmaId: dadosCollapseLocalizarEstudante.turmaId,
          alunoCodigo: dadosCollapseLocalizarEstudante.codigoAluno,
          situacao,
        };
        valoresParaSalvar.secoes = formsQuestionarioDinamico.map(
          (item, secaoId) => {
            const form = item.form();
            const campos = form.state.values;
            const questoes = [];

            Object.keys(campos).forEach(key => {
              const questaoAtual = QuestionarioDinamicoFuncoes.obterQuestaoPorId(
                item.dadosQuestionarioAtual,
                key
              );

              let questao = {
                questaoId: key,
                tipoQuestao: questaoAtual.tipoQuestao,
              };

              switch (questao.tipoQuestao) {
                case tipoQuestao.AtendimentoClinico:
                  questao.resposta = JSON.stringify(campos[key] || '');
                  break;
                case tipoQuestao.Upload:
                  if (campos[key]?.length) {
                    const arquivosId = campos[key].map(a => a.xhr);
                    questao.resposta = arquivosId;
                  } else {
                    questao.resposta = '';
                  }
                  break;
                case tipoQuestao.ComboMultiplaEscolha:
                  if (campos[key]?.length) {
                    questao.resposta = campos[key];
                  } else {
                    questao.resposta = '';
                  }
                  break;
                default:
                  questao.resposta = campos[key] || '';
                  break;
              }

              if (
                questao.tipoQuestao === tipoQuestao.Upload &&
                questao?.resposta?.length
              ) {
                questao.resposta.forEach(codigo => {
                  if (codigo) {
                    if (questaoAtual?.resposta?.length) {
                      const arquivoResposta = questaoAtual.resposta.find(
                        a => a?.arquivo?.codigo === codigo
                      );

                      if (arquivoResposta) {
                        questoes.push({
                          ...questao,
                          resposta: codigo,
                          respostaEncaminhamentoId: arquivoResposta.id,
                        });
                      } else {
                        questoes.push({
                          ...questao,
                          resposta: codigo,
                        });
                      }
                    } else {
                      questoes.push({
                        ...questao,
                        resposta: codigo,
                      });
                    }
                  }
                });
              } else if (
                (questao.tipoQuestao === tipoQuestao.ComboMultiplaEscolha ||
                  questao.tipoQuestao === tipoQuestao.Checkbox) &&
                questao?.resposta?.length
              ) {
                questao.resposta.forEach(valorSelecionado => {
                  if (valorSelecionado) {
                    if (questaoAtual?.resposta?.length) {
                      const temResposta = questaoAtual.resposta.find(
                        a =>
                          String(a?.opcaoRespostaId) ===
                          String(valorSelecionado)
                      );

                      if (temResposta) {
                        questoes.push({
                          ...questao,
                          resposta: valorSelecionado,
                          respostaEncaminhamentoId: temResposta.id,
                        });
                      } else {
                        questoes.push({
                          ...questao,
                          resposta: valorSelecionado,
                        });
                      }
                    } else {
                      questoes.push({
                        ...questao,
                        resposta: valorSelecionado,
                      });
                    }
                  }
                });
              } else {
                if (questaoAtual?.resposta[0]?.id) {
                  questao.respostaEncaminhamentoId =
                    questaoAtual.resposta[0].id;
                }

                if (
                  (questao.tipoQuestao === tipoQuestao.Upload ||
                    questao.tipoQuestao === tipoQuestao.ComboMultiplaEscolha) &&
                  !questao.resposta
                ) {
                  questao = null;
                }

                if (questao) {
                  questoes.push(questao);
                }
              }
            });
            return {
              questoes,
              secaoId,
              concluido:
                Object.keys(form.getFormikContext().errors)?.length === 0,
            };
          }
        );

        valoresParaSalvar.secoes = valoresParaSalvar.secoes
          .filter(a => a)
          .filter(b => b.questoes?.length);

        if (valoresParaSalvar?.secoes?.length) {
          dispatch(setExibirLoaderEncaminhamentoAEE(true));

          const resposta = await api
            .post(`${urlPadrao}/salvar`, valoresParaSalvar)
            .catch(e => erros(e))
            .finally(() => dispatch(setExibirLoaderEncaminhamentoAEE(false)));

          if (resposta?.status === 200) {
            return true;
          }
        }
      } else {
        dispatch(setExibirModalErrosEncaminhamento(true));
      }
    }
    return false;
  };

  excluirEncaminhamento = encaminhamentoId => {
    const url = `${urlPadrao}/${encaminhamentoId}`;
    return api.delete(url);
  };

  podeCadastrarEncaminhamentoEstudante = async codigoEstudante => {
    const resultado = await api
      .get(`${urlPadrao}/estudante/${codigoEstudante}/pode-cadastrar`)
      .catch(e => erros(e));

    if (resultado?.data) {
      return true;
    }
    return false;
  };

  removerArquivo = arquivoCodigo => {
    return api.delete(`${urlPadrao}/arquivo?arquivoCodigo=${arquivoCodigo}`);
  };

  encerramentoEncaminhamentoAEE = (encaminhamentoId, motivoEncerramento) => {
    const parametro = { encaminhamentoId, motivoEncerramento };
    return api.post(`${urlPadrao}/encerrar`, parametro);
  };

  enviarParaAnaliseEncaminhamento = encaminhamentoId => {
    return api.post(`${urlPadrao}/enviar-analise/${encaminhamentoId}`);
  };

  obterResponsaveis = (dreId, ueId, turmaId, alunoCodigo, situacao, anoLetivo) => {
    let url = `${urlPadrao}/responsaveis?dreId=${dreId}&ueId=${ueId}&anoLetivo=${anoLetivo}`;

    if (turmaId) {
      url += `&turmaId=${turmaId}`;
    }
    if (alunoCodigo) {
      url += `&alunoCodigo=${alunoCodigo}`;
    }
    if (situacao) {
      url += `&situacao=${situacao}`;
    }

    return api.get(url);
  };

  atribuirResponsavelEncaminhamento = (rfResponsavel, encaminhamentoId) => {
    const params = {
      rfResponsavel,
      encaminhamentoId,
    };
    return api.post(`${urlPadrao}/atribuir-responsavel`, params);
  };

  concluirEncaminhamento = encaminhamentoId => {
    return api.post(`${urlPadrao}/concluir/${encaminhamentoId}`);
  };

  removerResponsavel = encaminhamentoId => {
    return api.post(`${urlPadrao}/remover-responsavel/${encaminhamentoId}`);
  };
}

export default new ServicoEncaminhamentoAEE();

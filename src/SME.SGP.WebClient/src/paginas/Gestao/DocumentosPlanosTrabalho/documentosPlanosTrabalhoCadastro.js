import { Form, Formik } from 'formik';
import PropTypes from 'prop-types';
import React, { useCallback, useEffect, useState } from 'react';
import * as Yup from 'yup';
import { Auditoria, Loader, Localizador, SelectComponent } from '~/componentes';
import { Cabecalho } from '~/componentes-sgp';
import DreDropDown from '~/componentes-sgp/DreDropDown/';
import UeDropDown from '~/componentes-sgp/UeDropDown/';
import UploadArquivos from '~/componentes-sgp/UploadArquivos/uploadArquivos';
import Button from '~/componentes/button';
import Card from '~/componentes/card';
import { Colors } from '~/componentes/colors';
import { RotasDto } from '~/dtos';
import {
  confirmar,
  erro,
  erros,
  setBreadcrumbManual,
  sucesso,
} from '~/servicos';
import ServicoArmazenamento from '~/servicos/Componentes/ServicoArmazenamento';
import history from '~/servicos/history';
import ServicoDocumentosPlanosTrabalho from '~/servicos/Paginas/Gestao/DocumentosPlanosTrabalho/ServicoDocumentosPlanosTrabalho';
import FiltroHelper from '~componentes-sgp/filtro/helper';

const DocumentosPlanosTrabalhoCadastro = ({ match }) => {
  const [listaAnosLetivo, setListaAnosLetivo] = useState([]);

  const [listaUes, setListaUes] = useState([]);

  const [listaTipoDocumento, setListaTipoDocumento] = useState([]);

  const [listaClassificacao, setListaClassificacao] = useState([]);

  const [idDocumentosPlanoTrabalho, setIdDocumentosPlanoTrabalho] = useState(0);

  const [anoAtual] = useState(window.moment().format('YYYY'));

  const [valoresIniciais, setValoresIniciais] = useState({
    anoLetivo: null,
  });

  const [urlBuscarDres, setUrlBuscarDres] = useState();

  const [modoEdicao, setModoEdicao] = useState(false);

  const [exibirLoader, setExibirLoader] = useState(false);

  const [carregouAnosLetivos, setCarregouAnosLetivos] = useState(false);
  const [carregouTiposDocumento, setCarregouTiposDocumento] = useState(false);

  const [listaDeArquivos, setListaDeArquivos] = useState([]);

  const [defaultFileList, setDefaultFileList] = useState([]);

  const montarUrlBuscarDres = anoLetivo => {
    setUrlBuscarDres();
    setUrlBuscarDres(`v1/abrangencias/false/dres?anoLetivo=${anoLetivo}`);
  };

  const validacoes = () => {
    return Yup.object({
      anoLetivo: Yup.string().required('Campo obrigatório'),
      dreId: Yup.string().required('Campo obrigatório'),
      ueId: Yup.string().required('Campo obrigatório'),
      tipoDocumentoId: Yup.string().required('Campo obrigatório'),
      classificacaoId: Yup.string().required('Campo obrigatório'),
      professorRf: Yup.string().required('Campo obrigatório'),
    });
  };

  useEffect(() => {
    if (match?.params?.id) {
      setIdDocumentosPlanoTrabalho(match.params.id);
      setBreadcrumbManual(
        match.url,
        'Upload do arquivo',
        RotasDto.DOCUMENTOS_PLANOS_TRABALHO
      );
    }
  }, [match]);

  const obterDadosDocumento = useCallback(async () => {
    setExibirLoader(true);

    const resposta = await ServicoDocumentosPlanosTrabalho.obterDocumento(
      idDocumentosPlanoTrabalho
    )
      .catch(e => erros(e))
      .finally(() => setExibirLoader(false));

    if (resposta && resposta.status === 200) {
      resposta.data.tipoDocumentoId = String(resposta.data.tipoDocumentoId);
      resposta.data.classificacaoId = String(resposta.data.classificacaoId);

      resposta.data.defaultFileList = [
        {
          uid: resposta.data.codigoArquivo,
          xhr: resposta.data.codigoArquivo,
          name: resposta.data.nomeArquivo,
          status: 'done',
          documentoId: resposta.data.id,
        },
      ];
      setDefaultFileList(resposta.data.defaultFileList);
      setListaDeArquivos([...resposta.data.defaultFileList]);
      setValoresIniciais(resposta.data);
    }
  }, [idDocumentosPlanoTrabalho]);

  useEffect(() => {
    if (idDocumentosPlanoTrabalho) {
      obterDadosDocumento();
    }
  }, [idDocumentosPlanoTrabalho, obterDadosDocumento]);

  const atualizaValoresIniciais = useCallback(
    (nomeCampo, valor) => {
      if (!match?.params?.id) {
        valoresIniciais[nomeCampo] = valor;
        setValoresIniciais(valoresIniciais);
      }
    },
    [valoresIniciais, match]
  );

  const obterAnosLetivos = useCallback(async () => {
    setExibirLoader(true);
    let anosLetivos = [];

    const anosLetivoComHistorico = await FiltroHelper.obterAnosLetivos({
      consideraHistorico: true,
    });
    const anosLetivoSemHistorico = await FiltroHelper.obterAnosLetivos({
      consideraHistorico: false,
    });

    anosLetivos = anosLetivos.concat(anosLetivoComHistorico);

    anosLetivoSemHistorico.forEach(ano => {
      if (!anosLetivoComHistorico.find(a => a.valor === ano.valor)) {
        anosLetivos.push(ano);
      }
    });

    if (!anosLetivos.length) {
      anosLetivos.push({
        desc: anoAtual,
        valor: anoAtual,
      });
    }

    if (anosLetivos && anosLetivos.length) {
      const temAnoAtualNaLista = anosLetivos.find(
        item => String(item.valor) === String(anoAtual)
      );
      if (temAnoAtualNaLista) {
        atualizaValoresIniciais('anoLetivo', anoAtual);
        montarUrlBuscarDres(anoAtual);
      } else {
        atualizaValoresIniciais('anoLetivo', String(anosLetivos[0].valor));
        montarUrlBuscarDres(String(anosLetivos[0].valor));
      }
    }

    setListaAnosLetivo(anosLetivos);
    setExibirLoader(false);
    setCarregouAnosLetivos(true);
  }, [anoAtual, atualizaValoresIniciais]);

  const obterTiposDocumento = useCallback(async () => {
    const resposta = await ServicoDocumentosPlanosTrabalho.obterTiposDeDocumentos().catch(
      e => erros(e)
    );

    if (resposta?.data?.length) {
      setListaTipoDocumento(resposta.data);
      if (resposta.data.length === 1) {
        const tipo = resposta.data[0];
        atualizaValoresIniciais('tipoDocumentoId', String(tipo.id));

        if (tipo.classificacoes.length === 1) {
          setListaClassificacao(tipo.classificacoes);
          const classificacao = tipo.classificacoes[0];
          atualizaValoresIniciais('classificacaoId', String(classificacao.id));
        }
      }

      if (match?.params?.id && valoresIniciais.tipoDocumentoId) {
        let classificacaoPorTipo = [];

        const lista = resposta.data.find(
          item => String(item.id) === String(valoresIniciais.tipoDocumentoId)
        );
        classificacaoPorTipo = lista.classificacoes;
        setListaClassificacao(classificacaoPorTipo);
      }
    }
    setCarregouTiposDocumento(true);
  }, [atualizaValoresIniciais]);

  useEffect(() => {
    obterAnosLetivos();
    obterTiposDocumento();
  }, [obterAnosLetivos, obterTiposDocumento]);

  const onChangeAnoLetivo = (ano, form) => {
    form.setFieldValue('dreId', '');
    form.setFieldValue('ueId', '');
    setModoEdicao(true);
    montarUrlBuscarDres(ano);
  };

  const onClickVoltar = async () => {
    if (modoEdicao) {
      const confirmado = await confirmar(
        'Atenção',
        'Você não salvou as informações preenchidas.',
        'Deseja voltar para tela de listagem agora?'
      );
      if (confirmado) {
        if (listaDeArquivos?.length && !listaDeArquivos[0].documentoId) {
          await ServicoArmazenamento.removerArquivo(
            listaDeArquivos[0].xhr
          ).catch(e => erros(e));
        }
        history.push(RotasDto.DOCUMENTOS_PLANOS_TRABALHO);
      }
    } else {
      history.push(RotasDto.DOCUMENTOS_PLANOS_TRABALHO);
    }
  };

  const onChangeTipoDocumento = (tipo, form) => {
    let classificacaoPorTipo = [];
    if (tipo) {
      const lista = listaTipoDocumento.find(
        item => String(item.id) === String(tipo)
      );
      classificacaoPorTipo = lista.classificacoes;
    }
    setListaClassificacao(classificacaoPorTipo);

    if (classificacaoPorTipo?.length === 1) {
      form.setFieldValue('classificacaoId', String(classificacaoPorTipo[0].id));
    } else {
      form.setFieldValue('classificacaoId', '');
    }
    setModoEdicao(true);
  };

  const onSubmitFormulario = async valores => {
    const {
      ueId,
      usuarioId,
      classificacaoId,
      tipoDocumentoId,
      anoLetivo,
    } = valores;

    const ueSelecionada = listaUes.find(
      item => String(item.valor) === String(ueId)
    );

    setExibirLoader(true);
    const existeRegistro = await ServicoDocumentosPlanosTrabalho.validacaoUsuarioDocumento(
      tipoDocumentoId,
      classificacaoId,
      usuarioId,
      ueSelecionada.id
    ).catch(e => {
      erros(e);
      setExibirLoader(false);
    });

    if (!(existeRegistro?.status === 200)) {
      setExibirLoader(false);
      return;
    }

    if (existeRegistro?.data) {
      erro(
        `Este RF já está vinculado a outro registro do mesmo tipo e classificação no ano letivo ${anoLetivo}`
      );
      setExibirLoader(false);
      return;
    }

    const arquivoCodigo = listaDeArquivos[0].xhr;
    const params = {
      arquivoCodigo,
      ueId: ueSelecionada.id,
      tipoDocumentoId,
      classificacaoId,
      usuarioId,
      anoLetivo,
    };

    // setExibirLoader(true);
    const resposta = await ServicoDocumentosPlanosTrabalho.salvarDocumento(
      params,
      idDocumentosPlanoTrabalho
    )
      .catch(e => erros(e))
      .finally(() => setExibirLoader(false));

    if (resposta && resposta.status === 200) {
      sucesso(
        `Registro ${
          idDocumentosPlanoTrabalho ? 'alterado' : 'salvo'
        } com sucesso`
      );
      history.push(RotasDto.DOCUMENTOS_PLANOS_TRABALHO);
    }
  };

  const resetarTela = form => {
    form.resetForm();
    setModoEdicao(false);
    montarUrlBuscarDres(valoresIniciais.anoLetivo);
  };

  const onClickCancelar = async form => {
    if (modoEdicao) {
      const confirmou = await confirmar(
        'Atenção',
        'Você não salvou as informações preenchidas.',
        'Deseja realmente cancelar as alterações?'
      );
      if (confirmou) {
        if (idDocumentosPlanoTrabalho) {
          setDefaultFileList([...valoresIniciais.defaultFileList]);
          setListaDeArquivos([...valoresIniciais.defaultFileList]);
        } else {
          setListaDeArquivos([]);
          setDefaultFileList([]);
        }

        if (listaDeArquivos?.length && !listaDeArquivos[0].documentoId) {
          await ServicoArmazenamento.removerArquivo(
            listaDeArquivos[0].xhr
          ).catch(e => erros(e));
        }
        resetarTela(form);
      }
    }
  };

  const validaAntesDoSubmit = form => {
    const arrayCampos = Object.keys(valoresIniciais);
    arrayCampos.forEach(campo => {
      form.setFieldTouched(campo, true, true);
    });
    form.validateForm().then(() => {
      if (form.isValid || Object.keys(form.errors).length === 0) {
        form.submitForm(form);
      }
    });
  };

  const onRemoveFile = async arquivo => {
    const codigoArquivo = arquivo.xhr;

    if (arquivo.documentoId) {
      setListaDeArquivos([]);
      sucesso(`Arquivo ${arquivo.name} removido com sucesso`);
      return true;
    }

    const resposta = await ServicoArmazenamento.removerArquivo(
      codigoArquivo
    ).catch(e => erros(e));

    if (resposta && resposta.status === 200) {
      sucesso(`Arquivo ${arquivo.name} removido com sucesso`);
      return true;
    }
    return false;
  };

  const onClickExcluir = async () => {
    const confirmado = await confirmar(
      'Atenção',
      'Você tem certeza que deseja excluir este registro?'
    );
    if (confirmado) {
      setExibirLoader(true);
      const resultado = await ServicoDocumentosPlanosTrabalho.excluirDocumento(
        idDocumentosPlanoTrabalho
      )
        .catch(e => erros(e))
        .finally(() => setExibirLoader(false));

      if (resultado && resultado.status === 200) {
        sucesso('Registro excluído com sucesso!');
        history.push(RotasDto.DOCUMENTOS_PLANOS_TRABALHO);
      }
    }
  };

  return (
    <Loader loading={exibirLoader}>
      <Cabecalho pagina="Upload de documentos e planos de trabalho" />
      {carregouAnosLetivos &&
      carregouTiposDocumento &&
      valoresIniciais.anoLetivo ? (
        <Card>
          <Formik
            enableReinitialize
            initialValues={valoresIniciais}
            validationSchema={validacoes}
            onSubmit={onSubmitFormulario}
            validateOnBlur
            validateOnChange
          >
            {form => (
              <Form>
                <div className="col-md-12">
                  <div className="row">
                    <div className="col-md-12 d-flex justify-content-end pb-4 justify-itens-end">
                      <Button
                        id="btn-voltar"
                        label="Voltar"
                        icon="arrow-left"
                        color={Colors.Azul}
                        border
                        className="mr-2"
                        onClick={onClickVoltar}
                      />
                      <Button
                        id="btn-cancelar"
                        label="Cancelar"
                        color={Colors.Roxo}
                        border
                        className="mr-2"
                        onClick={() => onClickCancelar(form)}
                        disabled={!modoEdicao}
                      />
                      <Button
                        label="Excluir"
                        color={Colors.Vermelho}
                        border
                        className="mr-2"
                        disabled={!idDocumentosPlanoTrabalho}
                        onClick={onClickExcluir}
                      />
                      <Button
                        id="btn-novo"
                        label={
                          idDocumentosPlanoTrabalho ? 'Alterar' : 'Cadastrar'
                        }
                        color={Colors.Roxo}
                        border
                        bold
                        onClick={() => validaAntesDoSubmit(form)}
                        disabled={!modoEdicao}
                      />
                    </div>
                  </div>
                  <div className="row">
                    <div className="col-sm-12 col-md-6 col-lg-3 col-xl-2 mb-2">
                      <SelectComponent
                        id="select-ano-letivo"
                        label="Ano Letivo"
                        lista={listaAnosLetivo}
                        valueOption="valor"
                        valueText="desc"
                        disabled={
                          (listaAnosLetivo && listaAnosLetivo.length === 1) ||
                          !!idDocumentosPlanoTrabalho
                        }
                        onChange={valor => onChangeAnoLetivo(valor, form)}
                        placeholder="Ano letivo"
                        form={form}
                        name="anoLetivo"
                        allowClear={false}
                      />
                    </div>
                  </div>
                  <div className="row">
                    <div className="col-sm-12 col-md-12 col-lg-6 col-xl-6 mb-2">
                      <DreDropDown
                        id="select-dre"
                        label="Diretoria Regional de Educação (DRE)"
                        url={urlBuscarDres}
                        form={form}
                        onChange={(valor, lista, changeManual) => {
                          if (changeManual) {
                            setModoEdicao(true);
                          }
                        }}
                        desabilitado={!!idDocumentosPlanoTrabalho}
                      />
                    </div>
                    <div className="col-sm-12 col-md-12 col-lg-6 col-xl-6 mb-2">
                      <UeDropDown
                        id="select-ue"
                        label="Unidade Escolar (UE)"
                        dreId={form.values.dreId}
                        form={form}
                        url={`v1/abrangencias/false/dres/${form.values.dreId}/ues?anoLetivo=${form.values.anoLetivo}`}
                        temParametros
                        onChange={(_, lista, changeManual) => {
                          setListaUes(lista);
                          if (changeManual) {
                            setModoEdicao(true);
                          }
                        }}
                        desabilitado={!!idDocumentosPlanoTrabalho}
                      />
                    </div>

                    <div className="col-sm-12 col-md-6 col-lg-6 col-xl-6 mb-2">
                      <SelectComponent
                        id="select-tipos-documento"
                        label="Tipo de documento"
                        lista={listaTipoDocumento}
                        valueOption="id"
                        valueText="tipoDocumento"
                        onChange={valor => onChangeTipoDocumento(valor, form)}
                        placeholder="Tipo de documento"
                        form={form}
                        name="tipoDocumentoId"
                        disabled={
                          listaTipoDocumento?.length === 1 ||
                          !!idDocumentosPlanoTrabalho
                        }
                      />
                    </div>
                    <div className="col-sm-12 col-md-6 col-lg-6 col-xl- mb-2">
                      <SelectComponent
                        id="select-classificacao-documento"
                        label="Classificação"
                        lista={listaClassificacao}
                        valueOption="id"
                        valueText="classificacao"
                        onChange={() => setModoEdicao(true)}
                        placeholder="Classificação do documento"
                        form={form}
                        name="classificacaoId"
                        disabled={
                          listaClassificacao?.length === 1 ||
                          !!idDocumentosPlanoTrabalho
                        }
                      />
                    </div>
                    <div className="col-md-12 mb-2">
                      <div className="row pr-3">
                        <Localizador
                          desabilitado={!!idDocumentosPlanoTrabalho}
                          dreId={form.values.dreId}
                          anoLetivo={form.values.anoLetivo}
                          rfEdicao={form.values.professorRf}
                          showLabel
                          form={form}
                          onChange={valor => {
                            const campos = Object.keys(valor);
                            const onChangeManual = campos.find(
                              item => item === 'professorNome'
                            );
                            if (
                              !idDocumentosPlanoTrabalho &&
                              onChangeManual === 'professorNome'
                            ) {
                              setModoEdicao(true);
                            }
                          }}
                        />
                      </div>
                    </div>
                    <div className="col-md-12 mt-2">
                      <UploadArquivos
                        desabilitar={listaDeArquivos.length > 0}
                        textoFormatoUpload="Permitido somente um arquivo. Tipo permitido PDF"
                        tiposArquivosPermitidos=".pdf"
                        onRemove={onRemoveFile}
                        urlUpload="v1/armazenamento/documentos/upload"
                        defaultFileList={defaultFileList}
                        onChangeListaArquivos={lista => {
                          setListaDeArquivos(lista);
                          setModoEdicao(true);
                        }}
                      />
                    </div>
                    <Auditoria
                      criadoEm={valoresIniciais?.criadoEm}
                      criadoPor={valoresIniciais?.criadoPor}
                      criadoRf={valoresIniciais?.criadoRf}
                      alteradoPor={valoresIniciais?.alteradoPor}
                      alteradoEm={valoresIniciais?.alteradoEm}
                      alteradoRf={valoresIniciais?.alteradoRf}
                    />
                  </div>
                </div>
              </Form>
            )}
          </Formik>
        </Card>
      ) : (
        ''
      )}
    </Loader>
  );
};

DocumentosPlanosTrabalhoCadastro.propTypes = {
  match: PropTypes.oneOfType([PropTypes.object]),
};

DocumentosPlanosTrabalhoCadastro.defaultProps = {
  match: {},
};

export default DocumentosPlanosTrabalhoCadastro;

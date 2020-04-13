import React, { useState, useCallback, useEffect, useRef } from 'react';
import { useSelector } from 'react-redux';
import PropTypes from 'prop-types';
import styled from 'styled-components';

import { Form, Formik } from 'formik';
import * as Yup from 'yup';

import { Cabecalho } from '~/componentes-sgp';
import {
  Loader,
  Card,
  ButtonGroup,
  Grid,
  SelectComponent,
  CampoTexto,
  CampoData,
  Label,
  TextEditor,
  momentSchema,
  Base,
} from '~/componentes';

import { Linha } from '~/componentes/EstilosGlobais';

import history from '~/servicos/history';
import RotasDto from '~/dtos/rotasDto';
import { verificaSomenteConsulta } from '~/servicos/servico-navegacao';
import ServicoComunicados from '~/servicos/Paginas/AcompanhamentoEscolar/Comunicados/ServicoComunicados';

const ComunicadosCadastro = ({ match }) => {
  const ErroValidacao = styled.span`
    color: ${Base.Vermelho};
  `;

  const InseridoAlterado = styled.div`
    color: ${Base.CinzaMako};
    font-weight: bold;
    font-style: normal;
    font-size: 10px;
    object-fit: contain;
    p {
      margin: 0;
    }
  `;

  const [loaderSecao] = useState(false);

  const [somenteConsulta, setSomenteConsulta] = useState(false);
  const permissoesTela = useSelector(store => store.usuario.permissoes);

  useCallback(() => {
    setSomenteConsulta(verificaSomenteConsulta(permissoesTela));
  }, [permissoesTela]);

  const [novoRegistro, setNovoRegistro] = useState(true);
  const [modoEdicao] = useState(false);

  const [idComunicado, setIdComunicado] = useState();

  useEffect(() => {
    if (match && match.params && match.params.id) {
      setIdComunicado(match.params.id);
    }
  }, [match]);

  const [valoresIniciais, setValoresIniciais] = useState({
    id: '',
    gruposId: [],
    dataEnvioInicio: '',
    dataEnvioFim: '',
    dataExpiracaoInicio: '',
    dataExpiracaoFim: '',
    titulo: '',
    descricao: '',
  });

  const [descricaoComunicado, setDescricaoComunicado] = useState('');

  const [inseridoAlterado, setInseridoAlterado] = useState({
    alteradoEm: '',
    alteradoPor: '',
    alteradoRF: '',
    criadoEm: '',
    criadoPor: '',
    criadoRF: '',
  });

  useEffect(() => {
    async function obterPorId(id) {
      const comunicado = await ServicoComunicados.consultarPorId(id);
      if (comunicado && Object.entries(comunicado).length) {
        setValoresIniciais({
          id: comunicado.id,
          gruposId: [...comunicado.grupos.map(grupo => grupo.id)],
          // dataEnvioInicio: '',
          // dataEnvioFim: '',
          // dataExpiracaoInicio: '',
          // dataExpiracaoFim: '',
          titulo: comunicado.titulo,
          descricao: comunicado.descricao,
        });

        setDescricaoComunicado(comunicado.descricao);

        setInseridoAlterado({
          alteradoEm: comunicado.alteradoEm,
          alteradoPor: comunicado.alteradoPor,
          alteradoRF: comunicado.alteradoRF,
          criadoEm: comunicado.criadoEm,
          criadoPor: comunicado.criadoPor,
          criadoRF: comunicado.criadoRF,
        });
      }
    }

    if (idComunicado) {
      obterPorId(idComunicado);
      setInseridoAlterado({});
      setDescricaoComunicado('');
      setNovoRegistro(false);
    }
  }, [idComunicado]);

  const [refForm, setRefForm] = useState({});
  const textEditorRef = useRef();

  const [gruposLista, setGruposLista] = useState([]);

  useEffect(() => {
    async function obterListaGrupos() {
      const lista = await ServicoComunicados.listarGrupos();
      setGruposLista(lista);
    }
    obterListaGrupos();
  }, []);

  const [validacoes] = useState(
    Yup.object({
      grupoId: Yup.string().required('Campo obrigatório'),
      dataEnvioInicio: momentSchema
        .required('Campo obrigatório')
        .test(
          'validaDataEnvio',
          'Data início não deve ser maior que a data fim',
          function validar() {
            const { dataEnvioInicio } = this.parent;
            const { dataEnvioFim } = this.parent;

            if (
              dataEnvioInicio &&
              dataEnvioFim &&
              window.moment(dataEnvioInicio) > window.moment(dataEnvioFim)
            ) {
              return false;
            }

            return true;
          }
        ),
      dataEnvioFim: Yup.string().required('Campo obrigatório'),
      dataExpiracaoInicio: momentSchema
        .test(
          'validaDataMaiorQueEnvio',
          'Data de expiração deve ser maior que a data de envio',
          function validar() {
            const { dataEnvioFim } = this.parent;
            const { dataExpiracaoInicio } = this.parent;

            if (
              dataEnvioFim &&
              dataExpiracaoInicio &&
              window.moment(dataExpiracaoInicio) < window.moment(dataEnvioFim)
            ) {
              return false;
            }

            return true;
          }
        )
        .test(
          'validaDataExpiracao',
          'Data início não deve ser maior que a data fim',
          function validar() {
            const { dataExpiracaoInicio } = this.parent;
            const { dataExpiracaoFim } = this.parent;

            if (
              dataExpiracaoInicio &&
              dataExpiracaoFim &&
              window.moment(dataExpiracaoInicio) >
                window.moment(dataExpiracaoFim)
            ) {
              return false;
            }

            return true;
          }
        ),
      dataExpiracaoFim: Yup.string(),
      titulo: Yup.string()
        .required('Campo obrigatório')
        .min(10, 'Deve conter no mínimo 10 caracteres')
        .max(50, 'Deve conter no máximo 50 caracteres'),
    })
  );

  const [descricaoValida, setDescricaoValida] = useState(true);

  const validarAntesDeSalvar = form => {
    const arrayCampos = Object.keys(valoresIniciais);

    arrayCampos.forEach(campo => {
      form.setFieldTouched(campo, true, true);
    });

    const descricao = textEditorRef.current.state.value.replace(
      '<p><br></p>',
      ''
    );

    form.validateForm().then(() => {
      setDescricaoValida(descricao.length);

      if (refForm && form.isValid && descricao.length) {
        setDescricaoComunicado(descricao);
        form.handleSubmit(form);
      }
    });
  };

  const onClickExcluir = async () => {};

  const onClickVoltar = () => {
    history.push(RotasDto.ACOMPANHAMENTO_COMUNICADOS);
  };

  const onClickBotaoPrincipal = form => {
    validarAntesDeSalvar(form);
  };

  const onClickSalvar = valores => {
    const dadosSalvar = {
      ...valores,
      descricao: textEditorRef.current.state.value,
    };
    return dadosSalvar;
  };

  const onChangeGruposId = grupoId => {
    setValoresIniciais({ ...refForm.state.values, gruposId: grupoId });
  };

  return (
    <>
      <Cabecalho pagina="Comunicação com pais ou responsáveis" />
      <Loader loading={loaderSecao}>
        <Card mx="mx-0">
          <Formik
            enableReinitialize
            initialValues={valoresIniciais}
            validationSchema={validacoes}
            ref={refFormik => setRefForm(refFormik)}
            onSubmit={valores => onClickSalvar(valores)}
            validateOnBlur
            validateOnChange
          >
            {form => (
              <Form className="col-md-12 mb-4">
                <ButtonGroup
                  form={form}
                  novoRegistro={novoRegistro}
                  modoEdicao={modoEdicao}
                  somenteConsulta={somenteConsulta}
                  permissoesTela={
                    permissoesTela[RotasDto.ACOMPANHAMENTO_COMUNICADOS]
                  }
                  onClickExcluir={onClickExcluir}
                  onClickVoltar={onClickVoltar}
                  onClickBotaoPrincipal={() => onClickBotaoPrincipal(form)}
                  labelBotaoPrincipal={idComunicado ? 'Salvar' : 'Cadastrar'}
                />
                <Linha className="row mb-2">
                  <Grid cols={4}>
                    <Label control="grupoId" text="Grupo" />
                    <SelectComponent
                      form={form}
                      name="gruposId"
                      placeholder="Selecione um grupo"
                      valueSelect={form.values.gruposId}
                      multiple
                      onChange={onChangeGruposId}
                      lista={gruposLista}
                      valueOption="id"
                      valueText="nome"
                    />
                  </Grid>
                  <Grid cols={2}>
                    <Label control="dataEnvioInicio" text="Data de envio" />
                    <CampoData
                      form={form}
                      name="dataEnvioInicio"
                      placeholder="Data início"
                      formatoData="DD/MM/YYYY"
                    />
                  </Grid>
                  <Grid cols={2}>
                    <Label
                      control="dataEnvioFim"
                      text="Data de envio"
                      className="text-white"
                    />
                    <CampoData
                      form={form}
                      name="dataEnvioFim"
                      placeholder="Data fim"
                      formatoData="DD/MM/YYYY"
                    />
                  </Grid>
                  <Grid cols={2}>
                    <Label
                      control="dataExpiracaoInicio"
                      text="Data de expiração"
                    />
                    <CampoData
                      form={form}
                      name="dataExpiracaoInicio"
                      placeholder="Data início"
                      formatoData="DD/MM/YYYY"
                    />
                  </Grid>
                  <Grid cols={2}>
                    <Label
                      control="dataExpiracaoFim"
                      text="Data de expiração"
                      className="text-white"
                    />
                    <CampoData
                      form={form}
                      name="dataExpiracaoFim"
                      placeholder="Data fim"
                      formatoData="DD/MM/YYYY"
                    />
                  </Grid>
                </Linha>
                <Linha className="row">
                  <Grid cols={12}>
                    <Label control="titulo" text="Tíutulo" />
                    <CampoTexto
                      form={form}
                      name="titulo"
                      placeholder="Procure pelo título do comunicado"
                      value={form.values.titulo}
                    />
                  </Grid>
                </Linha>
                <Linha className="row">
                  <Grid cols={12}>
                    <Label control="textEditor" text="Descrição" />
                    <TextEditor
                      ref={textEditorRef}
                      id="textEditor"
                      height="120px"
                      maxHeight="calc(100vh)"
                      className={`${!descricaoValida && 'is-invalid'}`}
                      value={descricaoComunicado}
                      disabled={somenteConsulta}
                    />
                    {!descricaoValida && (
                      <ErroValidacao>Campo obrigatório</ErroValidacao>
                    )}
                    <InseridoAlterado>
                      {inseridoAlterado &&
                      inseridoAlterado.criadoPor &&
                      inseridoAlterado.criadoPor.length ? (
                        <p className="pt-2">
                          INSERIDO por {inseridoAlterado.criadoPor} (
                          {inseridoAlterado.criadoRF}) em{' '}
                          {inseridoAlterado.criadoEm}
                        </p>
                      ) : (
                        ''
                      )}
                      {inseridoAlterado &&
                      inseridoAlterado.alteradoPor &&
                      inseridoAlterado.alteradoPor.length ? (
                        <p>
                          ALTERADO por {inseridoAlterado.alteradoPor} (
                          {inseridoAlterado.alteradoRF}) em{' '}
                          {inseridoAlterado.alteradoEm}
                        </p>
                      ) : (
                        ''
                      )}
                    </InseridoAlterado>
                  </Grid>
                </Linha>
              </Form>
            )}
          </Formik>
        </Card>
      </Loader>
    </>
  );
};

ComunicadosCadastro.propTypes = {
  match: PropTypes.oneOfType([PropTypes.array, PropTypes.object]),
};

ComunicadosCadastro.defaultProps = {
  match: {},
};

export default ComunicadosCadastro;

import React, { useState, useEffect } from 'react';
import PropTypes from 'prop-types';

// Form
import { Formik, Form } from 'formik';
import * as Yup from 'yup';

// Redux
import { useSelector, useDispatch } from 'react-redux';
import { setLoaderSecao } from '~/redux/modulos/loader/actions';

// Serviços
import RotasDto from '~/dtos/rotasDto';
import history from '~/servicos/history';
import { erros, erro, sucesso, confirmar } from '~/servicos/alertas';
import { setBreadcrumbManual } from '~/servicos/breadcrumb-services';
import AtribuicaoEsporadicaServico from '~/servicos/Paginas/AtribuicaoEsporadica';

// Componentes SGP
import { Cabecalho } from '~/componentes-sgp';

// Componentes
import {
  Card,
  ButtonGroup,
  Grid,
  Localizador,
  CampoData,
  momentSchema,
  Loader,
} from '~/componentes';
import DreDropDown from '../componentes/DreDropDown';
import UeDropDown from '../componentes/UeDropDown';
import AnoLetivoDropDown from '../componentes/AnoLetivoDropDown';

// Styles
import { Row } from './styles';

// Funçoes
import { validaSeObjetoEhNuloOuVazio } from '~/utils/funcoes/gerais';

function AtribuicaoEsporadicaForm({ match }) {
  const dispatch = useDispatch();
  const carregando = useSelector(store => store.loader.loaderSecao);
  const permissoesTela = useSelector(store => store.usuario.permissoes);
  const filtroListagem = useSelector(
    store => store.atribuicaoEsporadica.filtro
  );
  const [dreId, setDreId] = useState('');
  const [novoRegistro, setNovoRegistro] = useState(true);
  const [modoEdicao, setModoEdicao] = useState(false);
  const [valoresIniciais, setValoresIniciais] = useState({
    professorRf: '',
    professorNome: '',
    dataInicio: '',
    dataFim: '',
  });

  const validacoes = () => {
    return Yup.object({
      dataInicio: momentSchema.required('Campo obrigatório'),
      dataFim: momentSchema.required('Campo obrigatório'),
      professorRf: Yup.number()
        .typeError('Informar um número inteiro')
        .required('Campo obrigatório'),
    });
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

  const onClickBotaoPrincipal = form => {
    validaAntesDoSubmit(form);
  };

  const onSubmitFormulario = async valores => {
    try {
      dispatch(setLoaderSecao(true));
      const cadastrado = await AtribuicaoEsporadicaServico.salvarAtribuicaoEsporadica(
        {
          ...filtroListagem,
          ...valores,
        }
      );
      if (cadastrado && cadastrado.status === 200) {
        dispatch(setLoaderSecao(false));
        sucesso('Atribuição esporádica salva com sucesso.');
        history.push('/gestao/atribuicao-esporadica');
      }
    } catch (err) {
      if (err) {
        dispatch(setLoaderSecao(false));
        erro(err.response.data.mensagens[0]);
      }
    }
  };

  const onClickVoltar = () => history.push('/gestao/atribuicao-esporadica');

  const onClickCancelar = async form => {
    if (!modoEdicao) return;
    const confirmou = await confirmar(
      'Atenção',
      'Você não salvou as informações preenchidas.',
      'Deseja realmente cancelar as alterações?'
    );
    if (confirmou) {
      form.resetForm();
      setModoEdicao(false);
    }
  };

  const buscarPorId = async id => {
    try {
      dispatch(setLoaderSecao(true));
      const registro = await AtribuicaoEsporadicaServico.buscarAtribuicaoEsporadica(
        id
      );
      if (registro && registro.data) {
        setValoresIniciais({
          ...registro.data,
          dataInicio: window.moment(registro.data.dataInicio),
          dataFim: window.moment(registro.data.dataFim),
        });
        dispatch(setLoaderSecao(false));
      }
    } catch (err) {
      dispatch(setLoaderSecao(false));
      erros(err);
    }
  };

  const validaFormulario = valores => {
    if (validaSeObjetoEhNuloOuVazio(valores)) return;
    if (!modoEdicao) {
      setModoEdicao(true);
    }
  };

  useEffect(() => {
    if (match && match.params && match.params.id) {
      setNovoRegistro(false);
      setBreadcrumbManual(
        match.url,
        'Atribuição',
        '/gestao/atribuicao-esporadica'
      );
      buscarPorId(match.params.id);
    }
  }, []);

  return (
    <>
      <Cabecalho pagina="Atribuição" />
      <Loader loading={carregando}>
        <Card>
          <Formik
            enableReinitialize
            initialValues={valoresIniciais}
            validationSchema={validacoes}
            onSubmit={valores => onSubmitFormulario(valores)}
            validate={valores => validaFormulario(valores)}
            validateOnBlur
            validateOnChange
          >
            {form => (
              <Form>
                <ButtonGroup
                  form={form}
                  permissoesTela={
                    permissoesTela[RotasDto.ATRIBUICAO_ESPORADICA_LISTA]
                  }
                  novoRegistro={novoRegistro}
                  labelBotaoPrincipal="Cadastrar"
                  onClickBotaoPrincipal={() => onClickBotaoPrincipal(form)}
                  onClickCancelar={formulario => onClickCancelar(formulario)}
                  onClickVoltar={() => onClickVoltar()}
                  modoEdicao={modoEdicao}
                />
                <Row className="row">
                  <Grid cols={2}>
                    <AnoLetivoDropDown
                      label="Ano Letivo"
                      form={form}
                      name="anoLetivo"
                      onChange={valor => null}
                    />
                  </Grid>
                  <Grid cols={5}>
                    <DreDropDown
                      label="Diretoria Regional de Educação (DRE)"
                      form={form}
                      onChange={valor => setDreId(valor)}
                    />
                  </Grid>
                  <Grid cols={5}>
                    <UeDropDown
                      label="Unidade Escolar (UE)"
                      dreId={dreId}
                      form={form}
                      onChange={valor => null}
                    />
                  </Grid>
                </Row>
                <Row className="row">
                  <Grid cols={8}>
                    <Row className="row">
                      <Localizador
                        dreId={form.values.dreId}
                        anoLetivo={form.values.anoLetivo}
                        showLabel
                        form={form}
                        onChange={() => null}
                      />
                    </Row>
                  </Grid>
                  <Grid cols={2}>
                    <CampoData
                      placeholder="Selecione"
                      label="Data Início"
                      form={form}
                      name="dataInicio"
                      formatoData="DD/MM/YYYY"
                    />
                  </Grid>
                  <Grid cols={2}>
                    <CampoData
                      placeholder="Selecione"
                      label="Data Fim"
                      form={form}
                      name="dataFim"
                      formatoData="DD/MM/YYYY"
                    />
                  </Grid>
                </Row>
              </Form>
            )}
          </Formik>
        </Card>
      </Loader>
    </>
  );
}

AtribuicaoEsporadicaForm.propTypes = {
  match: PropTypes.oneOfType([
    PropTypes.objectOf(PropTypes.object),
    PropTypes.any,
  ]),
};

AtribuicaoEsporadicaForm.defaultProps = {
  match: {},
};

export default AtribuicaoEsporadicaForm;

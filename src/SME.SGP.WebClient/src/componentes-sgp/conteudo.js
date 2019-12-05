import React, { useEffect, useState } from 'react';
import { Switch, Route } from 'react-router-dom';

import { useSelector, useDispatch } from 'react-redux';
import { Modal, Row } from 'antd';
import styled from 'styled-components';
import shortid from 'shortid';
import { alertaFechar } from '../redux/modulos/alertas/actions';
import Button from '../componentes/button';
import { Colors } from '../componentes/colors';
import BreadcrumbSgp from './breadcrumb-sgp';
import Alert from '~/componentes/alert';
import Grid from '~/componentes/grid';
import rotasArray from '~/rotas/rotas';
import RotaAutenticadaEstruturada from '../rotas/rotaAutenticadaDesestruturada';

const ContainerModal = styled.div`
  .ant-modal-footer {
    border-top: 0px !important;
  }
`;

const ContainerBotoes = styled.div`
  display: flex;
  justify-content: flex-end;
`;
const Conteudo = props => {
  const NavegacaoStore = useSelector(store => store.navegacao);
  const [retraido, setRetraido] = useState(false);
  const dispatch = useDispatch();

  useEffect(() => {
    setRetraido(NavegacaoStore.retraido);
  }, [NavegacaoStore.retraido]);

  const confirmacao = useSelector(state => state.alertas.confirmacao);

  useEffect(() => {
    setRetraido(NavegacaoStore.retraido);
  }, [NavegacaoStore.retraido]);

  const fecharConfirmacao = resultado => {
    confirmacao.resolve(resultado);
    dispatch(alertaFechar());
  };
  const alertas = useSelector(state => state.alertas);

  return (
    <div style={{ marginLeft: retraido ? '115px' : '250px' }}>
      <BreadcrumbSgp />
      <div className="row h-100">
        <main role="main" className="col-md-12 col-lg-12 col-sm-12 col-xl-12">
          <ContainerModal>
            <Modal
              title={confirmacao.titulo}
              visible={confirmacao.visivel}
              onOk={() => fecharConfirmacao(true)}
              onCancel={() => fecharConfirmacao(false)}
              footer={[
                <ContainerBotoes key={shortid.generate()}>
                  <Button
                    key={shortid.generate()}
                    onClick={() => fecharConfirmacao(true)}
                    S
                    label={confirmacao.textoOk}
                    color={Colors.Azul}
                    border
                  />
                  <Button
                    key={shortid.generate()}
                    onClick={() => fecharConfirmacao(false)}
                    label={confirmacao.textoCancelar}
                    type="primary"
                    color={Colors.Azul}
                  />
                </ContainerBotoes>,
              ]}
            >
              {confirmacao.texto && Array.isArray(confirmacao.texto)
                ? confirmacao.texto.map((item, i) => (
                    <div key={item + '-' + i}>{item}</div>
                  ))
                : confirmacao.texto}
              {confirmacao.texto ? <br /> : ''}
              <b>{confirmacao.textoNegrito}</b>
            </Modal>
          </ContainerModal>
          <div className="card-body m-r-0 m-l-0 p-l-0 p-r-0 m-t-0">
            {alertas.alertas.map(alerta => (
              <Row key={shortid.generate()}>
                <Grid cols={12}>
                  <Alert alerta={alerta} key={alerta.id} closable />
                </Grid>
              </Row>
            ))}
            <Row
              key={shortid.generate()}
              hidden={!NavegacaoStore.somenteConsulta}
            >
              <Grid cols={12}>
                <Alert
                  alerta={{
                    tipo: 'warning',
                    id: 'AlertaPrincipal',
                    mensagem:
                      'Você tem apenas permissão de consulta nesta tela.',
                    estiloTitulo: { fontSize: '18px' },
                  }}
                />
              </Grid>
            </Row>
          </div>
        </main>
      </div>
      <Switch>
        {rotasArray.map(rota => (
          <RotaAutenticadaEstruturada
            path={rota.path}
            component={rota.component}
            exact={rota.exact}
          />
        ))}
      </Switch>
    </div>
  );
};
export default Conteudo;

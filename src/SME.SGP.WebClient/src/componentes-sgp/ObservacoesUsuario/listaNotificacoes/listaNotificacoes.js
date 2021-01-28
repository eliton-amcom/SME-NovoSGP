import React, { useEffect, useState } from 'react';
import PropTypes from 'prop-types';

import { Container } from './listaNotificacoes.css';

const ListaNotificacoes = ({ obs }) => {
  const [usuariosNotificacao, setUsuariosNotificacao] = useState();

  useEffect(() => {
    if (!usuariosNotificacao && obs.usuariosNotificacao) {
      const usuariosNotificacaoConcatenado = obs.usuariosNotificacao
        .map(usuario => usuario.nome)
        .join(', ');
      setUsuariosNotificacao(usuariosNotificacaoConcatenado);
    }
  }, [usuariosNotificacao, obs.usuariosNotificacao]);

  return (
    <>
      {obs.usuariosNotificacao && (
        <Container
          temLinhaAlteradoPor={obs?.auditoria?.alteradoPor}
          listagemDiario={obs?.listagemDiario}
        >
          <span>Usuários notificados: {usuariosNotificacao}</span>
        </Container>
      )}
    </>
  );
};

ListaNotificacoes.propTypes = {
  obs: PropTypes.oneOfType([PropTypes.object]),
};

ListaNotificacoes.defaultProps = {
  obs: {},
};

export default ListaNotificacoes;

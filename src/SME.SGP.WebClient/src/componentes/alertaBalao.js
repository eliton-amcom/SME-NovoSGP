import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import { Base } from '~/componentes/colors';

const AlertaBalao = props => {
  const { maxWidth, texto, marginTop, background, color, mostrarAlerta } = props;


  const SetaNotificacao = styled.div`
  padding-left: ${((props.maxWidth/2)-5)+'px'};
  position: relative;
  display: block;
  top: -8px;
  .seta{
    height: 14px;
    width:14px;
    border-top: 2px ${props.color} solid;
    border-left: 2px ${props.color} solid;
    transform: translateX(-50%) rotate(45deg);
    left: 50%;
    background: ${props.background};
    display:flex;
    text-align: center;
    justify-content: center;
  }
  `;
  const Notificacao = styled.div`
    max-width: ${props.maxWidth+'px'};
    height: auto;
    border-radius: 4px;
    border: 2px ${props.color} solid;
    color: ${props.color};
    font-size: 14px;
    font-weight: bold;
    margin-top: ${props.marginTop+'px'};
    background: ${props.background};

    .texto{
      padding: 14px 5px;
      display:flex;
      text-align: center;
      justify-content: center;
    }
  `;

  return (
    <Notificacao hidden={!props.mostrarAlerta}>
      <SetaNotificacao className="text-center">
        <div className="seta" />
      </SetaNotificacao>
      <span className="texto">{props.texto}</span>
    </Notificacao>

  )
}

AlertaBalao.propTypes = {
  maxWidth: PropTypes.number,
  texto: PropTypes.string,
  marginTop: PropTypes.number,
  background: PropTypes.string,
  color: PropTypes.string,
  mostrarAlerta: PropTypes.bool
};

AlertaBalao.defaultProps = {
  maxWidth: 100,
  texto: '',
  marginTop: 0,
  background: Base.Branco,
  color: Base.VermelhoAlerta,
  mostrarAlerta: true
}

export default AlertaBalao;

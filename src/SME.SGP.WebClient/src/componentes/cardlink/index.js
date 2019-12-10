import React, { useState, useEffect } from 'react';
import styled from 'styled-components';
import { Link } from 'react-router-dom';
import { Base } from '../colors';
import CardBody from '../cardBody';
import Icon from '../icon';
import PropTypes from 'prop-types';
import LinkRouter from '../linkRouter';

const CardLink = props => {
  const {
    icone,
    pack,
    label,
    url,
    disabled,
    alt,
    cols,
    className,
    classHidden,
    iconSize,
    minHeight,
  } = props;

  const color = disabled ? Base.CinzaDesabilitado : Base.Roxo;
  const colorActive = disabled ? Base.CinzaDesabilitado : Base.Branco;
  const backgroundActive = disabled ? Base.Branco : Base.Roxo;
  const background = Base.Branco;

  const CardLine = styled.div`
    border-bottom: 5.8px solid ${color} !important;
    color: ${color} !important;
    background: ${background} !important;

    &:hover {
      color: ${colorActive} !important;
      background: ${backgroundActive} !important;

      h2,
      i {
        color: ${colorActive};
      }
    }

    &:not(:hover) {
      color: ${color} !important;
      background: ${background} !important;

      h2,
      i {
        color: ${color};
      }
    }
  `;

  const Label = styled.h2`
    font-size: 16px;
    margin-top: 20px;
    font-family: Roboto;
    font-size: 16px;
    font-weight: 500;
  `;

  const Div = styled.div`
    min-height: ${minHeight};
    height: 100%;
    .card-body {
      min-height: ${minHeight} !important;
    }
    .altura-100 {
      height: 100%;
    }
  `;

  const getCols = () => {
    let retorno = 'col-lg-3';

    if (!cols) return retorno;

    if (cols[0]) retorno = `col-lg-${cols[0]}`;

    if (cols[1]) retorno += ` col-md-${cols[1]}`;

    if (cols[2]) retorno += ` col-sm-${cols[2]}`;

    if (cols[3]) retorno += ` col-xs-${cols[3]}`;

    return retorno;
  };

  return (
    <Div className={`${getCols()} px-2 ${classHidden}`}>
      <LinkRouter
        className="altura-100"
        to={url}
        alt={disabled ? '' : alt}
        isactive={!disabled}
      >
        <Div className={className + ' altura-100'}>
          <CardLine className={`card altura-100`}>
            <CardBody className={`text-center altura-100`}>
              <div className={`col-md-12 p-0 text-center`}>
                <Icon
                  icon={icone}
                  pack={pack}
                  color={color}
                  iconSize={iconSize}
                />
                <Label>{label}</Label>
              </div>
            </CardBody>
          </CardLine>
        </Div>
      </LinkRouter>
    </Div>
  );
};

CardLink.defaultProps = {
  icone: 'fa-check',
  pack: 'fa fa-5x',
  label: 'Insira a label',
  url: '/',
  disabled: false,
  alt: '',
  cols: [3],
  className: '',
  iconSize: '15px',
  classHidden: '',
  minHeight: '20px',
};

CardLink.propTypes = {
  icone: PropTypes.string,
  pack: PropTypes.string,
  label: PropTypes.string,
  url: PropTypes.string,
  disabled: PropTypes.bool,
  alt: PropTypes.string,
  cols: PropTypes.array,
  className: PropTypes.string,
  iconSize: PropTypes.string,
  classHidden: PropTypes.string,
  minHeight: PropTypes.string,
};

export default CardLink;

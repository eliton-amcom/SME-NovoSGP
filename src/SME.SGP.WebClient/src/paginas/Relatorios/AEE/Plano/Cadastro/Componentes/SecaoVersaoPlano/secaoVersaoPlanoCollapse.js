import * as moment from 'moment';
import PropTypes from 'prop-types';
import React from 'react';
import CardCollapse from '~/componentes/cardCollapse';

const SecaoVersaoPlanoCollapse = props => {
  const { versoes } = props;
  return (
    <>
      <div className="col-md-12 mb-2">
        <strong>Planos anteriores para consulta</strong>
      </div>
      {versoes.map(plano => (
        // colocar em um outro arquivo
        <CardCollapse
          key={`secao-informacoes-plano-${plano.id}-collapse-key`}
          titulo={`Informações do Plano - v${plano.numero} (${moment(
            plano.criadoEm
          ).format('DD/MM/YYYY')})`}
          indice={`secao-informacoes-plano-${plano.id}-collapse-indice`}
          alt={`secao-informacoes-plano-${plano.id}-alt`}
        >
          <></>
        </CardCollapse>
      ))}
    </>
  );
};

SecaoVersaoPlanoCollapse.propTypes = {
  versoes: PropTypes.oneOfType([PropTypes.object]),
};

SecaoVersaoPlanoCollapse.defaultProps = {
  versoes: [],
};

export default SecaoVersaoPlanoCollapse;

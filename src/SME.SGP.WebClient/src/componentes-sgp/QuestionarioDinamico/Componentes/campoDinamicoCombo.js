import PropTypes from 'prop-types';
import React from 'react';
import SelectComponent from '~/componentes/select';

const CampoDinamicoCombo = props => {
  const { questaoAtual, form, label, desabilitado, onChange } = props;

  const lista = questaoAtual?.opcaoResposta.map(item => {
    return { label: item.nome, value: item.id };
  });

  return (
    <>
      <div className="col-sm-12 col-md-12 col-lg-6 col-xl-6 mb-3">
        {label}
        <SelectComponent
          id={String(questaoAtual.id)}
          name={String(questaoAtual.id)}
          form={form}
          lista={lista}
          valueOption="value"
          valueText="label"
          disabled={desabilitado}
          onChange={valorAtualSelecionado => {
            onChange(valorAtualSelecionado);
          }}
        />
      </div>
    </>
  );
};

CampoDinamicoCombo.propTypes = {
  questaoAtual: PropTypes.oneOfType([PropTypes.any]),
  form: PropTypes.oneOfType([PropTypes.any]),
  label: PropTypes.oneOfType([PropTypes.any]),
  desabilitado: PropTypes.bool,
  onChange: PropTypes.oneOfType([PropTypes.any]),
};

CampoDinamicoCombo.defaultProps = {
  questaoAtual: null,
  form: null,
  label: '',
  desabilitado: false,
  onChange: () => {},
};

export default CampoDinamicoCombo;

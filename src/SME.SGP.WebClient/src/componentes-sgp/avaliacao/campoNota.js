import PropTypes from 'prop-types';
import React, { useEffect, useState, useCallback } from 'react';
import CampoNumero from '~/componentes/campoNumero';
import { erros } from '~/servicos/alertas';
import api from '~/servicos/api';

const CampoNota = props => {
  const { nota, onChangeNotaConceito, desabilitarCampo } = props;

  const [notaValorAtual, setNotaValorAtual] = useState();
  const [notaAlterada, setNotaAlterada] = useState(false);

  const validaSeTeveAlteracao = useCallback((notaOriginal, notaNova) => {
    if (
      notaOriginal != undefined &&
      notaOriginal != null &&
      notaOriginal.trim() !== ''
    ) {
      setNotaAlterada(
        Number(notaNova).toFixed(1) !== Number(notaOriginal).toFixed(1)
      );
    }
  }, []);

  useEffect(() => {
    setNotaValorAtual(nota.notaConceito);
    validaSeTeveAlteracao(nota.notaOriginal, nota.notaConceito);
  }, [nota.notaConceito, nota.notaOriginal, validaSeTeveAlteracao]);

  const setarValorNovo = async valorNovo => {
    setNotaValorAtual(valorNovo);
    const notaArredondada = await api
      .get(
        `v1/avaliacoes/${nota.atividadeAvaliativaId}/notas/${Number(
          valorNovo
        )}/arredondamento`
      )
      .catch(e => erros(e));
    setNotaValorAtual(notaArredondada.data);
    onChangeNotaConceito(notaArredondada.data);
    validaSeTeveAlteracao(nota.notaOriginal, notaArredondada.data);
  };

  return (
    <CampoNumero
      onBlur={valorNovo => setarValorNovo(valorNovo.target.value)}
      value={notaValorAtual}
      min={0}
      max={10}
      step={0.5}
      placeholder="Nota"
      classNameCampo={`${nota.ausente ? 'aluno-ausente-notas' : ''}`}
      desabilitado={desabilitarCampo || !nota.podeEditar}
      className={`${notaAlterada ? 'border-registro-alterado' : ''}`}
    />
  );
};

CampoNota.defaultProps = {
  onChangeNotaConceito: PropTypes.func,
};

CampoNota.propTypes = {
  onChangeNotaConceito: () => {},
};

export default CampoNota;

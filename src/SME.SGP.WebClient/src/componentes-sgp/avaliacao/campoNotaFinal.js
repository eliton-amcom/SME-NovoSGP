import { Tooltip } from 'antd';
import PropTypes from 'prop-types';
import React, { useCallback, useEffect, useState } from 'react';
import { useSelector } from 'react-redux';
import CampoNumero from '~/componentes/campoNumero';
import { erros } from '~/servicos/alertas';
import api from '~/servicos/api';

const CampoNotaFinal = props => {
  const {
    montaNotaFinal,
    onChangeNotaConceitoFinal,
    desabilitarCampo,
    podeEditar,
    periodoFim,
    mediaAprovacaoBimestre,
    label,
    podeLancarNotaFinal,
  } = props;

  const modoEdicaoGeral = useSelector(
    store => store.notasConceitos.modoEdicaoGeral
  );

  const [notaBimestre, setNotaBimestre] = useState();
  const [notaValorAtual, setNotaValorAtual] = useState(0);
  const [notaAlterada, setNotaAlterada] = useState(false);
  const [abaixoDaMedia, setAbaixoDaMedia] = useState(false);

  const validaSeTeveAlteracao = useCallback(
    notaArredondada => {
      if (
        notaBimestre.notaOriginal != undefined &&
        notaBimestre.notaOriginal != null &&
        notaBimestre.notaOriginal !== ''
      ) {
        const alterada =
          Number(notaArredondada).toFixed(1) !==
          Number(notaBimestre.notaOriginal).toFixed(1);
        notaBimestre.notaAlterada = alterada;
        setNotaAlterada(alterada);
      }
    },
    [notaBimestre]
  );

  const validaSeEstaAbaixoDaMedia = useCallback(
    valorAtual => {
      valorAtual = removerCaracteresInvalidos(String(valorAtual));
      if (valorAtual && valorAtual < mediaAprovacaoBimestre) {
        notaBimestre.abaixoDaMedia = true;
        setAbaixoDaMedia(true);
      } else {
        notaBimestre.abaixoDaMedia = false;
        setAbaixoDaMedia(false);
      }
    },
    [mediaAprovacaoBimestre, notaBimestre]
  );

  useEffect(() => {
    setNotaBimestre(montaNotaFinal());
  }, [montaNotaFinal]);

  useEffect(() => {
    if (notaBimestre) {
      validaSeEstaAbaixoDaMedia(notaBimestre.notaConceito);
      validaSeTeveAlteracao(String(notaBimestre.notaConceito));
      setNotaValorAtual(notaBimestre.notaConceito);
    }
  }, [notaBimestre, validaSeTeveAlteracao, validaSeEstaAbaixoDaMedia]);

  const removerCaracteresInvalidos = texto => {
    return texto.replace(/[^0-9,.]+/g, '');
  };

  const editouCampo = (notaOriginal, notaNova) => {
    notaOriginal = removerCaracteresInvalidos(String(notaOriginal));
    notaNova = removerCaracteresInvalidos(String(notaNova));
    if (notaOriginal === '' && notaNova === '') {
      return false;
    }
    return notaOriginal !== notaNova;
  };

  const setarValorNovo = async valorNovo => {
    if (!desabilitarCampo && podeEditar) {
      setNotaValorAtual(valorNovo);
      const retorno = await api
        .get(
          `v1/avaliacoes/notas/${Number(
            valorNovo
          )}/arredondamento?data=${periodoFim}`
        )
        .catch(e => erros(e));

      let notaArredondada = valorNovo;
      if (retorno && retorno.data) {
        notaArredondada = retorno.data;
      }

      validaSeEstaAbaixoDaMedia(notaArredondada);
      validaSeTeveAlteracao(notaArredondada);
      onChangeNotaConceitoFinal(notaBimestre, notaArredondada);
      setNotaValorAtual(notaArredondada);
    }
  };

  const valorInvalido = valorNovo => {
    const regexValorInvalido = /[^0-9,.]+/g;
    return regexValorInvalido.test(String(valorNovo));
  };

  return (
    <Tooltip placement="bottom" title={abaixoDaMedia ? 'Abaixo da Média' : ''}>
      <div>
        <CampoNumero
          label={label ? label : ''}
          onChange={valorNovo => {
            const invalido = valorInvalido(valorNovo);
            if (!invalido && editouCampo(notaValorAtual, valorNovo)) {
              setarValorNovo(valorNovo);
            }
          }}
          value={notaValorAtual}
          min={0}
          max={10}
          step={0.5}
          placeholder="Nota Final"
          disabled={
            desabilitarCampo ||
            !podeEditar ||
            !podeLancarNotaFinal ||
            modoEdicaoGeral
          }
          className={`tamanho-conceito-final ${
            abaixoDaMedia
              ? 'border-abaixo-media'
              : notaAlterada
              ? 'border-registro-alterado'
              : ''
          } `}
        />
      </div>
    </Tooltip>
  );
};

CampoNotaFinal.defaultProps = {
  onChangeNotaConceitoFinal: PropTypes.func,
  montaNotaFinal: PropTypes.func,
  desabilitarCampo: PropTypes.bool,
  podeEditar: PropTypes.bool,
  periodoFim: PropTypes.string,
  mediaAprovacaoBimestre: PropTypes.number,
};

CampoNotaFinal.propTypes = {
  onChangeNotaConceitoFinal: () => {},
  montaNotaFinal: () => {},
  desabilitarCampo: false,
  podeEditar: false,
  periodoFim: '',
  mediaAprovacaoBimestre: 0,
};

export default CampoNotaFinal;

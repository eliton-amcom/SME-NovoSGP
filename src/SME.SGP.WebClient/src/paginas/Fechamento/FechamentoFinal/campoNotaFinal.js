import { Tooltip } from 'antd';
import PropTypes from 'prop-types';
import React, { useCallback, useEffect, useState } from 'react';
import CampoNumero from '~/componentes/campoNumero';
import { erros } from '~/servicos/alertas';
import api from '~/servicos/api';
import { Loader } from '~/componentes';

const CampoNotaFinal = props => {
  const {
    montaNotaFinal,
    onChangeNotaConceitoFinal,
    desabilitarCampo,
    podeEditar,
    eventoData,
    mediaAprovacaoBimestre,
    label,
  } = props;

  const [notaBimestre, setNotaBimestre] = useState();
  const [notaValorAtual, setNotaValorAtual] = useState();
  const [notaAlterada, setNotaAlterada] = useState(false);
  const [abaixoDaMedia, setAbaixoDaMedia] = useState(false);
  const [valorIncremento] = useState(0.5);
  const [
    carregandoValorArredondamento,
    setCarregandoValorArredondamento,
  ] = useState(false);

  const validaSeTeveAlteracao = useCallback(
    notaArredondada => {
      if (
        notaBimestre.notaConceitoAtual != undefined &&
        notaBimestre.notaConceitoAtual != null &&
        notaBimestre.notaConceitoAtual.trim() !== ''
      ) {
        const alterada =
          Number(notaArredondada).toFixed(1) !==
          Number(notaBimestre.notaConceitoAtual).toFixed(1);
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
      const notaConceitoParseada = String(notaBimestre.notaConceito);
      const notaConceitoAlterada = notaConceitoParseada.replace(',', '.');
      const nota = Number(notaConceitoAlterada);
      validaSeEstaAbaixoDaMedia(nota);
      validaSeTeveAlteracao(String(nota));
      setNotaValorAtual(nota);
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
      const resto = valorNovo % valorIncremento;
      let notaArredondada = valorNovo;
      if (resto > 0.0) {
        setCarregandoValorArredondamento(true);
        const retorno = await api
          .get(
            `v1/avaliacoes/notas/${Number(
              valorNovo
            )}/arredondamento?data=${eventoData}`
          )
          .catch(e => erros(e));

        if (retorno && retorno.data) {
          notaArredondada = retorno.data;
        }
        setCarregandoValorArredondamento(false);
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
        <Loader loading={carregandoValorArredondamento} tip="">
          <CampoNumero
            label={label || ''}
            onChange={valorNovo => {
              const invalido = valorInvalido(valorNovo);
              if (!invalido && editouCampo(notaValorAtual, valorNovo)) {
                setarValorNovo(valorNovo);
              }
            }}
            value={notaValorAtual ? notaValorAtual : ''}
            min={0}
            max={10}
            step={valorIncremento}
            disabled={desabilitarCampo || !podeEditar}
            className={`tamanho-conceito-final ${
              abaixoDaMedia
                ? 'border-abaixo-media'
                : notaAlterada
                ? 'border-registro-alterado'
                : ''
            } `}
          />
        </Loader>
      </div>
    </Tooltip>
  );
};

CampoNotaFinal.defaultProps = {
  onChangeNotaConceitoFinal: PropTypes.func,
  montaNotaFinal: PropTypes.func,
  desabilitarCampo: PropTypes.bool,
  podeEditar: PropTypes.bool,
  eventoData: PropTypes.string,
  mediaAprovacaoBimestre: PropTypes.number,
};

CampoNotaFinal.propTypes = {
  onChangeNotaConceitoFinal: () => {},
  montaNotaFinal: () => {},
  desabilitarCampo: false,
  podeEditar: false,
  eventoData: '',
  mediaAprovacaoBimestre: 0,
};

export default CampoNotaFinal;

import PropTypes from 'prop-types';
import React, { useCallback, useEffect, useState } from 'react';
import { Label } from '~/componentes';
import { erro, erros } from '~/servicos/alertas';
import { removerNumeros } from '~/utils/funcoes/gerais';
import InputCodigo from './componentes/InputCodigo';
import InputNome from './componentes/InputNome';
import ServicoLocalizadorFuncionario from './services/ServicoLocalizadorFuncionario';

const LocalizadorFuncionario = props => {
  const {
    onChange,
    desabilitado,
    codigoUe,
    codigoDre,
    codigoTurma,
    exibirCampoRf,
    valorInicial,
    placeholder,
  } = props;

  const [dataSource, setDataSource] = useState([]);
  const [funcionarioSelecionado, setFuncionarioSelecionado] = useState({});
  const [desabilitarCampo, setDesabilitarCampo] = useState({
    codigoRF: false,
    nomeServidor: false,
  });
  const [timeoutBuscarPorCodigoNome, setTimeoutBuscarPorCodigoNome] = useState(
    ''
  );
  const [exibirLoader, setExibirLoader] = useState(false);

  useEffect(() => {
    setFuncionarioSelecionado({
      nomeServidor: '',
      codigoRF: '',
    });
    setDataSource([]);
  }, [codigoDre, codigoUe, codigoTurma]);

  const limparDados = useCallback(() => {
    onChange();
    setDataSource([]);
    setFuncionarioSelecionado({
      nomeServidor: '',
      codigoRF: '',
    });
    setTimeout(() => {
      setDesabilitarCampo(() => ({
        codigoRF: false,
        nomeServidor: false,
      }));
    }, 200);
  }, [onChange]);

  const onChangeNome = async valor => {
    valor = removerNumeros(valor);
    if (valor.length === 0) {
      limparDados();
      return;
    }

    if (valor.length < 3) return;

    const params = {
      nome: valor,
    };

    if (codigoDre) {
      params.codigoDre = codigoDre;
    }
    if (codigoUe) {
      params.codigoUe = codigoUe;
    }
    if (codigoTurma) {
      params.codigoTurma = codigoTurma;
    }
    setExibirLoader(true);
    const retorno = await ServicoLocalizadorFuncionario.buscarPorNome(params)
      .catch(e => {
        erros(e);
        limparDados();
      })
      .finally(() => setExibirLoader(false));

    if (retorno?.data?.items?.length > 0) {
      setDataSource([]);
      setDataSource(
        retorno.data.items.map(funcionario => ({
          codigoRF: funcionario.codigoRf,
          nomeServidor: funcionario.nomeServidor,
        }))
      );
    } else {
      setDataSource([]);
      setDesabilitarCampo(() => ({
        codigoRF: false,
        nomeServidor: false,
      }));
      setFuncionarioSelecionado({
        codigoRF: '',
        nomeServidor: valor,
      });
    }
  };

  const onBuscarPorCodigo = useCallback(
    async valor => {
      if (!valor) {
        limparDados();
        return;
      }
      const params = {
        codigoRF: valor,
      };

      if (codigoDre) {
        params.codigoDre = codigoDre;
      }
      if (codigoUe) {
        params.codigoUe = codigoUe;
      }
      if (codigoTurma) {
        params.codigoTurma = codigoTurma;
      }

      setExibirLoader(true);
      const retorno = await ServicoLocalizadorFuncionario.buscarPorCodigo(
        params
      )
        .catch(e => {
          if (e?.response?.status === 601) {
            erro('Funcionário não encontrado no EOL');
          } else {
            erros(e);
          }
          limparDados();
        })
        .finally(() => setExibirLoader(false));

      if (retorno?.data?.items?.length > 0) {
        const { codigoRf, nomeServidor } = retorno.data.items[0];

        const funcionarioRetorno = {
          codigoRF: codigoRf,
          nomeServidor,
        };
        setDataSource(
          retorno.data.items.map(funcionario => ({
            codigoRF: funcionario.codigoRf,
            nomeServidor: funcionario.nomeServidor,
          }))
        );
        setFuncionarioSelecionado(funcionarioRetorno);
        setDesabilitarCampo(estado => ({
          ...estado,
          nomeServidor: true,
        }));
        onChange(funcionarioRetorno);
      } else {
        setDataSource([]);
        setDesabilitarCampo(() => ({
          codigoRF: false,
          nomeServidor: false,
        }));
        setFuncionarioSelecionado({
          codigoRF: valor,
          nomeServidor: '',
        });
      }
    },
    [codigoDre, codigoTurma, codigoUe, limparDados, onChange]
  );

  const validaAntesBuscarPorCodigo = useCallback(
    valor => {
      if (timeoutBuscarPorCodigoNome) {
        clearTimeout(timeoutBuscarPorCodigoNome);
      }

      const timeout = setTimeout(() => {
        onBuscarPorCodigo(valor);
      }, 500);

      setTimeoutBuscarPorCodigoNome(timeout);
    },
    [onBuscarPorCodigo, timeoutBuscarPorCodigoNome]
  );

  const validaAntesBuscarPorNome = valor => {
    if (timeoutBuscarPorCodigoNome) {
      clearTimeout(timeoutBuscarPorCodigoNome);
    }

    const timeout = setTimeout(() => {
      onChangeNome(valor);
    }, 500);

    setTimeoutBuscarPorCodigoNome(timeout);
  };

  const onChangeCodigo = valor => {
    if (valor.length === 0) {
      limparDados();
    }
  };

  const onSelectFuncionario = objeto => {
    const funcionario = {
      codigoRF: objeto.key,
      nomeServidor: objeto.props.value,
    };
    setFuncionarioSelecionado(funcionario);
    onChange(funcionario);
    setDesabilitarCampo(estado => ({
      ...estado,
      codigoRF: true,
    }));
  };

  useEffect(() => {
    if (
      valorInicial &&
      !funcionarioSelecionado?.codigoRF &&
      !dataSource?.length
    ) {
      validaAntesBuscarPorCodigo(valorInicial);
    }
  }, [
    valorInicial,
    dataSource,
    funcionarioSelecionado,
    validaAntesBuscarPorCodigo,
  ]);

  return (
    <>
      <div
        className={`${
          exibirCampoRf ? 'col-sm-12 col-md-6 col-lg-8 col-xl-8' : 'col-md-12'
        } `}
      >
        <Label text="Nome" />
        <InputNome
          placeholder={placeholder}
          dataSource={dataSource}
          onSelect={onSelectFuncionario}
          onChange={validaAntesBuscarPorNome}
          funcionarioSelecionado={funcionarioSelecionado}
          name="nomeServidor"
          desabilitado={desabilitado || desabilitarCampo.nomeServidor}
          regexIgnore={/\d+/}
          exibirLoader={exibirLoader}
        />
      </div>
      {exibirCampoRf ? (
        <div className="col-sm-12 col-md-6 col-lg-4 col-xl-4">
          <Label text="RF" />
          <InputCodigo
            funcionarioSelecionado={funcionarioSelecionado}
            onSelect={validaAntesBuscarPorCodigo}
            onChange={onChangeCodigo}
            name="codigoRF"
            desabilitado={desabilitado || desabilitarCampo.codigoRF}
            exibirLoader={exibirLoader}
          />
        </div>
      ) : (
        ''
      )}
    </>
  );
};

LocalizadorFuncionario.propTypes = {
  onChange: PropTypes.func,
  desabilitado: PropTypes.bool,
  codigoUe: PropTypes.oneOfType([PropTypes.any]),
  codigoDre: PropTypes.oneOfType([PropTypes.any]),
  codigoTurma: PropTypes.oneOfType([PropTypes.any]),
  exibirCampoRf: PropTypes.bool,
  valorInicial: PropTypes.oneOfType([PropTypes.any]),
  placeholder: PropTypes.string,
};

LocalizadorFuncionario.defaultProps = {
  onChange: () => {},
  desabilitado: false,
  codigoUe: '',
  codigoDre: '',
  codigoTurma: '',
  exibirCampoRf: true,
  valorInicial: '',
  placeholder: '',
};

export default LocalizadorFuncionario;

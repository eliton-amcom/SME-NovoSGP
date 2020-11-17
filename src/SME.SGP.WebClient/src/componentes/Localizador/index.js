import React, { useState, useEffect, useCallback } from 'react';
import PropTypes from 'prop-types';

// Redux
import { useSelector } from 'react-redux';

// Componentes
import InputRF from './componentes/InputRF';
import InputNome from './componentes/InputNome';
import { Grid, Label } from '~/componentes';

// Services
import service from './services/LocalizadorService';
import { erros } from '~/servicos/alertas';

// Funções
import { validaSeObjetoEhNuloOuVazio } from '~/utils/funcoes/gerais';

// Utils
import RFNaoEncontradoExcecao from '~/utils/excecoes/RFNãoEncontradoExcecao';

function Localizador({
  onChange,
  showLabel,
  form,
  dreId,
  anoLetivo,
  desabilitado,
  incluirEmei,
  rfEdicao,
  buscarOutrosCargos,
}) {
  const usuario = useSelector(store => store.usuario);
  const [dataSource, setDataSource] = useState([]);
  const [pessoaSelecionada, setPessoaSelecionada] = useState({});
  const [desabilitarCampo, setDesabilitarCampo] = useState({
    rf: false,
    nome: false,
  });
  const { ehPerfilProfessor, rf } = usuario;

  const onChangeInput = async valor => {
    if (valor.length === 0) {
      setPessoaSelecionada({
        professorRf: '',
        professorNome: '',
        usuarioId: '',
      });
      setTimeout(() => {
        setDesabilitarCampo(() => ({
          rf: false,
          nome: false,
        }));
      }, 200);
    }

    if (valor.length < 2) return;
    const { data: dados } = await service.buscarAutocomplete({
      nome: valor,
      dreId,
      anoLetivo,
      incluirEmei,
    });

    if (dados && dados.length > 0) {
      setDataSource(
        dados.map(x => ({
          professorRf: x.codigoRF,
          professorNome: x.nome,
          usuarioId: x.usuarioId,
        }))
      );
    }
  };

  const onBuscarPorRF = useCallback(
    async ({ rf }) => {
      try {
        const { data: dados } = await service.buscarPorRf({
          rf,
          anoLetivo,
          buscarOutrosCargos,
        });
        if (!dados) throw new RFNaoEncontradoExcecao();

        setPessoaSelecionada({
          professorRf: dados.codigoRF,
          professorNome: dados.nome,
          usuarioId: dados.usuarioId,
        });

        setDesabilitarCampo(estado => ({
          ...estado,
          nome: true,
        }));
      } catch (error) {
        erros(error);
      }
    },
    [anoLetivo, buscarOutrosCargos]
  );

  const onChangeRF = valor => {
    if (valor.length === 0) {
      setPessoaSelecionada({
        professorRf: '',
        professorNome: '',
        usuarioId: '',
      });
      setDesabilitarCampo(estado => ({
        ...estado,
        nome: false,
      }));
    }
  };

  const onSelectPessoa = objeto => {
    setPessoaSelecionada({
      professorRf: parseInt(objeto.key, 10),
      professorNome: objeto.props.value,
    });
    setDesabilitarCampo(estado => ({
      ...estado,
      rf: true,
    }));
  };

  useEffect(() => {
    if (rfEdicao && !pessoaSelecionada?.professorRf) {
      onBuscarPorRF({ rf: rfEdicao });
    }
  }, [rfEdicao]);

  useEffect(() => {
    onChange(pessoaSelecionada);
    form.setValues({
      ...form.values,
      ...pessoaSelecionada,
    });
  }, [pessoaSelecionada]);

  useEffect(() => {
    if (validaSeObjetoEhNuloOuVazio(form.initialValues)) return;
    if (form.initialValues) {
      setPessoaSelecionada(form.initialValues);
    }
  }, [form.initialValues]);

  useEffect(() => {
    if (dreId && ehPerfilProfessor) {
      onBuscarPorRF({ rf });
    }
  }, [dreId, ehPerfilProfessor, rf, onBuscarPorRF]);

  useEffect(() => {
    const { values: valores } = form;
    if (valores && !valores.professorRf && pessoaSelecionada.professorRf) {
      setPessoaSelecionada({
        professorRf: '',
        professorNome: '',
        usuarioId: '',
      });
    }
  }, [form, form.values]);

  return (
    <>
      <Grid cols={4}>
        {showLabel && (
          <Label text="Registro Funcional (RF)" control="professorRf" />
        )}
        <InputRF
          pessoaSelecionada={pessoaSelecionada}
          onSelect={onBuscarPorRF}
          onChange={onChangeRF}
          name="professorRf"
          form={form}
          desabilitado={
            desabilitado || ehPerfilProfessor || desabilitarCampo.rf
          }
        />
      </Grid>
      <Grid className="pr-0" cols={8}>
        {showLabel && <Label text="Nome" control="professorNome" />}
        <InputNome
          dataSource={dataSource}
          onSelect={onSelectPessoa}
          onChange={onChangeInput}
          pessoaSelecionada={pessoaSelecionada}
          form={form}
          name="professorNome"
          desabilitado={
            desabilitado || ehPerfilProfessor || desabilitarCampo.nome
          }
        />
      </Grid>
    </>
  );
}

Localizador.propTypes = {
  onChange: () => {},
  form: PropTypes.oneOfType([
    PropTypes.objectOf(PropTypes.object),
    PropTypes.any,
  ]),
  showLabel: PropTypes.bool,
  dreId: PropTypes.string,
  anoLetivo: PropTypes.oneOfType([PropTypes.number, PropTypes.string]),
  desabilitado: PropTypes.bool,
  rfEdicao: PropTypes.string,
  buscarOutrosCargos: PropTypes.bool,
};

Localizador.defaultProps = {
  onChange: PropTypes.func,
  form: {},
  showLabel: false,
  dreId: null,
  anoLetivo: null,
  desabilitado: false,
  rfEdicao: '',
  buscarOutrosCargos: false,
};

export default Localizador;

import PropTypes from 'prop-types';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import Button from '~/componentes/button';
import { Colors } from '~/componentes/colors';
import { RotasDto } from '~/dtos';
import situacaoAEE from '~/dtos/situacaoAEE';
import {
  setExibirLoaderEncaminhamentoAEE,
  setExibirModalEncerramentoEncaminhamentoAEE,
} from '~/redux/modulos/encaminhamentoAEE/actions';
import { confirmar, erros, sucesso } from '~/servicos';
import history from '~/servicos/history';
import ServicoEncaminhamentoAEE from '~/servicos/Paginas/Relatorios/AEE/ServicoEncaminhamentoAEE';

const BotoesAcoesEncaminhamentoAEE = props => {
  const { match } = props;

  const dispatch = useDispatch();

  const encaminhamentoAEEEmEdicao = useSelector(
    store => store.encaminhamentoAEE.encaminhamentoAEEEmEdicao
  );

  const dadosEncaminhamento = useSelector(
    store => store.encaminhamentoAEE.dadosEncaminhamento
  );

  const onClickSalvar = async () => {
    const encaminhamentoId = match?.params?.id;
    let situacao = situacaoAEE.Rascunho;

    if (encaminhamentoId) {
      situacao = dadosEncaminhamento?.situacao;
    }

    let validarCamposObrigatorios = false;
    if (dadosEncaminhamento?.situacao === situacaoAEE.Encaminhado) {
      validarCamposObrigatorios = true;
    }

    ServicoEncaminhamentoAEE.salvarEncaminhamento(
      encaminhamentoId,
      situacao,
      validarCamposObrigatorios
    );
  };

  const onClickEnviar = async () => {
    const encaminhamentoId = match?.params?.id;
    ServicoEncaminhamentoAEE.salvarEncaminhamento(
      encaminhamentoId,
      situacaoAEE.Encaminhado,
      true
    );
  };

  const onClickVoltar = async () => {
    if (encaminhamentoAEEEmEdicao) {
      const confirmou = await confirmar(
        'Atenção',
        '',
        'Suas alterações não foram salvas, deseja salvar agora?'
      );
      if (confirmou) {
        const encaminhamentoId = match?.params?.id;
        ServicoEncaminhamentoAEE.salvarEncaminhamento(encaminhamentoId);
      } else {
        history.push(RotasDto.RELATORIO_AEE_ENCAMINHAMENTO);
      }
    } else {
      history.push(RotasDto.RELATORIO_AEE_ENCAMINHAMENTO);
    }
  };

  const onClickCancelar = async () => {
    if (encaminhamentoAEEEmEdicao) {
      const confirmou = await confirmar(
        'Atenção',
        'Você não salvou as informações preenchidas.',
        'Deseja realmente cancelar as alterações?'
      );
      if (confirmou) {
        ServicoEncaminhamentoAEE.resetarTelaDadosOriginais();
      }
    }
  };

  const onClickExcluir = async () => {
    const encaminhamentoId = match?.params?.id;
    if (encaminhamentoId) {
      const confirmado = await confirmar(
        'Excluir',
        '',
        'Você tem certeza que deseja excluir este registro?'
      );

      if (confirmado) {
        dispatch(setExibirLoaderEncaminhamentoAEE(true));
        const resposta = await ServicoEncaminhamentoAEE.excluirEncaminhamento(
          encaminhamentoId
        )
          .catch(e => erros(e))
          .finally(() => dispatch(setExibirLoaderEncaminhamentoAEE(false)));

        if (resposta?.status === 200) {
          sucesso('Registro excluído com sucesso');
          history.push(RotasDto.RELATORIO_AEE_ENCAMINHAMENTO);
        }
      }
    }
  };

  const onClickEncerrar = () => {
    dispatch(setExibirModalEncerramentoEncaminhamentoAEE(true));
  };

  const onClickEncaminharAEE = async () => {
    const encaminhamentoId = match?.params?.id;

    dispatch(setExibirLoaderEncaminhamentoAEE(true));
    const resposta = await ServicoEncaminhamentoAEE.enviarParaAnaliseEncaminhamento(
      encaminhamentoId
    )
      .catch(e => erros(e))
      .finally(() => dispatch(setExibirLoaderEncaminhamentoAEE(false)));

    if (resposta?.status === 200) {
      sucesso('Encaminhamento enviado para a AEE');
      history.push(RotasDto.RELATORIO_AEE_ENCAMINHAMENTO);
    }
  };

  return (
    <>
      <Button
        id="btn-voltar"
        label="Voltar"
        icon="arrow-left"
        color={Colors.Azul}
        border
        className="mr-3"
        onClick={onClickVoltar}
      />
      <Button
        id="btn-cancelar"
        label="Cancelar"
        color={Colors.Roxo}
        border
        className="mr-3"
        onClick={onClickCancelar}
        disabled={!encaminhamentoAEEEmEdicao}
      />
      <Button
        id="btn-excluir"
        label="Excluir"
        color={Colors.Vermelho}
        border
        className="mr-3"
        onClick={onClickExcluir}
        disabled={
          !match?.params?.id ||
          (match?.params?.id && !dadosEncaminhamento?.podeEditar)
        }
      />
      <Button
        id="btn-salvar"
        label={match?.params?.id ? 'Alterar' : 'Salvar'}
        color={Colors.Azul}
        border
        bold
        onClick={onClickSalvar}
        disabled={
          !encaminhamentoAEEEmEdicao ||
          (match?.params?.id && !dadosEncaminhamento?.podeEditar)
        }
      />
      <Button
        id="btn-enviar"
        label="Enviar"
        color={Colors.Roxo}
        border
        bold
        className="ml-3"
        onClick={onClickEnviar}
        hidden={dadosEncaminhamento?.situacao !== situacaoAEE.Rascunho}
        disabled={!encaminhamentoAEEEmEdicao}
      />
      <Button
        id="btn-encerrar"
        label="Encerrar"
        color={Colors.Azul}
        border
        bold
        className="ml-3"
        onClick={onClickEncerrar}
        hidden={dadosEncaminhamento?.situacao === situacaoAEE.Rascunho}
        disabled={
          encaminhamentoAEEEmEdicao ||
          !dadosEncaminhamento?.podeEditar ||
          situacaoAEE.Analise
        }
      />
      <Button
        id="btn-encaminhar-aee"
        label="Encaminhar AEE"
        color={Colors.Roxo}
        border
        bold
        className="ml-3"
        onClick={onClickEncaminharAEE}
        hidden={dadosEncaminhamento?.situacao === situacaoAEE.Rascunho}
        disabled={
          encaminhamentoAEEEmEdicao ||
          !dadosEncaminhamento?.podeEditar ||
          situacaoAEE.Analise
        }
      />
    </>
  );
};

BotoesAcoesEncaminhamentoAEE.propTypes = {
  match: PropTypes.oneOfType([PropTypes.object]),
};

BotoesAcoesEncaminhamentoAEE.defaultProps = {
  match: {},
};

export default BotoesAcoesEncaminhamentoAEE;

import PropTypes from 'prop-types';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import Button from '~/componentes/button';
import { Colors } from '~/componentes/colors';
import { setObservacaoEmEdicao } from '~/redux/modulos/observacoesChat/actions';
import { CampoObservacao, LinhaObservacao } from './observacoesChat.css';

const LinhaObservacaoProprietario = props => {
  const {
    observacao,
    onClickSalvarEdicao,
    onClickExcluir,
    index,
    children,
  } = props;

  const dispatch = useDispatch();

  const observacaoEmEdicao = useSelector(
    store => store.observacoesChat.observacaoEmEdicao
  );

  const onClickEditar = () => {
    const obs = [...observacaoEmEdicao];
    obs[index] = { ...observacao };
    dispatch(setObservacaoEmEdicao([...obs]));
  };
  const onClickSalvar = () => {
    onClickSalvarEdicao(observacaoEmEdicao[index]).then(resultado => {
      if (resultado && resultado.status === 200) {
        dispatch(setObservacaoEmEdicao([]));
      }
    });
  };

  const onClickCancelar = () => {
    dispatch(setObservacaoEmEdicao([]));
  };

  const onChangeObs = ({ target: { value } }) => {
    const obs = [...observacaoEmEdicao];
    obs[index].texto = value;
    dispatch(setObservacaoEmEdicao([...obs]));
  };

  const btnSalvarCancelar = () => {
    return (
      <div className="d-flex mt-2">
        <Button
          id="btn-cancelar-obs-novo"
          label="Cancelar"
          color={Colors.Roxo}
          border
          bold
          className="mr-3"
          onClick={onClickCancelar}
          height="30px"
        />
        <Button
          id="btn-salvar-obs-novo"
          label="Salvar"
          color={Colors.Roxo}
          border
          bold
          onClick={onClickSalvar}
          height="30px"
        />
      </div>
    );
  };

  const btnEditarExcluir = () => {
    return (
      <div className="d-flex mt-2">
        <Button
          id="btn-editar"
          icon="edit"
          iconType="far"
          color={Colors.Azul}
          border
          className="btn-acao mr-2"
          onClick={onClickEditar}
          height="30px"
          width="30px"
          disabled={
            observacaoEmEdicao &&
            observacaoEmEdicao.length &&
            !observacaoEmEdicao[index]
          }
        />
        <Button
          id="btn-excluir"
          icon="trash-alt"
          iconType="far"
          color={Colors.Azul}
          border
          className="btn-acao"
          onClick={() => onClickExcluir(observacao)}
          height="30px"
          width="30px"
          disabled={
            observacaoEmEdicao &&
            observacaoEmEdicao.length &&
            !observacaoEmEdicao[index]
          }
        />
      </div>
    );
  };

  return (
    <>
      {observacaoEmEdicao &&
      observacaoEmEdicao[index] &&
      observacaoEmEdicao[index].id ? (
        <>
          <CampoObservacao
            id="editando-observacao"
            autoSize={{ minRows: 3 }}
            value={observacaoEmEdicao[index].texto}
            onChange={onChangeObs}
          />
          <div className="d-flex justify-content-between">
            {children}
            {btnSalvarCancelar()}
          </div>
        </>
      ) : (
        <>
          <LinhaObservacao className="col-md-12">
            <div>{observacao.texto}</div>
          </LinhaObservacao>
          <div className="d-flex justify-content-between">
            {children}
            {btnEditarExcluir()}
          </div>
        </>
      )}
    </>
  );
};

LinhaObservacaoProprietario.propTypes = {
  observacao: PropTypes.oneOfType([PropTypes.object]),
  onClickSalvarEdicao: PropTypes.func,
  onClickExcluir: PropTypes.func,
  index: PropTypes.number,
  children: PropTypes.node,
};

LinhaObservacaoProprietario.defaultProps = {
  observacao: {},
  onClickSalvarEdicao: () => {},
  onClickExcluir: () => {},
  index: null,
  children: () => {},
};

export default LinhaObservacaoProprietario;

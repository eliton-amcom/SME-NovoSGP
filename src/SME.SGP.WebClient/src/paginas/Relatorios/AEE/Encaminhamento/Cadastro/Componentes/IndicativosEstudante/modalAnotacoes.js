import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import shortid from 'shortid';
import { Colors, ModalConteudoHtml } from '~/componentes';
import Button from '~/componentes/button';
import JoditEditor from '~/componentes/jodit-editor/joditEditor';
import {
  setDadosModalAnotacao,
  setExibirModalAnotacao,
} from '~/redux/modulos/encaminhamentoAEE/actions';

const ModalAnotacoesEncaminhamentoAEE = () => {
  const dispatch = useDispatch();

  const dadosModalAnotacao = useSelector(
    store => store.encaminhamentoAEE.dadosModalAnotacao
  );

  const exibirModalAnotacao = useSelector(
    store => store.encaminhamentoAEE.exibirModalAnotacao
  );

  const onClose = () => {
    dispatch(setDadosModalAnotacao());
    dispatch(setExibirModalAnotacao(false));
  };
  return (
    <ModalConteudoHtml
      id={shortid.generate()}
      key="anotacao"
      visivel={exibirModalAnotacao}
      titulo="Anotações"
      onClose={onClose}
      esconderBotaoPrincipal
      esconderBotaoSecundario
      width={750}
      closable
    >
      <JoditEditor
        value={dadosModalAnotacao?.justificativaAusencia}
        readonly
        removerToolbar
      />
      <div className="col-md-12 mt-2 p-0 d-flex justify-content-end">
        <Button
          key="btn-voltar"
          id="btn-voltar"
          label="Voltar"
          icon="arrow-left"
          color={Colors.Azul}
          border
          onClick={onClose}
          className="mt-2"
        />
      </div>
    </ModalConteudoHtml>
  );
};

export default ModalAnotacoesEncaminhamentoAEE;

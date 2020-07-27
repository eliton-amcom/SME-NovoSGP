import PropTypes from 'prop-types';
import React from 'react';
import { useSelector } from 'react-redux';
import Auditoria from '~/componentes/auditoria';
import LinhaObservacaoProprietario from './linhaObservacaoProprietario';
import { LinhaObservacao } from './observacoesChat.css';

const ObservacoesChatMontarDados = props => {
  const { onClickSalvarEdicao, onClickExcluir } = props;

  const dadosObservacoes = useSelector(
    store => store.observacoesChat.dadosObservacoes
  );

  const auditoria = observacao => {
    return (
      <div className="row mt-1">
        <Auditoria
          alteradoEm={observacao.auditoria.alteradoEm}
          alteradoPor={observacao.auditoria.alteradoPor}
          alteradoRf={observacao.auditoria.alteradoRF}
          criadoEm={observacao.auditoria.criadoEm}
          criadoPor={observacao.auditoria.criadoPor}
          criadoRf={observacao.auditoria.criadoRF}
          ignorarMarginTop
        />
      </div>
    );
  };

  const montaLinhaObservacao = obs => {
    return (
      <div className="mb-5">
        <LinhaObservacao className="col-md-12">
          <div>{obs.texto}</div>
        </LinhaObservacao>
        {obs.auditoria ? <>{auditoria(obs)}</> : ''}
      </div>
    );
  };

  const montarValores = (obs, index) => {
    if (obs && obs.proprietario) {
      return (
        <div className="mb-5">
          <LinhaObservacaoProprietario
            observacao={obs}
            onClickSalvarEdicao={onClickSalvarEdicao}
            onClickExcluir={onClickExcluir}
            index={index}
          >
            {obs.auditoria ? <>{auditoria(obs)}</> : ''}
          </LinhaObservacaoProprietario>
        </div>
      );
    }
    return montaLinhaObservacao(obs);
  };

  return (
    <div className="col-md-12 mb-2 mt-2">
      {dadosObservacoes && dadosObservacoes.length
        ? dadosObservacoes.map((obs, index) => {
            return <> {montarValores(obs, index)} </>;
          })
        : ''}
    </div>
  );
};

ObservacoesChatMontarDados.propTypes = {
  onClickSalvarEdicao: PropTypes.func,
  onClickExcluir: PropTypes.func,
};

ObservacoesChatMontarDados.defaultProps = {
  onClickSalvarEdicao: () => {},
  onClickExcluir: () => {},
};

export default ObservacoesChatMontarDados;

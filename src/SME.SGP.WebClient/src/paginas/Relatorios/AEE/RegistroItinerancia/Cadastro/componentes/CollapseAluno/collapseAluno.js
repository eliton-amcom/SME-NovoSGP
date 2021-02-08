import React, { useState } from 'react';
import { Divider } from 'antd';
import shortid from 'shortid';
import PropTypes from 'prop-types';

import {
  Base,
  Button,
  Colors,
  JoditEditor,
  PainelCollapse,
} from '~/componentes';

const CollapseAluno = ({ aluno, removerAlunos, desabilitar }) => {
  const [acompanhamentoSituacao, setAcompanhamentoSituacao] = useState();
  const [descritivoEstudante, setDescritivoEstudante] = useState();
  const [encaminhamentos, setEncaminhamentos] = useState();

  const id = 'collapseAluno';

  return (
    <div className="row mb-4">
      <div className="col-12">
        <PainelCollapse onChange={() => {}}>
          <PainelCollapse.Painel
            key={id}
            espacoPadrao
            corBorda={Base.AzulBordaCollapse}
            temBorda
            header={aluno.alunoNome}
          >
            <div className="row mb-4 mt-n2">
              <div className="col-12">
                <JoditEditor
                  id="descritivoEstudante"
                  label="Descritivo do estudante"
                  name="descritivoEstudante"
                  value={descritivoEstudante}
                  onChange={e => setDescritivoEstudante(e)}
                  desabilitar={desabilitar}
                />
              </div>
            </div>
            <div className="row mb-4">
              <div className="col-12">
                <JoditEditor
                  label="Acompanhamento da situação"
                  value={acompanhamentoSituacao}
                  name="acompanhamentoSituacao"
                  id="acompanhamentoSituacao"
                  onChange={e => setAcompanhamentoSituacao(e)}
                  desabilitar={desabilitar}
                />
              </div>
            </div>
            <div className="row">
              <div className="col-12">
                <JoditEditor
                  label="Encaminhamentos"
                  value={encaminhamentos}
                  name="encaminhamentos"
                  id="encaminhamentos"
                  onChange={e => setEncaminhamentos(e)}
                  desabilitar={desabilitar}
                />
              </div>
            </div>
            <div className="row mt-n2">
              <Divider />
            </div>
            <div className="row">
              <div className="col-sm-12 d-flex justify-content-end">
                <Button
                  id={shortid.generate()}
                  label="Remover estudante"
                  icon="user-minus"
                  color={Colors.Azul}
                  border
                  onClick={removerAlunos}
                  disabled={desabilitar}
                />
              </div>
            </div>
          </PainelCollapse.Painel>
        </PainelCollapse>
      </div>
    </div>
  );
};

CollapseAluno.defaultProps = {
  aluno: [],
  removerAlunos: () => {},
  desabilitar: false,
};

CollapseAluno.propTypes = {
  aluno: PropTypes.instanceOf(PropTypes.any),
  removerAlunos: PropTypes.func,
  desabilitar: PropTypes.bool,
};

export default CollapseAluno;

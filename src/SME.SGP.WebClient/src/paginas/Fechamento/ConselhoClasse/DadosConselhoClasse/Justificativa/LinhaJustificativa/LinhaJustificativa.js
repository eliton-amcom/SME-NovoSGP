import React from 'react';
import { useSelector } from 'react-redux';
import Justificativa from '../Justificativa';

const LinhaJustificativa = props => {
  const { idCampo, ehRegencia, alunoDesabilitado } = props;

  const expandirLinha = useSelector(
    store => store.conselhoClasse.expandirLinha[idCampo]
  );

  return (
    <>
      {expandirLinha ? (
        <tr>
          <td style={{ height: '100px' }} colSpan={ehRegencia ? '4' : '8'}>
            <Justificativa alunoDesabilitado={alunoDesabilitado} />
          </td>
        </tr>
      ) : (
        ''
      )}
    </>
  );
};

export default LinhaJustificativa;

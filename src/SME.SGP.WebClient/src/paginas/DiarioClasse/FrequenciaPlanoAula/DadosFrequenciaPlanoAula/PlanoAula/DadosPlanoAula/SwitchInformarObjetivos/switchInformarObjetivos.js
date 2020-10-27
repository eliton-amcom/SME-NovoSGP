import { Switch } from 'antd';
import React from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { Label } from '~/componentes';
import { setCheckedExibirEscolhaObjetivos } from '~/redux/modulos/frequenciaPlanoAula/actions';

function SwitchInformarObjetivos() {
  const dispatch = useDispatch();

  const checkedExibirEscolhaObjetivos = useSelector(
    store => store.frequenciaPlanoAula.checkedExibirEscolhaObjetivos
  );

  const desabilitarCamposPlanoAula = useSelector(
    state => state.frequenciaPlanoAula.desabilitarCamposPlanoAula
  );

  const exibirSwitchEscolhaObjetivos = useSelector(
    state => state.frequenciaPlanoAula.exibirSwitchEscolhaObjetivos
  );

  return (
    <>
      {exibirSwitchEscolhaObjetivos ? (
        <>
          <Label text="Informar Objetivos de Aprendizagem e Desenvolvimento" />
          <Switch
            onChange={() =>
              dispatch(
                setCheckedExibirEscolhaObjetivos(!checkedExibirEscolhaObjetivos)
              )
            }
            checked={checkedExibirEscolhaObjetivos}
            size="default"
            className="ml-2 mr-2"
            disabled={desabilitarCamposPlanoAula}
          />
        </>
      ) : (
        ''
      )}
    </>
  );
}

export default SwitchInformarObjetivos;
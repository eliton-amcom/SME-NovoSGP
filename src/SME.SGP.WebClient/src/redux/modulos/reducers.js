import { combineReducers } from 'redux';

import navegacao from './navegacao/reducers';
import notificacoes from './notificacoes/reducers';
import alertas from './alertas/reducers';
import bimestres from './planoAnual/reducers';
import usuario from './usuario/reducers';

export default combineReducers({
  navegacao,
  notificacoes,
  alertas,
  bimestres,
  usuario,
});

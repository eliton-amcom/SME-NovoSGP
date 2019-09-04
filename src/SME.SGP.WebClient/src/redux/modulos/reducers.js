import { combineReducers } from 'redux';

import notificacoes from './alertas/reducers';
import bimestres from './planoAnual/reducers';
import usuario from './usuario/reducers';

export default combineReducers({
  notificacoes, bimestres
  usuario,
});

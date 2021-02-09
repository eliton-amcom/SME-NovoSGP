import { combineReducers } from 'redux';

import navegacao from './navegacao/reducers';
import alertas from './alertas/reducers';
import usuario from './usuario/reducers';
import notificacoes from './notificacoes/reducers';
import perfil from './perfil/reducers';
import calendarioEscolar from './calendarioEscolar/reducers';
import calendarioProfessor from './calendarioProfessor/reducers';
import bimestres from './planoAnual/reducers';
import filtro from './filtro/reducers';
import atribuicaoEsporadica from './atribuicaoEsporadica/reducers';
import loader from './loader/reducer';
import notasConceitos from './notasConceitos/reducer';
import mensagens from './mensagens/reducers';
import conselhoClasse from './conselhoClasse/reducers';
import relatorioSemestralPAP from './relatorioSemestralPAP/reducers';
import sistema from './sistema/reducers';
import localizadorEstudante from './localizadorEstudante/reducers';
import observacoesUsuario from './observacoesUsuario/reducers';
import cartaIntencoes from './cartaIntencoes/reducers';
import devolutivas from './devolutivas/reducers';
import dashboard from './dashboard/reducers';
import planoAnual from './anual/reducers';
import frequenciaPlanoAula from './frequenciaPlanoAula/reducers';
import dashboardEscolaAqui from './dashboardEscolaAqui/reducers';
import encaminhamentoAEE from './encaminhamentoAEE/reducers';
import registroIndividual from './registroIndividual/reducers';
import acompanhamentoFrequencia from './acompanhamentoFrequencia/reducers';
import itinerancia from './itinerancia/reducers';

const reducers = combineReducers({
  navegacao,
  alertas,
  usuario,
  perfil,
  calendarioEscolar,
  notificacoes,
  bimestres,
  filtro,
  calendarioProfessor,
  atribuicaoEsporadica,
  loader,
  notasConceitos,
  mensagens,
  conselhoClasse,
  relatorioSemestralPAP,
  sistema,
  localizadorEstudante,
  observacoesUsuario,
  cartaIntencoes,
  devolutivas,
  dashboard,
  planoAnual,
  frequenciaPlanoAula,
  dashboardEscolaAqui,
  encaminhamentoAEE,
  registroIndividual,
  acompanhamentoFrequencia,
  itinerancia,
});

const rootReducer = (state, action) => {
  if (action.type === '@sessao/limpar') state = undefined;

  return reducers(state, action);
};

export default rootReducer;

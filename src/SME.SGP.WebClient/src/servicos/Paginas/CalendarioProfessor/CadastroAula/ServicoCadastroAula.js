import api from '~/servicos/api';

class ServicoCadastroAula {
  obterPorId = idAula => {
    const url = `v1/calendarios/professores/aulas/${idAula}`;
    return api.get(url);
  };

  salvar = (id, aula) => {
    let metodo = 'post';
    let url = 'v1/calendarios/professores/aulas';
    if (id > 0) {
      metodo = 'put';
      url = `${url}/${id}`;
    }
    return api[metodo](url, aula);
    // return api.post('v1/calendarios/professores/aulas/', aula);
  };

  obterGradePorComponenteETurma = (
    turmaId,
    componenteId,
    dataAula,
    ehRegencia
  ) => {
    const url = `v1/grades/aulas/turmas/${turmaId}/disciplinas/${componenteId}?ehRegencia=${ehRegencia}`;
    return api.get(url, {
      params: {
        data: dataAula.format('YYYY-MM-DD'),
      },
    });
  };
}

export default new ServicoCadastroAula();

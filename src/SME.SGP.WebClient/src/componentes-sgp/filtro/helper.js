import ServicoFiltro from '~/servicos/Componentes/ServicoFiltro';
import tipoEscolaDTO from '~/dtos/tipoEscolaDto';

class FiltroHelper {
  obterModalidades = async ({ consideraHistorico, anoLetivoSelecionado }) => {
    const modalidadesLista = [];

    return ServicoFiltro.listarModalidades({
      consideraHistorico,
      anoLetivoSelecionado,
    })
      .then(resposta => {
        if (resposta.data) {
          resposta.data.forEach(modalidade => {
            modalidadesLista.push({
              desc: modalidade.descricao,
              valor: modalidade.id,
            });
          });
        }

        return modalidadesLista;
      })
      .catch(() => modalidadesLista);
  };

  obterPeriodos = async ({
    consideraHistorico,
    modalidadeSelecionada,
    anoLetivoSelecionado,
  }) => {
    const periodos = [];

    return ServicoFiltro.listarPeriodos({
      consideraHistorico,
      modalidadeSelecionada,
      anoLetivoSelecionado,
    })
      .then(resposta => {
        if (resposta.data) {
          resposta.data.forEach(periodo => {
            periodos.push({ desc: periodo, valor: periodo });
          });
        }

        return periodos;
      })
      .catch(() => periodos);
  };

  obterDres = async ({
    consideraHistorico,
    modalidadeSelecionada,
    periodoSelecionado,
    anoLetivoSelecionado,
  }) => {
    const dres = [];

    return ServicoFiltro.listarDres({
      consideraHistorico,
      modalidadeSelecionada,
      periodoSelecionado,
      anoLetivoSelecionado,
    })
      .then(resposta => {
        if (resposta.data) {
          resposta.data.forEach(dre => {
            dres.push({
              desc: dre.nome,
              valor: dre.codigo,
              abrev: dre.abreviacao,
            });
          });
        }
        return dres.sort(this.ordenarLista('desc'));
      })
      .catch(() => dres);
  };

  obterUnidadesEscolares = async ({
    consideraHistorico,
    modalidadeSelecionada,
    dreSelecionada,
    periodoSelecionado,
    anoLetivoSelecionado,
  }) => {
    const unidadesEscolares = [];

    return ServicoFiltro.listarUnidadesEscolares({
      consideraHistorico,
      modalidadeSelecionada,
      dreSelecionada,
      periodoSelecionado,
      anoLetivoSelecionado,
    })
      .then(resposta => {
        if (resposta.data) {
          resposta.data.forEach(unidade => {
            unidadesEscolares.push({
              desc: `${tipoEscolaDTO[unidade.tipoEscola]} ${unidade.nome}`,
              valor: unidade.codigo,
            });
          });
        }
        return unidadesEscolares.sort(this.ordenarLista('desc'));
      })
      .catch(() => unidadesEscolares);
  };

  obterTurmas = async ({
    consideraHistorico,
    modalidadeSelecionada,
    unidadeEscolarSelecionada,
    periodoSelecionado,
    anoLetivoSelecionado,
  }) => {
    const turmas = [];

    return ServicoFiltro.listarTurmas({
      consideraHistorico,
      modalidadeSelecionada,
      unidadeEscolarSelecionada,
      periodoSelecionado,
      anoLetivoSelecionado,
    })
      .then(resposta => {
        if (resposta.data) {
          resposta.data.forEach(turma => {
            turmas.push({
              desc: turma.nome,
              valor: turma.codigo,
              ano: turma.ano,
            });
          });
        }
        return turmas;
      })
      .catch(() => turmas);
  };

  ordenarLista = indice => {
    return function innerSort(a, b) {
      if (!a.hasOwnProperty(indice) || !b.hasOwnProperty(indice)) return 0;

      const itemA =
        typeof a[indice] === 'string' ? a[indice].toUpperCase() : a[indice];
      const itemB =
        typeof b[indice] === 'string' ? b[indice].toUpperCase() : b[indice];

      let ordem = 0;
      if (itemA > itemB) {
        ordem = 1;
      } else if (itemA < itemB) {
        ordem = -1;
      }
      return ordem;
    };
  };
}

export default new FiltroHelper();

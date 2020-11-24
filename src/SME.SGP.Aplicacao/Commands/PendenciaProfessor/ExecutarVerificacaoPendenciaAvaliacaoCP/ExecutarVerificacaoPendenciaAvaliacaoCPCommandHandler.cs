﻿using MediatR;
using Sentry;
using SME.SGP.Aplicacao.Integracoes;
using SME.SGP.Dominio;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public class ExecutarVerificacaoPendenciaAvaliacaoCPCommandHandler : IRequestHandler<ExecutarVerificacaoPendenciaAvaliacaoCPCommand, bool>
    {
        private readonly IMediator mediator;
        private readonly IServicoEol servicoEol;

        public ExecutarVerificacaoPendenciaAvaliacaoCPCommandHandler(IMediator mediator, IServicoEol servicoEol)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
        }

        public async Task<bool> Handle(ExecutarVerificacaoPendenciaAvaliacaoCPCommand request, CancellationToken cancellationToken)
        {
            var periodosEncerrando = await mediator.Send(new ObterPeriodosFechamentoEscolasPorDataFinalQuery(DateTime.Now.Date.AddDays(request.DiasParaGeracaoDePendencia)));
            foreach (var periodoEncerrando in periodosEncerrando)
            {
                try
                {
                    var turmasSemAvaliacao = await mediator.Send(new ObterTurmaEComponenteSemAvaliacaoNoPeriodoPorUeQuery(periodoEncerrando.PeriodoFechamento.UeId.Value,
                                                                                                                 periodoEncerrando.PeriodoEscolar.TipoCalendarioId,
                                                                                                                 periodoEncerrando.PeriodoEscolar.PeriodoInicio,
                                                                                                                 periodoEncerrando.PeriodoEscolar.PeriodoFim));

                    if (turmasSemAvaliacao != null && turmasSemAvaliacao.Any())
                    {
                        var componentesCurriculares = await mediator.Send(new ObterComponentesCurricularesQuery());
                        foreach (var turmaSemAvaliacao in turmasSemAvaliacao.GroupBy(a => (a.TurmaCodigo, a.TurmaId)))
                        {
                            await IncluirPendenciaCP(turmaSemAvaliacao, componentesCurriculares, periodoEncerrando);
                        }
                    }
                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                }
            }

            return true;
        }

        private async Task IncluirPendenciaCP(IGrouping<(string TurmaCodigo, long TurmaId), TurmaEComponenteDto> turmaSemAvaliacao, IEnumerable<ComponenteCurricularDto> componentesCurriculares, PeriodoFechamentoBimestre periodoEncerrando)
        {
            var professoresTurma = await servicoEol.ObterProfessoresTitularesDisciplinas(turmaSemAvaliacao.Key.TurmaCodigo);
            var turma = await mediator.Send(new ObterTurmaComUeEDrePorIdQuery(turmaSemAvaliacao.Key.TurmaId));

            var pendenciaId = await ObterPendenciaIdDaTurma(turmaSemAvaliacao.Key.TurmaId);
            var gerarPendenciasProfessor = new List<(long componenteCurricularId, string professorRf)>();

            foreach (var componenteCurricularNaTurma in turmaSemAvaliacao)
            {
                var professorComponente = professoresTurma.FirstOrDefault(c => c.DisciplinaId == componenteCurricularNaTurma.ComponenteCurricularId);
                var componenteCurricular = componentesCurriculares.FirstOrDefault(c => c.Codigo == componenteCurricularNaTurma.ComponenteCurricularId.ToString());

                if (professorComponente != null && !await ExistePendenciaProfessor(pendenciaId, turma.Id, componenteCurricular.Codigo, professorComponente.ProfessorRf))
                    gerarPendenciasProfessor.Add((long.Parse(componenteCurricular.Codigo), professorComponente.ProfessorRf));
            }

            if (gerarPendenciasProfessor.Any())
                await GerarPendenciasProfessor(pendenciaId, gerarPendenciasProfessor, turma, periodoEncerrando.PeriodoEscolar.Bimestre);
        }

        private async Task GerarPendenciasProfessor(long pendenciaId, List<(long componenteCurricularId, string professorRf)> gerarPendenciasProfessor, Turma turma, int bimestre)
        {
            if (pendenciaId == 0)
                pendenciaId = await IncluirPendenciaProfessor(turma, bimestre);

            await mediator.Send(new SalvarPendenciaAusenciaDeAvaliacaoCPCommand(pendenciaId, turma.Id, turma.Ue.CodigoUe, gerarPendenciasProfessor));
        }

        private async Task<long> IncluirPendenciaProfessor(Turma turma, int bimestre)
        {
            var escolaUe = $"{turma.Ue.TipoEscola.ShortName()} {turma.Ue.Nome} (DRE - {turma.Ue.Dre.Abreviacao})";
            var titulo = $"Ausência de avaliação no {bimestre}º bimestre {escolaUe}";

            var descricao = $"<i>Os componentes curriculares abaixo não possuem nenhuma avaliação cadastrada no {bimestre}º bimestre {escolaUe}</i>";
            var instrucao = "Oriente os professores a cadastrarem as avaliações.";

            return await mediator.Send(new SalvarPendenciaCommand(TipoPendencia.AusenciaDeAvaliacaoCP, descricao, instrucao, titulo));
        }

        private async Task<bool> ExistePendenciaProfessor(long pendenciaId, long turmaId, string componenteCurricularId, string professorRf)
            => pendenciaId != 0 &&
            await mediator.Send(new ExistePendenciaProfessorPorTurmaEComponenteQuery(turmaId,
                                                                                     long.Parse(componenteCurricularId),
                                                                                     professorRf,
                                                                                     TipoPendencia.AusenciaDeAvaliacaoCP));

        private async Task<long> ObterPendenciaIdDaTurma(long turmaId)
            => await mediator.Send(new ObterPendenciaIdPorTurmaQuery(turmaId, TipoPendencia.AusenciaDeAvaliacaoCP));
    }
}

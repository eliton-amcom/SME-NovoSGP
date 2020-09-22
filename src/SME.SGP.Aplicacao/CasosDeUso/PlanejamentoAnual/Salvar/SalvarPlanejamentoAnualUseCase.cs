﻿using MediatR;
using SME.SGP.Dominio;
using SME.SGP.Infra;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public class SalvarPlanejamentoAnualUseCase : ISalvarPlanejamentoAnualUseCase
    {
        private readonly IMediator mediator;
        private readonly IUnitOfWork unitOfWork;

        public SalvarPlanejamentoAnualUseCase(IMediator mediator,
                                              IUnitOfWork unitOfWork)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            this.unitOfWork = unitOfWork ?? throw new System.ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<AuditoriaDto> Executar(long turmaId, long componenteCurricularId, SalvarPlanejamentoAnualDto dto)
        {
            unitOfWork.IniciarTransacao();

            var auditoria = await mediator.Send(new SalvarPlanejamentoAnualCommand()
            {
                TurmaId = turmaId,
                ComponenteCurricularId = componenteCurricularId,
                Id = dto.Id,
                PeriodosEscolares = dto.PeriodosEscolares
            });

            unitOfWork.PersistirTransacao();

            return auditoria;
        }
    }
}

﻿using MediatR;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public class PublicaFilaExcluirPendenciaAusenciaFechamentoCommandHandler : IRequestHandler<PublicaFilaExcluirPendenciaAusenciaFechamentoCommand, bool>
    {
        private readonly IMediator mediator;

        public PublicaFilaExcluirPendenciaAusenciaFechamentoCommandHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(PublicaFilaExcluirPendenciaAusenciaFechamentoCommand request, CancellationToken cancellationToken)
        {
            await mediator.Send(new PublicaFilaWorkerSgpCommand(RotasRabbit.RotaExecutaExclusaoPendenciasAusenciaFechamento,
                                                       new VerificaExclusaoPendenciasAusenciaFechamentoCommand
                                                       (request.DisciplinaId,
                                                       request.PeriodoEscolarId,
                                                       request.TurmaCodigo),
                                                       Guid.NewGuid(),
                                                       request.UsuarioLogado));
            return true;
        }
    }
}

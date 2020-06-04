﻿using MediatR;
using SME.SGP.Infra.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public class RelatorioGamesCommandHandler : IRequestHandler<RelatorioGamesCommand, bool>
    {
        private readonly IServicoFila servicoFila;

        public RelatorioGamesCommandHandler(IServicoFila servicoFila)
        {
            this.servicoFila = servicoFila ?? throw new ArgumentNullException(nameof(servicoFila));
        }

        public async Task<bool> Handle(RelatorioGamesCommand request, CancellationToken cancellationToken)
        {

            await servicoFila.AdicionaFila(new Infra.Dtos.AdicionaFilaDto("relatorios", new { Teste = "Ronaldo" }));
            return true;
        }
    }
}

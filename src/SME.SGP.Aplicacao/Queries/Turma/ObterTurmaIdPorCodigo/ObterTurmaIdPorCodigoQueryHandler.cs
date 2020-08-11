﻿using MediatR;
using SME.SGP.Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao.Queries.Turma.ObterTurmaIdPorCodigo
{
    public class ObterTurmaIdPorCodigoQueryHandler : IRequestHandler<ObterTurmaIdPorCodigoQuery, long>
    {
        private readonly IRepositorioTurma repositorioTurma;

        public ObterTurmaIdPorCodigoQueryHandler(IRepositorioTurma repositorioTurma)
        {
            this.repositorioTurma = repositorioTurma ?? throw new ArgumentNullException(nameof(repositorioTurma));
        }
        public async Task<long> Handle(ObterTurmaIdPorCodigoQuery request, CancellationToken cancellationToken)
        {
            return await repositorioTurma.ObterTurmaIdPorCodigo(request.TurmaCodigo);
        }
    }
}

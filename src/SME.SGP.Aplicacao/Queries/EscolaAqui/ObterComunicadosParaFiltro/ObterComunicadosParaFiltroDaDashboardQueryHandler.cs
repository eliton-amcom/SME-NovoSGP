﻿using MediatR;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra.Dtos.EscolaAqui.Dashboard.ComunicadosFiltro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao.Queries.EscolaAqui.ObterComunicadosParaFiltro
{
    public class ObterComunicadosParaFiltroDaDashboardQueryHandler : IRequestHandler<ObterComunicadosParaFiltroDaDashboardQuery, IEnumerable<ComunicadoParaFiltroDaDashboardDto>>
    {
        private readonly IRepositorioComunicado repositorioComunicado;

        public ObterComunicadosParaFiltroDaDashboardQueryHandler(IRepositorioComunicado repositorioComunicado)
        {
            this.repositorioComunicado = repositorioComunicado;
        }

        public async Task<IEnumerable<ComunicadoParaFiltroDaDashboardDto>> Handle(ObterComunicadosParaFiltroDaDashboardQuery request, CancellationToken cancellationToken)
        {
            var filtro = new FiltroObterComunicadosParaFiltroDaDashboardDto
            {
                AnoEscolar = request.AnoEscolar,
                AnoLetivo = request.AnoLetivo,
                CodigoDre = request.CodigoDre,
                CodigoTurma = request.CodigoTurma,
                CodigoUe = request.CodigoUe,
                DataEnvioFinal = request.DataEnvioFinal,
                DataEnvioInicial = request.DataEnvioInicial,
                Titulo = request.Descricao,
                GruposIds = request.GruposIds.ToArray(),
                Modalidade = request.Modalidade,
                Semestre = request.Semestre
            };

            return await repositorioComunicado.ObterComunicadosParaFiltroDaDashboard(filtro);
        }
    }
}

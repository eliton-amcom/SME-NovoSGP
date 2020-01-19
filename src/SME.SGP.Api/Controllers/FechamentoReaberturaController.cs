﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.SGP.Api.Filtros;
using SME.SGP.Aplicacao;
using SME.SGP.Infra;
using System.Threading.Tasks;

namespace SME.SGP.Api.Controllers
{
    [ApiController]
    [Route("api/v1/fechamentos/reaberturas")]
    [Authorize("Bearer")]
    public class FechamentoReaberturaController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        [ProducesResponseType(typeof(RetornoBaseDto), 601)]
        [Permissao(Permissao.PFR_C, Policy = "Bearer")]
        public async Task<IActionResult> Listar([FromServices] IConsultasFechamentoReabertura consultasFechamentoReabertura, [FromQuery]FechamentoReaberturaFiltroDto fechamentoReaberturaFiltroDto)
        {
            return Ok(await consultasFechamentoReabertura.Listar(fechamentoReaberturaFiltroDto.TipoCalendarioId, fechamentoReaberturaFiltroDto.DreId, fechamentoReaberturaFiltroDto.UeId));
        }
    }
}
﻿using Microsoft.AspNetCore.Mvc;
using SME.SGP.Aplicacao;
using SME.SGP.Infra;
using SME.SGP.Infra.Dtos.Relatorios;
using System;
using System.Threading.Tasks;

namespace SME.SGP.Api.Controllers
{
    [ApiController]
    [Route("api/v1/relatorios/plano-aula")]
    public class RelatorioPlanoAulaController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(Boolean), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        [ProducesResponseType(typeof(RetornoBaseDto), 601)]
        public async Task<IActionResult> Gerar(FiltroRelatorioPlanoAulaDto filtro, [FromServices] IRelatorioPlanoAulaUseCase useCase)
        {
            return Ok(await useCase.Executar(filtro));
        }
    }


}
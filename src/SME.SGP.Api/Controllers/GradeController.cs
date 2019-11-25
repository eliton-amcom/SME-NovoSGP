﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.SGP.Api.Filtros;
using SME.SGP.Aplicacao;
using SME.SGP.Infra;
using SME.SGP.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Api.Controllers
{
    [ApiController]
    [Route("api/v1/grades/")]
    [ValidaDto]
    [Authorize("Bearer")]
    public class GradeController : ControllerBase
    {
        [HttpGet("aulas/turmas/{codigoTurma}/disciplinas/{codigoDisciplina}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IEnumerable<GradeComponenteTurmaAulasDto>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> ObterGradeAulasTurma([FromQuery] DateTime data, string codigoTurma, int codigoDisciplina, [FromServices] IConsultasGrade consultasGrade)
        {
            var semana = (data.DayOfYear / 7) + 1;
            var horasGrade = await consultasGrade.ObterGradeAulasTurma(codigoTurma, codigoDisciplina, semana.ToString());

            if (horasGrade != null)
                return Ok(horasGrade);
            else
                return StatusCode(204);
        }
    }
}
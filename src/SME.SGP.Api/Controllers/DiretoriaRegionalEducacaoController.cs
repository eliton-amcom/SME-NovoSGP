﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.SGP.Aplicacao;
using SME.SGP.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SGP.Api.Controllers
{
    [ApiController]
    [Route("api/v1/dres")]
    [Authorize("Bearer")]
    public class DiretoriaRegionalEducacaoController : ControllerBase
    {
        private readonly IConsultaDres consultaDres;

        public DiretoriaRegionalEducacaoController(IConsultaDres consultaDres)
        {
            this.consultaDres = consultaDres ?? throw new System.ArgumentNullException(nameof(consultaDres));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CicloDto>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public IActionResult Get()
        {
            return Ok(consultaDres.ObterTodos());
        }

        [HttpGet("{dreId}/ues/sem-atribuicao")]
        [ProducesResponseType(typeof(IEnumerable<UnidadeEscolarDto>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> ObterEscolasSemAtribuicao(string dreId)
        {
            var retorno = await consultaDres.ObterEscolasSemAtribuicao(dreId);
            if (retorno.Count() > 0)
                return Ok(retorno);
            else return StatusCode(204);
        }

        [HttpGet("{dreId}/ues")]
        [ProducesResponseType(typeof(IEnumerable<UnidadeEscolarDto>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> ObterUesPorDre(string dreId)
        {
            var retorno = await consultaDres.ObterEscolasPorDre(dreId);
            if (retorno.Count() > 0)
                return Ok(retorno);
            else return StatusCode(204);
        }
    }
}
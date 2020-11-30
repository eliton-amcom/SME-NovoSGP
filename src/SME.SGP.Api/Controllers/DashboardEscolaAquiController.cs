﻿using Microsoft.AspNetCore.Mvc;
using SME.SGP.Aplicacao;
using SME.SGP.Infra;
using SME.SGP.Infra.Dtos.EscolaAqui;
using SME.SGP.Infra.Dtos.EscolaAqui.Dashboard.ComunicadosPesquisa;
using System.Threading.Tasks;

namespace SME.SGP.Api.Controllers
{
    [ApiController]
    [Route("api/v1/ea/dashboard")]
    //[Authorize("Bearer")]
    public class DashboardEAController : ControllerBase
    {

        [HttpGet("adesao")]
        [ProducesResponseType(typeof(UsuarioEscolaAquiDto), 200)]
        [ProducesResponseType(typeof(UsuarioEscolaAquiDto), 204)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> ObterTotaisAdesao([FromQuery] string codigoDre, [FromQuery] string codigoUe, [FromServices] IObterTotaisAdesaoUseCase obterTotaisAdesaoUseCase)
        {
            return Ok(await obterTotaisAdesaoUseCase.Executar(codigoDre, codigoUe));
        }

        [HttpGet("adesao/agrupados")]
        [ProducesResponseType(typeof(UsuarioEscolaAquiDto), 200)]
        [ProducesResponseType(typeof(UsuarioEscolaAquiDto), 204)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> ObterTotaisAdesaoAgrupadosPorDre([FromServices] IObterTotaisAdesaoAgrupadosPorDreUseCase obterTotaisAdesaoAgrupadosPorDreUseCase)
        {
            return Ok(await obterTotaisAdesaoAgrupadosPorDreUseCase.Executar());
        }

        [HttpGet("ultimoProcessamento")]
        [ProducesResponseType(typeof(UsuarioEscolaAquiDto), 200)]
        [ProducesResponseType(typeof(UsuarioEscolaAquiDto), 204)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> ObterUltimaAtualizacaoPorProcesso([FromQuery] string nomeProcesso, [FromServices] IObterUltimaAtualizacaoPorProcessoUseCase obterUltimaAtualizacaoPorProcessoUseCase)
        {
            return Ok(await obterUltimaAtualizacaoPorProcessoUseCase.Executar(nomeProcesso));
        }

        [HttpGet("comunicados/totais")]
        [ProducesResponseType(typeof(UsuarioEscolaAquiDto), 200)]
        [ProducesResponseType(typeof(UsuarioEscolaAquiDto), 204)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> ObterComunicadosTotaisSme([FromQuery] int anoLetivo, [FromQuery] string codigoDre, [FromQuery] string codigoUe, [FromServices] IObterComunicadosTotaisUseCase obterComunicadosTotaisSmeUseCase)
        {
            return Ok(await obterComunicadosTotaisSmeUseCase.Executar(anoLetivo, codigoDre, codigoUe));
        }

        [HttpGet("comunicados/totais/agrupados")]
        [ProducesResponseType(typeof(UsuarioEscolaAquiDto), 200)]
        [ProducesResponseType(typeof(UsuarioEscolaAquiDto), 204)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        public async Task<IActionResult> ObterComunicadosTotaisAgrupadosPorDre([FromQuery] int anoLetivo, [FromServices] IObterComunicadosTotaisAgrupadosPorDreUseCase obterComunicadosTotaisAgrupadosPorDreUseCase)
        {
            return Ok(await obterComunicadosTotaisAgrupadosPorDreUseCase.Executar(anoLetivo));
        }

        [HttpGet("comunicados/filtro")]
        public async Task<IActionResult> ObterComunicadosParaFiltroDaDashboard([FromQuery] ObterComunicadosParaFiltroDaDashboardDto obterComunicadosFiltroDto, [FromServices] IObterComunicadosParaFiltroDaDashboardUseCase obterComunicadosParaFiltroUseCase)
        {
            return Ok(await obterComunicadosParaFiltroUseCase.Executar(obterComunicadosFiltroDto));
        }
    }
}
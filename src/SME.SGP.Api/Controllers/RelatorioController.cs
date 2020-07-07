﻿using Microsoft.AspNetCore.Mvc;
using SME.SGP.Aplicacao;
using SME.SGP.Aplicacao.Integracoes;
using SME.SGP.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SGP.Api.Controllers
{
    [ApiController]
    [Route("api/v1/relatorios")]
    public class RelatorioController : ControllerBase
    {
        [HttpGet("{codigoCorrelacao}")]
        public async Task<IActionResult> Download(Guid codigoCorrelacao, [FromServices] IReceberDadosDownloadRelatorioUseCase downloadRelatorioUseCase, [FromServices] ISevicoJasper servicoJasper)
        {
            var (relatorio, contentType, nomeArquivo) = await downloadRelatorioUseCase.Executar(codigoCorrelacao);

            return File(relatorio, contentType, nomeArquivo);
        }
        [HttpPost("conselhos-classe/atas-finais")]
        public async Task<IActionResult> ConselhoClasseAtaFinal([FromBody]FiltroRelatorioConselhoClasseAtaFinalDto filtroRelatorioConselhoClasseAtaFinalDto, [FromServices] IRelatorioConselhoClasseAtaFinalUseCase relatorioConselhoClasseAtaFinalUseCase)
        {
            return Ok(await relatorioConselhoClasseAtaFinalUseCase.Executar(filtroRelatorioConselhoClasseAtaFinalDto));
        }
        [HttpPost("notas-frequencias")]
        public async Task<IActionResult> NotasFrequencias([FromBody]FiltroRelatorioFaltasFrequenciasDto filtroRelatorioFaltasFrequenciasDto, [FromServices] IRelatorioFaltasFrequenciasUseCase relatorioFaltasFrequenciasUseCase)
        {
            return Ok(await relatorioFaltasFrequenciasUseCase.Executar(filtroRelatorioFaltasFrequenciasDto));
        }

    }
}
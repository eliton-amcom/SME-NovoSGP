﻿using Microsoft.AspNetCore.Mvc;
using SME.SGP.Aplicacao;
using SME.SGP.Aplicacao.Integracoes;
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
            var dadosRelatorio = await downloadRelatorioUseCase.Executar(codigoCorrelacao);
            if (dadosRelatorio != null)
                return File(await servicoJasper.DownloadRelatorio(dadosRelatorio.ExportacaoId, dadosRelatorio.RequisicaoId, dadosRelatorio.JSessionId), dadosRelatorio.ContentType, dadosRelatorio.NomeArquivo);
            
            return NoContent();
        }

    }
}
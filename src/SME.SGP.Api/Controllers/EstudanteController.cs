﻿using Microsoft.AspNetCore.Mvc;
using SME.SGP.Aplicacao.Interfaces.CasosDeUso;
using SME.SGP.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;
using AlunoDto = SME.SGP.Infra.Dtos.Relatorios.HistoricoEscolar.AlunoDto;

namespace SME.SGP.Api.Controllers
{
    [ApiController]
    [Route("api/v1/estudante")]
    public class EstudanteController : ControllerBase
    {

        [HttpPost]
        [Route("pesquisa")]
        [ProducesResponseType(typeof(IEnumerable<AlunoDto>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        [ProducesResponseType(typeof(RetornoBaseDto), 601)]
        public async Task<IActionResult> ObterAlunos(FiltroBuscaEstudanteDto filtroBuscaAlunosDto, [FromServices] IObterAlunosPorCodigoEolNomeUseCase obterAlunosPorCodigoEolNomeUseCase)
        {
            return Ok(await obterAlunosPorCodigoEolNomeUseCase.Executar(filtroBuscaAlunosDto));
        }

        [HttpGet]
        [Route("informacoes-escolares")]
        [ProducesResponseType(typeof(IEnumerable<AlunoDto>), 200)]
        [ProducesResponseType(typeof(RetornoBaseDto), 500)]
        [ProducesResponseType(typeof(RetornoBaseDto), 601)]
        public async Task<IActionResult> ObterInformacoesEscolaresDoAluno(int codigoAluno, [FromServices] IObterInformacoesEscolaresDoAlunoUseCase ObterInformacoesEscolaresDoAlunoUseCase)
        {
            return Ok(await ObterInformacoesEscolaresDoAlunoUseCase.Executar(codigoAluno));
        }
    }
}
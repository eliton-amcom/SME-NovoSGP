﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SME.SGP.Api.Filtros;
using SME.SGP.Aplicacao;
using SME.SGP.Dominio;
using SME.SGP.Infra;
using System.Threading.Tasks;

namespace SME.SGP.Api.Controllers
{
    [ApiController]
    [Route("api/v1/unidades-escolares")]
    [Authorize("Bearer")]
    public class UnidadesEscolaresController : ControllerBase
    {
        //[Route("{ueId}/funcionarios")]
        //[HttpPost]
        //[Permissao(Permissao.AS_C, Policy = "Bearer")]
        //public async Task<IActionResult> ObterFuncionariosPorUe([FromServices]IConsultasUnidadesEscolares consultasUnidadesEscolares,
        //    BuscaFuncionariosFiltroDto buscaFuncionariosFiltroDto, string ueId)
        //{
        //    if (string.IsNullOrEmpty(ueId))
        //        throw new NegocioException("É necessário informar o código da UE.");
        //    buscaFuncionariosFiltroDto.AtualizaCodigoUe(ueId);
        //    return Ok(await consultasUnidadesEscolares.ObtemFuncionariosPorUe(buscaFuncionariosFiltroDto));
        //}


        [Route("dres/{dresId}/ues/{ueId}/funcionarios")]
        [HttpPost]
        [Permissao(Permissao.AS_C, Policy = "Bearer")]
        public async Task<IActionResult> ObterFuncionariosPorDreEUe([FromServices] IReiniciarSenhaUseCase reiniciarSenhaUseCase, 
            FiltroFuncionariosDto filtroFuncionariosDto, string dreId, string ueId)
        {
            filtroFuncionariosDto.AtualizaCodigoDre(dreId);
            filtroFuncionariosDto.AtualizaCodigoUe(ueId);

            return Ok(await reiniciarSenhaUseCase.Executar(filtroFuncionariosDto));
        }
    }
}
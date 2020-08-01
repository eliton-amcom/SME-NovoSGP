﻿using MediatR;
using SME.SGP.Aplicacao.Interfaces.CasosDeUso;
using SME.SGP.Dominio;
using System;
using System.Threading.Tasks;
using SME.SGP.Aplicacao.Queries;
using SME.SGP.Infra.Dtos.Relatorios.RecuperacaoParalela;

namespace SME.SGP.Aplicacao.CasosDeUso
{
    public class RelatorioRecuperacaoParalelaUseCase : IRelatorioRecuperacaoParalelaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioRecuperacaoParalelaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(FiltroRelatorioRecuperacaoParalelaDto filtro)
        {
            var usuarioLogado = await mediator.Send(new ObterUsuarioLogadoQuery());
            await mediator.Send(new ValidaSeExisteTurmaPorCodigoQuery(filtro.TurmaCodigo));

            return await mediator.Send(new GerarRelatorioCommand(TipoRelatorio.RecuperacaoParalela, filtro, usuarioLogado));
        }
    }
}

﻿using MediatR;
using Newtonsoft.Json;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao.Commands
{
    public class SalvarPlanoAEERespostaCommandHandler : IRequestHandler<SalvarPlanoAEERespostaCommand, long>
    {
        private readonly IMediator mediator;
        private readonly IRepositorioPlanoAEEResposta repositorioPlanoAEEResposta;

        public SalvarPlanoAEERespostaCommandHandler(IMediator mediator, IRepositorioPlanoAEEResposta repositorioPlanoAEEResposta)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.repositorioPlanoAEEResposta = repositorioPlanoAEEResposta ?? throw new ArgumentNullException(nameof(repositorioPlanoAEEResposta));
        }

        public async Task<long> Handle(SalvarPlanoAEERespostaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var planoAEEQuestao = await MapearParaEntidade(request);
                return await repositorioPlanoAEEResposta.SalvarAsync(planoAEEQuestao);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<PlanoAEEResposta> MapearParaEntidade(SalvarPlanoAEERespostaCommand request)
        {

            var resposta = new PlanoAEEResposta()
            {
                PlanoAEEQuestaoId = request.PlanoAEEQuestaoId
            };

            if (EnumExtension.EhUmDosValores(request.TipoQuestao, new Enum[] { TipoQuestao.Periodo }))
            {
                ConveterRespostaPeriodoEmDatas(request, resposta);
            }

            if (!String.IsNullOrEmpty(request.Resposta) && EnumExtension.EhUmDosValores(request.TipoQuestao, new Enum[] { TipoQuestao.Radio, TipoQuestao.Combo, TipoQuestao.Checkbox, TipoQuestao.ComboMultiplaEscolha }))
            {
                resposta.RespostaId = long.Parse(request.Resposta);
            }

            if (EnumExtension.EhUmDosValores(request.TipoQuestao, new Enum[] { TipoQuestao.Frase, TipoQuestao.Texto, TipoQuestao.AtendimentoClinico }))
            {
                resposta.Texto = request.Resposta;
            }

            if (!String.IsNullOrEmpty(request.Resposta) && EnumExtension.EhUmDosValores(request.TipoQuestao, new Enum[] { TipoQuestao.Upload }))
            {
                var arquivoCodigo = Guid.Parse(request.Resposta);
                resposta.ArquivoId = await mediator.Send(new ObterArquivoIdPorCodigoQuery(arquivoCodigo));
            }

            return resposta;
        }

        private static void ConveterRespostaPeriodoEmDatas(SalvarPlanoAEERespostaCommand request, PlanoAEEResposta resposta)
        {
            var respostaRetorno = request.Resposta.Replace("\\", "").Replace("\"", "").Replace("[", "").Replace("]", "");
            string[] periodos = respostaRetorno.ToString().Split(',');
            resposta.PeriodoInicio = DateTime.Parse(periodos[0]).Date;
            resposta.PeriodoFim = DateTime.Parse(periodos[1]).Date;
        }
    }
}

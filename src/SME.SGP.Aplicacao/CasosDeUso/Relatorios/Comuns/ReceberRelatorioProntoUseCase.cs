﻿using MediatR;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Entidades;
using SME.SGP.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{


    public class ReceberRelatorioProntoUseCase : IReceberRelatorioProntoUseCase
    {
        private readonly IMediator mediator;
        private readonly IUnitOfWork unitOfWork;

        public ReceberRelatorioProntoUseCase(IMediator mediator, IUnitOfWork unitOfWork)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            this.unitOfWork = unitOfWork ?? throw new System.ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<bool> Executar(MensagemRabbit mensagemRabbit)
        {

            var relatorioCorrelacao = await mediator.Send(new ObterCorrelacaoRelatorioQuery(mensagemRabbit.CodigoCorrelacao));

            if (relatorioCorrelacao == null)
                throw new NegocioException($"Não foi possível obter a correlação do relatório pronto {mensagemRabbit.CodigoCorrelacao}");

            unitOfWork.IniciarTransacao();

            var receberRelatorioProntoCommand = mensagemRabbit.ObterObjetoFiltro<ReceberRelatorioProntoCommand>();
            receberRelatorioProntoCommand.RelatorioCorrelacao = relatorioCorrelacao;

            var relatorioCorrelacaoJasper = await mediator.Send(receberRelatorioProntoCommand);

            relatorioCorrelacao.AdicionarCorrelacaoJasper(relatorioCorrelacaoJasper);

            switch (relatorioCorrelacao.TipoRelatorio)
            {
                case TipoRelatorio.RelatorioExemplo:
                    break;
                case TipoRelatorio.ConselhoClasseAluno:
                case TipoRelatorio.ConselhoClasseTurma:
                    EnviaNotificacaoCriador(relatorioCorrelacao);
                    break;
                default:
                    break;
            }


            unitOfWork.PersistirTransacao();
            return await Task.FromResult(true);
        }

        private void EnviaNotificacaoCriador(RelatorioCorrelacao relatorioCorrelacao)
        {
            //mediator.Send()   
        }
    }
}

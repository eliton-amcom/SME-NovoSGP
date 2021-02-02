﻿using MediatR;
using SME.SGP.Aplicacao.Integracoes;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public class EnviarParaAnaliseEncaminhamentoAEECommandHandler : IRequestHandler<EnviarParaAnaliseEncaminhamentoAEECommand, bool>
    {
        private readonly IMediator mediator;
        private readonly IRepositorioEncaminhamentoAEE repositorioEncaminhamentoAEE;
        private readonly IServicoEol servicoEol;



        public EnviarParaAnaliseEncaminhamentoAEECommandHandler(IMediator mediator, IRepositorioEncaminhamentoAEE repositorioEncaminhamentoAEE, IServicoEol servicoEol)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.repositorioEncaminhamentoAEE = repositorioEncaminhamentoAEE ?? throw new ArgumentNullException(nameof(repositorioEncaminhamentoAEE));
            this.servicoEol = servicoEol ?? throw new ArgumentNullException(nameof(servicoEol));
        }

        public async Task<bool> Handle(EnviarParaAnaliseEncaminhamentoAEECommand request, CancellationToken cancellationToken)
        {
            var encaminhamentoAEE = await mediator.Send(new ObterEncaminhamentoAEEComTurmaPorIdQuery(request.EncaminhamentoId));

            var turma = await mediator.Send(new ObterTurmaComUeEDrePorIdQuery(encaminhamentoAEE.TurmaId));

            if (turma == null)
                throw new NegocioException("turma não encontrada");

            FiltroFuncionarioDto filtro = new FiltroFuncionarioDto()
            {
                CodigoUE = turma.UeId.ToString()
            };

            if (encaminhamentoAEE == null)
                throw new NegocioException("O encaminhamento informado não foi encontrado");

            encaminhamentoAEE.Situacao = Dominio.Enumerados.SituacaoAEE.AtribuicaoResponsavel;
            
            var funciorarioPAEE = await servicoEol.ObterFuncionariosPorDre(Perfis.PERFIL_PAEE, filtro);

            if (funciorarioPAEE != null && funciorarioPAEE.Count() == 1)
            {
                encaminhamentoAEE.Situacao = Dominio.Enumerados.SituacaoAEE.Analise;
                encaminhamentoAEE.ResponsavelId = await mediator.Send(new ObterUsuarioIdPorRfOuCriaQuery(funciorarioPAEE.FirstOrDefault().CodigoRf));
            }            

            var idEntidadeEncaminhamento = await repositorioEncaminhamentoAEE.SalvarAsync(encaminhamentoAEE);

            return idEntidadeEncaminhamento != 0;
        }
    }
}

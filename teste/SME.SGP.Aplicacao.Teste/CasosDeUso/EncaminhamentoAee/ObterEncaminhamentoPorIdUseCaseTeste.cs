﻿using MediatR;
using Moq;
using SME.SGP.Dominio;
using SME.SGP.Infra.Dtos;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.SGP.Aplicacao.Teste.CasosDeUso
{
    public class ObterEncaminhamentoPorIdUseCaseTeste
    {
        private readonly Mock<IMediator> mediator;
        private readonly ObterEncaminhamentoPorIdUseCase useCase;

        public ObterEncaminhamentoPorIdUseCaseTeste()
        {
            mediator = new Mock<IMediator>();
            useCase = new ObterEncaminhamentoPorIdUseCase(mediator.Object);
        }

        [Fact]
        public async Task Deve_Listar_Encaminhamento()
        {
            mediator.Setup(a => a.Send(It.IsAny<ObterEncaminhamentoAEEComTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EncaminhamentoAEE()
                {
                    Id = 6,
                    TurmaId = 1,
                    Situacao = Dominio.Enumerados.SituacaoAEE.Encaminhado,
                    AlunoCodigo = "1234567",
                    Turma = new Turma()
                    {
                        AnoLetivo = 2021
                    }
                });

            mediator.Setup(a => a.Send(It.IsAny<ObterAlunoPorCodigoEAnoQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new AlunoReduzidoDto()
               {
                   CodigoAluno = "1234567"
               });

            mediator.Setup(a => a.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(new Usuario() { PerfilAtual = Perfis.PERFIL_PROFESSOR });

            var encaminhamentoId = 6;

            //Act
            var retorno = await useCase.Executar(encaminhamentoId);

            //Asert
            Assert.NotNull(retorno);
        }        

        [Fact]
        public async Task Pode_Editar_Encaminhamento_CP()
        {
            mediator.Setup(a => a.Send(It.IsAny<ObterEncaminhamentoAEEComTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EncaminhamentoAEE()
                {
                    Id = 6,
                    TurmaId = 1,
                    Situacao = Dominio.Enumerados.SituacaoAEE.Encaminhado,
                    AlunoCodigo = "1234567",
                    Turma = new Turma()
                    {
                        AnoLetivo = 2021
                    }
                });

            mediator.Setup(a => a.Send(It.IsAny<ObterAlunoPorCodigoEAnoQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new AlunoReduzidoDto()
               {
                   CodigoAluno = "1234567"
               });

            mediator.Setup(a => a.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Usuario() { PerfilAtual = Perfis.PERFIL_CP });            

            var encaminhamentoId = 6;

            //Act
            var retorno = await useCase.Executar(encaminhamentoId);

            //Asert
            Assert.NotNull(retorno);
            Assert.True(retorno.PodeEditar);
        }

        [Fact]
        public async Task Nao_Pode_Editar_Encaminhamento_CP()
        {
            mediator.Setup(a => a.Send(It.IsAny<ObterEncaminhamentoAEEComTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EncaminhamentoAEE()
                {
                    Id = 6,
                    TurmaId = 1,
                    Situacao = Dominio.Enumerados.SituacaoAEE.Rascunho,
                    AlunoCodigo = "1234567",
                    Turma = new Turma()
                    {
                        AnoLetivo = 2021
                    }
                });

            mediator.Setup(a => a.Send(It.IsAny<ObterAlunoPorCodigoEAnoQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new AlunoReduzidoDto()
               {
                   CodigoAluno = "1234567"
               });

            mediator.Setup(a => a.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(new Usuario() { PerfilAtual = Perfis.PERFIL_CP });

            var encaminhamentoId = 6;

            //Act
            var retorno = await useCase.Executar(encaminhamentoId);

            //Asert
            Assert.NotNull(retorno);
            Assert.True(!retorno.PodeEditar);
        }

        [Fact]
        public async Task Pode_Editar_Encaminhamento_Professor()
        {
            mediator.Setup(a => a.Send(It.IsAny<ObterEncaminhamentoAEEComTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EncaminhamentoAEE()
                {
                    Id = 6,
                    TurmaId = 1,
                    AlunoCodigo = "1234567",
                    Situacao = Dominio.Enumerados.SituacaoAEE.Rascunho,
                    Turma = new Turma()
                    {
                        AnoLetivo = 2021
                    }
                });

            mediator.Setup(a => a.Send(It.IsAny<ObterAlunoPorCodigoEAnoQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new AlunoReduzidoDto()
               {
                   CodigoAluno = "1234567"
               });

            mediator.Setup(a => a.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(new Usuario() { PerfilAtual = Perfis.PERFIL_PROFESSOR });

            var encaminhamentoId = 6;

            //Act
            var retorno = await useCase.Executar(encaminhamentoId);

            //Asert
            Assert.NotNull(retorno);
            Assert.True(retorno.PodeEditar);
        }

        [Fact]
        public async Task Nao_Pode_Editar_Encaminhamento_Professor()
        {
            mediator.Setup(a => a.Send(It.IsAny<ObterEncaminhamentoAEEComTurmaPorIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EncaminhamentoAEE()
                {
                    Id = 6,
                    TurmaId = 1,
                    AlunoCodigo = "1234567",
                    Situacao = Dominio.Enumerados.SituacaoAEE.Encaminhado,
                    Turma = new Turma()
                    {
                        AnoLetivo = 2021
                    }
                });

            mediator.Setup(a => a.Send(It.IsAny<ObterAlunoPorCodigoEAnoQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new AlunoReduzidoDto()
               {
                   CodigoAluno = "1234567"
               });

            mediator.Setup(a => a.Send(It.IsAny<ObterUsuarioLogadoQuery>(), It.IsAny<CancellationToken>()))
              .ReturnsAsync(new Usuario() { PerfilAtual = Perfis.PERFIL_PROFESSOR });

            var encaminhamentoId = 6;

            //Act
            var retorno = await useCase.Executar(encaminhamentoId);

            //Asert
            Assert.NotNull(retorno);
            Assert.True(!retorno.PodeEditar);
        }
    }
}
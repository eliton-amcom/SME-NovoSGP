﻿using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.SGP.Aplicacao.Teste.CasosDeUso
{
    public class InserirDevolutivaUseCaseTeste
    {
        private readonly InserirDevolutivaUseCase inserirDevolutivaUseCase;
        private readonly Mock<IMediator> mediator;

        public InserirDevolutivaUseCaseTeste()
        {
            mediator = new Mock<IMediator>();
            inserirDevolutivaUseCase = new InserirDevolutivaUseCase(mediator.Object);
        }

        [Fact]
        public async Task Deve_Inserir_Devolutiva()
        {
            //Arrange
            mediator.Setup(a => a.Send(It.IsAny<InserirDevolutivaCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Infra.AuditoriaDto()
                {
                    Id = 1
                });

            //Act
            var auditoriaDto = await inserirDevolutivaUseCase.Executar(new Infra.InserirDevolutivaDto()
            {
                CodigoComponenteCurricular = 1,
                Descricao = "teste",
                PeriodoInicio = DateTime.Today.AddDays(-15),
                PeriodoFim = DateTime.Today.AddDays(15)
            }); ;

            //Asert
            mediator.Verify(x => x.Send(It.IsAny<InserirDevolutivaCommand>(), It.IsAny<CancellationToken>()), Times.Once);

            Assert.True(auditoriaDto.Id == 1);
        }
    }
}

﻿using FluentValidation;
using MediatR;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SGP.Aplicacao
{
    public class SolicitaRelatorioFaltasFrequenciaCommand : IRequest<Guid>
    {
        public SolicitaRelatorioFaltasFrequenciaCommand(FiltroRelatorioFaltasFrequenciaDto filtro)
        {
            Filtro = filtro;
        }

        public FiltroRelatorioFaltasFrequenciaDto Filtro { get; set; }
    }

    public class SolicitaRelatorioFaltasFrequenciaCommandValidator : AbstractValidator<SolicitaRelatorioFaltasFrequenciaCommand>
    {
        public SolicitaRelatorioFaltasFrequenciaCommandValidator()
        {
            RuleFor(c => c.Filtro)
               .NotEmpty()
               .WithMessage("O filtro deve ser informado para solicitação do relatório.");

        }
    }
}

﻿using FluentValidation;
using MediatR;
using SME.SGP.Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SGP.Aplicacao
{
    public class RemoverProcessoEmExecucaoCommand: IRequest<bool>
    {
        public RemoverProcessoEmExecucaoCommand(IEnumerable<ProcessoExecutando> processosExecutando)
        {
            ProcessosExecutando = processosExecutando;
        }

        public IEnumerable<ProcessoExecutando> ProcessosExecutando { get; set; }
    }

    public class RemoverProcessoEmExecucaoCommandValidator: AbstractValidator<RemoverProcessoEmExecucaoCommand>
    {
        public RemoverProcessoEmExecucaoCommandValidator()
        {
            RuleFor(a => a.ProcessosExecutando)
                .NotEmpty()
                .WithMessage("Necessário informar os processos a serem removidos de execução.");
        }
    }
}

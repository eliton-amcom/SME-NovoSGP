﻿using FluentValidation;
using MediatR;

namespace SME.SGP.Aplicacao
{
    public class SalvarHistoricoNotaFechamentoCommand : IRequest<long>
    {
        public SalvarHistoricoNotaFechamentoCommand(double notaAnterior, double notaNova, long fechamentoNotaId, string criadoRF = "", string criadoPor = "")
        {
            NotaAnterior = notaAnterior;
            NotaNova = notaNova;
            FechamentoNotaId = fechamentoNotaId;
            CriadoRF = criadoRF;
            CriadoPor = criadoPor;
        }

        public double NotaAnterior { get; set; }
        public double NotaNova { get; set; }
        public long FechamentoNotaId { get; set; }
        public string CriadoRF { get; set; }
        public string CriadoPor { get; set; }
    }

    public class SalvarHistoricoNotaFechamentoCommandValidator : AbstractValidator<SalvarHistoricoNotaFechamentoCommand>
    {
        public SalvarHistoricoNotaFechamentoCommandValidator()
        {
            RuleFor(c => c.NotaAnterior)
            .NotEmpty()
            .WithMessage("A nota anteior deve ser informada para geração do histórico");

            RuleFor(c => c.NotaNova)
            .NotEmpty()
            .WithMessage("A nota nova deve ser informada para geração do histórico");

            RuleFor(a => a.FechamentoNotaId)
            .NotEmpty()
            .WithMessage("O id da nota do fechamento deve ser informada para geração do histórico!");
        }
    }
}

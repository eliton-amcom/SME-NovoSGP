﻿using FluentValidation;
using System.Collections.Generic;

namespace SME.SGP.Infra
{
    public class FiltroRelatorioConselhoClasseAtaFinalDto
    {
        public List<string> TurmasCodigos { get; set; }
    }


    public class FiltroRelatorioConselhoClasseAtaFinalDtoValidator : AbstractValidator<FiltroRelatorioConselhoClasseAtaFinalDto>
    {
        public FiltroRelatorioConselhoClasseAtaFinalDtoValidator()
        {
            RuleFor(c => c.TurmasCodigos)
            .NotEmpty()
            .WithMessage("A lista de turmas deve ser informada.");

        }
    }

}

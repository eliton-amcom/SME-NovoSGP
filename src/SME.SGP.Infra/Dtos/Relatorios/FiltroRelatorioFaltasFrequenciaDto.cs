﻿using FluentValidation;
using SME.SGP.Dominio;
using System.Collections.Generic;

namespace SME.SGP.Infra
{
    public class FiltroRelatorioFaltasFrequenciaDto
    {
        public int AnoLetivo { get; set; }
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public Modalidade Modalidade { get; set; }
        public int? Semestre { get; set; }
        public IEnumerable<string> AnosEscolares { get; set; }
        public IEnumerable<long> ComponentesCurriculares { get; set; }
        public IEnumerable<int> Bimestres { get; set; }
        public TipoRelatorioFaltasFrequencia TipoRelatorio { get; set; }
        public CondicoesRelatorioFaltasFrequencia Condicao { get; set; }
        public int ValorCondicao { get; set; }
        //TODO PEGAR ENUM NA BRANCH DE DEVELOPMENT
        public string Formato { get; set; }
    }


    public class FiltroRelatorioFaltasFrequenciaDtoValidator : AbstractValidator<FiltroRelatorioFaltasFrequenciaDto>
    {
        public FiltroRelatorioFaltasFrequenciaDtoValidator()
        {

            RuleFor(c => c.TipoRelatorio)
            .IsInEnum()
            .WithMessage("O tipo de relatório de faltas ou frequência deve ser informado.");

            RuleFor(c => c.Modalidade)
            .IsInEnum()
            .WithMessage("A modalidade deve ser informada.");

            RuleFor(c => c.Semestre)
            .NotEmpty()
            .WithMessage("Quando a modalidade é EJA o Semestre deve ser informado.")
            .When(c => c.Modalidade == Modalidade.EJA);


            RuleFor(c => c.Condicao)
            .IsInEnum()
            .WithMessage("A condição deve ser informada.");

            RuleFor(c => c.ValorCondicao)
            .NotEmpty()
            .WithMessage("O valor para a condição deve ser informado.");

            RuleFor(c => c.Formato)
           .NotEmpty()
           .WithMessage("O formato deve ser informado.");
        }
    }

}

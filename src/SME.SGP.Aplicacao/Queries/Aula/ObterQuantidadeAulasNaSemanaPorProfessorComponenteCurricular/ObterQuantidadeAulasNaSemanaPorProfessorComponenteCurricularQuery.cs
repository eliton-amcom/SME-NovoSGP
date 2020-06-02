﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SGP.Aplicacao
{
    public class ObterQuantidadeAulasNaSemanaPorProfessorComponenteCurricularQuery: IRequest<int>
    {
        public ObterQuantidadeAulasNaSemanaPorProfessorComponenteCurricularQuery(string turmaCodigo, long componenteCurricular, int semana, string professorRf, bool experienciaPedagogica)
        {
            TurmaCodigo = turmaCodigo;
            ComponenteCurricular = componenteCurricular;
            Semana = semana;
            ProfessorRf = professorRf;
            ExperienciaPedagogica = experienciaPedagogica;
        }

        public string TurmaCodigo { get; set; }
        public long ComponenteCurricular { get; set; }
        public int Semana { get; set; }
        public string ProfessorRf { get; set; }
        public bool ExperienciaPedagogica { get; set; }
    }
}

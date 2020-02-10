﻿using System;
using System.Collections.Generic;

namespace SME.SGP.Infra
{
    public class FechamentoFinalConsultaRetornoDto
    {
        public FechamentoFinalConsultaRetornoDto()
        {
            Alunos = new List<FechamentoFinalConsultaRetornoAlunoDto>();
        }

        public IList<FechamentoFinalConsultaRetornoAlunoDto> Alunos { get; set; }
        public string AuditoriaAlteracao { get; set; }
        public string AuditoriaInclusao { get; set; }
        public bool EhNota { get; set; }
        public bool EhRegencia { get; set; }
        public DateTime EventoData { get; set; }
    }
}
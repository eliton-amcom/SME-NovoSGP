﻿using System.Collections.Generic;

namespace SME.SGP.Infra
{
    public class FechamentoFinalConsultaRetornoAlunoDto
    {
        public FechamentoFinalConsultaRetornoAlunoDto()
        {
            NotasConceitoBimestre = new List<FechamentoFinalConsultaRetornoAlunoNotaConceitoDto>();
            NotasConceitoFinal = new List<FechamentoFinalConsultaRetornoAlunoNotaConceitoDto>();
        }

        public decimal Frequencia { get; set; }
        public string Nome { get; set; }
        public IList<FechamentoFinalConsultaRetornoAlunoNotaConceitoDto> NotasConceitoBimestre { get; set; }
        public IList<FechamentoFinalConsultaRetornoAlunoNotaConceitoDto> NotasConceitoFinal { get; set; }
        public int NumeroChamada { get; set; }
        public int TotalAusenciasCompensadas { get; set; }
    }
}
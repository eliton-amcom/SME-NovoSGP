﻿using SME.SGP.Dominio.Enumerados;
using System.Collections.Generic;

namespace SME.SGP.Dominio
{
    public class EncaminhamentoAEE : EntidadeBase
    {
        public EncaminhamentoAEE()
        {
            Secoes = new List<EncaminhamentoAEESecao>();
        }

        public Turma Turma { get; set; }
        public long TurmaId { get; set; }
        public string AlunoCodigo { get; set; }
        public string AlunoNome { get; set; }
        public SituacaoAEE Situacao { get; set; }
        public bool Excluido { get; set; }
        public string JustificativaEncerramento { get; set; }
        public List<EncaminhamentoAEESecao> Secoes { get; set; }

    }
}

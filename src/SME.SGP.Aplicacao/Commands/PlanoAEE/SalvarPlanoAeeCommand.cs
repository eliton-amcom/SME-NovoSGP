﻿using MediatR;
using SME.SGP.Dominio.Enumerados;
using SME.SGP.Infra;

namespace SME.SGP.Aplicacao
{
    public class SalvarPlanoAeeCommand : IRequest<RetornoPlanoAEEDto>
    {
        public long TurmaId { get; set; }
        public SituacaoPlanoAEE Situacao { get; set; }
        public int AlunoNumero { get; set; }
        public string AlunoNome { get; set; }
        public string AlunoCodigo { get; set; }
       
        public SalvarPlanoAeeCommand()
        {
        }

        public SalvarPlanoAeeCommand(long turmaId, string alunoNome, string alunoCodigo, int alunoNumero)
        {
            TurmaId = turmaId;
            AlunoNome = alunoNome;
            AlunoCodigo = alunoCodigo;
            AlunoNumero = alunoNumero;
            Situacao = SituacaoPlanoAEE.EmAndamento;
        }
    }
}

﻿using SME.SGP.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Dominio.Interfaces
{
    public interface IRepositorioFechamentoNota : IRepositorioBase<FechamentoNota>
    {
        Task<IEnumerable<FechamentoNota>> ObterPorFechamentoTurma(long fechamentoTurmaDisciplinaId);
        Task<FechamentoNota> ObterPorAlunoEFechamento(long fechamentoTurmaDisciplinaId, string codigoAluno);
        IEnumerable<WfAprovacaoNotaFechamento> ObterNotasEmAprovacaoWf(long workFlowId);
        IEnumerable<WfAprovacaoNotaFechamento> ObterNotasEmAprovacaoPorFechamento(long fechamentoTurmaDisciplinaId);
    }
}
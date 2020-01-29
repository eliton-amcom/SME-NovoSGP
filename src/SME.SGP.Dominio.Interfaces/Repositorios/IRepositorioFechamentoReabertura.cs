﻿using SME.SGP.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Dominio.Interfaces
{
    public interface IRepositorioFechamentoReabertura : IRepositorioBase<FechamentoReabertura>
    {
        void ExcluirBimestres(long id);

        Task<IEnumerable<FechamentoReabertura>> Listar(long tipoCalendarioId, long? dreId, long? ueId, long[] ids);

        Task<PaginacaoResultadoDto<FechamentoReabertura>> ListarPaginado(long tipoCalendarioId, string dreCodigo, string ueCodigo, Paginacao paginacao);

        FechamentoReabertura ObterCompleto(long idFechamentoReabertura = 0, long workflowId = 0);

        Task<IEnumerable<FechamentoReaberturaNotificacao>> ObterNotificacoes(long id);

        Task SalvarBimestreAsync(FechamentoReaberturaBimestre fechamentoReabertura);

        Task SalvarNotificacaoAsync(FechamentoReaberturaNotificacao fechamentoReaberturaNotificacao);
    }
}
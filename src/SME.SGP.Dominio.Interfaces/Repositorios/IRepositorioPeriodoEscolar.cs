﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SME.SGP.Infra;

namespace SME.SGP.Dominio.Interfaces
{
    public interface IRepositorioPeriodoEscolar : IRepositorioBase<PeriodoEscolar>
    {
        IEnumerable<PeriodoEscolar> ObterPorTipoCalendario(long tipoCalendarioId);
        Task<IEnumerable<PeriodoEscolar>> ObterPorTipoCalendarioAsync(long tipoCalendarioId);

        PeriodoEscolar ObterPorTipoCalendarioData(long tipoCalendarioId, DateTime data);

        PeriodoEscolar ObterPorTipoCalendarioData(long tipoCalendarioId, DateTime dataInicio, DateTime dataFim);

        Task<IEnumerable<PeriodoEscolar>> ObterPeriodosEmAbertoPorTipoCalendarioData(long tipoCalendarioId, DateTime data);

        Task<bool> PeriodoEmAbertoAsync(long tipoCalendarioId, DateTime dataReferencia, int bimestre = 0, bool ehAnoLetivo = false);

        Task<PeriodoEscolar> ObterUltimoBimestreAsync(int anoLetivo, ModalidadeTipoCalendario modalidade, int semestre = 0);
        Task<int> ObterBimestreAtualAsync(string codigoTurma, ModalidadeTipoCalendario modalidade, DateTime dataReferencia);
    }
}
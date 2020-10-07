﻿using SME.SGP.Infra;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public interface IMigrarPlanejamentoAnualUseCase
    {
        Task<AuditoriaDto> Executar(MigrarPlanejamentoAnualDto dto);
    }
}
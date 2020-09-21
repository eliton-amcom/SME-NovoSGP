﻿using SME.SGP.Infra;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public interface ISalvarPlanejamentoAnualUseCase
    {
        Task<AuditoriaDto> Executar(long turmaId, long componenteCurricularId, SalvarPlanejamentoAnualDto dto);
    }
}
﻿using SME.SGP.Dto;
using System.Collections.Generic;

namespace SME.SGP.Aplicacao
{
    public interface IConsultasSupervisor
    {
        IEnumerable<SupervisorEscolasDto> ObterPorDreESupervisor(string supervisorId, string dreId);
    }
}
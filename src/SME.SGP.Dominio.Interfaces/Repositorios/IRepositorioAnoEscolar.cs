﻿using SME.SGP.Infra;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Dominio.Interfaces
{
    public interface IRepositorioAnoEscolar
    {
        Task<IEnumerable<ModalidadeAnoDto>> ObterPorModalidadeCicloId(Modalidade modalidade, long cicloId);
    }
}

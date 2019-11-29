﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SGP.Dominio.Interfaces
{
    public interface IRepositorioPlanoAula : IRepositorioBase<PlanoAula>
    {
        Task<PlanoAula> ObterPlanoAulaPorAula(long aulaId);
        Task<PlanoAula> ObterPlanoAulaPorDataDisciplina(DateTime data, string turmaId, string disciplinaId);
    }
}

﻿using SME.SGP.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SGP.Dominio.Interfaces
{
    public interface IRepositorioTurma
    {
        IEnumerable<Turma> Sincronizar(IEnumerable<Turma> entidades, IEnumerable<Ue> ues);
    }
}

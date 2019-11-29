﻿using SME.SGP.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SGP.Dominio.Interfaces
{
    public interface IRepositorioUe
    {
        void Sincronizar(IEnumerable<Ue> entidades);
        IEnumerable<Ue> ObterPorCodigos(string[] codigos);
    }
}

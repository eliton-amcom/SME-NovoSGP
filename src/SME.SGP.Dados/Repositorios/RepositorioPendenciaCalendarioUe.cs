﻿using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioPendenciaCalendarioUe : RepositorioBase<PendenciaCalendarioUe>, IRepositorioPendenciaCalendarioUe
    {
        public RepositorioPendenciaCalendarioUe(ISgpContext database) : base(database)
        {
        }
    }
}

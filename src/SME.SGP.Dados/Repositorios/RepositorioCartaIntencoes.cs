﻿using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioCartaIntencoes : RepositorioBase<CartaIntencoes>, IRepositorioCartaIntencoes
    {
        public RepositorioCartaIntencoes(ISgpContext database) : base(database)
        {
        }
    }
}

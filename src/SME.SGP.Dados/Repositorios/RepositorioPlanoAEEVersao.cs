﻿using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioPlanoAEEVersao : RepositorioBase<PlanoAEEVersao>, IRepositorioPlanoAEEVersao
    {
        public RepositorioPlanoAEEVersao(ISgpContext database) : base(database)
        {
        }
    }
}

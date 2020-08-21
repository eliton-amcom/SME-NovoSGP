﻿using Dapper.FluentMap.Dommel.Mapping;
using SME.SGP.Dominio;

namespace SME.SGP.Dados.Mapeamentos
{
    public class MotivoAusenciaMap : DommelEntityMap<MotivoAusencia>
    {
        public MotivoAusenciaMap()
        {
            ToTable("motivo_ausencia");
            Map(e => e.Id).ToColumn("id");
            Map(e => e.descricao).ToColumn("descricao");
        }  
    }
}

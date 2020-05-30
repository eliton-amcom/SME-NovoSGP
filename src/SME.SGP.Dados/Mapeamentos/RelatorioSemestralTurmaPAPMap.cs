﻿using Dapper.FluentMap.Dommel.Mapping;
using SME.SGP.Dominio;

namespace SME.SGP.Dados.Mapeamentos
{
    public class RelatorioSemestralTurmaPAPMap: DommelEntityMap<RelatorioSemestralTurmaPAP>
    {
        public RelatorioSemestralTurmaPAPMap()
        {
            ToTable("relatorio_semestral_turma_pap");
            Map(c => c.TurmaId).ToColumn("turma_id");
        }
    }
}

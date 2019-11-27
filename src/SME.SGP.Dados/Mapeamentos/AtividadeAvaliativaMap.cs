﻿using SME.SGP.Dominio;

namespace SME.SGP.Dados.Mapeamentos
{
    public class AtividadeAvaliativaMap : BaseMap<AtividadeAvaliativa>
    {
        public AtividadeAvaliativaMap()
        {
            ToTable("atividade_avaliativa");
            Map(t => t.DreId).ToColumn("dre_id");
            Map(t => t.UeId).ToColumn("ue_id");
            Map(t => t.ProfessorRf).ToColumn("professor_rf");
            Map(t => t.TurmaId).ToColumn("turma_id");
            Map(t => t.CategoriaId).ToColumn("categoria_id");
            Map(t => t.DisciplinaId).ToColumn("disciplina_id");
            Map(t => t.TipoAvaliacaoId).ToColumn("tipo_avaliacao_id");
            Map(t => t.NomeAvaliacao).ToColumn("nome_avaliacao");
            Map(t => t.DescricaoAvaliacao).ToColumn("descricao_avaliacao");
            Map(t => t.DataAvaliacao).ToColumn("data_avaliacao");
        }
    }
}
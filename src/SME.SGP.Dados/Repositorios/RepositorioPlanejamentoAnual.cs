﻿using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioPlanejamentoAnual : RepositorioBase<PlanejamentoAnual>, IRepositorioPlanejamentoAnual
    {
        public RepositorioPlanejamentoAnual(ISgpContext database) : base(database)
        {
        }

        public override async Task<PlanejamentoAnual> ObterPorIdAsync(long id)
        {
            var sql = @"";
            return await database.Conexao.QueryFirstOrDefaultAsync<PlanejamentoAnual>(sql, new { id });
        }

        public async Task<PlanejamentoAnualPeriodoEscolar> ObterPlanejamentoAnualPeriodoEscolarPorIdAsync(long id)
        {
            var sql = @"";
            return await database.Conexao.QueryFirstOrDefaultAsync<PlanejamentoAnualPeriodoEscolar>(sql, new { id });
        }

        public async Task<PlanejamentoAnual> ObterPorTurmaEComponenteCurricular(long turmaId, long componenteCurricularId, long periodoEscolarId)
        {
            var sql = @"select
	                        pa.*,
	                        pape.*,
                            pe.*,
	                        pac.*,
	                        paoa.*
                        from
	                        planejamento_anual pa
                        inner join planejamento_anual_periodo_escolar pape on
	                        pape.planejamento_anual_id = pa.id
                        inner join planejamento_anual_componente pac on
	                        pac.planejamento_anual_periodo_escolar_id = pape.id
                        inner join planejamento_anual_objetivos_aprendizagem paoa on
	                        paoa.planejamento_anual_componente_id = pac.id
                        inner join periodo_escolar pe on pape.periodo_escolar_id = pe.id
                        where
	                        turma_id = @turmaId
	                        and pa.componente_curricular_id = @componenteCurricularId
	                        and pape.periodo_escolar_id = @periodoEscolarId";

            var planejamentos = new List<PlanejamentoAnual>();
            await database.Conexao.QueryAsync<PlanejamentoAnual, PlanejamentoAnualPeriodoEscolar, PeriodoEscolar, PlanejamentoAnualComponente, PlanejamentoAnualObjetivoAprendizagem, PlanejamentoAnual>(sql,
                (planejamento, periodo, periodosEscolares, componente, objetivo) =>
                {
                    PlanejamentoAnual planejamentoAdicionado = planejamentos.FirstOrDefault(c => c.Id == planejamento.Id);
                    if (planejamentoAdicionado == null)
                    {
                        componente.ObjetivosAprendizagem.Add(objetivo);
                        periodo.ComponentesCurriculares.Add(componente);
                        periodo.PeriodoEscolar = periodosEscolares;
                        planejamento.PeriodosEscolares.Add(periodo);
                        planejamentos.Add(planejamento);
                    }
                    else
                    {
                        var periodoEscolar = planejamentoAdicionado.PeriodosEscolares.FirstOrDefault(c => c.Id == periodo.Id);
                        if (periodoEscolar != null)
                        {
                            var componenteCurricular = periodoEscolar.ComponentesCurriculares.FirstOrDefault(c => c.Id == componente.Id);
                            if (componenteCurricular != null)
                            {
                                var objetivoAprendizagem = componenteCurricular.ObjetivosAprendizagem.FirstOrDefault(c => c.Id == objetivo.Id);
                                if (objetivoAprendizagem == null)
                                {
                                    componenteCurricular.ObjetivosAprendizagem.Add(objetivo);
                                }
                            }
                            else
                            {
                                componenteCurricular.ObjetivosAprendizagem.Add(objetivo);
                                periodoEscolar.ComponentesCurriculares.Add(componenteCurricular);
                            }
                        }
                        else
                        {
                            componente.ObjetivosAprendizagem.Add(objetivo);
                            periodo.ComponentesCurriculares.Add(componente);
                            periodo.PeriodoEscolar = periodosEscolares;
                            planejamentoAdicionado.PeriodosEscolares.Add(periodo);
                        }
                    }
                    return planejamento;
                },
                new { turmaId, componenteCurricularId, periodoEscolarId });

            return planejamentos.FirstOrDefault();
        }

        public async Task<PlanejamentoAnual> ObterPlanejamentoSimplificadoPorTurmaEComponenteCurricular(long turmaId, long componenteCurricularId)
        {
            var sql = @"select
	                        pa.*
                        from
	                        planejamento_anual pa
                        where
	                        turma_id = @turmaId
	                        and pa.componente_curricular_id = @componenteCurricularId";

            return await database.Conexao.QueryFirstOrDefaultAsync<PlanejamentoAnual>(sql, new { turmaId, componenteCurricularId });
        }

        public async Task<PlanejamentoAnual> ObterPlanejamentoAnualPorAnoEscolaBimestreETurma(long turmaId, long periodoEscolarId, long componenteCurricularId)
        {
            var query = @"select pa.id, pa.turma_id, pa.componente_curricular_id, pa.migrado, 
	                        pa.criado_em, pa.alterado_em, pa.criado_por, pa.alterado_por, pa.criado_rf, pa.alterado_rf
                        from planejamento_anual pa
                        inner join planejamento_anual_periodo_escolar pe on pe.planejamento_anual_id = pa.id
                        where turma_id = @turmaId 
                          and periodo_escolar_id = @periodoEscolarId 
                          and componente_curricular_id = @componenteCurricularId";

            return await database.Conexao.QueryFirstOrDefaultAsync<PlanejamentoAnual>(query.ToString(),
                new
                {
                    turmaId,
                    periodoEscolarId,
                    componenteCurricularId
                });

        }

        public async Task<PlanejamentoAnualDto> ObterPlanejamentoAnualSimplificadoPorTurma(long turmaId)
        {
            var sql = @"select
	                        id, 	
	                        turma_id, 	
	                        componente_curricular_id	
                        from
	                        planejamento_anual pa
                        where
	                        turma_id = @turmaId";

            return await database.Conexao.QueryFirstOrDefaultAsync<PlanejamentoAnualDto>(sql, new { turmaId });
        }

        public async Task<long> ExistePlanejamentoAnualParaTurmaPeriodoEComponente(long turmaId, long periodoEscolarId, long componenteCurricularId)
        {
            var query = @"select pe.id
                            from planejamento_anual pa
                           inner join planejamento_anual_periodo_escolar pe on pe.planejamento_anual_id = pa.id
                           where turma_id = @turmaId 
                             and periodo_escolar_id = @periodoEscolarId 
                              and componente_curricular_id = @componenteCurricularId";

            return await database.Conexao.QueryFirstOrDefaultAsync<long>(query.ToString(),
                new
                {
                    turmaId,
                    periodoEscolarId,
                    componenteCurricularId
                });
        }
    }
}

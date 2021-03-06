﻿using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioFechamentoTurma : RepositorioBase<FechamentoTurma>, IRepositorioFechamentoTurma
    {
        public RepositorioFechamentoTurma(ISgpContext database) : base(database)
        {
        }

        public async Task<FechamentoTurma> ObterCompletoPorIdAsync(long fechamentoTurmaId)
        {
            var query = @"select f.*, t.*, ue.*, dre.*, pe.*
                           from fechamento_turma f
                          inner join turma t on t.id = f.turma_id
                          inner join ue on ue.id = t.ue_id
                          inner join dre on dre.id = ue.dre_id
                           left join periodo_escolar pe on pe.id = f.periodo_escolar_id
                          where f.id = @fechamentoTurmaId";

            return (await database.Conexao.QueryAsync<FechamentoTurma, Turma, Ue, Dre, PeriodoEscolar, FechamentoTurma>(query
                , (fechamentoTurma, turma, ue, dre, periodoEscolar) =>
                {
                    ue.Dre = dre;
                    turma.Ue = ue;
                    fechamentoTurma.Turma = turma;
                    fechamentoTurma.PeriodoEscolar = periodoEscolar;

                    return fechamentoTurma;
                }
                , new { fechamentoTurmaId })).FirstOrDefault();
        }

        public async Task<FechamentoTurma> ObterPorTurmaCodigoBimestreAsync(string turmaCodigo, int bimestre = 0)
        {
            var query = new StringBuilder(@"select f.* 
                            from fechamento_turma f
                          inner join turma t on t.id = f.turma_id
                           left join periodo_escolar p on p.id = f.periodo_escolar_id
                           left join tipo_calendario tp on tp.id = p.tipo_calendario_id 
                          where not f.excluido  
                            and t.turma_id = @turmaCodigo ");
            if (bimestre > 0)
                query.AppendLine(" and p.bimestre = @bimestre and not tp.excluido");
            else
                query.AppendLine(" and f.periodo_escolar_id is null");

            return await database.Conexao.QueryFirstOrDefaultAsync<FechamentoTurma>(query.ToString(), new { turmaCodigo, bimestre });
        }

        public async Task<FechamentoTurma> ObterPorTurmaPeriodo(long turmaId, long periodoId = 0)
        {
            var query = new StringBuilder(@"select * 
                            from fechamento_turma 
                           where not excluido 
                            and turma_id = @turmaId ");
            if (periodoId > 0)
                query.AppendLine(" and periodo_escolar_id = @periodoId");
            else
                query.AppendLine(" and periodo_escolar_id is null");

            return await database.Conexao.QueryFirstOrDefaultAsync<FechamentoTurma>(query.ToString(), new { turmaId, periodoId });
        }

        public async Task<bool> VerificaExistePorTurmaCCPeriodoEscolar(long turmaId, long componenteCurricularId, long? periodoEscolarId)
        {
            var query = new StringBuilder(@"select 1 from fechamento_turma ft
                    inner join fechamento_turma_disciplina ftd on
                    ft.id = ftd.fechamento_turma_id 
                    where 
                        not ft.excluido and ft.turma_id = @turmaId and 
                        ftd.disciplina_id = @componenteCurricularId and 
                        ft.periodo_escolar_id = @periodoEscolarId  ");
            
            return await database.Conexao.QueryFirstOrDefaultAsync<bool>(query.ToString(), new { turmaId, componenteCurricularId, periodoEscolarId });
        }
    }
}

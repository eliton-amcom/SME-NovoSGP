﻿using Dapper;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioDiarioBordo: RepositorioBase<DiarioBordo>, IRepositorioDiarioBordo
    {
        public RepositorioDiarioBordo(ISgpContext conexao) : base(conexao) { }

        public async Task<DiarioBordo> ObterPorAulaId(long aulaId)
        {
            var sql = @"select id, aula_id, devolutiva_id, planejamento, reflexoes_replanejamento,
                    criado_em, criado_por, criado_rf, alterado_em, alterado_por, alterado_rf
                    from diario_bordo where aula_id = @aulaId";

            var parametros = new { aulaId = aulaId };

            return await database.QueryFirstOrDefaultAsync<DiarioBordo>(sql, parametros);
        }

        public async Task<bool> ExisteDiarioParaAula(long aulaId)
        {
            var query = "select 1 from diario_bordo where aula_id = @aulaId";

            return (await database.Conexao.QueryAsync<int>(query, new { aulaId })).Any();
        }

        public async Task ExcluirDiarioBordoDaAula(long aulaId)
        {
            // Excluir plano de aula
            var command = "update diario_bordo set excluido = true where not excluido and aula_id = @aulaId";
            await database.ExecuteAsync(command, new { aulaId });
        }

        public async Task<PaginacaoResultadoDto<DiarioBordoDevolutivaDto>> ObterDiariosBordoPorPeriodoPaginado(string turmaCodigo, long componenteCurricularCodigo, DateTime periodoInicio, DateTime periodoFim, Paginacao paginacao)
        {
            var condicao = @"from diario_bordo db 
                         inner join aula a on a.id = db.aula_id
                         where a.turma_id = @turmaCodigo
                           and a.disciplina_id = @componenteCurricularCodigo
                           and a.data_aula between @periodoInicio and @periodoFim ";

            var query = $"select count(0) {condicao}";

            var totalRegistrosDaQuery = await database.Conexao.QueryFirstOrDefaultAsync<int>(query,
                new { turmaCodigo, componenteCurricularCodigo = componenteCurricularCodigo.ToString(), periodoInicio, periodoFim });

            var offSet = "offset @qtdeRegistrosIgnorados rows fetch next @qtdeRegistros rows only";

            query = $"select db.planejamento, a.aula_cj as AulaCj, a.data_aula as Data {condicao} {offSet}";

            return new PaginacaoResultadoDto<DiarioBordoDevolutivaDto>()
            {
                Items = await database.Conexao.QueryAsync<DiarioBordoDevolutivaDto>(query,
                                                    new
                                                    {
                                                        turmaCodigo,
                                                        componenteCurricularCodigo = componenteCurricularCodigo.ToString(),
                                                        periodoInicio,
                                                        periodoFim,
                                                        qtdeRegistrosIgnorados = paginacao.QuantidadeRegistrosIgnorados,
                                                        qtdeRegistros = paginacao.QuantidadeRegistros
                                                    }),
                TotalRegistros = totalRegistrosDaQuery,
                TotalPaginas = (int)Math.Ceiling((double)totalRegistrosDaQuery / paginacao.QuantidadeRegistros)
            };
        }

        public async Task<IEnumerable<long>> ObterIdsPorDevolutiva(long devolutivaId)
        {
            var query = "select id from diario_bordo where devolutiva_id = @devolutivaId";

            return await database.Conexao.QueryAsync<long>(query, new { devolutivaId });
        }
    }
}

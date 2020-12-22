﻿using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using NpgsqlTypes;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioPendenciaAula : IRepositorioPendenciaAula
    {
        private readonly ISgpContext database;
        private readonly string connectionString;

        public RepositorioPendenciaAula(ISgpContext database, IConfiguration configuration)
        {
            this.database = database;
            this.connectionString = configuration.GetConnectionString("SGP_Postgres");
        }


        public async Task<IEnumerable<Aula>> ListarPendenciasPorTipo(TipoPendenciaAula tipoPendenciaAula, string tabelaReferencia, long[] modalidades)
        {
            var query = $@"select
	                        aula.id as Id
                        from
	                        aula
                        inner join turma on 
	                        aula.turma_id = turma.turma_id
                        left join pendencia_aula on
	                        aula.id = aula_id
	                        and pendencia_aula.tipo = @tipo
                        left join {tabelaReferencia} on
	                        aula.id = {tabelaReferencia}.aula_id
                        where
	                        not aula.excluido
	                        and aula.data_aula < @hoje
	                        and turma.modalidade_codigo = ANY(@modalidades)
	                        and pendencia_aula.id is null
	                        and {tabelaReferencia}.id is null
                        group by
	                        aula.id";

            return (await database.Conexao.QueryAsync<Aula>(query, new { hoje = DateTime.Today, tipo = tipoPendenciaAula, modalidades }));
        }


        public async Task<IEnumerable<Aula>> ListarPendenciasAtividadeAvaliativa()
        {
            var sql = @"select
	                        a.id as Id
                        from
	                        atividade_avaliativa aa
                        inner join atividade_avaliativa_disciplina aad on
	                        aad.atividade_avaliativa_id = aa.id
                        left join notas_conceito n on
	                        aa.id = n.atividade_avaliativa
                        inner join aula a on
	                        aa.turma_id = a.turma_id
	                        and aa.data_avaliacao::date = a.data_aula::date
	                        and aad.disciplina_id = a.disciplina_id
                        left join pendencia_aula on
	                        a.id = aula_id
	                        and pendencia_aula.tipo = @tipo
                        where
	                        not a.excluido
	                        and a.data_aula::date < @hoje
	                        and n.id is null
	                        and pendencia_aula.id is null
                        group by
	                        a.id";

            return (await database.Conexao.QueryAsync<Aula>(sql.ToString(), new { hoje = DateTime.Today, tipo = TipoPendenciaAula.Avaliacao }));
        }


        public async Task<PendenciaAula> ObterPendenciaPorAulaIdETipo(TipoPendenciaAula tipoPendenciaAula, long aulaId)
        {
            var query = $@"select id as Id, aula_id as AulaId, tipo as TipoPendenciaAula from pendencia_aula 
                    WHERE tipo = @tipo AND aula_id = @aulaid";

            using (var conexao = new NpgsqlConnection(connectionString))
            {
                await conexao.OpenAsync();

                var pendencia = (await database.Conexao.QueryFirstOrDefaultAsync<PendenciaAula>(query, new { aulaid = aulaId, tipo = tipoPendenciaAula }));

                conexao.Close();

                return pendencia;
            }


        }

        public async Task ExcluirPorIdAsync(long id)
        {
            using (var conexao = new NpgsqlConnection(connectionString))
            {
                await conexao.OpenAsync();

                await database.Conexao.ExecuteAsync("delete from pendencia_aula where @id = @id", new { id });

                conexao.Close();
            }
        }

        public async Task Salvar(PendenciaAula pendencia)
        {
            await database.Conexao.InsertAsync(pendencia);
        }

        public void SalvarVarias(IEnumerable<Aula> aulas, TipoPendenciaAula tipoPendenciaAula)
        {
            var sql = @"copy pendencia_aula (aula_id, tipo)
                            from
                            stdin (FORMAT binary)";
            using (var writer = ((NpgsqlConnection)database.Conexao).BeginBinaryImport(sql))
            {
                foreach (var aula in aulas)
                {
                    writer.StartRow();
                    writer.Write(aula.Id, NpgsqlDbType.Bigint);
                    writer.Write((long)tipoPendenciaAula, NpgsqlDbType.Bigint);
                }
                writer.Complete();
            }
        }

        public async Task<long[]> ListarPendenciasPorAulaId(long aulaId)
        {
            var sql = @"select tipo from pendencia_aula where aula_id = @aula group by tipo";

            return (await database.Conexao.QueryAsync<long>(sql.ToString(), new { aula = aulaId })).AsList().ToArray();
        }

        public async Task<long[]> ListarPendenciasPorAulasId(long[] aulas)
        {

            var sql = @"select tipo from pendencia_aula where aula_id =ANY(@aulas) group by tipo";

            return (await database.Conexao.QueryAsync<long>(sql.ToString(), new { aulas })).AsList().ToArray();
        }

        public async Task<bool> PossuiPendenciasPorAulasId(long[] aulasId, bool ehInfantil)
        {
            var tabelaReferencia = ehInfantil ? "diario_bordo" : "plano_aula";

            var sql = $@"select
	                       1
                        from
	                        aula
                        inner join turma on 
	                        aula.turma_id = turma.turma_id
	                    left join registro_frequencia rf on
	                        aula.id = rf.aula_id
                        left join {tabelaReferencia} tr on
                            aula.id = tr.aula_id
                        where
	                        not aula.excluido
	                        and aula.id = ANY(@aulas)
                            and (rf.id is null or tr.id is null)
	                        group by
	                        1 ";

            return (await database.Conexao.QuerySingleOrDefaultAsync<bool>(sql, new { aulas = aulasId }));
        }

        public async Task<bool> PossuiPendenciasAtividadeAvaliativaPorAulasId(long[] aulasId)
        {
            var sql = @"select
	                       1
                        from
	                        atividade_avaliativa aa
                        inner join atividade_avaliativa_disciplina aad on
	                        aad.atividade_avaliativa_id = aa.id
                        left join notas_conceito n on
	                        aa.id = n.atividade_avaliativa
                        inner join aula a on
	                        aa.turma_id = a.turma_id
	                        and aa.data_avaliacao::date = a.data_aula::date
	                        and aad.disciplina_id = a.disciplina_id
                        where
	                        not a.excluido
	                        and a.id = ANY(@aulas)
	                        and n.id is null
                        group by
	                        1";

            return (await database.Conexao.QuerySingleOrDefaultAsync<bool>(sql, new { aulas = aulasId }));
        }

        public async Task<PendenciaAulaDto> PossuiPendenciasPorAulaId(long aulaId, bool ehInfantil)
        {
            var tabelaReferencia = ehInfantil ? "diario_bordo" : "plano_aula";
            var propriedadeReferencia = ehInfantil ? "PossuiPendenciaDiarioBordo" : "PossuiPendenciaPlanoAula";

            var sql = $@"select
	                          CASE WHEN rf.id is null THEN 1
                                    ELSE 0
                              END PossuiPendenciaFrequencia,
                              CASE WHEN tr.id is null THEN 1
                                    ELSE 0
                               END {propriedadeReferencia} 
                           from
	                            aula
                            inner join turma on 
	                            aula.turma_id = turma.turma_id
	                        left join registro_frequencia rf on
	                            aula.id = rf.aula_id
                            left join {tabelaReferencia} tr on
                                aula.id =  tr.aula_id
                            where
	                            not aula.excluido
	                            and aula.id = @aula
                                and (rf.id is null or tr.id is null) ";

            return (await database.Conexao.QueryFirstOrDefaultAsync<PendenciaAulaDto>(sql, new { aula = aulaId }));
        }

        public async Task<bool> PossuiPendenciasAtividadeAvaliativaPorAulaId(long aulaId)
        {
            var sql = @"select
	                       1
                        from
	                        atividade_avaliativa aa
                        inner join atividade_avaliativa_disciplina aad on
	                        aad.atividade_avaliativa_id = aa.id
                        left join notas_conceito n on
	                        aa.id = n.atividade_avaliativa
                        inner join aula a on
	                        aa.turma_id = a.turma_id
	                        and aa.data_avaliacao::date = a.data_aula::date
	                        and aad.disciplina_id = a.disciplina_id
                        where
	                        not a.excluido
	                        and a.id = @aula
	                        and n.id is null";

            return (await database.Conexao.QuerySingleOrDefaultAsync<bool>(sql, new { aula = aulaId }));
        }
    }
}

﻿using SME.SGP.Dados.Repositorios;
using SME.SGP.Dominio;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SGP.Dados
{
    public class RepositorioOcorrencia : RepositorioBase<Ocorrencia>, IRepositorioOcorrencia
    {
        public RepositorioOcorrencia(ISgpContext conexao) : base(conexao) { }

        public async Task<IEnumerable<Ocorrencia>> Listar(long turmaId, string titulo, string alunoNome, DateTime? dataOcorrenciaInicio, DateTime? dataOcorrenciaFim, string[] codigosAluno)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(@"select
							o.id,
							o.turma_id,
							o.titulo,
							o.data_ocorrencia,
							o.hora_ocorrencia,
							o.descricao,
							o.ocorrencia_tipo_id,
							o.excluido,
							o.criado_rf,
							o.criado_em,
							o.alterado_em,
							o.alterado_por,
							o.alterado_rf,
							ot.id,
							ot.descricao,
							oa.id,
							oa.codigo_aluno
						from
							ocorrencia o
						inner join ocorrencia_tipo ot on ot.id = o.ocorrencia_tipo_id 
						inner join ocorrencia_aluno oa on oa.ocorrencia_id = o.id
						where not excluido ");

            if (!string.IsNullOrEmpty(titulo))
                query.AppendLine("and lower(f_unaccent(o.titulo)) LIKE lower(f_unaccent(@titulo))");

            if (dataOcorrenciaInicio.HasValue)
                query.AppendLine("and data_ocorrencia::date >= @dataOcorrenciaInicio  ");

            if (dataOcorrenciaFim.HasValue)
                query.AppendLine("and data_ocorrencia::date <= @dataOcorrenciaFim");

            if (codigosAluno != null)
                query.AppendLine("and oa.codigo_aluno = ANY(@codigosAluno)");


            var lstOcorrencias = new Dictionary<long, Ocorrencia>();

            await database.Conexao.QueryAsync<Ocorrencia, OcorrenciaTipo, OcorrenciaAluno, Ocorrencia>(query.ToString(), (ocorrencia, tipo, aluno) =>
            {
                if (!lstOcorrencias.TryGetValue(ocorrencia.Id, out Ocorrencia ocorrenciaEntrada))
                {
                    ocorrenciaEntrada = ocorrencia;
                    ocorrenciaEntrada.OcorrenciaTipo = tipo;
                    lstOcorrencias.Add(ocorrenciaEntrada.Id, ocorrenciaEntrada);
                }

                ocorrenciaEntrada.Alunos.Add(aluno);
                return ocorrenciaEntrada;
            }, new { titulo, alunoNome, dataOcorrenciaInicio, dataOcorrenciaFim, codigosAluno }, splitOn: "id, id");

            return lstOcorrencias.Values.ToList();
        }

        public async Task<IEnumerable<string>> ObterAlunosPorOcorrencia(long ocorrenciaId)
        {
            string query = @"select
							oa.codigo_aluno
						from
							ocorrencia o
						inner join ocorrencia_tipo ot on ot.id = o.ocorrencia_tipo_id 
						inner join ocorrencia_aluno oa on oa.ocorrencia_id = o.id
						where not excluido and o.id = @ocorrenciaId ";

            return await database.Conexao.QueryAsync<string>(query.ToString(), new { ocorrenciaId });
        }

		public override async Task<Ocorrencia> ObterPorIdAsync(long id)
		{
			const string sql = @"select
									o.id,
									o.alterado_em as AlteradoEm,
									o.alterado_por as AlteradoPor,
									o.alterado_rf as AlteradoRf,
									o.criado_em as CriadoEm,
									o.criado_por as CriadoPor,
									o.criado_rf as CriadoRf,
									o.data_ocorrencia as DataOcorrencia,
									o.hora_ocorrencia as HoraOcorrencia,
									o.ocorrencia_tipo_id as OcorrenciaTipoId,
									o.titulo as Titulo,
									o.descricao as Descricao,
									o.excluido as Excluido,
									'-' as splitOn,
									oa.id,
									oa.codigo_aluno as CodigoAluno,
									oa.ocorrencia_id as OcorrenciaId
								FROM 
									public.ocorrencia o
								INNER JOIN
									public.ocorrencia_aluno oa
									ON o.id = oa.ocorrencia_id
								WHERE
									o.id = @id;";

			var cache = new Dictionary<long, Ocorrencia>();
			await database.Conexao.QueryAsync<Ocorrencia, OcorrenciaAluno, Ocorrencia>(sql,
				(ocorrencia, ocorrenciaAluno) =>
				{
					if (!cache.ContainsKey(ocorrencia.Id))
					{
						cache.Add(ocorrencia.Id, ocorrencia);
					}

					var cachedOcorrencia = cache[ocorrencia.Id];
					cachedOcorrencia.Alunos = cachedOcorrencia.Alunos ?? new List<OcorrenciaAluno>();
					cachedOcorrencia.Alunos.Add(ocorrenciaAluno);
					return cachedOcorrencia;
				},
				new { id }, splitOn: "splitOn");

			return cache.Any() ? cache.First().Value : null;
		}

		public async Task<IEnumerable<Ocorrencia>> Listar(long diarioBordoId, long usuarioLogadoId)
		{
			var sql = @"select
							id,
							data_ocorrencia,
							descricao,
							excluido,
							hora_ocorrencia,
							criado_rf as CriadoRf,
							alterado_em as AlteradoEm,
							alterado_por as AlteradoPor,
							alterado_rf as AlteradoRf
						from
							diario_bordo_observacao
						where
							diario_bordo_id = @diarioBordoId
							and not excluido 
                        order by criado_em desc";

			return await database.Conexao.QueryAsync<Ocorrencia>(sql, new { diarioBordoId, usuarioLogadoId });
		}
	}
}

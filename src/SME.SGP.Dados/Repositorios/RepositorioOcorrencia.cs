﻿using SME.SGP.Dados.Repositorios;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
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

        public async Task<PaginacaoResultadoDto<Ocorrencia>> ListarPaginado(long turmaId, string titulo, string alunoNome, DateTime? dataOcorrenciaInicio, DateTime? dataOcorrenciaFim, long[] codigosAluno, Paginacao paginacao)
        {
            StringBuilder condicao = new StringBuilder();

            condicao.AppendLine(@" from
							ocorrencia o
						inner join ocorrencia_tipo ot on ot.id = o.ocorrencia_tipo_id 
						inner join ocorrencia_aluno oa on oa.ocorrencia_id = o.id
						where not o.excluido and turma_id = @turmaId ");

            if (!string.IsNullOrEmpty(titulo))
                condicao.AppendLine("and lower(f_unaccent(o.titulo)) LIKE lower(f_unaccent(@titulo))");

            if (dataOcorrenciaInicio.HasValue)
                condicao.AppendLine("and data_ocorrencia::date >= @dataOcorrenciaInicio  ");

            if (dataOcorrenciaFim.HasValue)
                condicao.AppendLine("and data_ocorrencia::date <= @dataOcorrenciaFim");

            if (codigosAluno != null)
                condicao.AppendLine("and oa.codigo_aluno = ANY(@codigosAluno)");

            var orderBy = "order by o.data_ocorrencia desc";

            if (paginacao == null || (paginacao.QuantidadeRegistros == 0 && paginacao.QuantidadeRegistrosIgnorados == 0))
                paginacao = new Paginacao(1, 10);

            var query = $"select count(0) {condicao}";

            var totalRegistrosDaQuery = await database.Conexao.QueryFirstOrDefaultAsync<int>(query,
               new { titulo, alunoNome, dataOcorrenciaInicio, dataOcorrenciaFim, codigosAluno, turmaId });

            var offSet = "offset @qtdeRegistrosIgnorados rows fetch next @qtdeRegistros rows only";

            query = $@"select
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
							oa.codigo_aluno {condicao} {orderBy} {offSet} ";


            var lstOcorrencias = new Dictionary<long, Ocorrencia>();

            await database.Conexao.QueryAsync<Ocorrencia, OcorrenciaTipo, OcorrenciaAluno, Ocorrencia>(query.ToString(), (ocorrencia, tipo, aluno) =>
            {
                if (!lstOcorrencias.TryGetValue(ocorrencia.Id, out Ocorrencia ocorrenciaEntrada))
                {
                    ocorrenciaEntrada = ocorrencia;
                    ocorrenciaEntrada.OcorrenciaTipo = tipo;
                    lstOcorrencias.Add(ocorrenciaEntrada.Id, ocorrenciaEntrada);
                }

                ocorrenciaEntrada.Alunos = ocorrenciaEntrada.Alunos ?? new List<OcorrenciaAluno>();
                ocorrenciaEntrada.Alunos.Add(aluno);
                return ocorrenciaEntrada;
            }, new
            {
                titulo,
                alunoNome,
                dataOcorrenciaInicio,
                dataOcorrenciaFim,
                codigosAluno,
                turmaId,
                qtdeRegistrosIgnorados = paginacao.QuantidadeRegistrosIgnorados,
                qtdeRegistros = paginacao.QuantidadeRegistros
            }, splitOn: "id, id");

            return new PaginacaoResultadoDto<Ocorrencia>()
            {
                Items = lstOcorrencias.Values.ToList(),
                TotalRegistros = totalRegistrosDaQuery,
                TotalPaginas = (int)Math.Ceiling((double)totalRegistrosDaQuery / paginacao.QuantidadeRegistros)
            };
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
									o.turma_id as TurmaId,
									o.titulo as Titulo,
									o.descricao as Descricao,
									o.excluido as Excluido,
									oa.id,
									oa.codigo_aluno as CodigoAluno,
									oa.ocorrencia_id as OcorrenciaId
								FROM 
									public.ocorrencia o
								INNER JOIN
									public.ocorrencia_aluno oa
									ON o.id = oa.ocorrencia_id
								WHERE
									o.id = @id
									AND not o.excluido;";

            Ocorrencia resultado = null;
            await database.Conexao.QueryAsync<Ocorrencia, OcorrenciaAluno, Ocorrencia>(sql,
                (ocorrencia, ocorrenciaAluno) =>
                {
                    if (resultado is null)
                    {
                        resultado = ocorrencia;
                    }

                    resultado.Alunos = resultado.Alunos ?? new List<OcorrenciaAluno>();
                    resultado.Alunos.Add(ocorrenciaAluno);
                    return resultado;
                },
                new { id });

            return resultado;
        }
    }
}

﻿using SME.SGP.Dominio;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioRegistroIndividual : RepositorioBase<RegistroIndividual>, IRepositorioRegistroIndividual
    {
        public RepositorioRegistroIndividual(ISgpContext database) : base(database)
        {
        }

        public async Task<RegistroIndividual> ObterPorAlunoData(long turmaId, long componenteCurricularId, long alunoCodigo, DateTime data)
        {
            var query = @"select id,
	                            turma_id,
	                            aluno_codigo,
	                            componente_curricular_id,
	                            data_registro,
	                            registro,
	                            criado_em,
	                            criado_por,
	                            criado_rf,
	                            alterado_em,
	                            alterado_por,
	                            alterado_rf,
	                            excluido,
	                            migrado
                        from registro_individual 
                       where not excluido 
                        and turma_id = @turmaId
                        and componente_curricular_id = @componenteCurricularId
                        and aluno_codigo = @alunoCodigo
                        and data_registro::date = @data ";

            return await database.Conexao.QueryFirstOrDefaultAsync<RegistroIndividual>(query, new { turmaId, componenteCurricularId, alunoCodigo, data });
        }

        public async Task<IEnumerable<RegistroIndividual>> ObterPorAlunoPeriodo(long turmaId, long componenteCurricularId, long alunoCodigo, DateTime dataInicio, DateTime dataFim)
        {
            var query = @"select id,
	                            turma_id,
	                            aluno_codigo,
	                            componente_curricular_id,
	                            data_registro,
	                            registro,
	                            criado_em,
	                            criado_por,
	                            criado_rf,
	                            alterado_em,
	                            alterado_por,
	                            alterado_rf,
	                            excluido,
	                            migrado
                        from registro_individual 
                       where not excluido 
                        and turma_id = @turmaId
                        and componente_curricular_id = @componenteCurricularId
                        and aluno_codigo = @alunoCodigo
                        and data_registro::date between @dataInicio and @dataFim ";

            return await database.Conexao.QueryAsync<RegistroIndividual>(query, new { turmaId, componenteCurricularId, alunoCodigo, dataInicio, dataFim });
        }
    }
}
﻿using Dapper;
using SME.SGP.Dados.Contexto;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Entidades;
using SME.SGP.Dominio.Enumerados;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioAbrangencia : IRepositorioAbrangencia
    {
        protected readonly ISgpContext database;

        public RepositorioAbrangencia(ISgpContext database)
        {
            this.database = database;
        }

        public void ExcluirAbrangencias(IEnumerable<long> ids)
        {
            const string comando = @"delete from public.abrangencia where id in (#ids)";

            for (int i = 0; i < ids.Count(); i = i + 900)
            {
                var iteracao = ids.Skip(i).Take(900);

                database.Conexao.Execute(comando.Replace("#ids", string.Join(",", iteracao.Concat(new long[] { 0 }))));
            }
        }

        public void InserirAbrangencias(IEnumerable<Abrangencia> abrangencias, string login)
        {
            foreach (var item in abrangencias)
            {
                const string comando = @"insert into public.abrangencia (usuario_id, dre_id, ue_id, turma_id, perfil)
                                        values ((select id from usuario where login = @login), @dreId, @ueId, @turmaId, @perfil)
                                        RETURNING id";

                database.Conexao.Execute(comando,
                    new
                    {
                        login = login,
                        dreId = item.DreId,
                        ueId = item.UeId,
                        turmaId = item.TurmaId,
                        perfil = item.Perfil
                    });
            }
        }

        public async Task<bool> JaExisteAbrangencia(string login, Guid perfil)
        {
            var query = @"select
	                            1
                            from
	                            abrangencia
                            where
	                            usuario_id = (select id from usuario where login = @login)
	                            and perfil = @perfil";

            return (await database.Conexao.QueryAsync<bool>(query, new { login, perfil })).FirstOrDefault();
        }

        public async Task<IEnumerable<AbrangenciaFiltroRetorno>> ObterAbrangenciaPorFiltro(string texto, string login, Guid perfil)
        {
            texto = $"%{texto.ToUpper()}%";

            var query = @"select
                            va.modalidade_codigo as modalidade,
                            va.turma_ano_letivo as anoLetivo,
                            va.turma_ano as  ano,
                            va.dre_codigo as codigoDre,
                            va.turma_id as codigoTurma,
                            va.ue_codigo as codigoUe,
                            u.tipo_escola as tipoEscola,
                            va.dre_nome as nomeDre,
                            va.turma_nome as nomeTurma,
                            va.ue_nome as nomeUe,
                            va.turma_semestre as semestre,
                            va.qt_duracao_aula as qtDuracaoAula,
                            va.tipo_turno as tipoTurno
                        from
                            v_abrangencia va
                        inner join ue u
	                        on u.ue_id = va.ue_codigo
                        where
                            va.usuario_id = (select id from usuario where login = @login)
                            and va.usuario_perfil = @perfil
                            and (upper(va.turma_nome) like @texto OR upper(f_unaccent(va.ue_nome)) LIKE @texto)
                        order by va.ue_nome
                        OFFSET 0 ROWS FETCH NEXT  10 ROWS ONLY";

            return (await database.Conexao.QueryAsync<AbrangenciaFiltroRetorno>(query, new { texto, login, perfil })).AsList();
        }

        public Task<IEnumerable<AbrangenciaSinteticaDto>> ObterAbrangenciaSintetica(string login, Guid perfil)
        {
            const string Query = @"
                            select
	                            id,
	                            usuario_id,
	                            login,
	                            dre_id,
	                            codigo_dre,
	                            ue_id,
	                            codigo_ue,
	                            turma_id,
	                            codigo_turma,
	                            perfil
                            from
	                            public.v_abrangencia_sintetica where login = @login and perfil = @perfil;";

            return database.Conexao.QueryAsync<AbrangenciaSinteticaDto>(Query, new { login, perfil });
        }

        public async Task<AbrangenciaFiltroRetorno> ObterAbrangenciaTurma(string turma, string login, Guid perfil)
        {
            var query = @"select
                            va.modalidade_codigo as modalidade,
                            va.turma_ano_letivo as anoLetivo,
	                        va.turma_ano as  ano,
                            va.dre_codigo as codigoDre,
                            va.turma_id as codigoTurma,
                            va.ue_codigo as codigoUe,
	                        u.tipo_escola as tipoEscola,
	                        va.dre_nome as nomeDre,
	                        va.turma_nome as nomeTurma,
	                        va.ue_nome as nomeUe,
	                        va.turma_semestre as semestre,
                            va.qt_duracao_aula as qtDuracaoAula,
                            va.tipo_turno as tipoTurno
                        from
                            v_abrangencia va
                        inner join ue u
                            on u.ue_id = va.ue_codigo
                        where
	                        va.usuario_id = (select id from usuario where login = @login)
                            and va.usuario_perfil = @perfil
                            and va.turma_id = @turma
                        order by va.ue_nome";

            return (await database.Conexao.QueryAsync<AbrangenciaFiltroRetorno>(query, new { turma, login, perfil }))
                .FirstOrDefault();
        }

        public async Task<AbrangenciaDreRetorno> ObterDre(string dreCodigo, string ueCodigo, string login, Guid perfil)
        {
            var query = new StringBuilder();

            query.AppendLine("select distinct");
            query.AppendLine("va.dre_codigo as codigo,");
            query.AppendLine("va.dre_nome as nome,");
            query.AppendLine("va.dre_abreviacao as abreviacao");
            query.AppendLine("from");
            query.AppendLine("v_abrangencia va");
            query.AppendLine("where 1=1 ");

            if (!string.IsNullOrEmpty(dreCodigo))
                query.AppendLine("and va.dre_codigo = @dreCodigo");

            if (!string.IsNullOrEmpty(ueCodigo))
                query.AppendLine("and va.ue_codigo = @ueCodigo");

            query.AppendLine("and va.usuario_id = (select id from usuario where login = @login)");
            query.AppendLine("and va.usuario_perfil = @perfil");

            return (await database.Conexao.QueryFirstOrDefaultAsync<AbrangenciaDreRetorno>(query.ToString(), new { dreCodigo, ueCodigo, login, perfil }));
        }

        public async Task<IEnumerable<AbrangenciaDreRetorno>> ObterDres(string login, Guid perfil, Modalidade? modalidade = null, int periodo = 0)
        {
            var query = new StringBuilder();

            query.AppendLine("select distinct");
            query.AppendLine("va.dre_abreviacao as abreviacao,");
            query.AppendLine("va.dre_codigo as codigo,");
            query.AppendLine("va.dre_nome as nome");
            query.AppendLine("from");
            query.AppendLine("v_abrangencia va");
            query.AppendLine("where");
            query.AppendLine("va.usuario_id = (select id from usuario where login = @login)");
            query.AppendLine("and va.usuario_perfil = @perfil");

            if (modalidade.HasValue)
                query.AppendLine("and av.modalidade_codigo = @modalidade");

            if (periodo > 0)
                query.AppendLine("and av.turma_semestre = @semestre");

            return (await database.Conexao.QueryAsync<AbrangenciaDreRetorno>(query.ToString(), new { login, perfil, modalidade = (modalidade.HasValue ? modalidade.Value : 0), semestre = periodo })).AsList();
        }

        public async Task<IEnumerable<int>> ObterModalidades(string login, Guid perfil)
        {
            var query = @"select
	                        distinct ab_turmas.modalidade_codigo
                        from
	                        abrangencia_dres ab_dres
                        inner join abrangencia_ues ab_ues on
	                        ab_ues.abrangencia_dres_id = ab_dres.id
                        inner join abrangencia_turmas ab_turmas on
	                        ab_turmas.abrangencia_ues_id = ab_ues.id
                        where
	                        ab_dres.usuario_id = ( select id from usuario where login = @login)
	                        and ab_dres.perfil = @perfil";

            return (await database.Conexao.QueryAsync<int>(query, new { login, perfil })).AsList();
        }

        public async Task<IEnumerable<int>> ObterSemestres(string login, Guid perfil, Modalidade modalidade)
        {
            var query = @"select distinct ab_turmas.semestre
                    from
	                    abrangencia_turmas ab_turmas
                    inner join abrangencia_ues ab_ues on
	                    ab_turmas.abrangencia_ues_id = ab_ues.id
                    inner join abrangencia_dres ab_dres on
	                    ab_ues.abrangencia_dres_id = ab_dres.id
                    where
                        ab_dres.usuario_id = (select id from usuario where login = @login)
	                    and ab_dres.perfil = @perfil
                        and ab_turmas.modalidade_codigo = @modalidade";

            return (await database.Conexao.QueryAsync<int>(query, new { login, perfil, modalidade })).AsList();
        }

        public async Task<IEnumerable<AbrangenciaTurmaRetorno>> ObterTurmas(string codigoUe, string login, Guid perfil, Modalidade modalidade, int periodo = 0)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(@"select distinct
	                    ab_turmas.ano,
	                    ab_turmas.ano_letivo as anoLetivo,
	                    ab_turmas.turma_id as codigo,
	                    ab_turmas.modalidade_codigo as codigoModalidade,
	                    ab_turmas.nome,
	                    ab_turmas.semestre,
                        ab_turmas.qt_duracao_aula as qtDuracaoAula,
                        ab_turmas.tipo_turno as tipoTurno
                    from
	                    abrangencia_turmas ab_turmas
                    inner join abrangencia_ues ab_ues on
	                    ab_turmas.abrangencia_ues_id = ab_ues.id
                    inner join abrangencia_dres ab_dres on
	                    ab_ues.abrangencia_dres_id = ab_dres.id
                    where
                        ab_ues.ue_id = @codigoUe
	                    and ab_dres.usuario_id = (select id from usuario where login = @login)
	                    and ab_dres.perfil = @perfil");

            if (modalidade > 0)
                query.AppendLine("and ab_turmas.modalidade_codigo = @modalidade");

            if (periodo > 0)
                query.AppendLine("and ab_turmas.semestre = @semestre");

            return (await database.Conexao.QueryAsync<AbrangenciaTurmaRetorno>(query.ToString(), new { codigoUe, login, perfil, modalidade, semestre = periodo })).AsList();
        }

        public async Task<AbrangenciaUeRetorno> ObterUe(string codigo, string login, Guid perfil)
        {
            var query = new StringBuilder();

            query.AppendLine("select distinct");
            query.AppendLine("ab_ues.ue_id as codigo,");
            query.AppendLine("ab_ues.nome,");
            query.AppendLine("ab_ues.tipo_escola as tipoEscola");
            query.AppendLine("from");
            query.AppendLine("abrangencia_turmas ab_turmas");
            query.AppendLine("inner join abrangencia_ues ab_ues on");
            query.AppendLine("ab_turmas.abrangencia_ues_id = ab_ues.id");
            query.AppendLine("inner join abrangencia_dres ab_dres on");
            query.AppendLine("ab_ues.abrangencia_dres_id = ab_dres.id");
            query.AppendLine("where");
            query.AppendLine("ab_ues.ue_id = @codigo");
            query.AppendLine("and ab_dres.usuario_id = (select id from usuario where login = @login)");
            query.AppendLine("and ab_dres.perfil = @perfil");

            return (await database.Conexao.QueryFirstOrDefaultAsync<AbrangenciaUeRetorno>(query.ToString(), new { codigo, login, perfil }));
        }

        public async Task<IEnumerable<AbrangenciaUeRetorno>> ObterUes(string codigoDre, string login, Guid perfil, Modalidade? modalidade = null, int periodo = 0)
        {
            var query = new StringBuilder();

            query.AppendLine("select distinct");
            query.AppendLine("ab_ues.ue_id as codigo,");
            query.AppendLine("ab_ues.nome,");
            query.AppendLine("ab_ues.tipo_escola as tipoEscola");
            query.AppendLine("from");
            query.AppendLine("abrangencia_turmas ab_turmas");
            query.AppendLine("inner join abrangencia_ues ab_ues on");
            query.AppendLine("ab_turmas.abrangencia_ues_id = ab_ues.id");
            query.AppendLine("inner join abrangencia_dres ab_dres on");
            query.AppendLine("ab_ues.abrangencia_dres_id = ab_dres.id");
            query.AppendLine("where");
            query.AppendLine("ab_dres.dre_id = @codigoDre");
            query.AppendLine("and ab_dres.usuario_id = (select id from usuario where login = @login)");
            query.AppendLine("and ab_dres.perfil = @perfil");

            if (modalidade.HasValue)
                query.AppendLine("and ab_turmas.modalidade_codigo = @modalidade");

            if (periodo > 0)
                query.AppendLine("and ab_turmas.semestre = @semestre");

            return (await database.Conexao.QueryAsync<AbrangenciaUeRetorno>(query.ToString(), new { codigoDre, login, perfil, modalidade = (modalidade.HasValue ? modalidade.Value : 0), semestre = periodo })).AsList();
        }

        public async Task RemoverAbrangencias(string login)
        {
            var query = "delete from abrangencia_dres where usuario_id = (select id from usuario where login = @login)";

            await database.ExecuteAsync(query, new { login });
        }

        public void RemoverAbrangenciasForaEscopo(string login, Guid perfil, TipoAbrangencia escopo)
        {
            var query = "delete from abrangencia where usuario_id = (select id from usuario where login = @login) and perfil = @perfil and #escopo";

            switch (escopo)
            {
                case TipoAbrangencia.PorDre:
                    query = query.Replace("#escopo", " ue_id is not null and turma_id is not null");
                    break;
                case TipoAbrangencia.PorUe:
                    query = query.Replace("#escopo", " dre_id is not null and turma_id is not null");
                    break;
                case TipoAbrangencia.PorTurma:
                    query = query.Replace("#escopo", " ue_id is not null and dre_id is not null");
                    break;
            }

            database.Execute(query, new { login, perfil });
        }

        public async Task<long> SalvarDre(AbrangenciaDreRetornoEolDto abrangenciaDre, string login, Guid perfil)
        {
            var query = @"insert into abrangencia_dres
            (usuario_id, dre_id, abreviacao, nome, perfil)values
            ((select id from usuario where login = @login), @dre_id, @abreviacao, @nome, @perfil)
            RETURNING id";

            var resultadoTask = await database.Conexao.QueryAsync<long>(query, new
            {
                dre_id = abrangenciaDre.Codigo,
                abreviacao = abrangenciaDre.Abreviacao,
                nome = abrangenciaDre.Nome,
                login,
                perfil
            });

            return resultadoTask.Single();
        }

        public async Task<long> SalvarTurma(AbrangenciaTurmaRetornoEolDto abrangenciaTurma, long idAbragenciaUe)
        {
            var query = @"insert
	                        into
	                        abrangencia_turmas (turma_id,
	                        abrangencia_ues_id,
	                        nome,
	                        ano_letivo,
	                        ano,
	                        modalidade_codigo,
	                        semestre,
                            qt_duracao_aula,
                            tipo_turno)
                        values(@turma_id,
                        @abrangencia_ues_id,
                        @nome,
                        @ano_letivo,
                        @ano,
                        @modalidade_codigo,
                        @semestre,
                        @qt_duracao_aula,
                        @tipo_turno) returning id";

            var resultadoTask = await database.Conexao.QueryAsync<long>(query, new
            {
                turma_id = abrangenciaTurma.Codigo,
                abrangencia_ues_id = idAbragenciaUe,
                nome = abrangenciaTurma.NomeTurma,
                ano_letivo = abrangenciaTurma.AnoLetivo,
                ano = abrangenciaTurma.Ano,
                modalidade_codigo = int.Parse(abrangenciaTurma.CodigoModalidade),
                semestre = abrangenciaTurma.Semestre,
                qt_duracao_aula = abrangenciaTurma.DuracaoTurno,
                tipo_turno = abrangenciaTurma.TipoTurno
            }); ;

            return resultadoTask.Single();
        }

        public async Task<long> SalvarUe(AbrangenciaUeRetornoEolDto abrangenciaUe, long idAbragenciaDre)
        {
            var query = @"insert into abrangencia_ues
            (ue_id, abrangencia_dres_id, nome, tipo_escola)values(@ue_id, @abrangencia_dres_id, @nome, @tipoEscola)
            RETURNING id";

            var resultadoTask = await database.Conexao.QueryAsync<long>(query, new
            {
                ue_id = abrangenciaUe.Codigo,
                abrangencia_dres_id = idAbragenciaDre,
                nome = abrangenciaUe.Nome,
                tipoEscola = abrangenciaUe.CodTipoEscola
            });

            return resultadoTask.Single();
        }
    }
}
﻿using Dapper;
using SME.SGP.Dados.Contexto;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SME.SGP.Infra;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioTipoCalendario : RepositorioBase<TipoCalendario>, IRepositorioTipoCalendario
    {
        public RepositorioTipoCalendario(ISgpContext conexao) : base(conexao)
        {
        }

        public IEnumerable<TipoCalendario> BuscarPorAnoLetivo(int anoLetivo)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("select *");
            query.AppendLine("from tipo_calendario");
            query.AppendLine("where excluido = false");
            query.AppendLine("and ano_letivo = @anoLetivo");

            return database.Conexao.Query<TipoCalendario>(query.ToString(), new { anoLetivo });
        }

        public TipoCalendario BuscarPorAnoLetivoEModalidade(int anoLetivo, ModalidadeTipoCalendario modalidade)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("select *");
            query.AppendLine("from tipo_calendario");
            query.AppendLine("where excluido = false");
            query.AppendLine("and ano_letivo = @anoLetivo");
            query.AppendLine("and modalidade = @modalidade");

            return database.Conexao.QueryFirstOrDefault<TipoCalendario>(query.ToString(), new { anoLetivo, modalidade = (int)modalidade });
        }

        public override TipoCalendario ObterPorId(long id)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("select * ");
            query.AppendLine("from tipo_calendario ");
            query.AppendLine("where excluido = false ");
            query.AppendLine("and id = @id ");

            return database.Conexao.QueryFirstOrDefault<TipoCalendario>(query.ToString(), new { id });
        }

        public IEnumerable<TipoCalendario> ObterTiposCalendario()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("select");
            query.AppendLine("id,");
            query.AppendLine("nome,");
            query.AppendLine("ano_letivo,");
            query.AppendLine("modalidade,");
            query.AppendLine("periodo");
            query.AppendLine("from tipo_calendario");
            query.AppendLine("where excluido = false");

            return database.Conexao.Query<TipoCalendario>(query.ToString());
        }

        public async Task<bool> VerificarRegistroExistente(long id, string nome)
        {
            StringBuilder query = new StringBuilder();

            var nomeMaiusculo = nome.ToUpper().Trim();
            query.AppendLine("select count(*) ");
            query.AppendLine("from tipo_calendario ");
            query.AppendLine("where upper(nome) = @nomeMaiusculo ");
            query.AppendLine("and excluido = false");

            if (id > 0)
                query.AppendLine("and id <> @id");

            int quantidadeRegistrosExistentes = await database.Conexao.QueryFirstAsync<int>(query.ToString(), new { id, nomeMaiusculo });

            return quantidadeRegistrosExistentes > 0;
        }
    }
}
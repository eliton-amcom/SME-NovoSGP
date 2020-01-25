﻿using Dapper;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioTipoCalendario : RepositorioBase<TipoCalendario>, IRepositorioTipoCalendario
    {
        public RepositorioTipoCalendario(ISgpContext conexao) : base(conexao)
        {
        }

        public TipoCalendario BuscarPorAnoLetivoEModalidade(int anoLetivo, ModalidadeTipoCalendario modalidade)
        {
            StringBuilder query = ObterQueryListarPorAnoLetivo();
            query.AppendLine("and modalidade = @modalidade");

            return database.Conexao.QueryFirstOrDefault<TipoCalendario>(query.ToString(), new { anoLetivo, modalidade = (int)modalidade });
        }

        public IEnumerable<TipoCalendario> ListarPorAnoLetivo(int anoLetivo)
        {
            StringBuilder query = ObterQueryListarPorAnoLetivo();

            return database.Conexao.Query<TipoCalendario>(query.ToString(), new { anoLetivo });
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

        private static StringBuilder ObterQueryListarPorAnoLetivo()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("select *");
            query.AppendLine("from tipo_calendario");
            query.AppendLine("where excluido = false");
            query.AppendLine("and ano_letivo = @anoLetivo");
            return query;
        }
    }
}
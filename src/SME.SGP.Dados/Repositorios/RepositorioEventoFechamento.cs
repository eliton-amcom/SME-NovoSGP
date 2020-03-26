﻿using System;
using System.Threading.Tasks;
using Dapper;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioEventoFechamento : RepositorioBase<EventoFechamento>, IRepositorioEventoFechamento
    {
        public RepositorioEventoFechamento(ISgpContext database) : base(database)
        {
        }

        public EventoFechamento ObterPorIdFechamento(long fechamentoId)
        {
            return database.Conexao.QueryFirstOrDefault<EventoFechamento>("select * from evento_fechamento where fechamento_id = @fechamentoId", new
            {
                fechamentoId,
            });
        }

        public async Task<bool> UeEmFechamento(DateTime dataReferencia, string dreCodigo, string ueCodigo, int bimestre, long tipoCalendarioId)
        {
            var query = @"select count(pf.id) from periodo_fechamento pf 
                        inner join periodo_fechamento_bimestre pfb on pf.id = pfb.periodo_fechamento_id 
                        inner join periodo_escolar pe on pe.id = pfb.periodo_escolar_id
                        inner join dre dre on dre.id = pf.dre_id 
                        inner join ue ue on ue.id = pf.ue_id 
                        where pe.tipo_calendario_id = @tipoCalendarioId
                        and pe.bimestre =@bimestre
                        and ue.ue_id = @ueCodigo
                        and dre.dre_id = @dreCodigo
                        and pfb.inicio_fechamento <= @dataReferencia
                        and pfb.final_fechamento >= @dataReferencia";

            return await database.Conexao.QueryFirstAsync<int>(query, new
            {
                dataReferencia,
                dreCodigo,
                ueCodigo,
                bimestre,
                tipoCalendarioId
            }) > 0;
        }
    }
}
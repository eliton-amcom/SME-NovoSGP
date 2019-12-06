﻿using Dapper;
using SME.SGP.Dados.Contexto;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioNotificacaoFrequencia : RepositorioBase<NotificacaoFrequencia>, IRepositorioNotificacaoFrequencia
    {
        public RepositorioNotificacaoFrequencia(ISgpContext database) : base(database)
        {
        }

        public bool UsuarioNotificado(long usuarioId, TipoNotificacaoFrequencia tipo)
        {
            var query = @"select 0 
                          from notificacao_frequencia f
                         inner join notificacao n on n.codigo = f.notificacao_codigo
                         where n.usuario_id = @usuarioId
                           and f.tipo = @tipo";

            return database.Conexao.Query<int>(query, new { usuarioId, tipo }).Any();
        }

        public IEnumerable<RegistroFrequenciaFaltanteDto> ObterTurmasSemRegistroDeFrequencia(TipoNotificacaoFrequencia tipoNotificacao)
        {
            var query = @"select distinct a.turma_id as CodigoTurma, t.nome as NomeTurma
	                        , ue.ue_id as CodigoUe, ue.nome as NomeUe
	                        , dre.dre_id as CodigoDre, dre.nome as NomeDre
	                        , a.disciplina_id as DisciplinaId
                           from aula a
                          inner join turma t on t.turma_id = a.turma_id
                          inner join ue on ue.id = t.ue_id
                          inner join dre on dre.id = ue.dre_id
                          left join registro_frequencia r on r.aula_id = a.id
                          left join notificacao_frequencia n on n.aula_id = a.id and n.tipo = @tipoNotificacao
                         where not a.excluido
                           and r.id is null
                           and n.id is null
                           and a.data_aula < DATE(now())
                        order by dre.dre_id, ue.ue_id, a.turma_id";

            return database.Conexao.Query<RegistroFrequenciaFaltanteDto>(query, new { tipoNotificacao } );
        }
    }
}

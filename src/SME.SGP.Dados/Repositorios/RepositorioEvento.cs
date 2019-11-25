﻿using Dapper;
using SME.SGP.Dados.Contexto;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Entidades;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.SGP.Dados.Repositorios
{
    public class RepositorioEvento : RepositorioBase<Evento>, IRepositorioEvento
    {
        public RepositorioEvento(ISgpContext conexao) : base(conexao)
        {
        }

        public bool EhEventoLetivoPorTipoDeCalendarioDataDreUe(long tipoCalendarioId, DateTime data, string dreId, string ueId)
        {
            string cabecalho = "select min(letivo) from evento e where e.excluido = false";
            string whereTipoCalendario = "and e.tipo_calendario_id = @tipoCalendarioId";
            StringBuilder query = new StringBuilder();
            query.AppendLine(cabecalho);
            query.AppendLine(whereTipoCalendario);
            if (!string.IsNullOrEmpty(dreId))
                query.AppendLine("and e.dre_id = @dreId and e.ue_id is null");
            else if (string.IsNullOrEmpty(ueId))
                query.AppendLine("and e.dre_id is null and e.ue_id is null");
            query.AppendLine("and e.data_inicio <= @data and e.data_fim >= @data");
            if (!string.IsNullOrEmpty(ueId))
            {
                query.AppendLine("UNION");
                query.AppendLine(cabecalho);
                query.AppendLine(whereTipoCalendario);
                query.AppendLine("and e.dre_id = @dreId and e.ue_id = @ueId");
                query.AppendLine("and e.data_inicio <= @data and e.data_fim >= @data");
            }

            if (!string.IsNullOrEmpty(dreId) || !string.IsNullOrEmpty(ueId))
            {
                query.AppendLine("UNION");
                query.AppendLine(cabecalho);
                query.AppendLine(whereTipoCalendario);
                query.AppendLine("and e.dre_id is null and e.ue_id is null");
                query.AppendLine("and e.data_inicio <= @data and e.data_fim >= @data");
            }

            var retorno = database.Conexao.QueryFirstOrDefault<int?>(query.ToString(), new { tipoCalendarioId, dreId, ueId, data });
            return retorno == 1 || retorno == null;
        }

        public async Task<IEnumerable<Evento>> EventosNosDiasETipo(DateTime dataInicio, DateTime dataFim, TipoEvento tipoEventoCodigo, long tipoCalendarioId, string UeId, string DreId)
        {
            var query = new StringBuilder();

            query.AppendLine("select");
            query.AppendLine("e.id,");
            query.AppendLine("e.nome,");
            query.AppendLine("e.descricao,");
            query.AppendLine("e.data_inicio,");
            query.AppendLine("e.data_fim,");
            query.AppendLine("e.dre_id,");
            query.AppendLine("e.ue_id,");
            query.AppendLine("e.letivo,");
            query.AppendLine("e.feriado_id,");
            query.AppendLine("e.tipo_calendario_id,");
            query.AppendLine("e.tipo_evento_id,");
            query.AppendLine("e.criado_em,");
            query.AppendLine("e.criado_por,");
            query.AppendLine("e.alterado_em,");
            query.AppendLine("e.alterado_por,");
            query.AppendLine("e.criado_rf,");
            query.AppendLine("e.alterado_rf,");
            query.AppendLine("e.status,");
            query.AppendLine("e.wf_aprovacao_id as WorkflowAprovacaoId,");
            query.AppendLine("et.id as TipoEventoId,");
            query.AppendLine("et.id,");
            query.AppendLine("et.codigo,");
            query.AppendLine("et.ativo,");
            query.AppendLine("et.tipo_data,");
            query.AppendLine("et.descricao,");
            query.AppendLine("et.excluido");
            query.AppendLine("from evento e");
            query.AppendLine("inner join");
            query.AppendLine("evento_tipo et");
            query.AppendLine("on e.tipo_evento_id = et.id");
            query.AppendLine("where");
            query.AppendLine("et.codigo = @tipoEventoCodigo");
            query.AppendLine("and et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.excluido = false");

            if (!string.IsNullOrEmpty(UeId))
                query.AppendLine("and e.ue_id = @ueId");

            if (!string.IsNullOrEmpty(DreId))
                query.AppendLine("and e.dre_id = @dreId");

            query.AppendLine("and ((e.data_inicio <= TO_DATE(@dataInicio, 'yyyy/mm/dd') and e.data_fim >= TO_DATE(@dataInicio, 'yyyy/mm/dd'))");
            query.AppendLine("or (e.data_inicio <= TO_DATE(@dataFim, 'yyyy/mm/dd') and e.data_fim >= TO_DATE(@dataFim, 'yyyy/mm/dd'))");
            query.AppendLine("or (e.data_inicio >= TO_DATE(@dataInicio, 'yyyy/mm/dd') and e.data_fim <= TO_DATE(@dataFim, 'yyyy/mm/dd'))");
            query.AppendLine(")");
            query.AppendLine("and e.tipo_calendario_id = @tipoCalendarioId");

            return (await database.Conexao.QueryAsync<Evento>(query.ToString(), new
            {
                dataInicio = dataInicio.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo),
                dataFim = dataFim.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo),
                tipoEventoCodigo = (int)tipoEventoCodigo,
                tipoCalendarioId,
                UeId,
                DreId
            }));
        }

        public bool ExisteEventoNaMesmaDataECalendario(DateTime dataInicio, long tipoCalendarioId)
        {
            var query = "select 1 from evento where data_inicio = @dataInicio and tipo_calendario_id = @tipoCalendarioId;";
            return database.Conexao.QueryFirstOrDefault<bool>(query, new { dataInicio, tipoCalendarioId });
        }

        public bool ExisteEventoPorEventoTipoId(long eventoTipoId)
        {
            var query = "select 1 from evento where tipo_evento_id = @eventoTipoId;";
            return database.Conexao.QueryFirstOrDefault<bool>(query, new { eventoTipoId }); ;
        }

        public bool ExisteEventoPorFeriadoId(long feriadoId)
        {
            var query = "select 1 from evento where feriado_id = @feriadoId;";
            return database.Conexao.QueryFirstOrDefault<bool>(query, new { feriadoId });
        }

        public bool ExisteEventoPorTipoCalendarioId(long tipoCalendarioId)
        {
            var query = "select 1 from evento where tipo_calendario_id = @tipoCalendarioId;";
            return database.Conexao.QueryFirstOrDefault<bool>(query, new { tipoCalendarioId });
        }

        #region Listar

        public async Task<PaginacaoResultadoDto<Evento>> Listar(long? tipoCalendarioId, long? tipoEventoId, string nomeEvento, DateTime? dataInicio, DateTime? dataFim,
            Paginacao paginacao, string dreId, string ueId, Usuario usuario, Guid usuarioPerfil, bool usuarioTemPerfilSupervisorOuDiretor)
        {
            StringBuilder query = new StringBuilder();

            if (paginacao == null)
                paginacao = new Paginacao(1, 10);

            MontaQueryCabecalho(query);
            MontaQueryListarFromWhereParaVisualizacaoGeral(query, tipoCalendarioId, tipoEventoId, dataInicio, dataFim, nomeEvento, dreId, ueId);
            query.AppendLine("union distinct");
            MontaQueryCabecalho(query);
            MontaQueryListarFromWhereParaCriador(query, tipoCalendarioId, tipoEventoId, dataInicio, dataFim, nomeEvento, dreId, ueId);

            if (usuarioTemPerfilSupervisorOuDiretor)
            {
                query.AppendLine("union distinct");
                MontaQueryCabecalho(query);
                MontaQueryListarFromWhereParaSupervisorDiretorVisualizarEmAprovacao(query, tipoCalendarioId, tipoEventoId, dataInicio, dataFim, nomeEvento, dreId, ueId);
            }

            if (paginacao.QuantidadeRegistros != 0)
                query.AppendFormat(" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY ", paginacao.QuantidadeRegistrosIgnorados, paginacao.QuantidadeRegistros);

            if (!string.IsNullOrEmpty(nomeEvento))
            {
                nomeEvento = $"%{nomeEvento}%";
            }

            var retornoPaginado = new PaginacaoResultadoDto<Evento>();

            retornoPaginado.Items = await database.Conexao.QueryAsync<Evento, EventoTipo, Evento>(query.ToString(), (evento, tipoEvento) =>
            {
                evento.AdicionarTipoEvento(tipoEvento);
                return evento;
            }, new
            {
                tipoCalendarioId,
                tipoEventoId,
                nomeEvento,
                dataInicio,
                dataFim,
                dreId,
                ueId,
                usuarioId = usuario.Id,
                usuarioPerfil,
                usuarioRf = usuario.CodigoRf
            },
            splitOn: "EventoId,TipoEventoId");

            var queryCountCabecalho = "select count(distinct e.id)";
            var queryCount = new StringBuilder(queryCountCabecalho);
            MontaQueryListarFromWhereParaVisualizacaoGeral(queryCount, tipoCalendarioId, tipoEventoId, dataInicio, dataFim, nomeEvento, dreId, ueId);

            queryCount.AppendLine("union distinct");
            queryCount.AppendLine(queryCountCabecalho);
            MontaQueryListarFromWhereParaCriador(queryCount, tipoCalendarioId, tipoEventoId, dataInicio, dataFim, nomeEvento, dreId, ueId);

            if (usuarioTemPerfilSupervisorOuDiretor)
            {
                queryCount.AppendLine("union distinct");
                queryCount.AppendLine(queryCountCabecalho);
                MontaQueryListarFromWhereParaSupervisorDiretorVisualizarEmAprovacao(queryCount, tipoCalendarioId, tipoEventoId, dataInicio, dataFim, nomeEvento, dreId, ueId);
            }

            retornoPaginado.TotalRegistros = (await database.Conexao.QueryAsync<int>(queryCount.ToString(), new
            {
                tipoCalendarioId,
                tipoEventoId,
                nomeEvento,
                dataInicio,
                dataFim,
                dreId,
                ueId,
                usuarioId = usuario.Id,
                usuarioPerfil,
                usuarioRf = usuario.CodigoRf
            })).Sum();

            retornoPaginado.TotalPaginas = (int)Math.Ceiling((double)retornoPaginado.TotalRegistros / paginacao.QuantidadeRegistros);

            return retornoPaginado;
        }

        private void MontaQueryListarFromWhereParaCriador(StringBuilder query, long? tipoCalendarioId, long? tipoEventoId, DateTime? dataInicio, DateTime? dataFim, string nomeEvento, string dreId, string ueId)
        {
            query.AppendLine("from");
            query.AppendLine("evento e");
            query.AppendLine("inner");
            query.AppendLine("join evento_tipo et on");
            query.AppendLine("e.tipo_evento_id = et.id");
            query.AppendLine("inner");
            query.AppendLine("join v_abrangencia a on");
            query.AppendLine("a.ue_codigo = e.ue_id");
            query.AppendLine("and a.dre_codigo = e.dre_id");
            query.AppendLine("and a.usuario_id = @usuarioId");
            query.AppendLine("and a.usuario_perfil = @usuarioPerfil");
            query.AppendLine("where");
            query.AppendLine("et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.excluido = false");
            query.AppendLine("and e.status in (2, 3)");
            query.AppendLine("and e.criado_rf = @usuarioRf");

            if (!string.IsNullOrEmpty(dreId))
                query.AppendLine($"and e.dre_id = @dreId");

            if (!string.IsNullOrEmpty(ueId))
                query.AppendLine($"and e.ue_id =  @ueId");

            if (tipoCalendarioId.HasValue)
                query.AppendLine("and e.tipo_calendario_id = @tipoCalendarioId");

            if (tipoEventoId.HasValue)
                query.AppendLine("and e.tipo_evento_id = @tipoEventoId");

            if (dataInicio.HasValue && dataFim.HasValue)
            {
                query.AppendLine("and e.data_inicio >= @dataInicio");
                query.AppendLine("and (e.data_fim is null OR e.data_fim <= @dataFim)");
            }

            if (!string.IsNullOrWhiteSpace(nomeEvento))
                query.AppendLine("and lower(f_unaccent(e.nome)) LIKE @nomeEvento");
        }

        private void MontaQueryListarFromWhereParaSupervisorDiretorVisualizarEmAprovacao(StringBuilder query, long? tipoCalendarioId, long? tipoEventoId, DateTime? dataInicio, DateTime? dataFim, string nomeEvento, string dreId, string ueId)
        {
            query.AppendLine("from");
            query.AppendLine("evento e");
            query.AppendLine("inner");
            query.AppendLine("join evento_tipo et on");
            query.AppendLine("e.tipo_evento_id = et.id");
            query.AppendLine("inner");
            query.AppendLine("join v_abrangencia a on");
            query.AppendLine("a.ue_codigo = e.ue_id");
            query.AppendLine("and a.dre_codigo = e.dre_id");
            query.AppendLine("and a.usuario_id = @usuarioId");
            query.AppendLine("and a.usuario_perfil = @usuarioPerfil");
            query.AppendLine("where");
            query.AppendLine("et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.excluido = false");
            query.AppendLine("and e.status = 2");

            if (!string.IsNullOrEmpty(dreId))
                query.AppendLine($"and e.dre_id = @dreId");

            if (!string.IsNullOrEmpty(ueId))
                query.AppendLine($"and e.ue_id =  @ueId");

            if (tipoCalendarioId.HasValue)
                query.AppendLine("and e.tipo_calendario_id = @tipoCalendarioId");

            if (tipoEventoId.HasValue)
                query.AppendLine("and e.tipo_evento_id = @tipoEventoId");

            if (dataInicio.HasValue && dataFim.HasValue)
            {
                query.AppendLine("and e.data_inicio >= @dataInicio");
                query.AppendLine("and (e.data_fim is null OR e.data_fim <= @dataFim)");
            }

            if (!string.IsNullOrWhiteSpace(nomeEvento))
                query.AppendLine("and lower(f_unaccent(e.nome)) LIKE @nomeEvento");
        }

        private void MontaQueryListarFromWhereParaVisualizacaoGeral(StringBuilder query, long? tipoCalendarioId, long? tipoEventoId, DateTime? dataInicio, DateTime? dataFim, string nomeEvento, string dreId, string ueId)
        {
            query.AppendLine("from");
            query.AppendLine("evento e");
            query.AppendLine("inner");
            query.AppendLine("join evento_tipo et on");
            query.AppendLine("e.tipo_evento_id = et.id");
            query.AppendLine("left");
            query.AppendLine("join v_abrangencia a on");
            query.AppendLine("a.ue_codigo = e.ue_id");
            query.AppendLine("and a.dre_codigo = e.dre_id");
            query.AppendLine("and a.usuario_id = @usuarioId");
            query.AppendLine("and a.usuario_perfil = @usuarioPerfil");
            query.AppendLine("where");
            query.AppendLine("et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.status = 1");
            query.AppendLine("and e.excluido = false");

            query.AppendLine("and((a.usuario_id is null");
            query.AppendLine("and(e.dre_id is null");
            query.AppendLine("and e.ue_id is null))");

            query.AppendLine("or(a.usuario_id is not null))");

            if (!string.IsNullOrEmpty(dreId))
                query.AppendLine($"and e.dre_id = @dreId");

            if (!string.IsNullOrEmpty(ueId))
                query.AppendLine($"and e.ue_id =  @ueId");

            if (tipoCalendarioId.HasValue)
                query.AppendLine("and e.tipo_calendario_id = @tipoCalendarioId");

            if (tipoEventoId.HasValue)
                query.AppendLine("and e.tipo_evento_id = @tipoEventoId");

            if (dataInicio.HasValue && dataFim.HasValue)
            {
                query.AppendLine("and e.data_inicio >= @dataInicio");
                query.AppendLine("and (e.data_fim is null OR e.data_fim <= @dataFim)");
            }

            if (!string.IsNullOrWhiteSpace(nomeEvento))
                query.AppendLine("and lower(f_unaccent(e.nome)) LIKE @nomeEvento");
        }

        #endregion Listar

        #region Eventos Por Dia

        public async Task<IEnumerable<CalendarioEventosNoDiaRetornoDto>> ObterEventosPorDia(CalendarioEventosFiltroDto calendarioEventosMesesFiltro, int mes, int dia,
            Usuario usuario, Guid usuarioPerfil, bool usuarioTemPerfilSupervisorOuDiretor)
        {
            var query = new StringBuilder();

            MontaQueryEventosPorDiaCabecalho(query);
            MontaQueryEventosPorDiaFromWhereVisualizacaoGeral(calendarioEventosMesesFiltro, query, true);

            query.AppendLine("union distinct");

            MontaQueryEventosPorDiaCabecalho(query);
            MontaQueryEventosPorDiaFromWhereVisualizacaoGeral(calendarioEventosMesesFiltro, query, false);

            query.AppendLine("union distinct");

            if (usuarioTemPerfilSupervisorOuDiretor)
            {
                MontaQueryEventosPorDiaCabecalho(query);
                MontaQueryEventosPorDiaVisualizacaoSupervisorDiretor(calendarioEventosMesesFiltro, query, false);

                query.AppendLine("union distinct");

                MontaQueryEventosPorDiaCabecalho(query);
                MontaQueryEventosPorDiaVisualizacaoSupervisorDiretor(calendarioEventosMesesFiltro, query, false);

                query.AppendLine("union distinct");
            }

            MontaQueryEventosPorDiaCabecalho(query);
            MontaQueryEventosPorDiaVisualizacaoCriador(calendarioEventosMesesFiltro, query, true);

            query.AppendLine("union distinct");

            MontaQueryEventosPorDiaCabecalho(query);
            MontaQueryEventosPorDiaVisualizacaoCriador(calendarioEventosMesesFiltro, query, false);

            return await database.Conexao.QueryAsync<CalendarioEventosNoDiaRetornoDto>(query.ToString(), new
            {
                IdTipoCalendario = calendarioEventosMesesFiltro.IdTipoCalendario,
                DreId = calendarioEventosMesesFiltro.DreId,
                UeId = calendarioEventosMesesFiltro.UeId,
                mes,
                dia,
                usuarioId = usuario.Id,
                usuarioPerfil,
                usuarioRf = usuario.CodigoRf
            });
        }

        private static void MontaQueryEventosPorDiaCabecalho(StringBuilder query)
        {
            query.AppendLine("select e.id, e.descricao,");
            query.AppendLine("case");
            query.AppendLine("when e.dre_id is not null then 'DRE'");
            query.AppendLine("when e.ue_id is not null then 'UE'");
            query.AppendLine("when e.ue_id is null and e.dre_id is null then 'SME'");
            query.AppendLine("end as TipoEvento");
        }

        private static void MontaQueryEventosPorDiaFromWhereVisualizacaoGeral(CalendarioEventosFiltroDto calendarioEventosMesesFiltro,
            StringBuilder query, bool EhDataInicial)
        {
            query.AppendLine("from");
            query.AppendLine("evento e");
            query.AppendLine("inner");
            query.AppendLine("join evento_tipo et on");
            query.AppendLine("e.tipo_evento_id = et.id");
            query.AppendLine("left");
            query.AppendLine("join v_abrangencia a on");
            query.AppendLine("a.ue_codigo = e.ue_id");
            query.AppendLine("and a.dre_codigo = e.dre_id");
            query.AppendLine("and a.usuario_id = @usuarioId");
            query.AppendLine("and a.usuario_perfil = @usuarioPerfil");
            query.AppendLine("where");
            query.AppendLine("et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.status = 1");
            query.AppendLine("and e.excluido = false");
            query.AppendLine("and((a.usuario_id is null");
            query.AppendLine("and(e.dre_id is null");
            query.AppendLine("and e.ue_id is null))");
            query.AppendLine("or(a.usuario_id is not null))");

            if (EhDataInicial)
            {
                query.AppendLine("and extract(month from e.data_inicio) = @mes");
                query.AppendLine("and extract(day from e.data_inicio) = @dia");
            }
            else
            {
                query.AppendLine("and extract(month from e.data_fim) = @mes");
                query.AppendLine("and extract(day from e.data_fim) = @dia");
            }
            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.DreId))
                query.AppendLine("and e.dre_id = @DreId");
            if (calendarioEventosMesesFiltro.IdTipoCalendario > 0)
                query.AppendLine("and e.tipo_calendario_id = @IdTipoCalendario");
            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.UeId))
                query.AppendLine("and e.ue_id = @UeId");

            if (calendarioEventosMesesFiltro.EhEventoSme)
                query.AppendLine("and e.ue_id is null and e.dre_id is null");
        }

        private void MontaQueryEventosPorDiaVisualizacaoCriador(CalendarioEventosFiltroDto calendarioEventosMesesFiltro, StringBuilder query, bool EhDataInicial)
        {
            query.AppendLine("from");
            query.AppendLine("evento e");
            query.AppendLine("inner");
            query.AppendLine("join evento_tipo et on");
            query.AppendLine("e.tipo_evento_id = et.id");
            query.AppendLine("inner");
            query.AppendLine("join v_abrangencia a on");
            query.AppendLine("a.ue_codigo = e.ue_id");
            query.AppendLine("and a.dre_codigo = e.dre_id");
            query.AppendLine("and a.usuario_id = @usuarioId");
            query.AppendLine("and a.usuario_perfil = @usuarioPerfil");
            query.AppendLine("where");
            query.AppendLine("et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.excluido = false");
            query.AppendLine("and e.status in (2, 3)");
            query.AppendLine("and e.criado_rf = @usuarioRf");

            if (EhDataInicial)
            {
                query.AppendLine("and extract(month from e.data_inicio) = @mes");
                query.AppendLine("and extract(day from e.data_inicio) = @dia");
            }
            else
            {
                query.AppendLine("and extract(month from e.data_fim) = @mes");
                query.AppendLine("and extract(day from e.data_fim) = @dia");
            }
            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.DreId))
                query.AppendLine("and e.dre_id = @DreId");
            if (calendarioEventosMesesFiltro.IdTipoCalendario > 0)
                query.AppendLine("and e.tipo_calendario_id = @IdTipoCalendario");
            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.UeId))
                query.AppendLine("and e.ue_id = @UeId");

            if (calendarioEventosMesesFiltro.EhEventoSme)
                query.AppendLine("and e.ue_id is null and e.dre_id is null");
        }

        private void MontaQueryEventosPorDiaVisualizacaoSupervisorDiretor(CalendarioEventosFiltroDto calendarioEventosMesesFiltro,
            StringBuilder query, bool EhDataInicial)
        {
            query.AppendLine("from");
            query.AppendLine("evento e");
            query.AppendLine("inner");
            query.AppendLine("join evento_tipo et on");
            query.AppendLine("e.tipo_evento_id = et.id");
            query.AppendLine("inner");
            query.AppendLine("join v_abrangencia a on");
            query.AppendLine("a.ue_codigo = e.ue_id");
            query.AppendLine("and a.dre_codigo = e.dre_id");
            query.AppendLine("and a.usuario_id = @usuarioId");
            query.AppendLine("and a.usuario_perfil = @usuarioPerfil");
            query.AppendLine("where");
            query.AppendLine("et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.excluido = false");
            query.AppendLine("and e.status in (2, 3)");

            if (EhDataInicial)
            {
                query.AppendLine("and extract(month from e.data_inicio) = @mes");
                query.AppendLine("and extract(day from e.data_inicio) = @dia");
            }
            else
            {
                query.AppendLine("and extract(month from e.data_fim) = @mes");
                query.AppendLine("and extract(day from e.data_fim) = @dia");
            }
            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.DreId))
                query.AppendLine("and e.dre_id = @DreId");
            if (calendarioEventosMesesFiltro.IdTipoCalendario > 0)
                query.AppendLine("and e.tipo_calendario_id = @IdTipoCalendario");
            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.UeId))
                query.AppendLine("and e.ue_id = @UeId");

            if (calendarioEventosMesesFiltro.EhEventoSme)
                query.AppendLine("and e.ue_id is null and e.dre_id is null");
        }

        #endregion Eventos Por Dia

        public IEnumerable<Evento> ObterEventosPorRecorrencia(long eventoId, long eventoPaiId, DateTime dataEvento)
        {
            var query = "select * from evento where id <> @eventoId and evento_pai_id = @eventoPaiId and data_inicio ::date >= @dataEvento ";
            return database.Conexao.Query<Evento>(query, new { eventoId, eventoPaiId, dataEvento });
        }

        public IEnumerable<Evento> ObterEventosPorTipoDeCalendarioDreUe(long tipoCalendarioId, string dreId, string ueId)
        {
            var query = ObterEventos(tipoCalendarioId, dreId, ueId);
            return database.Conexao.Query<Evento>(query.ToString(), new { tipoCalendarioId, dreId, ueId });
        }

        public async Task<IEnumerable<Evento>> ObterEventosPorTipoDeCalendarioDreUeDia(long tipoCalendarioId, string dreId, string ueId, DateTime data)
        {
            var query = ObterEventos(tipoCalendarioId, dreId, ueId, null, data.Date);
            return await database.Conexao.QueryAsync<Evento>(query.ToString(), new { tipoCalendarioId, dreId, ueId, data });
        }

        public async Task<IEnumerable<Evento>> ObterEventosPorTipoDeCalendarioDreUeMes(long tipoCalendarioId, string dreId, string ueId, int mes)
        {
            var query = ObterEventos(tipoCalendarioId, dreId, ueId, mes);
            return await database.Conexao.QueryAsync<Evento>(query.ToString(), new { tipoCalendarioId, dreId, ueId, mes });
        }

        public async Task<IEnumerable<Evento>> ObterEventosPorTipoETipoCalendario(long tipoEventoCodigo, long tipoCalendarioId)
        {
            var query = new StringBuilder();
            query.AppendLine("select* from evento e");
            query.AppendLine("where");
            query.AppendLine("e.excluido = false");
            query.AppendLine("and e.tipo_calendario_id = @tipoCalendarioId");
            query.AppendLine("and tipo_evento_id = @tipoEventoCodigo");

            return await database.Conexao.QueryAsync<Evento>(query.ToString(), new
            {
                tipoEventoCodigo,
                tipoCalendarioId
            });
        }

        public Evento ObterPorWorkflowId(long workflowId)
        {
            var query = @"select
	                e.id,
	                e.nome,
	                e.descricao,
	                e.data_inicio,
	                e.data_fim,
	                e.dre_id,
	                e.ue_id,
	                e.letivo,
	                e.feriado_id,
	                e.tipo_calendario_id,
	                e.tipo_evento_id,
	                e.criado_em,
	                e.criado_por,
	                e.alterado_em,
	                e.alterado_por,
	                e.criado_rf,
	                e.alterado_rf,
	                e.status,
                    e.wf_aprovacao_id as WorkflowAprovacaoId,
	                et.id as TipoEventoId,
	                et.id,
	                et.codigo,
	                et.ativo,
	                et.tipo_data,
	                et.descricao,
	                et.excluido,
                    tc.id as TipoCalendarioId,
                    tc.Nome,
                    tc.Ano_Letivo,
                    tc.Situacao
                from
	                evento e
                inner join evento_tipo et on
	                e.tipo_evento_id = et.id
                inner join tipo_calendario tc
                on e.tipo_calendario_id = tc.id
                where et.ativo = true
	            and et.excluido = false
	            and e.excluido = false
                and e.wf_aprovacao_id = @workflowId ";

            return database.Conexao.Query<Evento, EventoTipo, TipoCalendario, Evento>(query.ToString(), (evento, tipoEvento, tipoCalendario) =>
           {
               evento.AdicionarTipoEvento(tipoEvento);
               evento.TipoCalendario = tipoCalendario;
               return evento;
           }, new
           {
               workflowId
           },
            splitOn: "EventoId,TipoEventoId,TipoCalendarioId").FirstOrDefault();
        }

        #region Quantidade Eventos Por Meses

        public async Task<IEnumerable<CalendarioEventosMesesDto>> ObterQuantidadeDeEventosPorMeses(CalendarioEventosFiltroDto calendarioEventosMesesFiltro, Usuario usuario, Guid usuarioPerfil)
        {
            var query = new StringBuilder();

            query.AppendLine("select");
            query.AppendLine("a.mes,");
            query.AppendLine("count(*) eventos");
            query.AppendLine("from");
            query.AppendLine("(");
            query.AppendLine("select");
            query.AppendLine("a.*,");
            query.AppendLine("rank() over(partition by a.id,");
            query.AppendLine("a.mes");
            query.AppendLine("order by");
            query.AppendLine("campo) rank_id");
            query.AppendLine("from");
            query.AppendLine("(");
            query.AppendLine("select");
            query.AppendLine("id,");
            query.AppendLine("extract(month from data_inicio) mes,");
            query.AppendLine("1 campo");
            query.AppendLine("from(");

            MontaQueryEventosPorMesesCabecalho(query, true);

            MontaQueryQuantidadeEventosPorMesesVisualizacaoGeral(calendarioEventosMesesFiltro, query);

            query.AppendLine("union distinct");

            MontaQueryEventosPorMesesCabecalho(query, true);
            MontaQueryEventosPorMesesVisualizacaoCriador(calendarioEventosMesesFiltro, query);

            if (usuario.TemPerfilSupervisorOuDiretor(usuarioPerfil))
            {
                query.AppendLine("union distinct");

                MontaQueryEventosPorMesesCabecalho(query, true);
                MontaQueryEventosPorMesesVisualizacaoDiretorSupervisor(calendarioEventosMesesFiltro, query);
            }
            query.AppendLine("-- fim");
            query.AppendLine(") a");
            query.AppendLine("union all");
            query.AppendLine("select");
            query.AppendLine("id,");
            query.AppendLine("extract(month from data_fim) mes,");
            query.AppendLine("1 campo");
            query.AppendLine("from(");
            query.AppendLine("--inicio");
            MontaQueryEventosPorMesesCabecalho(query, false);

            MontaQueryQuantidadeEventosPorMesesVisualizacaoGeral(calendarioEventosMesesFiltro, query);

            query.AppendLine("union distinct");

            MontaQueryEventosPorMesesCabecalho(query, false);
            MontaQueryEventosPorMesesVisualizacaoCriador(calendarioEventosMesesFiltro, query);

            if (usuario.TemPerfilSupervisorOuDiretor(usuarioPerfil))
            {
                query.AppendLine("union distinct");

                MontaQueryEventosPorMesesCabecalho(query, false);
                MontaQueryEventosPorMesesVisualizacaoDiretorSupervisor(calendarioEventosMesesFiltro, query);
            }
            query.AppendLine("--fim");
            query.AppendLine(") a) a) a");
            query.AppendLine("where");
            query.AppendLine("a.rank_id = 1");
            query.AppendLine("group by");
            query.AppendLine("a.mes");

            return await database.Conexao.QueryAsync<CalendarioEventosMesesDto>(query.ToString(), new
            {
                IdTipoCalendario = calendarioEventosMesesFiltro.IdTipoCalendario,
                DreId = calendarioEventosMesesFiltro.DreId,
                UeId = calendarioEventosMesesFiltro.UeId,
                usuarioId = usuario.Id,
                usuarioPerfil,
                usuarioRf = usuario.CodigoRf
            });
        }

        private static void MontaQueryEventosPorMesesCabecalho(StringBuilder query, bool ehDataInicial)
        {
            if (ehDataInicial)
                query.AppendLine("select distinct e.id, e.data_inicio");
            else query.AppendLine("select distinct e.id, e.data_fim");
        }

        private static void MontaQueryEventosPorMesesVisualizacaoCriador(CalendarioEventosFiltroDto calendarioEventosMesesFiltro, StringBuilder query)
        {
            query.AppendLine("from");
            query.AppendLine("evento e");
            query.AppendLine("inner join evento_tipo et on");
            query.AppendLine("e.tipo_evento_id = et.id");
            query.AppendLine("inner join v_abrangencia a on");
            query.AppendLine("a.ue_codigo = e.ue_id");
            query.AppendLine("and a.dre_codigo = e.dre_id");
            query.AppendLine("and a.usuario_id = @usuarioId");
            query.AppendLine("and a.usuario_perfil = @usuarioPerfil");
            query.AppendLine("where");
            query.AppendLine("et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.excluido = false");
            query.AppendLine("and e.status in (2,3)");
            query.AppendLine("and e.criado_rf = @usuarioRf");

            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.DreId))
                query.AppendLine("and e.dre_id = @DreId");

            if (calendarioEventosMesesFiltro.IdTipoCalendario > 0)
                query.AppendLine("and e.tipo_calendario_id = @IdTipoCalendario");

            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.UeId))
                query.AppendLine("and e.ue_id = @UeId");

            if (calendarioEventosMesesFiltro.EhEventoSme)
                query.AppendLine("and e.ue_id is null and e.dre_id is null");
        }

        private static void MontaQueryEventosPorMesesVisualizacaoDiretorSupervisor(CalendarioEventosFiltroDto calendarioEventosMesesFiltro, StringBuilder query)
        {
            query.AppendLine("from");
            query.AppendLine("evento e");
            query.AppendLine("inner join evento_tipo et on");
            query.AppendLine("e.tipo_evento_id = et.id");
            query.AppendLine("inner join v_abrangencia a on");
            query.AppendLine("a.ue_codigo = e.ue_id");
            query.AppendLine("and a.dre_codigo = e.dre_id");
            query.AppendLine("and a.usuario_id = @usuarioId");
            query.AppendLine("and a.usuario_perfil = @UsuarioPerfil");
            query.AppendLine("where");
            query.AppendLine("et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.excluido = false");
            query.AppendLine("and e.status in (2, 3)");

            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.DreId))
                query.AppendLine("and e.dre_id = @DreId");

            if (calendarioEventosMesesFiltro.IdTipoCalendario > 0)
                query.AppendLine("and e.tipo_calendario_id = @IdTipoCalendario");

            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.UeId))
                query.AppendLine("and e.ue_id = @UeId");

            if (calendarioEventosMesesFiltro.EhEventoSme)
                query.AppendLine("and e.ue_id is null and e.dre_id is null");
        }

        private static void MontaQueryQuantidadeEventosPorMesesVisualizacaoGeral(CalendarioEventosFiltroDto calendarioEventosMesesFiltro, StringBuilder query)
        {
            query.AppendLine("from");
            query.AppendLine("evento e");
            query.AppendLine("inner join evento_tipo et on");
            query.AppendLine("e.tipo_evento_id = et.id");
            query.AppendLine("left join v_abrangencia a on");
            query.AppendLine("a.ue_codigo = e.ue_id");
            query.AppendLine("and a.dre_codigo = e.dre_id");
            query.AppendLine("and a.usuario_id = @usuarioId");
            query.AppendLine("and a.usuario_perfil = @usuarioPerfil");
            query.AppendLine("where");
            query.AppendLine("et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.status = 1");
            query.AppendLine("and e.excluido = false");
            query.AppendLine("-- regra da abrangencia");
            query.AppendLine("and((a.usuario_id is null");
            query.AppendLine("and(e.dre_id is null");
            query.AppendLine("and e.ue_id is null))");
            query.AppendLine("-- caso seja um evento Global, sem vinculo com UE e DRE");
            query.AppendLine("or(a.usuario_id is not null))");

            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.DreId))
                query.AppendLine("and e.dre_id = @DreId");

            if (calendarioEventosMesesFiltro.IdTipoCalendario > 0)
                query.AppendLine("and e.tipo_calendario_id = @IdTipoCalendario");

            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.UeId))
                query.AppendLine("and e.ue_id = @UeId");

            if (calendarioEventosMesesFiltro.EhEventoSme)
                query.AppendLine("and e.ue_id is null and e.dre_id is null");
        }

        #endregion Quantidade Eventos Por Meses

        public async Task<bool> TemEventoNosDiasETipo(DateTime dataInicio, DateTime dataFim, TipoEvento tipoEventoCodigo, long tipoCalendarioId, string UeId, string DreId)
        {
            var query = new StringBuilder();

            query.AppendLine("select count(e.id) from evento e");
            query.AppendLine("inner join");
            query.AppendLine("evento_tipo et");
            query.AppendLine("on e.tipo_evento_id = et.id");
            query.AppendLine("where");
            query.AppendLine("et.codigo = @tipoEventoCodigo");
            query.AppendLine("and et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.excluido = false");

            if (!string.IsNullOrEmpty(UeId))
                query.AppendLine("and e.ue_id = @ueId");

            if (!string.IsNullOrEmpty(DreId))
                query.AppendLine("and e.dre_id = @dreId");

            query.AppendLine("and ((e.data_inicio <= TO_DATE(@dataInicio, 'yyyy/mm/dd') and e.data_fim >= TO_DATE(@dataInicio, 'yyyy/mm/dd'))");
            query.AppendLine("or (e.data_inicio <= TO_DATE(@dataFim, 'yyyy/mm/dd') and e.data_fim >= TO_DATE(@dataFim, 'yyyy/mm/dd'))");
            query.AppendLine("or (e.data_inicio >= TO_DATE(@dataInicio, 'yyyy/mm/dd') and e.data_fim <= TO_DATE(@dataFim, 'yyyy/mm/dd'))");
            query.AppendLine(")");
            query.AppendLine("and e.tipo_calendario_id = @tipoCalendarioId");

            return (await database.Conexao.QueryFirstOrDefaultAsync<int>(query.ToString(), new
            {
                dataInicio = dataInicio.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo),
                dataFim = dataFim.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo),
                tipoEventoCodigo = (int)tipoEventoCodigo,
                tipoCalendarioId,
                UeId,
                DreId
            })) > 0;
        }

        private static void MontaFiltroTipoCalendario(StringBuilder query)
        {
            query.AppendLine("where");
            query.AppendLine("e.excluido = false");
            query.AppendLine("and et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.tipo_calendario_id = @tipoCalendarioId");
        }

        private static void MontaQueryCabecalho(StringBuilder query)
        {
            query.AppendLine("select");
            query.AppendLine("e.id as EventoId,");
            query.AppendLine("e.id,");
            query.AppendLine("e.nome,");
            query.AppendLine("e.descricao,");
            query.AppendLine("e.data_inicio,");
            query.AppendLine("e.data_fim,");
            query.AppendLine("e.dre_id,");
            query.AppendLine("e.letivo,");
            query.AppendLine("e.feriado_id,");
            query.AppendLine("e.tipo_calendario_id,");
            query.AppendLine("e.tipo_evento_id,");
            query.AppendLine("e.ue_id,");
            query.AppendLine("e.criado_em,");
            query.AppendLine("e.criado_por,");
            query.AppendLine("e.alterado_em,");
            query.AppendLine("e.alterado_por,");
            query.AppendLine("e.criado_rf,");
            query.AppendLine("e.alterado_rf,");
            query.AppendLine("et.id as TipoEventoId,");
            query.AppendLine("et.id,");
            query.AppendLine("et.ativo,");
            query.AppendLine("et.tipo_data,");
            query.AppendLine("et.descricao,");
            query.AppendLine("et.excluido");
        }

        private static void MontaQueryFrom(StringBuilder query)
        {
            query.AppendLine("from");
            query.AppendLine("evento e");
            query.AppendLine("inner join evento_tipo et on");
            query.AppendLine("e.tipo_evento_id = et.id");
        }

        #region Quantidade De Eventos Por Dia

        public async Task<IEnumerable<EventosPorDiaRetornoQueryDto>> ObterQuantidadeDeEventosPorDia(CalendarioEventosFiltroDto calendarioEventosMesesFiltro, int mes,
              Usuario usuario, Guid usuarioPerfil, bool usuarioTemPerfilSupervisorOuDiretor)
        {
            var query = new StringBuilder();
            query.AppendLine("select a.dia, a.tipoevento from(");

            MontaQueryQuantidadeDeEventosPorDiaCabecalho(query);
            MontaQueryQuantidadeDeEventosPorDiaWhereFromParaVisualizacaoGeral(calendarioEventosMesesFiltro, query, true);

            query.AppendLine("group by e.id, dia, tipoevento");
            query.AppendLine("union distinct");

            MontaQueryQuantidadeDeEventosPorDiaCabecalho(query);
            MontaQueryQuantidadeDeEventosPorDiaWhereFromParaVisualizacaoGeral(calendarioEventosMesesFiltro, query, false);

            query.AppendLine("group by e.id, dia, tipoevento");
            query.AppendLine("union");

            if (usuarioTemPerfilSupervisorOuDiretor)
            {
                MontaQueryQuantidadeDeEventosPorDiaCabecalho(query);
                MontaQueryQuantidadeDeEventosPorDiaWhereFromParaSupervisorEDiretor(calendarioEventosMesesFiltro, query, true);

                query.AppendLine("group by e.id, dia, tipoevento");
                query.AppendLine("union distinct");

                MontaQueryQuantidadeDeEventosPorDiaCabecalho(query);
                MontaQueryQuantidadeDeEventosPorDiaWhereFromParaSupervisorEDiretor(calendarioEventosMesesFiltro, query, false);

                query.AppendLine("group by e.id, dia, tipoevento");
                query.AppendLine("union distinct");
            }

            MontaQueryQuantidadeDeEventosPorDiaCabecalho(query);
            MontaQueryQuantidadeDeEventosPorDiaWhereFromParaVisualizacaoParaCriador(calendarioEventosMesesFiltro, query, true);

            query.AppendLine("group by e.id, dia, tipoevento");
            query.AppendLine("union distinct");

            MontaQueryQuantidadeDeEventosPorDiaCabecalho(query);
            MontaQueryQuantidadeDeEventosPorDiaWhereFromParaVisualizacaoParaCriador(calendarioEventosMesesFiltro, query, false);

            query.AppendLine("group by e.id, dia, tipoevento");

            query.AppendLine(")a");
            query.AppendLine("order by a.dia");

            return await database.Conexao.QueryAsync<EventosPorDiaRetornoQueryDto>(query.ToString(), new
            {
                IdTipoCalendario = calendarioEventosMesesFiltro.IdTipoCalendario,
                DreId = calendarioEventosMesesFiltro.DreId,
                UeId = calendarioEventosMesesFiltro.UeId,
                mes,
                usuarioId = usuario.Id,
                usuarioPerfil,
                usuarioRf = usuario.CodigoRf
            });
        }

        private static void MontaQueryQuantidadeDeEventosPorDiaCabecalho(StringBuilder query)
        {
            query.AppendLine("select e.id, extract(day from e.data_inicio) as dia,");
            query.AppendLine("case");
            query.AppendLine("when e.dre_id is not null then 'DRE'");
            query.AppendLine("when e.ue_id is not null then 'UE'");
            query.AppendLine("when e.ue_id is null and e.dre_id is null then 'SME'");
            query.AppendLine("end as TipoEvento");
        }

        private static void MontaQueryQuantidadeDeEventosPorDiaWhereFromParaVisualizacaoGeral(CalendarioEventosFiltroDto calendarioEventosMesesFiltro, StringBuilder query, bool ehDataInicio)
        {
            query.AppendLine("from evento e");
            query.AppendLine("inner");
            query.AppendLine("join evento_tipo et on");
            query.AppendLine("e.tipo_evento_id = et.id");
            query.AppendLine("left");
            query.AppendLine("join v_abrangencia a on");
            query.AppendLine("a.ue_codigo = e.ue_id");
            query.AppendLine("and a.dre_codigo = e.dre_id");
            query.AppendLine("and a.usuario_id = @usuarioId");
            query.AppendLine("and a.usuario_perfil = @usuarioPerfil");
            query.AppendLine("where");
            query.AppendLine("et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.status = 1");
            query.AppendLine("and e.excluido = false");
            query.AppendLine("-- regra da abrangencia");
            query.AppendLine("and((a.usuario_id is null");
            query.AppendLine("and(e.dre_id is null");
            query.AppendLine("and e.ue_id is null))");
            query.AppendLine("-- caso seja um evento Global, sem vinculo com UE e DRE");
            query.AppendLine("or(a.usuario_id is not null))");
            query.AppendLine("and ue_id is null and dre_id is null");

            if (ehDataInicio)
                query.AppendLine("and extract(month from e.data_inicio) = @mes");
            else query.AppendLine("and extract(month from e.data_fim) = @mes");

            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.DreId))
                query.AppendLine("and dre_id = @DreId");

            if (calendarioEventosMesesFiltro.IdTipoCalendario > 0)
                query.AppendLine("and tipo_calendario_id = @IdTipoCalendario");

            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.UeId))
                query.AppendLine("and ue_id = @UeId");

            if (calendarioEventosMesesFiltro.EhEventoSme)
                query.AppendLine("and ue_id is null and dre_id is null");
        }

        private void MontaQueryQuantidadeDeEventosPorDiaWhereFromParaSupervisorEDiretor(CalendarioEventosFiltroDto calendarioEventosMesesFiltro, StringBuilder query, bool ehDataInicio)
        {
            query.AppendLine("from");
            query.AppendLine("evento e");
            query.AppendLine("inner");
            query.AppendLine("join evento_tipo et on");
            query.AppendLine("e.tipo_evento_id = et.id");
            query.AppendLine("inner");
            query.AppendLine("join v_abrangencia a on");
            query.AppendLine("a.ue_codigo = e.ue_id");
            query.AppendLine("and a.dre_codigo = e.dre_id");
            query.AppendLine("and a.usuario_id = @usuarioId");
            query.AppendLine("and a.usuario_perfil = @usuarioPerfil");
            query.AppendLine("where");
            query.AppendLine("et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.excluido = false");
            query.AppendLine("and e.status in (2, 3)");

            if (ehDataInicio)
                query.AppendLine("and extract(month from e.data_inicio) = @mes");
            else query.AppendLine("and extract(month from e.data_fim) = @mes");

            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.DreId))
                query.AppendLine("and e.dre_id = @DreId");

            if (calendarioEventosMesesFiltro.IdTipoCalendario > 0)
                query.AppendLine("and e.tipo_calendario_id = @IdTipoCalendario");

            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.UeId))
                query.AppendLine("and e.ue_id = @UeId");

            if (calendarioEventosMesesFiltro.EhEventoSme)
                query.AppendLine("and e.ue_id is null and e.dre_id is null");
        }

        private void MontaQueryQuantidadeDeEventosPorDiaWhereFromParaVisualizacaoParaCriador(CalendarioEventosFiltroDto calendarioEventosMesesFiltro, StringBuilder query, bool ehDataInicio)
        {
            query.AppendLine("from");
            query.AppendLine("evento e");
            query.AppendLine("inner");
            query.AppendLine("join evento_tipo et on");
            query.AppendLine("e.tipo_evento_id = et.id");
            query.AppendLine("inner");
            query.AppendLine("join v_abrangencia a on");
            query.AppendLine("a.ue_codigo = e.ue_id");
            query.AppendLine("and a.dre_codigo = e.dre_id");
            query.AppendLine("and a.usuario_id = @usuarioId");
            query.AppendLine("and a.usuario_perfil = @usuarioPerfil");
            query.AppendLine("where");
            query.AppendLine("et.ativo = true");
            query.AppendLine("and et.excluido = false");
            query.AppendLine("and e.excluido = false");
            query.AppendLine("and e.status in (2, 3)");
            query.AppendLine("and e.criado_rf = @usuarioRf");

            if (ehDataInicio)
                query.AppendLine("and extract(month from e.data_inicio) = @mes");
            else query.AppendLine("and extract(month from e.data_fim) = @mes");

            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.DreId))
                query.AppendLine("and e.dre_id = @DreId");

            if (calendarioEventosMesesFiltro.IdTipoCalendario > 0)
                query.AppendLine("and e.tipo_calendario_id = @IdTipoCalendario");

            if (!string.IsNullOrEmpty(calendarioEventosMesesFiltro.UeId))
                query.AppendLine("and e.ue_id = @UeId");

            if (calendarioEventosMesesFiltro.EhEventoSme)
                query.AppendLine("and e.ue_id is null and e.dre_id is null");
        }

        #endregion Quantidade De Eventos Por Dia

        private string ObterEventos(long tipoCalendarioId, string dreId, string ueId, int? mes = null, DateTime? data = null)
        {
            StringBuilder query = new StringBuilder();
            MontaQueryCabecalho(query);
            MontaQueryFrom(query);
            MontaFiltroTipoCalendario(query);

            if (!string.IsNullOrEmpty(dreId))
                query.AppendLine("and e.dre_id = @dreId and e.ue_id is null");
            else if (string.IsNullOrEmpty(ueId))
                query.AppendLine("and e.dre_id is null and e.ue_id is null");
            if (mes.HasValue)
            {
                query.AppendLine("and (extract(month from e.data_inicio) = @mes");
                query.AppendLine("  or extract(month from e.data_fim) = @mes)");
            }
            if (data.HasValue)
            {
                query.AppendLine("and e.data_inicio <= @data");
                query.AppendLine("and e.data_fim >= @data");
            }

            if (!string.IsNullOrEmpty(ueId))
            {
                query.AppendLine("UNION");
                MontaQueryCabecalho(query);
                MontaQueryFrom(query);
                MontaFiltroTipoCalendario(query);
                query.AppendLine("and e.dre_id = @dreId and e.ue_id = @ueId");
            }
            else if (!string.IsNullOrEmpty(dreId))
                query.AppendLine("and e.ue_id is null");
            if (mes.HasValue)
            {
                query.AppendLine("and (extract(month from e.data_inicio) = @mes");
                query.AppendLine("  or extract(month from e.data_fim) = @mes)");
            }
            if (data.HasValue)
            {
                query.AppendLine("and e.data_inicio <= @data");
                query.AppendLine("and e.data_fim >= @data");
            }

            if (!string.IsNullOrEmpty(dreId) || !string.IsNullOrEmpty(ueId))
            {
                query.AppendLine("UNION");
                MontaQueryCabecalho(query);
                MontaQueryFrom(query);
                MontaFiltroTipoCalendario(query);
                query.AppendLine("and e.dre_id is null and e.ue_id is null");
                if (mes.HasValue)
                {
                    query.AppendLine("and (extract(month from e.data_inicio) = @mes");
                    query.AppendLine("  or extract(month from e.data_fim) = @mes)");
                }
                if (data.HasValue)
                {
                    query.AppendLine("and e.data_inicio <= @data");
                    query.AppendLine("and e.data_fim >= @data");
                }
            }
            return query.ToString();
        }
    }
}
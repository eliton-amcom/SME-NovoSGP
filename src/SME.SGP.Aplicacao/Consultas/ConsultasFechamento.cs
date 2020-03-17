﻿using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public class ConsultasFechamento : IConsultasFechamento
    {
        private readonly IRepositorioDre repositorioDre;
        private readonly IRepositorioEvento repositorioEvento;
        private readonly IRepositorioEventoFechamento repositorioEventoFechamento;
        private readonly IRepositorioFechamentoReabertura repositorioFechamentoReabertura;
        private readonly IConsultasTipoCalendario consultasTipoCalendario;
        private readonly IRepositorioTurma repositorioTurma;
        private readonly IRepositorioUe repositorioUe;
        private readonly IServicoFechamento servicoFechamento;

        public ConsultasFechamento(IServicoFechamento servicoFechamento,
                                IRepositorioTurma repositorioTurma,
                                IRepositorioUe repositorioUe,
                                IRepositorioDre repositorioDre,
                                IConsultasTipoCalendario consultasTipoCalendario,
                                IRepositorioEvento repositorioEvento,
                                IRepositorioEventoFechamento repositorioEventoFechamento,
                                IRepositorioFechamentoReabertura repositorioFechamentoReabertura)
        {
            this.servicoFechamento = servicoFechamento ?? throw new System.ArgumentNullException(nameof(servicoFechamento));
            this.repositorioTurma = repositorioTurma ?? throw new System.ArgumentNullException(nameof(repositorioTurma));
            this.repositorioUe = repositorioUe ?? throw new System.ArgumentNullException(nameof(repositorioUe));
            this.repositorioDre = repositorioDre ?? throw new System.ArgumentNullException(nameof(repositorioDre));
            this.consultasTipoCalendario = consultasTipoCalendario ?? throw new System.ArgumentNullException(nameof(consultasTipoCalendario));
            this.repositorioEvento = repositorioEvento ?? throw new System.ArgumentNullException(nameof(repositorioEvento));
            this.repositorioEventoFechamento = repositorioEventoFechamento ?? throw new System.ArgumentNullException(nameof(repositorioEventoFechamento));
            this.repositorioFechamentoReabertura = repositorioFechamentoReabertura ?? throw new System.ArgumentNullException(nameof(repositorioFechamentoReabertura));
        }

        public async Task<IEnumerable<PeriodoEscolarDto>> ObterPeriodosEmAberto(long ueId)
            => await repositorioEventoFechamento.ObterPeriodosEmAberto(ueId, DateTime.Now.Date);

        public async Task<FechamentoDto> ObterPorTipoCalendarioDreEUe(FiltroFechamentoDto fechamentoDto)
        {
            return await servicoFechamento.ObterPorTipoCalendarioDreEUe(fechamentoDto.TipoCalendarioId, fechamentoDto.DreId, fechamentoDto.UeId);
        }

        public async Task<bool> TurmaEmPeriodoDeFechamento(string turmaCodigo, DateTime dataReferencia, int bimestre = 0)
        {
            var turma = repositorioTurma.ObterPorCodigo(turmaCodigo);
            var tipoCalendario = await consultasTipoCalendario.ObterPorTurma(turma, dataReferencia);

            return await TurmaEmPeriodoDeFechamento(turma, tipoCalendario, dataReferencia);
        }

        public async Task<bool> TurmaEmPeriodoDeFechamento(Turma turma, TipoCalendario tipoCalendario, DateTime dataReferencia, int bimestre = 0)
        {
            var ue = repositorioUe.ObterPorId(turma.UeId);
            var dre = repositorioDre.ObterPorId(ue.DreId);
            var ueEmFechamento = await repositorioEventoFechamento.UeEmFechamento(dataReferencia, dre.CodigoDre, ue.CodigoUe, tipoCalendario.Id, bimestre);

            return ueEmFechamento || await UeEmReaberturaDeFechamento(tipoCalendario.Id, ue.CodigoUe, dre.CodigoDre, bimestre, dataReferencia);
        }

        private async Task<bool> UeEmReaberturaDeFechamento(long tipoCalendarioId, string ueCodigo, string dreCodigo, int bimestre, DateTime dataReferencia)
        {
            // Busca eventos de fechamento na data atual
            var eventosFechamento = await repositorioEvento.EventosNosDiasETipo(dataReferencia, dataReferencia,
                                            TipoEvento.FechamentoBimestre, tipoCalendarioId, ueCodigo, dreCodigo);

            foreach (var eventoFechamento in eventosFechamento)
            {
                // Verifica existencia de reabertura de fechamento com mesmo inicio e fim do evento de fechamento
                var reaberturasPeriodo = await repositorioFechamentoReabertura.ObterReaberturaFechamentoBimestre(
                                                                bimestre,
                                                                eventoFechamento.DataInicio,
                                                                eventoFechamento.DataFim,
                                                                tipoCalendarioId,
                                                                dreCodigo,
                                                                ueCodigo);

                if (reaberturasPeriodo != null && reaberturasPeriodo.Any())
                    return true;
            }

            return false;
        }
    }
}
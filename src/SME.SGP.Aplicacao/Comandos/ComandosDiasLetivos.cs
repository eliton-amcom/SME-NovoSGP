using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Dto;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public class ComandosDiasLetivos : IComandosDiasLetivos
    {
        private readonly IRepositorioParametrosSistema repositorioParametrosSistema;
        private readonly IRepositorioPeriodoEscolar repositorioPeriodoEscolar;

        public ComandosDiasLetivos(
            IRepositorioPeriodoEscolar repositorioPeriodoEscolar,
            IRepositorioEvento repositorioEvento,
            IRepositorioTipoCalendario repositorioTipoCalendario,
            IRepositorioParametrosSistema repositorioParametrosSistema)
        {
            this.repositorioPeriodoEscolar = repositorioPeriodoEscolar ?? throw new ArgumentNullException(nameof(repositorioPeriodoEscolar));
            this.repositorioParametrosSistema = repositorioParametrosSistema ?? throw new ArgumentNullException(nameof(repositorioParametrosSistema));
        }

        public async Task<List<DateTime>> BuscarDiasLetivos(long tipoCalendarioId)
        {
            List<DateTime> dias = new List<DateTime>();
            var periodoEscolar = await repositorioPeriodoEscolar.ObterPorTipoCalendario(tipoCalendarioId);
            periodoEscolar
                .ToList()
                .ForEach(x => dias
                    .AddRange(
                        Enumerable
                        .Range(0, 1 + (x.PeriodoFim - x.PeriodoInicio).Days)
                        .Select(y => x.PeriodoInicio.AddDays(y))
                        .Where(w => EhDiaUtil(w))
                        .ToList())
            );

            return dias;
        }

        private async Task<string> ObterParametroDiasLetivosFundMedio(int anoLetivo)
        {
            var parametros = await repositorioParametrosSistema.ObterParametrosPorTipoEAno(TipoParametroSistema.EjaDiasLetivos, anoLetivo);
            return parametros.FirstOrDefault(a => a.Nome == "EjaDiasLetivos").Valor;
        }
        public async Task<DiasLetivosDto> CalcularDiasLetivos(FiltroDiasLetivosDTO filtro)
        {
            //se for letivo em um fds que esteja no calend�rio somar
            bool estaAbaixo = false;

            //buscar os dados
            var periodoEscolar = await repositorioPeriodoEscolar.ObterPorTipoCalendario(filtro.TipoCalendarioId);
            var diasLetivosCalendario = BuscarDiasLetivos(periodoEscolar);
            var eventos = repositorioEvento.ObterEventosPorTipoDeCalendarioDreUe(filtro.TipoCalendarioId, filtro.DreId, filtro.UeId, false, false);
            var tipoCalendario = repositorioTipoCalendario.ObterPorId(filtro.TipoCalendarioId);

            if (filtro.DreId != null && filtro.UeId == null)
            {
                eventos = eventos.Where(e => e.DreId == null);
            }
            if (tipoCalendario == null)
                throw new NegocioException("Tipo de calendario n�o encontrado");

            var anoLetivo = tipoCalendario.AnoLetivo;

            List<DateTime> diasEventosNaoLetivos = new List<DateTime>();
            List<DateTime> diasEventosLetivos = new List<DateTime>();

            //transforma em dias
            diasEventosNaoLetivos = ObterDias(eventos, diasEventosNaoLetivos, EventoLetivo.Nao);
            diasEventosLetivos = ObterDias(eventos, diasEventosLetivos, EventoLetivo.Sim);

            //adicionar os finais de semana letivos se houver
            //se n�o houver dia letivo em fds n�o precisa adicionar
            foreach (var dia in diasEventosLetivos.Where(x => !EhDiaUtil(x)))
            {
                if (periodoEscolar.Any(w => w.PeriodoInicio <= dia && dia <= w.PeriodoFim))
                    diasLetivosCalendario.Add(dia);
            }

            //retirar eventos n�o letivos que n�o est�o no calend�rio
            diasEventosNaoLetivos = diasEventosNaoLetivos.Where(w => diasLetivosCalendario.Contains(w)).ToList();
            //retirar eventos n�o letivos que n�o contenha letivo
            diasEventosNaoLetivos = diasEventosNaoLetivos.Where(w => !diasEventosLetivos.Contains(w)).ToList();

            //subtrai os dias nao letivos
            var diasLetivos = diasLetivosCalendario.Distinct().Count() - diasEventosNaoLetivos.Distinct().Count();

            //verificar se eh eja ou nao
            var diasLetivosPermitidos = Convert.ToInt32(tipoCalendario.Modalidade == ModalidadeTipoCalendario.EJA ?
                await repositorioParametrosSistema.ObterValorPorTipoEAno(TipoParametroSistema.EjaDiasLetivos, anoLetivo) :
                await repositorioParametrosSistema.ObterValorPorTipoEAno(TipoParametroSistema.FundamentalMedioDiasLetivos, anoLetivo));

            estaAbaixo = diasLetivos < diasLetivosPermitidos;

            return new DiasLetivosDto
            {
                Dias = diasLetivos,
                EstaAbaixoPermitido = estaAbaixo
            };
        private async Task<string> ObterParametroDiasLetivosEja(int anoLetivo)
        {
            var parametros = await repositorioParametrosSistema.ObterParametrosPorTipoEAno(TipoParametroSistema.EjaDiasLetivos, anoLetivo);
            return parametros.FirstOrDefault(a => a.Nome == "FundamentalMedioDiasLetivos").Valor;
        }

        public List<DateTime> ObterDias(IEnumerable<Dominio.Evento> eventos, List<DateTime> dias, Dominio.EventoLetivo eventoTipo)
        {
            eventos
                            .Where(w => w.Letivo == eventoTipo)
                            .ToList()
                            .ForEach(x => dias
                                .AddRange(
                                    Enumerable
                                    .Range(0, 1 + (x.DataFim - x.DataInicio).Days)
                                    .Select(y => x.DataInicio.AddDays(y))
                                    .Where(w => (eventoTipo == Dominio.EventoLetivo.Nao
                                                && EhDiaUtil(w))
                                            || eventoTipo == Dominio.EventoLetivo.Sim)
                            ));
            return dias.Distinct().ToList();
        }

        public bool VerificarSeDataLetiva(IEnumerable<Evento> eventos, DateTime data)
        {
            bool possuiEventoLetivo = eventos.Any(x => x.Letivo == EventoLetivo.Sim);

            bool possuiEventoNaoLetivo = eventos.Any(x => x.Letivo == EventoLetivo.Nao);

            bool ehDiaUtil = EhDiaUtil(data);

            if (possuiEventoLetivo) return true;

            if (ehDiaUtil && !possuiEventoNaoLetivo) return true;

            return false;
        }

        private bool EhDiaUtil(DateTime data)
        {
            return data.DayOfWeek != DayOfWeek.Saturday && data.DayOfWeek != DayOfWeek.Sunday;
        }
    }
}
﻿using SME.SGP.Aplicacao.Interfaces;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public class ComandosEvento : IComandosEvento
    {
        private readonly IRepositorioEvento repositorioEvento;
        private readonly IServicoEvento servicoEvento;

        public ComandosEvento(IRepositorioEvento repositorioEvento,
                              IServicoEvento servicoEvento)
        {
            this.repositorioEvento = repositorioEvento ?? throw new System.ArgumentNullException(nameof(repositorioEvento));
            this.servicoEvento = servicoEvento ?? throw new System.ArgumentNullException(nameof(servicoEvento));
        }

        public void Excluir(long[] idsEventos)
        {
            List<long> idsComErroAoExcluir = new List<long>();

            foreach (var idEvento in idsEventos)
            {
                try
                {
                    var evento = repositorioEvento.ObterPorId(idEvento);

                    evento.Excluir();

                    repositorioEvento.Salvar(evento);
                }
                catch (Exception)
                {
                    idsComErroAoExcluir.Add(idEvento);
                }
            }

            if (idsComErroAoExcluir.Any())
                throw new NegocioException($"Não foi possível excluir os eventos de ids {string.Join(",", idsComErroAoExcluir)}");
        }

        public async Task Salvar(EventoDto eventoDto)
        {
            var evento = repositorioEvento.ObterPorId(eventoDto.Id);

            evento = MapearParaEntidade(evento, eventoDto);
            await servicoEvento.Salvar(evento);
        }

        private Evento MapearParaEntidade(Evento evento, EventoDto eventoDto)
        {
            if (evento == null)
            {
                evento = new Evento();
            }
            evento.DataFim = eventoDto.DataFim;
            evento.DataInicio = eventoDto.DataInicio;
            evento.Descricao = eventoDto.Descricao;
            evento.DreId = eventoDto.DreId;
            evento.FeriadoId = eventoDto.FeriadoId;
            evento.Letivo = eventoDto.Letivo;
            evento.Nome = eventoDto.Nome;
            evento.TipoCalendarioId = eventoDto.TipoCalendarioId;
            evento.TipoEventoId = eventoDto.TipoEventoId;
            evento.UeId = eventoDto.UeId;
            return evento;
        }
    }
}
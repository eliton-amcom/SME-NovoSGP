﻿using SME.SGP.Dominio;
using SME.SGP.Dto;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;

namespace SME.SGP.Aplicacao
{
    public interface IComandosDiasLetivos
    {
        List<DateTime> BuscarDiasLetivos(long tipoCalendarioId);

        DiasLetivosDto CalcularDiasLetivos(FiltroDiasLetivosDTO filtro);

        List<DateTime> ObterDias(IEnumerable<Evento> eventos, List<DateTime> dias, EventoLetivo eventoTipo);
    }
}
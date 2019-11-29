﻿using SME.SGP.Dominio.Entidades;
using SME.SGP.Dominio.Enumerados;
using SME.SGP.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Dominio.Interfaces
{
    public interface IRepositorioAbrangencia
    {
        Task<bool> JaExisteAbrangencia(string login, Guid perfil);

        Task<IEnumerable<AbrangenciaFiltroRetorno>> ObterAbrangenciaPorFiltro(string texto, string login, Guid perfil);

        Task<AbrangenciaFiltroRetorno> ObterAbrangenciaTurma(string turma, string login, Guid perfil);

        Task<AbrangenciaDreRetorno> ObterDre(string dreCodigo, string ueCodigo, string login, Guid perfil);

        Task<IEnumerable<AbrangenciaDreRetorno>> ObterDres(string login, Guid perfil, Modalidade? modalidade = null, int periodo = 0);

        Task<IEnumerable<int>> ObterModalidades(string login, Guid perfil);

        Task<IEnumerable<int>> ObterSemestres(string login, Guid perfil, Modalidade modalidade);

        Task<IEnumerable<AbrangenciaTurmaRetorno>> ObterTurmas(string codigoUe, string login, Guid perfil, Modalidade modalidade, int periodo = 0);

        Task<AbrangenciaUeRetorno> ObterUe(string codigo, string login, Guid perfil);

        Task<IEnumerable<AbrangenciaUeRetorno>> ObterUes(string codigoDre, string login, Guid perfil, Modalidade? modalidade = null, int periodo = 0);

        Task<IEnumerable<AbrangenciaSinteticaDto>> ObterAbrangenciaSintetica(string login, Guid perfil);
        void RemoverAbrangenciasForaEscopo(string login, Guid perfil, TipoAbrangencia porTurma);
        void InserirAbrangencias(IEnumerable<Abrangencia> enumerable, string login);
        void ExcluirAbrangencias(IEnumerable<long> ids);
    }
}
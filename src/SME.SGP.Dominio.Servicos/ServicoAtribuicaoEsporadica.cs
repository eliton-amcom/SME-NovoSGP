﻿using SME.SGP.Aplicacao.Integracoes;
using SME.SGP.Dominio.Interfaces;
using System;
using System.Linq;

namespace SME.SGP.Dominio.Servicos
{
    public class ServicoAtribuicaoEsporadica : IServicoAtribuicaoEsporadica
    {
        private readonly IRepositorioAtribuicaoEsporadica repositorioAtribuicaoEsporadica;
        private readonly IRepositorioPeriodoEscolar repositorioPeriodoEscolar;
        private readonly IRepositorioTipoCalendario repositorioTipoCalendario;
        private readonly IServicoUsuario servicoUsuario;
        private readonly IServicoEOL servicoEOL;

        public ServicoAtribuicaoEsporadica(IRepositorioPeriodoEscolar repositorioPeriodoEscolar, IRepositorioTipoCalendario repositorioTipoCalendario,
            IRepositorioAtribuicaoEsporadica repositorioAtribuicaoEsporadica, IServicoUsuario servicoUsuario, IServicoEOL servicoEOL)
        {
            this.repositorioPeriodoEscolar = repositorioPeriodoEscolar ?? throw new System.ArgumentNullException(nameof(repositorioPeriodoEscolar));
            this.repositorioTipoCalendario = repositorioTipoCalendario ?? throw new System.ArgumentNullException(nameof(repositorioTipoCalendario));
            this.repositorioAtribuicaoEsporadica = repositorioAtribuicaoEsporadica ?? throw new System.ArgumentNullException(nameof(repositorioAtribuicaoEsporadica));
            this.servicoUsuario = servicoUsuario ?? throw new System.ArgumentNullException(nameof(servicoUsuario));
            this.servicoEOL = servicoEOL ?? throw new System.ArgumentNullException(nameof(servicoEOL));
        }

        public void Salvar(AtribuicaoEsporadica atribuicaoEsporadica, int anoLetivo)
        {
            var tipoCalendario = repositorioTipoCalendario.BuscarPorAnoLetivoEModalidade(anoLetivo, ModalidadeTipoCalendario.FundamentalMedio);

            if (tipoCalendario == null)
                throw new NegocioException("Nenhum tipo de calendario para o ano letivo vigente encontrado");

            var periodosEscolares = repositorioPeriodoEscolar.ObterPorTipoCalendario(tipoCalendario.Id);

            if (periodosEscolares == null || !periodosEscolares.Any())
                throw new NegocioException("Nenhum periodo escolar encontrado para o ano letivo vigente");

            bool ehPerfilSelecionadoSME = servicoUsuario.UsuarioLogadoPossuiPerfilSme();

            atribuicaoEsporadica.Validar(ehPerfilSelecionadoSME, anoLetivo, periodosEscolares);

            repositorioAtribuicaoEsporadica.Salvar(atribuicaoEsporadica);

            AdicionarAtribuicaoEOL(atribuicaoEsporadica.ProfessorRf);
        }

        private void AdicionarAtribuicaoEOL(string codigoRF)
        {
            try
            {
                var resumo = servicoEOL.ObterResumoCore(codigoRF).Result;

                servicoEOL.AtribuirCJSeNecessario(resumo.Id);
            }
            catch (Exception)
            {
                throw new NegocioException("Não foi possivel realizar a atribuição esporadica, por favor contate o suporte");
            }           
        }
    }
}
﻿using SME.SGP.Dominio.Interfaces;
using System;
using System.Threading.Tasks;

namespace SME.SGP.Dominio.Servicos
{
    public class ServicoNotificacao : IServicoNotificacao
    {
        private readonly IRepositorioNotificacao repositorioNotificacao;

        public ServicoNotificacao(IRepositorioNotificacao repositorioNotificacao)
        {
            this.repositorioNotificacao = repositorioNotificacao ?? throw new ArgumentNullException(nameof(repositorioNotificacao));
        }

        public async Task ExcluirFisicamenteAsync(long[] ids)
        {
            await repositorioNotificacao.ExcluirPorIdsAsync(ids);
        }

        public void GeraNovoCodigo(Notificacao notificacao)
        {
            if (notificacao.Codigo == 0)
                notificacao.Codigo = ObtemNovoCodigo();
        }

        public long ObtemNovoCodigo()
        {
            return repositorioNotificacao.ObterUltimoCodigoPorAno(DateTime.Now.Year) + 1;
        }

        public void Salvar(Notificacao notificacao)
        {
            GeraNovoCodigo(notificacao);
            repositorioNotificacao.Salvar(notificacao);
        }
    }
}
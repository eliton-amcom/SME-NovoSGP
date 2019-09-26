﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SGP.Dominio
{
    public class Usuario : EntidadeBase
    {
        private readonly Guid PERFIL_PROFESSOR = Guid.Parse("40E1E074-37D6-E911-ABD6-F81654FE895D");
        public string CodigoRf { get; set; }
        public IEnumerable<Notificacao> Notificacoes { get { return notificacoes; } }
        private IList<Notificacao> notificacoes { get; set; }

        public void Adicionar(Notificacao notificacao)
        {
            if (notificacao != null)
                notificacoes.Add(notificacao);
        }

        public Guid ObterPerfilPrioritario(IEnumerable<PrioridadePerfil> perfisUsuario)
        {
            var possuiPerfilPrioritario = perfisUsuario.Any(c => c.CodigoPerfil == PERFIL_PROFESSOR);
            if (possuiPerfilPrioritario)
            {
                return PERFIL_PROFESSOR;
            }
            return perfisUsuario.FirstOrDefault().CodigoPerfil;
        }
    }
}
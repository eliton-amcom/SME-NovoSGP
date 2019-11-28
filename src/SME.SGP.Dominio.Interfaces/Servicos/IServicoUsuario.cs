﻿using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.SGP.Dominio.Interfaces
{
    public interface IServicoUsuario
    {
        Task AlterarEmailUsuarioPorLogin(string login, string novoEmail);

        Task AlterarEmailUsuarioPorRfOuInclui(string codigoRf, string novoEmail);

        string ObterClaim(string nomeClaim);

        string ObterLoginAtual();

        string ObterNomeLoginAtual();

        Guid ObterPerfilAtual();

        IEnumerable<Permissao> ObterPermissoes();

        string ObterRf();

        Task<Usuario> ObterUsuarioLogado();

        Usuario ObterUsuarioPorCodigoRfLoginOuAdiciona(string codigoRf, string login = "", string nome = "", string email = "");

        Task PodeModificarPerfil(Guid perfilParaModificar, string login);

        bool UsuarioLogadoPossuiPerfilSme();

    }
}
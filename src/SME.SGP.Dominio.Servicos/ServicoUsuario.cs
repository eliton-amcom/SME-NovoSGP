﻿using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SME.SGP.Aplicacao.Integracoes;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SME.SGP.Dominio
{
    public class ServicoUsuario : IServicoUsuario
    {
        private const string CLAIM_PERFIL_ATUAL = "perfil";
        private const string CLAIM_PERMISSAO = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        private const string CLAIM_RF = "rf";
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IRepositorioCache repositorioCache;
        private readonly IRepositorioPrioridadePerfil repositorioPrioridadePerfil;
        private readonly IRepositorioUsuario repositorioUsuario;
        private readonly IServicoEOL servicoEOL;
        private readonly IUnitOfWork unitOfWork;

        public ServicoUsuario(IRepositorioUsuario repositorioUsuario,
                              IServicoEOL servicoEOL,
                              IRepositorioPrioridadePerfil repositorioPrioridadePerfil,
                              IUnitOfWork unitOfWork,
                              IHttpContextAccessor httpContextAccessor,
                              IRepositorioCache repositorioCache)
        {
            this.repositorioUsuario = repositorioUsuario ?? throw new ArgumentNullException(nameof(repositorioUsuario));
            this.servicoEOL = servicoEOL ?? throw new ArgumentNullException(nameof(servicoEOL));
            this.repositorioPrioridadePerfil = repositorioPrioridadePerfil;
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            this.repositorioCache = repositorioCache ?? throw new ArgumentNullException(nameof(repositorioCache));
        }

        public async Task AlterarEmailUsuarioPorLogin(string login, string novoEmail)
        {
            var usuario = repositorioUsuario.ObterPorCodigoRfLogin(string.Empty, login);
            if (usuario == null)
                throw new NegocioException("Usuário não encontrado.");

            await AlterarEmail(usuario, novoEmail);
        }

        public async Task AlterarEmailUsuarioPorRfOuInclui(string codigoRf, string novoEmail)
        {
            unitOfWork.IniciarTransacao();

            var usuario = ObterUsuarioPorCodigoRfLoginOuAdiciona(codigoRf);
            await AlterarEmail(usuario, novoEmail);

            unitOfWork.PersistirTransacao();
        }

        public IEnumerable<Claim> DefinirPermissoesUsuarioLogado(string usuarioLogin, string usuarioNome, string codigoRf, Guid guidPerfil, IEnumerable<Permissao> permissionamentos)
        {
            IList<Claim> claims = new List<Claim>();

            claims.Add(new Claim("login", usuarioLogin));
            claims.Add(new Claim("nome", usuarioNome ?? string.Empty));
            claims.Add(new Claim("rf", codigoRf ?? string.Empty));
            claims.Add(new Claim("perfil", guidPerfil.ToString()));

            foreach (var permissao in permissionamentos)
            {
                claims.Add(new Claim("roles", permissao.ToString()));
            }

            httpContextAccessor.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

            return claims;
        }

        public string ObterClaim(string nomeClaim)
        {
            var claim = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == nomeClaim);
            return claim?.Value;
        }

        public string ObterLoginAtual()
        {
            var loginAtual = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "login");
            if (loginAtual == null)
                throw new NegocioException("Não foi possível localizar o login no token");

            return loginAtual.Value;
        }

        public string ObterNomeLoginAtual()
        {
            var nomeLoginAtual = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(a => a.Type == "nome");
            if (nomeLoginAtual == null)
                throw new NegocioException("Não foi possível localizar o nome do login no token");

            return nomeLoginAtual.Value;
        }

        public Guid ObterPerfilAtual()
        {
            return Guid.Parse(ObterClaim(CLAIM_PERFIL_ATUAL));
        }

        public IEnumerable<Permissao> ObterPermissoes()
        {
            var claims = httpContextAccessor.HttpContext.User.Claims.Where(a => a.Type == CLAIM_PERMISSAO);
            List<Permissao> retorno = new List<Permissao>();

            if (claims.Any())
            {
                foreach (var claim in claims)
                {
                    var permissao = (Permissao)Enum.Parse(typeof(Permissao), claim.Value);
                    retorno.Add(permissao);
                }
            }
            return retorno;
        }

        public string ObterRf()
        {
            var rf = ObterClaim(CLAIM_RF);
            return rf;
        }

        public async Task<Usuario> ObterUsuarioLogado()
        {
            var login = ObterLoginAtual();
            var usuario = repositorioUsuario.ObterPorCodigoRfLogin(string.Empty, login);

            if (usuario == null)
            {
                throw new NegocioException("Usuário não encontrado.");
            }

            var chaveRedis = $"perfis-usuario-{login}";
            var perfisUsuarioString = repositorioCache.Obter(chaveRedis);

            IEnumerable<PrioridadePerfil> perfisDoUsuario = null;

            if (string.IsNullOrWhiteSpace(perfisUsuarioString))
            {
                var perfisPorLogin = await servicoEOL.ObterPerfisPorLogin(login);
                if (perfisPorLogin == null)
                    throw new NegocioException($"Não foi possível obter os perfis do usuário {login}");

                perfisDoUsuario = repositorioPrioridadePerfil.ObterPerfisPorIds(perfisPorLogin.Perfis);
                _ = repositorioCache.SalvarAsync(chaveRedis, JsonConvert.SerializeObject(perfisDoUsuario));
            }
            else
            {
                perfisDoUsuario = JsonConvert.DeserializeObject<IEnumerable<PrioridadePerfil>>(perfisUsuarioString);
            }
            usuario.DefinirPerfis(perfisDoUsuario);

            return usuario;
        }

        public Usuario ObterUsuarioPorCodigoRfLoginOuAdiciona(string codigoRf, string login = "", string nome = "", string email = "")
        {
            var usuario = repositorioUsuario.ObterPorCodigoRfLogin(codigoRf, login);
            if (usuario != null)
                return usuario;

            if (string.IsNullOrEmpty(login))
                login = codigoRf;

            usuario = new Usuario() { CodigoRf = codigoRf, Login = login, Nome = nome, Email = email };

            repositorioUsuario.Salvar(usuario);

            return usuario;
        }

        public async Task PodeModificarPerfil(Guid perfilParaModificar, string login)
        {
            var perfisDoUsuario = await servicoEOL.ObterPerfisPorLogin(login);
            if (perfisDoUsuario == null)
                throw new NegocioException($"Não foi possível obter os perfis do usuário {login}");

            if (!perfisDoUsuario.Perfis.Contains(perfilParaModificar))
                throw new NegocioException($"O usuário {login} não possui acesso ao perfil {perfilParaModificar}");
        }

        private async Task AlterarEmail(Usuario usuario, string novoEmail)
        {
            var outrosUsuariosComMesmoEmail = repositorioUsuario.ExisteUsuarioComMesmoEmail(novoEmail, usuario.Id);

            if (outrosUsuariosComMesmoEmail)
                throw new NegocioException("Já existe outro usuário com o e-mail informado.");

            var retornoEol = await servicoEOL.ObterPerfisPorLogin(usuario.Login);
            if (retornoEol == null || retornoEol.Status != AutenticacaoStatusEol.Ok)
                throw new NegocioException("Ocorreu um erro ao obter os dados do usuário no EOL.");

            var perfisUsuario = repositorioPrioridadePerfil.ObterPerfisPorIds(retornoEol.Perfis);
            usuario.DefinirPerfis(perfisUsuario);
            usuario.DefinirEmail(novoEmail);
            repositorioUsuario.Salvar(usuario);
        }
    }
}
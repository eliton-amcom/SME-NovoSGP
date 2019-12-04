﻿using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public class ComandosAula : IComandosAula
    {
        private readonly IRepositorioAula repositorioAula;
        private readonly IServicoAula servicoAula;
        private readonly IServicoUsuario servicoUsuario;

        public ComandosAula(IRepositorioAula repositorio,
                            IServicoUsuario servicoUsuario,
                            IServicoAula servicoAula)
        {
            this.repositorioAula = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
            this.servicoUsuario = servicoUsuario ?? throw new ArgumentNullException(nameof(servicoUsuario));
            this.servicoAula = servicoAula ?? throw new ArgumentNullException(nameof(servicoAula));
        }

        public async Task<string> Alterar(AulaDto dto, long id)
        {
            var usuario = await servicoUsuario.ObterUsuarioLogado();
            var aula = MapearDtoParaEntidade(dto, id, usuario.CodigoRf);

            return await servicoAula.Salvar(aula, usuario, dto.RecorrenciaAula);
        }

        public async Task<string> Excluir(long id, RecorrenciaAula recorrencia)
        {
            var usuario = await servicoUsuario.ObterUsuarioLogado();
            var aula = repositorioAula.ObterPorId(id);

            return await servicoAula.Excluir(aula, recorrencia, usuario);
        }

        public async Task<string> Inserir(AulaDto dto)
        {
            var usuario = await servicoUsuario.ObterUsuarioLogado();
            var aula = MapearDtoParaEntidade(dto, 0L, usuario.CodigoRf);

            return await servicoAula.Salvar(aula, usuario, aula.RecorrenciaAula);
        }

        private Aula MapearDtoParaEntidade(AulaDto dto, long id, string usuarioRf)
        {
            Aula aula = new Aula();
            if (id > 0L)
            {
                aula = repositorioAula.ObterPorId(id);
            }
            if (string.IsNullOrEmpty(aula.ProfessorRf))
            {
                aula.ProfessorRf = usuarioRf;
                // Alteração não muda recorrencia da aula
                aula.RecorrenciaAula = dto.RecorrenciaAula;
            }
            aula.UeId = dto.UeId;
            aula.DisciplinaId = dto.DisciplinaId;
            aula.TurmaId = dto.TurmaId;
            aula.TipoCalendarioId = dto.TipoCalendarioId;
            aula.DataAula = dto.DataAula;
            aula.Quantidade = dto.Quantidade;
            aula.TipoAula = dto.TipoAula;
            return aula;
        }
    }
}
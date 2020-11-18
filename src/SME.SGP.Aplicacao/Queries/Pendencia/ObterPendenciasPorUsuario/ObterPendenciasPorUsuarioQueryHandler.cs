﻿using MediatR;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra;
using SME.SGP.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public class ObterPendenciasPorUsuarioQueryHandler : ConsultasBase, IRequestHandler<ObterPendenciasPorUsuarioQuery, PaginacaoResultadoDto<PendenciaDto>>
    {
        private readonly IRepositorioPendencia repositorioPendencia;
        private readonly IMediator mediator;

        public ObterPendenciasPorUsuarioQueryHandler(IContextoAplicacao contextoAplicacao, IMediator mediator, IRepositorioPendencia repositorioPendencia) : base(contextoAplicacao)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.repositorioPendencia = repositorioPendencia ?? throw new ArgumentNullException(nameof(repositorioPendencia));
        }

        public async Task<PaginacaoResultadoDto<PendenciaDto>> Handle(ObterPendenciasPorUsuarioQuery request, CancellationToken cancellationToken)
            => await MapearParaDtoPaginado(await repositorioPendencia.ListarPendenciasUsuario(request.UsuarioId, Paginacao));

        private async Task<PaginacaoResultadoDto<PendenciaDto>> MapearParaDtoPaginado(PaginacaoResultadoDto<Pendencia> pendenciasPaginadas)
        {
            return new PaginacaoResultadoDto<PendenciaDto>()
            {
                Items = await MapearParaDto(pendenciasPaginadas.Items),
                TotalPaginas = pendenciasPaginadas.TotalPaginas,
                TotalRegistros = pendenciasPaginadas.TotalRegistros
            };
        }

        private async Task<IEnumerable<PendenciaDto>> MapearParaDto(IEnumerable<Pendencia> pendencias)
        {
            var listaPendenciasDto = new List<PendenciaDto>();

            foreach (var pendencia in pendencias)
            {
                listaPendenciasDto.Add(new PendenciaDto()
                {
                    Tipo = pendencia.Tipo.GroupName(),
                    Titulo = pendencia.Tipo.Name(),
                    Detalhe = await ObterDescricaoPendencia(pendencia),
                    Turma = pendencia.EhPendenciaAula() ? await ObterDescricaoTurma(pendencia.Id) : ""
                });
            }

            return listaPendenciasDto;
        }

        private async Task<string> ObterDescricaoPendencia(Pendencia pendencia)
        {
            if (pendencia.EhPendenciaAula())
                return await ObterDescricaoPendenciaAula(pendencia);
            if (pendencia.EhPendenciaCadastroEvento())
                return await ObterDescricaoPendenciaEvento(pendencia);
            else
                return ObterDescricaoPendenciaGeral(pendencia);
        }

        private string ObterDescricaoPendenciaGeral(Pendencia pendencia)
        {
            return $"{pendencia.Descricao}<br />{pendencia.Instrucao}";
        }

        private async Task<string> ObterDescricaoPendenciaEvento(Pendencia pendencia)
        {
            var pendenciasEventos = await mediator.Send(new ObterPendenciasParametroEventoPorPendenciaQuery(pendencia.Id));

            var descricao = new StringBuilder(pendencia.Descricao);
            descricao.AppendLine("<br /><ul>");

            foreach (var pendenciaEvento in pendenciasEventos)
            {
                descricao.AppendLine($"<li>{pendenciaEvento.Descricao} ({pendenciaEvento.Valor})</li>");
            }
            descricao.AppendLine("</ul>");
            descricao.AppendLine(pendencia.Instrucao);

            return descricao.ToString();
        }

        private async Task<string> ObterDescricaoPendenciaAula(Pendencia pendencia)
        {
            var pendenciasAulas = await mediator.Send(new ObterPendenciasAulasPorPendenciaQuery(pendencia.Id));

            var descricao = new StringBuilder(pendencia.Descricao);
            descricao.AppendLine("<br /><ul>");

            foreach (var pendenciaAula in pendenciasAulas)
            {
                descricao.AppendLine($"<li>{pendenciaAula.DataAula:dd/MM} - {pendenciaAula.Motivo}</li>");
            }
            descricao.AppendLine("</ul>");
            descricao.AppendLine(pendencia.Instrucao);

            return descricao.ToString();
        }

        private async Task<string> ObterDescricaoTurma(long pendenciaId)
        {
            var turma = await mediator.Send(new ObterTurmaDaPendenciaQuery(pendenciaId));
            if(turma != null)
                return $"{turma.ModalidadeCodigo.ShortName()} - {turma.Nome}";
            return "";
        }
    }
}
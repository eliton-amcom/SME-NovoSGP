﻿using MediatR;
using Microsoft.AspNetCore.Http;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SME.SGP.Infra;

namespace SME.SGP.Aplicacao
{
    public class UploadArquivoCommandHandler : IRequestHandler<UploadArquivoCommand, Guid>
    {
        private readonly IMediator mediator;

        public UploadArquivoCommandHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Guid> Handle(UploadArquivoCommand request, CancellationToken cancellationToken)
        {
            if (request.TipoConteudo != TipoConteudoArquivo.Indefinido)
            {
                if (request.Arquivo.ContentType != request.TipoConteudo.Name())
                    throw new NegocioException("O formato de arquivo enviado não é aceito");
            }

            var nomeArquivo = request.Arquivo.FileName;
            var caminhoArquivo = ObterCaminhoArquivo(request.Tipo, request.Arquivo);

            var arquivo = await mediator.Send(new SalvarArquivoRepositorioCommand(nomeArquivo, request.Tipo, request.Arquivo.ContentType));
            await mediator.Send(new ArmazenarArquivoFisicoCommand(request.Arquivo, arquivo.Codigo.ToString(), caminhoArquivo));

            return arquivo.Codigo;
        }

        private string ObterCaminhoArquivo(TipoArquivo tipo, IFormFile arquivo)
        {
            var caminho = Path.Combine(ObterCaminhoArquivos(), tipo.ToString());
            return VerificaCaminhoExiste(caminho);
        }

        private string ObterCaminhoArquivos()
        {
            var caminho = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Arquivos");
            return VerificaCaminhoExiste(caminho);
        }

        private string VerificaCaminhoExiste(string caminho)
        {
            if (!Directory.Exists(caminho))
                Directory.CreateDirectory(caminho);

            return caminho;
        }
    }
}

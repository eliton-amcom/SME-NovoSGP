﻿using MediatR;
using Newtonsoft.Json;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra.Dtos.EscolaAqui.DadosDeLeituraDeComunicados;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public class ObterDadosDeLeituraDeComunicadosQueryHandler : IRequestHandler<ObterDadosDeLeituraDeComunicadosQuery, IEnumerable<DadosDeLeituraDoComunicadoDto>>
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IRepositorioComunicado repositorioComunicado;
        private const string BaseUrl = "/api/v1/dashboard/comunicados/leitura";

        public ObterDadosDeLeituraDeComunicadosQueryHandler(IHttpClientFactory httpClientFactory, IRepositorioComunicado repositorioComunicado)
        {
            this.httpClientFactory = httpClientFactory;
            this.repositorioComunicado = repositorioComunicado;
        }

        public async Task<IEnumerable<DadosDeLeituraDoComunicadoDto>> Handle(ObterDadosDeLeituraDeComunicadosQuery request, CancellationToken cancellationToken)
        {
            if(!await repositorioComunicado.Exists(request.ComunicadoId))
                throw new NegocioException("O comunicado informado não existe. Por favor tente novamente.", HttpStatusCode.BadRequest);

            var httpClient = httpClientFactory.CreateClient("servicoAcompanhamentoEscolar");
            var url = new StringBuilder(BaseUrl);

            url.Append(@"?comunicadoId=" + request.ComunicadoId);
            if (!string.IsNullOrEmpty(request.CodigoDre))
            {
                url.Append(@"&codigoDre=" + request.CodigoDre);

                if(!string.IsNullOrEmpty(request.CodigoUe))
                    url.Append(@"&codigoUe=" + request.CodigoUe);
            }

            var resposta = await httpClient.GetAsync($"{url}", cancellationToken);
            if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
                throw new NegocioException("Não foi possível obter dados de de leitura de comunicados pelo aplicativo.", HttpStatusCode.InternalServerError);

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<DadosDeLeituraDoComunicadoDto>>(json);
        }
    }
}
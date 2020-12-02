using MediatR;
using Newtonsoft.Json;
using SME.SGP.Dominio;
using SME.SGP.Dominio.Interfaces;
using SME.SGP.Infra.Dtos.EscolaAqui.DadosDeLeituraDeComunicados;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SGP.Aplicacao
{
    public class ObterDadosDeLeituraDeComunicadosAgrupadosPorDreQueryHandler : IRequestHandler<ObterDadosDeLeituraDeComunicadosAgrupadosPorDreQuery, IEnumerable<DadosDeLeituraDoComunicadoDto>>
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IRepositorioComunicado repositorioComunicado;
        private const string BaseUrl = "/api/v1/dashboard/leitura/agrupados";

        public ObterDadosDeLeituraDeComunicadosAgrupadosPorDreQueryHandler(IHttpClientFactory httpClientFactory, IRepositorioComunicado repositorioComunicado)
        {
            this.httpClientFactory = httpClientFactory;
            this.repositorioComunicado = repositorioComunicado;
        }

        public async Task<IEnumerable<DadosDeLeituraDoComunicadoDto>> Handle(ObterDadosDeLeituraDeComunicadosAgrupadosPorDreQuery request, CancellationToken cancellationToken)
        {
            if (!await repositorioComunicado.Exists(request.ComunicadoId))
                throw new NegocioException("O comunicado informado não existe. Por favor tente novamente.", HttpStatusCode.BadRequest);

            var httpClient = httpClientFactory.CreateClient("servicoAcompanhamentoEscolar");
            var url = new StringBuilder(BaseUrl);

            url.Append(@"?notificacaoId=" + request.ComunicadoId);
            url.Append(@"&modoVisualizacao=" + request.ModoVisualizacao);

            var resposta = await httpClient.GetAsync($"{url}", cancellationToken);
            if (!resposta.IsSuccessStatusCode || resposta.StatusCode == HttpStatusCode.NoContent)
                throw new NegocioException("Não foi possível obter dados de de leitura de comunicados pelo aplicativo.", HttpStatusCode.InternalServerError);

            var json = await resposta.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<DadosDeLeituraDoComunicadoDto>>(json);
        }
    }
}
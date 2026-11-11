using System.Net;
using MultasApp.DTO;

namespace MultasApp.Servicos
{
    // Integração 3: alteração de dados no microsserviço de Leitores.
    public class LeitorClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public LeitorClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task AlterarDivida(Guid codigoLeitor, AlterarDividaDTO alterarDividaDto)
        {
            var urlLeitores = _configuration["LeitoresUrl"];

            if (string.IsNullOrEmpty(urlLeitores))
            {
                throw new Exception("A URL do microsserviço de leitores não foi configurada.");
            }

            var metodoAlterarDivida = urlLeitores.TrimEnd('/') + "/api/leitores/" + codigoLeitor + "/divida";

            var response = await _httpClient.PatchAsJsonAsync(metodoAlterarDivida, alterarDividaDto);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception("Leitor não encontrado.");
            }

            if (!response.IsSuccessStatusCode)
            {
                var detalhe = await response.Content.ReadAsStringAsync();

                throw new Exception("Erro ao alterar a dívida do leitor. " + detalhe);
            }
        }
    }
}

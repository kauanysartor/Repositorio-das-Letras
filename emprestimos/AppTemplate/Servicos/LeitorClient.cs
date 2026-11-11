using System.Net;
using EmprestimosApp.DTO;

namespace EmprestimosApp.Servicos
{
    // Integração 1: busca de dados no microsserviço de Leitores.
    public class LeitorClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public LeitorClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<LeitorDTO?> BuscarLeitor(Guid codigoLeitor)
        {
            var urlLeitores = _configuration["LeitoresUrl"];

            if (string.IsNullOrEmpty(urlLeitores))
            {
                throw new Exception("A URL do microsserviço de leitores não foi configurada.");
            }

            var metodoBuscarLeitor = urlLeitores.TrimEnd('/') + "/api/leitores/" + codigoLeitor;

            var response = await _httpClient.GetAsync(metodoBuscarLeitor);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Erro ao buscar leitor.");
            }

            return await response.Content.ReadFromJsonAsync<LeitorDTO>();
        }
    }
}

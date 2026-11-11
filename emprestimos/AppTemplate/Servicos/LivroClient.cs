using System.Net;
using EmprestimosApp.DTO;

namespace EmprestimosApp.Servicos
{
    // Integração 2: busca de dados no microsserviço de Livros.
    public class LivroClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public LivroClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<LivroDTO?> BuscarLivro(Guid codigoLivro)
        {
            var urlLivros = _configuration["LivrosUrl"];

            if (string.IsNullOrEmpty(urlLivros))
            {
                throw new Exception("A URL do microsserviço de livros não foi configurada.");
            }

            var metodoBuscarLivro = urlLivros.TrimEnd('/') + "/api/livros/" + codigoLivro;

            var response = await _httpClient.GetAsync(metodoBuscarLivro);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Erro ao buscar livro.");
            }

            return await response.Content.ReadFromJsonAsync<LivroDTO>();
        }
    }
}

namespace EmprestimosApp.DTO
{
    // Dados do leitor retornados pelo microsserviço de Leitores.
    public class LeitorDTO
    {
        public Guid Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Cpf { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        public string Situacao { get; set; } = string.Empty;

        public decimal Divida { get; set; }
    }
}

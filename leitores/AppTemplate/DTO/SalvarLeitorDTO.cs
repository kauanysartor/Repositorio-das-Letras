namespace LeitoresApp.DTO
{
    public class SalvarLeitorDTO
    {
        public string Nome { get; set; } = string.Empty;

        public string Cpf { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Telefone { get; set; }
    }
}

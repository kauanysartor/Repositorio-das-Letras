namespace LivrosApp.DTO
{
    public class SalvarLivroDTO
    {
        public string Titulo { get; set; } = string.Empty;

        public string Autor { get; set; } = string.Empty;

        public string? Isbn { get; set; }

        public string? Categoria { get; set; }

        public int QuantidadeExemplares { get; set; }
    }
}

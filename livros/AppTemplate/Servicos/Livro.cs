namespace LivrosApp.Servicos
{
    public class Livro
    {
        public Guid Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string Autor { get; set; } = string.Empty;

        public string Isbn { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        public int QuantidadeExemplares { get; set; }
    }
}

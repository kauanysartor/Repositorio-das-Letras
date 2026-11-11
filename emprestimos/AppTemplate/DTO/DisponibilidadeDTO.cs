namespace EmprestimosApp.DTO
{
    public class DisponibilidadeDTO
    {
        public Guid CodigoLivro { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public int ExemplaresTotal { get; set; }

        public int EmprestimosEmAberto { get; set; }

        public int ExemplaresDisponiveis { get; set; }

        public bool Disponivel { get; set; }
    }
}

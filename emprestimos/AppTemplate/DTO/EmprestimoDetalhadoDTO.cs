namespace EmprestimosApp.DTO
{
    public class EmprestimoDetalhadoDTO
    {
        public Guid Id { get; set; }

        public DateTime DataEmprestimo { get; set; }

        public DateTime DataPrevistaDevolucao { get; set; }

        public DateTime? DataDevolucao { get; set; }

        public bool Devolvido { get; set; }

        public LeitorDTO Leitor { get; set; } = new LeitorDTO();

        public LivroDTO Livro { get; set; } = new LivroDTO();
    }
}

namespace EmprestimosApp.Servicos
{
    public class Emprestimo
    {
        public Guid Id { get; set; }

        public Guid CodigoLeitor { get; set; }

        public Guid CodigoLivro { get; set; }

        public DateTime DataEmprestimo { get; set; }

        public DateTime DataPrevistaDevolucao { get; set; }

        public DateTime? DataDevolucao { get; set; }
    }
}

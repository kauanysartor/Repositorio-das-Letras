namespace EmprestimosApp.DTO
{
    public class InserirEmprestimoDTO
    {
        public Guid CodigoLeitor { get; set; }

        public Guid CodigoLivro { get; set; }

        public DateTime DataPrevistaDevolucao { get; set; }
    }
}

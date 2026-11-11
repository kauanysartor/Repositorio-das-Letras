namespace MultasApp.DTO
{
    public class InserirMultaDTO
    {
        public Guid CodigoLeitor { get; set; }

        public Guid CodigoEmprestimo { get; set; }

        public decimal Valor { get; set; }

        public string Motivo { get; set; } = string.Empty;
    }
}

namespace MultasApp.Servicos
{
    public class Multa
    {
        public Guid Id { get; set; }

        public Guid CodigoLeitor { get; set; }

        public Guid CodigoEmprestimo { get; set; }

        public decimal Valor { get; set; }

        public string Motivo { get; set; } = string.Empty;

        public EnumSituacaoMulta Situacao { get; set; }

        public DateTime DataLancamento { get; set; }

        public DateTime? DataPagamento { get; set; }
    }

    public enum EnumSituacaoMulta
    {
        Pendente = 1,
        Paga = 2
    }
}

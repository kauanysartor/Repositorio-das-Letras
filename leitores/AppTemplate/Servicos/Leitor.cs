namespace LeitoresApp.Servicos
{
    public class Leitor
    {
        public Guid Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Cpf { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Telefone { get; set; } = string.Empty;

        public EnumSituacaoLeitor Situacao { get; set; }

        public decimal Divida { get; set; }
    }

    public enum EnumSituacaoLeitor
    {
        Ativo = 1,
        Inativo = 2,
        Bloqueado = 3
    }
}

namespace LeitoresApp.DTO
{
    public class AlterarDividaDTO
    {
        public decimal Valor { get; set; }

        public EnumTipoAlteracaoDivida Tipo { get; set; }
    }

    public enum EnumTipoAlteracaoDivida
    {
        Acrescimo = 1,
        Reducao = 2
    }
}

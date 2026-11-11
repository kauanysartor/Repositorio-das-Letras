namespace MultasApp.DTO
{
    // Mesmo formato esperado pelo PATCH /api/leitores/{id}/divida.
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

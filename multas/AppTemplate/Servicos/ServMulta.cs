using MultasApp.DTO;

namespace MultasApp.Servicos
{
    public class ServMulta
    {
        private readonly DataContext _dataContext;
        private readonly LeitorClient _leitorClient;

        public ServMulta(DataContext dataContext, LeitorClient leitorClient)
        {
            _dataContext = dataContext;
            _leitorClient = leitorClient;
        }

        public async Task<Multa> Inserir(InserirMultaDTO dto)
        {
            if (dto.Valor <= 0)
            {
                throw new Exception("O valor da multa deve ser maior que zero.");
            }

            if (string.IsNullOrWhiteSpace(dto.Motivo))
            {
                throw new Exception("Falta informar o motivo da multa.");
            }

            var multa = new Multa
            {
                Id = Guid.NewGuid(),
                CodigoLeitor = dto.CodigoLeitor,
                CodigoEmprestimo = dto.CodigoEmprestimo,
                Valor = dto.Valor,
                Motivo = dto.Motivo.Trim(),
                Situacao = EnumSituacaoMulta.Pendente,
                DataLancamento = DateTime.Today,
                DataPagamento = null
            };

            // Integração 3: o serviço de Leitores soma a dívida e bloqueia o leitor.
            // Se o leitor não existir, o serviço de Leitores devolve erro e a multa não é salva.
            await _leitorClient.AlterarDivida(dto.CodigoLeitor, new AlterarDividaDTO
            {
                Valor = dto.Valor,
                Tipo = EnumTipoAlteracaoDivida.Acrescimo
            });

            _dataContext.Multas.Add(multa);
            await _dataContext.SaveChangesAsync();

            return multa;
        }

        public async Task<Multa?> Pagar(Guid id)
        {
            var multa = _dataContext.Multas.FirstOrDefault(p => p.Id == id);

            if (multa == null)
            {
                return null;
            }

            if (multa.Situacao == EnumSituacaoMulta.Paga)
            {
                throw new Exception("Esta multa já foi paga.");
            }

            // Integração 3: o serviço de Leitores reduz a dívida e libera o leitor se zerar.
            await _leitorClient.AlterarDivida(multa.CodigoLeitor, new AlterarDividaDTO
            {
                Valor = multa.Valor,
                Tipo = EnumTipoAlteracaoDivida.Reducao
            });

            multa.Situacao = EnumSituacaoMulta.Paga;
            multa.DataPagamento = DateTime.Today;

            await _dataContext.SaveChangesAsync();

            return multa;
        }

        public List<Multa> Listar(Guid? leitorId, EnumSituacaoMulta? situacao)
        {
            var consulta = _dataContext.Multas.AsQueryable();

            if (leitorId.HasValue)
            {
                var codigoLeitor = leitorId.Value;
                consulta = consulta.Where(p => p.CodigoLeitor == codigoLeitor);
            }

            if (situacao.HasValue)
            {
                var situacaoFiltro = situacao.Value;
                consulta = consulta.Where(p => p.Situacao == situacaoFiltro);
            }

            return consulta.ToList();
        }
    }
}

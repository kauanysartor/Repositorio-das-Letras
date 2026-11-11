using LeitoresApp.DTO;

namespace LeitoresApp.Servicos
{
    public class ServLeitor
    {
        private readonly DataContext _dataContext;

        public ServLeitor(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Leitor Inserir(SalvarLeitorDTO dto)
        {
            Validar(dto, null);

            var leitor = new Leitor
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome.Trim(),
                Cpf = dto.Cpf.Trim(),
                Email = dto.Email?.Trim() ?? string.Empty,
                Telefone = dto.Telefone?.Trim() ?? string.Empty,
                Situacao = EnumSituacaoLeitor.Ativo,
                Divida = 0
            };

            _dataContext.Leitores.Add(leitor);
            _dataContext.SaveChanges();

            return leitor;
        }

        public void Validar(SalvarLeitorDTO dto, Guid? idAtual)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
            {
                throw new Exception("Falta informar o nome do leitor.");
            }

            if (string.IsNullOrWhiteSpace(dto.Cpf))
            {
                throw new Exception("Falta informar o CPF do leitor.");
            }

            var cpf = dto.Cpf.Trim();

            var consulta = _dataContext.Leitores.Where(p => p.Cpf == cpf);

            if (idAtual.HasValue)
            {
                var id = idAtual.Value;
                consulta = consulta.Where(p => p.Id != id);
            }

            if (consulta.Any())
            {
                throw new Exception("Já existe um leitor cadastrado com este CPF.");
            }
        }

        public List<Leitor> Listar()
        {
            return _dataContext.Leitores.OrderBy(p => p.Nome).ToList();
        }

        public Leitor? BuscarPorId(Guid id)
        {
            return _dataContext.Leitores.FirstOrDefault(p => p.Id == id);
        }

        public Leitor? Editar(Guid id, SalvarLeitorDTO dto)
        {
            Validar(dto, id);

            var leitor = BuscarPorId(id);

            if (leitor == null)
            {
                return null;
            }

            leitor.Nome = dto.Nome.Trim();
            leitor.Cpf = dto.Cpf.Trim();
            leitor.Email = dto.Email?.Trim() ?? string.Empty;
            leitor.Telefone = dto.Telefone?.Trim() ?? string.Empty;

            _dataContext.SaveChanges();

            return leitor;
        }

        public Leitor? Inativar(Guid id)
        {
            var leitor = BuscarPorId(id);

            if (leitor == null)
            {
                return null;
            }

            leitor.Situacao = EnumSituacaoLeitor.Inativo;

            _dataContext.SaveChanges();

            return leitor;
        }

        // Usado pelo microsserviço de Multas.
        // Acrescimo: soma o valor à dívida e bloqueia o leitor.
        // Reducao: subtrai o valor da dívida e libera o leitor quando a dívida zerar.
        public Leitor? AlterarDivida(Guid id, AlterarDividaDTO dto)
        {
            if (dto.Valor <= 0)
            {
                throw new Exception("O valor deve ser maior que zero.");
            }

            if (!Enum.IsDefined(dto.Tipo))
            {
                throw new Exception("Tipo de alteração de dívida inválido.");
            }

            var leitor = BuscarPorId(id);

            if (leitor == null)
            {
                return null;
            }

            if (dto.Tipo == EnumTipoAlteracaoDivida.Acrescimo)
            {
                leitor.Divida = leitor.Divida + dto.Valor;

                if (leitor.Situacao == EnumSituacaoLeitor.Ativo)
                {
                    leitor.Situacao = EnumSituacaoLeitor.Bloqueado;
                }
            }
            else
            {
                leitor.Divida = leitor.Divida - dto.Valor;

                if (leitor.Divida <= 0)
                {
                    leitor.Divida = 0;

                    if (leitor.Situacao == EnumSituacaoLeitor.Bloqueado)
                    {
                        leitor.Situacao = EnumSituacaoLeitor.Ativo;
                    }
                }
            }

            _dataContext.SaveChanges();

            return leitor;
        }
    }
}

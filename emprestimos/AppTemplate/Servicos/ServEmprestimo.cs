using EmprestimosApp.DTO;

namespace EmprestimosApp.Servicos
{
    public class ServEmprestimo
    {
        private readonly DataContext _dataContext;
        private readonly LeitorClient _leitorClient;
        private readonly LivroClient _livroClient;

        public ServEmprestimo(
            DataContext dataContext,
            LeitorClient leitorClient,
            LivroClient livroClient)
        {
            _dataContext = dataContext;
            _leitorClient = leitorClient;
            _livroClient = livroClient;
        }

        public async Task<Emprestimo> Inserir(InserirEmprestimoDTO dto)
        {
            if (dto.DataPrevistaDevolucao.Date < DateTime.Today)
            {
                throw new Exception("A data prevista de devolução não pode ser anterior a hoje.");
            }

            // Integração 1: busca o leitor e valida se pode emprestar.
            var leitor = await _leitorClient.BuscarLeitor(dto.CodigoLeitor);

            if (leitor == null)
            {
                throw new Exception("Leitor não encontrado.");
            }

            if (string.Equals(leitor.Situacao, "Inativo", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Leitor inativo. Não é possível registrar o empréstimo.");
            }

            if (string.Equals(leitor.Situacao, "Bloqueado", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Leitor bloqueado por dívida. Não é possível registrar o empréstimo.");
            }

            // Integração 2: busca o livro e valida se há exemplar disponível.
            var disponibilidade = await ConsultarDisponibilidade(dto.CodigoLivro);

            if (!disponibilidade.Disponivel)
            {
                throw new Exception("O livro não possui exemplar disponível.");
            }

            var emprestimo = new Emprestimo
            {
                Id = Guid.NewGuid(),
                CodigoLeitor = dto.CodigoLeitor,
                CodigoLivro = dto.CodigoLivro,
                DataEmprestimo = DateTime.Today,
                DataPrevistaDevolucao = dto.DataPrevistaDevolucao.Date,
                DataDevolucao = null
            };

            _dataContext.Emprestimos.Add(emprestimo);
            await _dataContext.SaveChangesAsync();

            return emprestimo;
        }

        // Exemplares disponíveis = exemplares do livro (serviço de Livros)
        // menos os empréstimos ainda não devolvidos (banco deste serviço).
        public async Task<DisponibilidadeDTO> ConsultarDisponibilidade(Guid codigoLivro)
        {
            var livro = await _livroClient.BuscarLivro(codigoLivro);

            if (livro == null)
            {
                throw new Exception("Livro não encontrado.");
            }

            var emAberto = _dataContext.Emprestimos
                .Count(p => p.CodigoLivro == codigoLivro && p.DataDevolucao == null);

            var disponiveis = livro.QuantidadeExemplares - emAberto;

            if (disponiveis < 0)
            {
                disponiveis = 0;
            }

            return new DisponibilidadeDTO
            {
                CodigoLivro = codigoLivro,
                Titulo = livro.Titulo,
                ExemplaresTotal = livro.QuantidadeExemplares,
                EmprestimosEmAberto = emAberto,
                ExemplaresDisponiveis = disponiveis,
                Disponivel = disponiveis > 0
            };
        }

        public List<Emprestimo> Listar()
        {
            return _dataContext.Emprestimos.ToList();
        }

        public async Task<EmprestimoDetalhadoDTO?> BuscarPorId(Guid id)
        {
            var emprestimo = _dataContext.Emprestimos.FirstOrDefault(p => p.Id == id);

            if (emprestimo == null)
            {
                return null;
            }

            // Integrações 1 e 2: exibe os dados do leitor e do livro no empréstimo.
            var leitor = await _leitorClient.BuscarLeitor(emprestimo.CodigoLeitor);

            if (leitor == null)
            {
                throw new Exception("Leitor do empréstimo não encontrado.");
            }

            var livro = await _livroClient.BuscarLivro(emprestimo.CodigoLivro);

            if (livro == null)
            {
                throw new Exception("Livro do empréstimo não encontrado.");
            }

            return new EmprestimoDetalhadoDTO
            {
                Id = emprestimo.Id,
                DataEmprestimo = emprestimo.DataEmprestimo,
                DataPrevistaDevolucao = emprestimo.DataPrevistaDevolucao,
                DataDevolucao = emprestimo.DataDevolucao,
                Devolvido = emprestimo.DataDevolucao != null,
                Leitor = leitor,
                Livro = livro
            };
        }

        public Emprestimo? Devolver(Guid id)
        {
            var emprestimo = _dataContext.Emprestimos.FirstOrDefault(p => p.Id == id);

            if (emprestimo == null)
            {
                return null;
            }

            if (emprestimo.DataDevolucao != null)
            {
                throw new Exception("Este empréstimo já foi devolvido.");
            }

            emprestimo.DataDevolucao = DateTime.Today;

            _dataContext.SaveChanges();

            return emprestimo;
        }
    }
}

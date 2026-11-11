using LivrosApp.DTO;

namespace LivrosApp.Servicos
{
    public class ServLivro
    {
        private readonly DataContext _dataContext;

        public ServLivro(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Livro Inserir(SalvarLivroDTO dto)
        {
            Validar(dto);

            var livro = new Livro
            {
                Id = Guid.NewGuid(),
                Titulo = dto.Titulo.Trim(),
                Autor = dto.Autor.Trim(),
                Isbn = dto.Isbn?.Trim() ?? string.Empty,
                Categoria = dto.Categoria?.Trim() ?? string.Empty,
                QuantidadeExemplares = dto.QuantidadeExemplares
            };

            _dataContext.Livros.Add(livro);
            _dataContext.SaveChanges();

            return livro;
        }

        public void Validar(SalvarLivroDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Titulo))
            {
                throw new Exception("Falta informar o título do livro.");
            }

            if (string.IsNullOrWhiteSpace(dto.Autor))
            {
                throw new Exception("Falta informar o autor do livro.");
            }

            if (dto.QuantidadeExemplares < 0)
            {
                throw new Exception("A quantidade de exemplares não pode ser negativa.");
            }
        }

        public List<Livro> Listar()
        {
            return _dataContext.Livros.OrderBy(p => p.Titulo).ToList();
        }

        public Livro? BuscarPorId(Guid id)
        {
            return _dataContext.Livros.FirstOrDefault(p => p.Id == id);
        }

        public Livro? Editar(Guid id, SalvarLivroDTO dto)
        {
            Validar(dto);

            var livro = BuscarPorId(id);

            if (livro == null)
            {
                return null;
            }

            livro.Titulo = dto.Titulo.Trim();
            livro.Autor = dto.Autor.Trim();
            livro.Isbn = dto.Isbn?.Trim() ?? string.Empty;
            livro.Categoria = dto.Categoria?.Trim() ?? string.Empty;
            livro.QuantidadeExemplares = dto.QuantidadeExemplares;

            _dataContext.SaveChanges();

            return livro;
        }

        public bool Excluir(Guid id)
        {
            var livro = BuscarPorId(id);

            if (livro == null)
            {
                return false;
            }

            _dataContext.Livros.Remove(livro);
            _dataContext.SaveChanges();

            return true;
        }
    }
}

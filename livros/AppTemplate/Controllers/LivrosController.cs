using LivrosApp.DTO;
using LivrosApp.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace LivrosApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LivrosController : ControllerBase
    {
        private readonly ServLivro _servLivro;

        public LivrosController(ServLivro servLivro)
        {
            _servLivro = servLivro;
        }

        [HttpPost]
        public IActionResult Inserir(SalvarLivroDTO dto)
        {
            try
            {
                var livro = _servLivro.Inserir(dto);

                return Ok(livro);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                return Ok(_servLivro.Listar());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPorId(Guid id)
        {
            try
            {
                var livro = _servLivro.BuscarPorId(id);

                if (livro == null)
                {
                    return NotFound("Livro não encontrado.");
                }

                return Ok(livro);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Editar(Guid id, SalvarLivroDTO dto)
        {
            try
            {
                var livro = _servLivro.Editar(id, dto);

                if (livro == null)
                {
                    return NotFound("Livro não encontrado.");
                }

                return Ok(livro);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Excluir(Guid id)
        {
            try
            {
                if (!_servLivro.Excluir(id))
                {
                    return NotFound("Livro não encontrado.");
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

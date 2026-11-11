using EmprestimosApp.DTO;
using EmprestimosApp.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace EmprestimosApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmprestimosController : ControllerBase
    {
        private readonly ServEmprestimo _servEmprestimo;

        public EmprestimosController(ServEmprestimo servEmprestimo)
        {
            _servEmprestimo = servEmprestimo;
        }

        [HttpPost]
        public async Task<IActionResult> Inserir(InserirEmprestimoDTO dto)
        {
            try
            {
                var emprestimo = await _servEmprestimo.Inserir(dto);

                return Ok(emprestimo);
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
                return Ok(_servEmprestimo.Listar());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(Guid id)
        {
            try
            {
                var emprestimo = await _servEmprestimo.BuscarPorId(id);

                if (emprestimo == null)
                {
                    return NotFound("Empréstimo não encontrado.");
                }

                return Ok(emprestimo);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}/devolucao")]
        public IActionResult Devolver(Guid id)
        {
            try
            {
                var emprestimo = _servEmprestimo.Devolver(id);

                if (emprestimo == null)
                {
                    return NotFound("Empréstimo não encontrado.");
                }

                return Ok(emprestimo);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("livros/{codigoLivro}/disponibilidade")]
        public async Task<IActionResult> ConsultarDisponibilidade(Guid codigoLivro)
        {
            try
            {
                var disponibilidade = await _servEmprestimo.ConsultarDisponibilidade(codigoLivro);

                return Ok(disponibilidade);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

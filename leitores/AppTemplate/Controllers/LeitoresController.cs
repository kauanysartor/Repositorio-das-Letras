using LeitoresApp.DTO;
using LeitoresApp.Servicos;
using Microsoft.AspNetCore.Mvc;

namespace LeitoresApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeitoresController : ControllerBase
    {
        private readonly ServLeitor _servLeitor;

        public LeitoresController(ServLeitor servLeitor)
        {
            _servLeitor = servLeitor;
        }

        [HttpPost]
        public IActionResult Inserir(SalvarLeitorDTO dto)
        {
            try
            {
                var leitor = _servLeitor.Inserir(dto);

                return Ok(leitor);
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
                return Ok(_servLeitor.Listar());
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
                var leitor = _servLeitor.BuscarPorId(id);

                if (leitor == null)
                {
                    return NotFound("Leitor não encontrado.");
                }

                return Ok(leitor);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Editar(Guid id, SalvarLeitorDTO dto)
        {
            try
            {
                var leitor = _servLeitor.Editar(id, dto);

                if (leitor == null)
                {
                    return NotFound("Leitor não encontrado.");
                }

                return Ok(leitor);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch("{id}/inativar")]
        public IActionResult Inativar(Guid id)
        {
            try
            {
                var leitor = _servLeitor.Inativar(id);

                if (leitor == null)
                {
                    return NotFound("Leitor não encontrado.");
                }

                return Ok(leitor);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPatch("{id}/divida")]
        public IActionResult AlterarDivida(Guid id, AlterarDividaDTO dto)
        {
            try
            {
                var leitor = _servLeitor.AlterarDivida(id, dto);

                if (leitor == null)
                {
                    return NotFound("Leitor não encontrado.");
                }

                return Ok(leitor);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

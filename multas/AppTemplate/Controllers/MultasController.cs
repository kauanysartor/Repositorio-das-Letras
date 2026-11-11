using Microsoft.AspNetCore.Mvc;
using MultasApp.DTO;
using MultasApp.Servicos;

namespace MultasApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MultasController : ControllerBase
    {
        private readonly ServMulta _servMulta;

        public MultasController(ServMulta servMulta)
        {
            _servMulta = servMulta;
        }

        [HttpPost]
        public async Task<IActionResult> Inserir(InserirMultaDTO dto)
        {
            try
            {
                var multa = await _servMulta.Inserir(dto);

                return Ok(multa);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}/pagamento")]
        public async Task<IActionResult> Pagar(Guid id)
        {
            try
            {
                var multa = await _servMulta.Pagar(id);

                if (multa == null)
                {
                    return NotFound("Multa não encontrada.");
                }

                return Ok(multa);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IActionResult Listar([FromQuery] Guid? leitorId, [FromQuery] EnumSituacaoMulta? situacao)
        {
            try
            {
                return Ok(_servMulta.Listar(leitorId, situacao));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}

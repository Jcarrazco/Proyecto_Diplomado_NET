using IndigoAssits.Core.Dtos;
using IndigoAssitsReglasDeNegocio.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndigoAssits.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ActivosController : ControllerBase
    {
        private readonly IActivoService _activoService;

        public ActivosController(IActivoService activoService)
        {
            _activoService = activoService;
        }

        [HttpGet]
        public async Task<ActionResult<ActivoPaginadoDto>> Get([FromQuery] ActivoFiltroDto filtros)
        {
            var result = await _activoService.GetActivosPaginadosAsync(filtros);
            return Ok(result);
        }

        [HttpGet("todos")]
        public async Task<ActionResult<IEnumerable<ActivoDto>>> GetAll([FromQuery] ActivoFiltroDto? filtros = null)
        {
            // Si no hay filtros, crear uno vacío para obtener todos los activos
            filtros ??= new ActivoFiltroDto();
            
            var result = await _activoService.GetActivosAsync(filtros);
            var resultList = result.ToList();
            
            // Log para diagnóstico
            System.Diagnostics.Debug.WriteLine($"GetAll activos - Filtros recibidos: {filtros != null}, Resultados: {resultList.Count}");
            
            return Ok(resultList);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ActivoDto>> GetById(int id)
        {
            var result = await _activoService.GetPorIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] ActivoCreateDto dto)
        {
            var id = await _activoService.CrearAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ActivoUpdateDto dto)
        {
            if (id != dto.IdActivo) return BadRequest("Id inconsistente");
            var ok = await _activoService.ActualizarAsync(dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _activoService.EliminarAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}


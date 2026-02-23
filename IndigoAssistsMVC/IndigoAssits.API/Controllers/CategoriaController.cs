using IndigoAssits.Core.Dtos;
using IndigoAssitsReglasDeNegocio.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IndigoAssits.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
        {
            var cats = await _categoriaService.GetCategoriasAsync();
            return Ok(cats);
        }

        [HttpGet("departamento/{id:int}")]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetPorDepartamento(int id)
        {
            if (id < 0 || id > 255) return BadRequest("El ID del departamento debe estar entre 0 y 255");
            var cats = await _categoriaService.GetCategoriasPorDepartamentoAsync((byte)id);
            return Ok(cats);
        }
    }
}

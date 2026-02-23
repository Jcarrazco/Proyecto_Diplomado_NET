using IndigoAssits.Core.Dtos;
using IndigoAssits.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndigoAssits.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CatalogoController : ControllerBase
    {
        private readonly ActivosDbContext _context;

        public CatalogoController(ActivosDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los tipos de activo
        /// </summary>
        [HttpGet("tipos-activo")]
        public async Task<ActionResult<IEnumerable<TipoActivoDto>>> GetTiposActivo()
        {
            var tipos = await _context.TiposActivo
                .Select(t => new TipoActivoDto
                {
                    IdTipoActivo = t.IdTipoActivo,
                    TipoActivoNombre = t.TipoActivoNombre
                })
                .ToListAsync();

            return Ok(tipos);
        }

        /// <summary>
        /// Obtiene todos los status de activos
        /// </summary>
        [HttpGet("status-activo")]
        public async Task<ActionResult<IEnumerable<StatusActivoDto>>> GetStatusActivo()
        {
            var status = await _context.Status
                .Select(s => new StatusActivoDto
                {
                    StatusId = s.StatusId,
                    StatusNombre = s.StatusNombre
                })
                .ToListAsync();

            return Ok(status);
        }

        /// <summary>
        /// Obtiene todos los proveedores
        /// </summary>
        [HttpGet("proveedores")]
        public async Task<ActionResult<IEnumerable<ProveedorDto>>> GetProveedores()
        {
            var proveedores = await _context.Proveedores
                .Select(p => new ProveedorDto
                {
                    IdProveedor = p.IdProveedor,
                    ProveedorNombre = p.ProveedorNombre
                })
                .ToListAsync();

            return Ok(proveedores);
        }

        /// <summary>
        /// Obtiene todos los componentes
        /// </summary>
        [HttpGet("componentes")]
        public async Task<ActionResult<IEnumerable<ComponenteDto>>> GetComponentes()
        {
            var componentes = await _context.Componentes
                .Select(c => new ComponenteDto
                {
                    IdComponente = c.IdComponente,
                    ComponenteNombre = c.ComponenteNombre,
                    ValorBit = c.ValorBit
                })
                .ToListAsync();

            return Ok(componentes);
        }

        /// <summary>
        /// Obtiene todos los departamentos
        /// </summary>
        [HttpGet("departamentos")]
        public async Task<ActionResult<IEnumerable<DepartamentoDto>>> GetDepartamentos()
        {
            var departamentos = await _context.Departamentos
                .Select(d => new DepartamentoDto
                {
                    IdDepto = d.IdDepto,
                    Departamento = d.Departamento,
                    Tickets = d.Tickets
                })
                .ToListAsync();

            return Ok(departamentos);
        }
    }
}


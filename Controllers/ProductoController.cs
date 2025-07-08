using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Api_Polleria.Modelos;
using Proyecto_Api_Polleria.Modelos.Dtos;
using Proyecto_Api_Polleria.Repositorio.IRepositorio;

namespace Proyecto_Api_Polleria.Controllers
{
    [Route("api/producto")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoRepositorio _prRepo;
        private readonly IMapper _mapper;

        public ProductoController(IProductoRepositorio prRepo, IMapper mapper)
        {
            _prRepo = prRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetProducto()
        {
            var listaProducto = _prRepo.GetProducto();
            var listaProductoDto = new List<ProductoDto>();

            foreach (var producto in listaProducto)
            {
                listaProductoDto.Add(_mapper.Map<ProductoDto>(producto));
            }

            return Ok(listaProductoDto);
        }

        [HttpGet("{productoId:int}", Name = "GetProducto")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetProducto(int productoId)
        {
            if (productoId <= 0)
            {
                return BadRequest("El Id del producto debe ser mayor a 0");
            }

            var producto = _prRepo.GetProducto(productoId);

            if (producto == null)
            {
                return NotFound($"No se encontro el producto con el Id {productoId}");
            }

            var productoDto = _mapper.Map<ProductoDto>(producto);
            return Ok(productoDto);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearProducto([FromBody] CrearProductoDto crearProductoDto)
        {
            if (crearProductoDto == null || !ModelState.IsValid)
            {
                return BadRequest("Datos invalidos para crear producto");
            }

            if (_prRepo.ExisteProducto(crearProductoDto.Nombre))
            {
                return Conflict("Ya existe un producto con ese nombre");
            }

            var producto = _mapper.Map<Producto>(crearProductoDto);

            if (!_prRepo.CrearProducto(producto))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al guardar el producto");
            }

            var productoDto = _mapper.Map<ProductoDto>(producto);
            return CreatedAtRoute("GetProducto", new { productoId = producto.Id }, productoDto);
        }

        [HttpPatch("{productoId:int}", Name = "ActualizarPatchProducto")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ActualizarPatchProducto(int productoId, [FromBody] ProductoDto productoDto)
        {
            if (productoDto == null || !ModelState.IsValid || productoId != productoDto.Id)
            {
                return BadRequest("Datos invalidos para actualizar el producto");
            }

            var productoExistente = _prRepo.GetProducto(productoId);

            if (productoExistente == null)
            {
                return NotFound($"No se encontro el producto con el Id {productoId}");
            }

            var producto = _mapper.Map<Producto>(productoDto);

            if (!_prRepo.ActualizarProducto(producto))
            {
                ModelState.AddModelError("", $"Algo salio mal al actualizar el registro{producto.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{productoId:int}", Name = "BorrarPatchProducto")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult BorrarPatchProducto(int productoId)
        {
            var producto = _prRepo.GetProducto(productoId);
            
            if (producto == null)
            {
                return NotFound($"No se encontro el producto con el Id {productoId}");
            }

            if (!_prRepo.EliminarProducto(producto))
            {
                ModelState.AddModelError("", $"Algo salio mal al eliminar el registro {producto.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Api_Polleria.Modelos;
using Proyecto_Api_Polleria.Modelos.Dtos;
using Proyecto_Api_Polleria.Repositorio.IRepositorio;

namespace Proyecto_Api_Polleria.Controllers
{
    [Route("api/detallePedido")]
    [ApiController]
    [Authorize]
    public class DetallePedidoController : ControllerBase
    {
        private readonly IDetallePedidoRepositorio _dpRepo;
        private readonly IMapper _mapper;

        public DetallePedidoController(IDetallePedidoRepositorio dpRepo, IMapper mapper)
        {
            _dpRepo = dpRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetDetallePedido()
        {
            var listaDetallePedido = _dpRepo.GetDetallePedido();
            var listaDetallePedidoDto = new List<DetallePedidoDto>();

            foreach (var detallepedido in listaDetallePedido)
            {
                listaDetallePedidoDto.Add(_mapper.Map<DetallePedidoDto>(detallepedido));
            }

            return Ok(listaDetallePedidoDto);
        }

        [HttpGet("{detallepedidoId:int}", Name = "GetDetallePedido")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetDetallePedido(int detallepedidoId)
        {
            if (detallepedidoId <= 0)
            {
                return BadRequest("El Id del detallepedido debe ser mayor a 0");
            }

            var detallepedido = _dpRepo.GetDetallePedido(detallepedidoId);

            if (detallepedido == null)
            {
                return NotFound($"No se encontro el detallepedido con el Id {detallepedidoId}");
            }

            var detallepedidoDto = _mapper.Map<DetallePedidoDto>(detallepedido);
            return Ok(detallepedidoDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearDetallePedido([FromBody] CrearDetallePedidoDto crearDetallePedidoDto)
        {
            if (crearDetallePedidoDto == null || !ModelState.IsValid)
            {
                return BadRequest("Datos inválidos para crear el detalle del pedido.");
            }

            var detallepedido = _mapper.Map<DetallePedido>(crearDetallePedidoDto);

            if (!_dpRepo.CrearDetallePedido(detallepedido))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al guardar el detalle del pedido.");
            }

            var detallepedidoDto = _mapper.Map<DetallePedidoDto>(detallepedido);
            return CreatedAtRoute("GetDetallePedido", new { detallepedidoId = detallepedido.Id }, detallepedidoDto);
        }

        [HttpPatch("{detallepedidoId:int}", Name = "ActualizarPatchDetallePedido")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ActualizarPatchDetallePedido(int detallepedidoId, [FromBody] DetallePedidoDto detallepedidoDto)
        {
            if (detallepedidoDto == null || !ModelState.IsValid || detallepedidoId != detallepedidoDto.Id)
            {
                return BadRequest("Datos invalidos para actualizar el detallepedido");
            }

            var detallepedidoExistente = _dpRepo.GetDetallePedido(detallepedidoId);

            if (detallepedidoExistente == null)
            {
                return NotFound($"No se encontro el detallepedido con el Id {detallepedidoId}");
            }

            var detallepedido = _mapper.Map<DetallePedido>(detallepedidoDto);

            if (!_dpRepo.ActualizarDetallePedido(detallepedido))
            {
                ModelState.AddModelError("", $"Algo salio mal al actualizar el registro{detallepedido.Id}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{detallepedidoId:int}", Name = "BorrarPatchDetallePedido")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult BorrarPatchDetallePedido(int detallepedidoId)
        {
            var detallepedido = _dpRepo.GetDetallePedido(detallepedidoId);

            if (detallepedido == null)
            {
                return NotFound($"No se encontro el detallepedido con el Id {detallepedidoId}");
            }

            if (!_dpRepo.EliminarDetallePedido(detallepedido))
            {
                ModelState.AddModelError("", $"Algo salio mal al eliminar el registro {detallepedido.Id}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
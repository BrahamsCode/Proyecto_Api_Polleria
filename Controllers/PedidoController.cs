﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Api_Polleria.Modelos;
using Proyecto_Api_Polleria.Modelos.Dtos;
using Proyecto_Api_Polleria.Repositorio.IRepositorio;

namespace Proyecto_Api_Polleria.Controllers
{
    [Route("api/pedido")]
    [ApiController]
    [Authorize] 
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoRepositorio _pRepo;
        private readonly IMapper _mapper;

        public PedidoController(IPedidoRepositorio pRepo, IMapper mapper)
        {
            _pRepo = pRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetPedido()
        {
            var listaPedido = _pRepo.GetPedido();
            var listaPedidoDto = new List<PedidoDto>();

            foreach (var pedido in listaPedido)
            {
                listaPedidoDto.Add(_mapper.Map<PedidoDto>(pedido));
            }

            return Ok(listaPedidoDto);
        }

        // GET por ID protegido
        [HttpGet("{pedidoId:int}", Name = "GetPedido")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetPedido(int pedidoId)
        {
            if (pedidoId <= 0)
            {
                return BadRequest("El Id del pedido debe ser mayor a 0");
            }

            var pedido = _pRepo.GetPedido(pedidoId);

            if (pedido == null)
            {
                return NotFound($"No se encontro el pedido con el Id {pedidoId}");
            }

            var pedidoDto = _mapper.Map<PedidoDto>(pedido);
            return Ok(pedidoDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CrearPedido([FromBody] CrearPedidoDto crearPedidoDto)
        {
            if (crearPedidoDto == null || !ModelState.IsValid)
            {
                return BadRequest("Datos invalidos para crear pedido");
            }

            var pedido = _mapper.Map<Pedido>(crearPedidoDto);

            if (!_pRepo.CrearPedido(pedido))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al guardar el pedido");
            }

            var pedidoDto = _mapper.Map<PedidoDto>(pedido);
            return CreatedAtRoute("GetPedido", new { pedidoId = pedido.Id }, pedidoDto);
        }

        [HttpPatch("{pedidoId:int}", Name = "ActualizarPatchPedido")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ActualizarPatchPedido(int pedidoId, [FromBody] PedidoDto pedidoDto)
        {
            if (pedidoDto == null || !ModelState.IsValid || pedidoId != pedidoDto.Id)
            {
                return BadRequest("Datos invalidos para actualizar el pedido");
            }

            var pedidoExistente = _pRepo.GetPedido(pedidoId);

            if (pedidoExistente == null)
            {
                return NotFound($"No se encontro el pedido con el Id {pedidoId}");
            }

            var pedido = _mapper.Map<Pedido>(pedidoDto);

            if (!_pRepo.ActualizarPedido(pedido))
            {
                ModelState.AddModelError("", $"Algo salio mal al actualizar el registro{pedido.Cliente}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{pedidoId:int}", Name = "BorrarPatchPedido")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult BorrarPatchPedido(int pedidoId)
        {
            var pedido = _pRepo.GetPedido(pedidoId);
            
            if (pedido == null)
            {
                return NotFound($"No se encontro el pedido con el Id {pedidoId}");
            }

            if (!_pRepo.EliminarPedido(pedido))
            {
                ModelState.AddModelError("", $"Algo salio mal al eliminar el registro {pedido.Cliente}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}

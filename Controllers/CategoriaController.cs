using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Api_Polleria.Modelos;
using Proyecto_Api_Polleria.Modelos.Dtos;
using Proyecto_Api_Polleria.Repositorio.IRepositorio;

namespace Proyecto_Api_Polleria.Controllers
{
    [Route("api/categorias")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaRepositorio _ctRepo;
        private readonly IMapper _mapper;

        public CategoriaController(ICategoriaRepositorio ctRepo, IMapper mapper)
        {
            _ctRepo = ctRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public IActionResult GetCategorias()
        {
            var listaCategorias = _ctRepo.GetCategoria();
            var listaCategoriasDto = new List<CategoriaDto>();

            foreach (var categoria in listaCategorias)
            {
                listaCategoriasDto.Add(_mapper.Map<CategoriaDto>(categoria));
            }

            return Ok(listaCategoriasDto);
        }

        [HttpGet("{categoriaId:int}", Name = "GetCategoria")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult GetCategoria(int categoriaId)
        {
            if (categoriaId <= 0)
            {
                return BadRequest("El Id de la categoria debe ser mayor a 0");
            }

            var categoria = _ctRepo.GetCategoria(categoriaId);

            if (categoria == null)
            {
                return NotFound($"No se encontro la categoria con el Id {categoriaId}");
            }

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);
            return Ok(categoriaDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult CrearCategoria([FromBody] CrearCategoriaDto crearCategoriaDto)
        {
            if (crearCategoriaDto == null || !ModelState.IsValid)
            {
                return BadRequest("Datos invalidos para crear categoria");
            }

            if (_ctRepo.ExisteCategoria(crearCategoriaDto.Nombre))
            {
                return Conflict("Ya existe una categoria con ese nombre");
            }

            var categoria = _mapper.Map<Categoria>(crearCategoriaDto);

            if (!_ctRepo.CrearCategoria(categoria))
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al guardar la categoria");
            }

            var categoriaDto = _mapper.Map<CategoriaDto>(categoria);

            return CreatedAtRoute("GetCategoria", new { categoriaId = categoriaDto.Id }, categoriaDto);
        }
        [HttpPatch("{categoriaId:int}", Name = "ActualizarPatchCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult ActualizarPatchCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {
            if (categoriaDto == null || !ModelState.IsValid || categoriaId != categoriaDto.Id)
            {
                return BadRequest("Datos invalidos para crear la categoria");
            }

            var categoriaExistente = _ctRepo.GetCategoria(categoriaId);

            if (categoriaExistente == null)
            {
                return NotFound($"No se encontro la categoria con el Id {categoriaId}");
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            if (!_ctRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal al actualizar el registro{categoria.Nombre}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
        //Borrar
        [HttpDelete("{categoriaId:int}", Name = "BorrarPatchCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public IActionResult BorrarPatchCategoria(int categoriaId)
        {
            if (_ctRepo.ExisteCategoria(categoriaId) == null)
            {
                return NotFound($"No se encontro la categoria con el Id {categoriaId}");
            }

            var categoria = _ctRepo.GetCategoria(categoriaId);

            if (!_ctRepo.EliminarCategoria(categoria)) //si no se elimina la categoria
            {
                ModelState.AddModelError("", $"Algo salio mal al eliminar el registro {categoria.Nombre}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}

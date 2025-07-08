using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Api_Polleria.Modelos;
using Proyecto_Api_Polleria.Modelos.Dtos;
using Proyecto_Api_Polleria.Repositorio.IRepositorio;
using System.Net;

namespace Proyecto_Api_Polleria.Controllers
{
    [Route("api/usuario")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepositorio _usuRepo;
        private readonly IMapper _mapper;
        protected RespuestasAPI _respuestasAPI;

        public UsuarioController(IUsuarioRepositorio usuRepo, IMapper mapper)
        {
            _usuRepo = usuRepo;
            _mapper = mapper;
            this._respuestasAPI = new();
        }

        [HttpGet]
        [Authorize] 
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetUsuario()
        {
            var listaUsuario = _usuRepo.GetUsuario();
            var listaUsuarioDto = new List<UsuarioDto>();

            foreach (var usuario in listaUsuario)
            {
                listaUsuarioDto.Add(_mapper.Map<UsuarioDto>(usuario));
            }

            return Ok(listaUsuarioDto);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto usuarioLoginDto)
        {
            var respuestaLogin = await _usuRepo.Login(usuarioLoginDto);

            if (respuestaLogin.usuario == null || string.IsNullOrEmpty(respuestaLogin.Token))
            {
                _respuestasAPI.StatusCode = HttpStatusCode.BadRequest;
                _respuestasAPI.IsSuccess = false;
                _respuestasAPI.ErrorMessages.Add("El nombre de usuario o password son incorrectos");
                return BadRequest(_respuestasAPI);
            }

            _respuestasAPI.StatusCode = HttpStatusCode.OK;
            _respuestasAPI.IsSuccess = true;
            _respuestasAPI.Result = respuestaLogin;
            
            return Ok(_respuestasAPI);
        }

        [HttpPost("Registro")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto usuarioRegistroDto)
        {
            bool validarNombreUsuarioUnico = _usuRepo.IsUniqueUsuario(usuarioRegistroDto.NombreUsuario);

            if (!validarNombreUsuarioUnico)
            {
                _respuestasAPI.StatusCode = HttpStatusCode.BadRequest;
                _respuestasAPI.IsSuccess = false;
                _respuestasAPI.ErrorMessages.Add("El nombre de usuario ya existe");
                return BadRequest(_respuestasAPI);
            }

            var usuario = await _usuRepo.Registro(usuarioRegistroDto);

            if (usuario == null)
            {
                _respuestasAPI.StatusCode = HttpStatusCode.BadRequest;
                _respuestasAPI.IsSuccess = false;
                _respuestasAPI.ErrorMessages.Add("Error en el registro");
                return BadRequest(_respuestasAPI);
            }

            _respuestasAPI.StatusCode = HttpStatusCode.OK;
            _respuestasAPI.IsSuccess = true;
            _respuestasAPI.Result = "Usuario registrado correctamente";
            
            return Ok(_respuestasAPI);
        }

        [HttpPatch("ActualizarPass")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarPassword([FromBody] UsuarioActualizarPasswordDto usuarioActualizarPasswordDto)
        {
            if (usuarioActualizarPasswordDto == null || string.IsNullOrWhiteSpace(usuarioActualizarPasswordDto.NombreUsuario)
                || string.IsNullOrWhiteSpace(usuarioActualizarPasswordDto.PasswordActual)
                || string.IsNullOrWhiteSpace(usuarioActualizarPasswordDto.NuevoPassword))
            {
                return BadRequest("Los datos deben estar completos");
            }

            var passwordActualizado = await _usuRepo.ActualizarPassword(usuarioActualizarPasswordDto);

            if (!passwordActualizado)
            {
                _respuestasAPI.StatusCode = HttpStatusCode.BadRequest;
                _respuestasAPI.IsSuccess = false;
                _respuestasAPI.ErrorMessages.Add("Usuario no encontrado o password actual incorrecto");
                return BadRequest(_respuestasAPI);
            }

            _respuestasAPI.StatusCode = HttpStatusCode.OK;
            _respuestasAPI.IsSuccess = true;
            _respuestasAPI.Result = "Contraseña actualizada correctamente";
            
            return Ok(_respuestasAPI);
        }
    }
}
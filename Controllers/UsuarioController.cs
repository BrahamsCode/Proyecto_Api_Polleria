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

        // ⚠️ RUTA PROTEGIDA - Solo administradores pueden ver todos los usuarios
        [Authorize(Roles = "Admin")] // 👈 Agregamos autorización
        [HttpGet]
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

            _respuestasAPI.StatusCode = HttpStatusCode.OK;
            _respuestasAPI.IsSuccess = true;
            _respuestasAPI.Result = listaUsuarioDto;

            return Ok(_respuestasAPI);
        }

        // 🔓 RUTA PÚBLICA - Login no requiere autenticación
        [AllowAnonymous]
        [HttpPost("Login")]
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

            // ✅ CORREGIDO: Devolver el token y datos del usuario
            _respuestasAPI.StatusCode = HttpStatusCode.OK;
            _respuestasAPI.IsSuccess = true;
            _respuestasAPI.Result = respuestaLogin; // 👈 Aquí estaba el problema!

            return Ok(_respuestasAPI);
        }

        // 🔓 RUTA PÚBLICA - Registro no requiere autenticación
        [AllowAnonymous]
        [HttpPost("Registro")]
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
            _respuestasAPI.Result = "Usuario registrado exitosamente";

            return Ok(_respuestasAPI);
        }

        // 🔒 RUTA PROTEGIDA - Solo usuarios autenticados pueden cambiar su contraseña
        [Authorize]
        [HttpPatch("ActualizarPass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ActualizarPassword([FromBody] UsuarioActualizarPasswordDto usuarioActualizarPasswordDto)
        {
            if (usuarioActualizarPasswordDto == null || string.IsNullOrWhiteSpace(usuarioActualizarPasswordDto.NombreUsuario)
                || string.IsNullOrWhiteSpace(usuarioActualizarPasswordDto.PasswordActual)
                || string.IsNullOrWhiteSpace(usuarioActualizarPasswordDto.NuevoPassword))
            {
                _respuestasAPI.StatusCode = HttpStatusCode.BadRequest;
                _respuestasAPI.IsSuccess = false;
                _respuestasAPI.ErrorMessages.Add("Los datos deben estar rellenados");
                return BadRequest(_respuestasAPI);
            }

            // 🔒 SEGURIDAD: Verificar que el usuario solo pueda cambiar su propia contraseña
            var currentUserName = User.Identity?.Name;
            if (currentUserName != usuarioActualizarPasswordDto.NombreUsuario)
            {
                _respuestasAPI.StatusCode = HttpStatusCode.Forbidden;
                _respuestasAPI.IsSuccess = false;
                _respuestasAPI.ErrorMessages.Add("No puedes cambiar la contraseña de otro usuario");
                return Forbid();
            }

            var passwordActualizado = await _usuRepo.ActualizarPassword(usuarioActualizarPasswordDto);

            if (!passwordActualizado)
            {
                _respuestasAPI.StatusCode = HttpStatusCode.BadRequest;
                _respuestasAPI.IsSuccess = false;
                _respuestasAPI.ErrorMessages.Add("Usuario no encontrado o contraseña actual incorrecta");
                return BadRequest(_respuestasAPI);
            }

            _respuestasAPI.StatusCode = HttpStatusCode.OK;
            _respuestasAPI.IsSuccess = true;
            _respuestasAPI.Result = "Contraseña actualizada correctamente";
            return Ok(_respuestasAPI);
        }

        // 🔒 NUEVA RUTA: Obtener información del usuario autenticado
        [Authorize]
        [HttpGet("perfil")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetPerfil()
        {
            var userId = User.FindFirst("UserId")?.Value;
            var userName = User.Identity?.Name;
            var userRole = User.FindFirst("Role")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _respuestasAPI.StatusCode = HttpStatusCode.Unauthorized;
                _respuestasAPI.IsSuccess = false;
                _respuestasAPI.ErrorMessages.Add("Token inválido");
                return Unauthorized(_respuestasAPI);
            }

            var usuario = _usuRepo.GetUsuario(int.Parse(userId));
            if (usuario == null)
            {
                _respuestasAPI.StatusCode = HttpStatusCode.NotFound;
                _respuestasAPI.IsSuccess = false;
                _respuestasAPI.ErrorMessages.Add("Usuario no encontrado");
                return NotFound(_respuestasAPI);
            }

            var usuarioDto = _mapper.Map<UsuarioDto>(usuario);
            
            _respuestasAPI.StatusCode = HttpStatusCode.OK;
            _respuestasAPI.IsSuccess = true;
            _respuestasAPI.Result = usuarioDto;

            return Ok(_respuestasAPI);
        }
    }
}
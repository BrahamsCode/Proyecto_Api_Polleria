using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Proyecto_Api_Polleria.Data;
using Proyecto_Api_Polleria.Modelos;
using Proyecto_Api_Polleria.Modelos.Dtos;
using Proyecto_Api_Polleria.Repositorio.IRepositorio;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace Proyecto_Api_Polleria.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _context;
        private string claveSecreta;

        public UsuarioRepositorio(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            claveSecreta = config.GetValue<string>("APISettings:Secreta");
        }
        public ICollection<Usuario> GetUsuario()
        {
            return _context.Usuario.OrderBy(u => u.NombreUsuario).ToList();
        }

        public Usuario GetUsuario(int id)
        {
            throw new NotImplementedException();
        }

        public bool IsUniqueUsuario(string usuario)
        {
            var usuarioBD = _context.Usuario.FirstOrDefault(u => u.NombreUsuario == usuario);

            if (usuarioBD == null)
            {
                return true;
            }
            return false;
        }

        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
            var passwordEncrypted = obtenermd5(usuarioLoginDto.Password);

            var usuario = _context.Usuario.FirstOrDefault(u => u.NombreUsuario.ToLower() == usuarioLoginDto.NombreUsuario.ToLower()
                && u.Password == passwordEncrypted);

            if (usuario == null)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = "",
                    usuario = null
                };
            }
            var manejadorToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name,usuario.NombreUsuario.ToString()) ,
                    new Claim(ClaimTypes.Role,usuario.Rol)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = manejadorToken.CreateToken(tokenDescriptor);

            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            {
                Token = manejadorToken.WriteToken(token),
                usuario = usuario,
            };

            return usuarioLoginRespuestaDto;
        }

        public async Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            var passwordEncriptado = obtenermd5(usuarioRegistroDto.Password);

            Usuario usuario = new Usuario()
            {
                NombreUsuario = usuarioRegistroDto.NombreUsuario,
                Password = passwordEncriptado,
                Nombre = usuarioRegistroDto.Nombre,
                Rol = usuarioRegistroDto.Rol
            };

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();
            usuario.Password = passwordEncriptado;

            return usuario;
        }

        public async Task<bool> ActualizarPassword(UsuarioActualizarPasswordDto usuarioActualizarPasswordDto)
        {
            var usuario = _context.Usuario.FirstOrDefault(u => u.NombreUsuario == usuarioActualizarPasswordDto.NombreUsuario);

            if (usuario == null)
            {
                return false;
            }

            var passActualHash = obtenermd5(usuarioActualizarPasswordDto.PasswordActual);

            if (usuario.Password != passActualHash)
            {
                return false;
            }

            usuario.Password = obtenermd5(usuarioActualizarPasswordDto.NuevoPassword);
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public static string obtenermd5(string valor)

        {
            MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();

            byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);

            data = x.ComputeHash(data);

            string resp = "";

            for (int i = 0; i < data.Length; i++)

                resp += data[i].ToString("x2").ToLower();

            return resp;

        }
    }
}

using Proyecto_Api_Polleria.Modelos;
using Proyecto_Api_Polleria.Modelos.Dtos;

namespace Proyecto_Api_Polleria.Repositorio.IRepositorio
{
    public interface IUsuarioRepositorio
    {
        ICollection<Usuario> GetUsuario();

        Usuario GetUsuario(int id);
        bool IsUniqueUsuario(string usuario);
        Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto);
        Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto);
        Task<bool> ActualizarPassword(UsuarioActualizarPasswordDto usuarioActualizarPasswordDto);
    }
}

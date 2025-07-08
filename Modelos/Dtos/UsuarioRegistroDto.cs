using System.ComponentModel.DataAnnotations;

namespace Proyecto_Api_Polleria.Modelos.Dtos
{
    public class UsuarioRegistroDto
    {
        [Required(ErrorMessage = "El nombre usuario es obligatorio")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El password es obligatorio")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Rol { get; set; }
    }
}

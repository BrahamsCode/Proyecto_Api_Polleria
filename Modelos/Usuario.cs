using System.ComponentModel.DataAnnotations;

namespace Proyecto_Api_Polleria.Modelos
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }
        public string NombreUsuario { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
    }
}

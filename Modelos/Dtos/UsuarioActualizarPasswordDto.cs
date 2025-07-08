namespace Proyecto_Api_Polleria.Modelos.Dtos
{
    public class UsuarioActualizarPasswordDto
    {
        public string NombreUsuario { get; set; }
        public string PasswordActual { get; set; }
        public string NuevoPassword { get; set; }
    }
}

namespace Proyecto_Api_Polleria.Modelos.Dtos
{
    public class UsuarioLoginRespuestaDto
    {
        public Usuario usuario { get; set; }
        public string Rol { get; set; }
        public string Token { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Proyecto_Api_Polleria.Modelos.Dtos
{
    public class CrearPedidoDto
    {
        [Required(ErrorMessage = "Debe ingresar el nombre del cliente")]
        public string Cliente { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public List<CrearDetallePedidoDto> Detalles { get; set; }
    }
}

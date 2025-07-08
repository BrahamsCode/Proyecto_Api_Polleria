using System.ComponentModel.DataAnnotations;

namespace Proyecto_Api_Polleria.Modelos
{
    public class Pedido
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string Cliente { get; set; }

        public DateTime FechaPedido { get; set; }

        [Required]
        public string Estado { get; set; }

        // Lista de detalles del pedido
        public List<DetallePedido> Detalles { get; set; }
    }
}

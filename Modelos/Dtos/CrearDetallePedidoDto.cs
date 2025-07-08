using System.ComponentModel.DataAnnotations;

namespace Proyecto_Api_Polleria.Modelos.Dtos
{
    public class CrearDetallePedidoDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}

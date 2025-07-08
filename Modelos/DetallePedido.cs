using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Api_Polleria.Modelos
{
    public class DetallePedido
    {
        [Key]
        public int Id { get; set; }

        public int PedidoId { get; set; }

        // Relación con Pedido
        [ForeignKey(nameof(PedidoId))]
        public Pedido Pedido { get; set; }

        public int ProductoId { get; set; }

        // Relación con Productos
        [ForeignKey(nameof(ProductoId))]
        public Producto Producto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }
    }
}

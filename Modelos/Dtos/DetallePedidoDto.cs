namespace Proyecto_Api_Polleria.Modelos.Dtos
{
    public class DetallePedidoDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}

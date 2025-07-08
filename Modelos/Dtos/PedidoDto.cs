namespace Proyecto_Api_Polleria.Modelos.Dtos
{
    public class PedidoDto
    {
        public int Id { get; set; }

        public string Cliente { get; set; }

        public DateTime FechaPedido { get; set; }

        public string Estado { get; set; }

        public List<DetallePedidoDto> Detalles { get; set; }
    }
}

using Proyecto_Api_Polleria.Modelos;

namespace Proyecto_Api_Polleria.Repositorio.IRepositorio
{
    public interface IDetallePedidoRepositorio
    {
        ICollection<DetallePedido> GetDetallePedido();

        DetallePedido GetDetallePedido(int id);
        bool ExisteDetallePedido(int id);
        bool CrearDetallePedido(DetallePedido detallepedido);
        bool ActualizarDetallePedido(DetallePedido detallepedido);
        bool EliminarDetallePedido(DetallePedido detallepedido);
        bool GuardarDetallePedido();
    }
}

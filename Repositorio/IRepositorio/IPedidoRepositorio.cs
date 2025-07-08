using Proyecto_Api_Polleria.Modelos;

namespace Proyecto_Api_Polleria.Repositorio.IRepositorio
{
    public interface IPedidoRepositorio
    {
        ICollection<Pedido> GetPedido();

        Pedido GetPedido(int id);
        bool ExistePedido(int id);
        bool ExistePedido(string cliente);
        bool CrearPedido(Pedido pedido);
        bool ActualizarPedido(Pedido pedido);
        bool EliminarPedido(Pedido pedido);
        bool GuardarPedido();
    }
}

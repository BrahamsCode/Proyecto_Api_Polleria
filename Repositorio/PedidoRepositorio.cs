using Proyecto_Api_Polleria.Data;
using Proyecto_Api_Polleria.Modelos;
using Proyecto_Api_Polleria.Repositorio.IRepositorio;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Proyecto_Api_Polleria.Repositorio
{
    public class PedidoRepositorio : IPedidoRepositorio
    {
        private readonly ApplicationDbContext _context;

        public PedidoRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool ActualizarPedido(Pedido pedido)
        {
            pedido.FechaPedido = DateTime.Now;
            var pedidoExistente = _context.Pedido.Find(pedido.Id);
            if (pedidoExistente != null)
            {
                _context.Entry(pedidoExistente).CurrentValues.SetValues(pedido);
            }

            return GuardarPedido();
        }

        public bool CrearPedido(Pedido pedido)
        {
            pedido.FechaPedido = DateTime.Now;
            _context.Pedido.Add(pedido);
            return GuardarPedido();
        }

        public bool EliminarPedido(Pedido pedido)
        {
            _context.Pedido.Remove(pedido);
            return GuardarPedido();
        }

        public ICollection<Pedido> GetPedido()
        {
            return _context.Pedido.OrderBy(p => p.Cliente).ToList();
        }

        public Pedido GetPedido(int id)
        {
            return _context.Pedido.FirstOrDefault(p => p.Id == id);
        }

        public bool ExistePedido(int id)
        {
            bool valor = _context.Pedido.Any(p => p.Id == id);
            return valor;
        }

        public bool ExistePedido(string cliente)
        {
            bool valor = _context.Pedido.Any(p => p.Cliente.ToLower().Trim() == cliente.ToLower().Trim());
            return valor;
        }

        public bool GuardarPedido()
        {
            return _context.SaveChanges() >= 0 ? true : false;
        }
    }
}

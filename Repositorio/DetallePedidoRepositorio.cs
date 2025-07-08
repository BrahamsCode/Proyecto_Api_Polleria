using Proyecto_Api_Polleria.Data;
using Proyecto_Api_Polleria.Modelos;
using Proyecto_Api_Polleria.Repositorio.IRepositorio;

namespace Proyecto_Api_Polleria.Repositorio
{
    public class DetallePedidoRepositorio : IDetallePedidoRepositorio
    {
        private readonly ApplicationDbContext _context;

        public DetallePedidoRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool ActualizarDetallePedido(DetallePedido detallepedido)
        {
            var detallepedidoExistente = _context.DetallePedido.Find(detallepedido.Id);
            if (detallepedidoExistente != null)
            {
                _context.Entry(detallepedidoExistente).CurrentValues.SetValues(detallepedido);
            }

            return GuardarDetallePedido();
        }

        public bool CrearDetallePedido(DetallePedido detallepedido)
        {
            _context.DetallePedido.Add(detallepedido);
            return GuardarDetallePedido();
        }

        public bool EliminarDetallePedido(DetallePedido detallepedido)
        {
            _context.DetallePedido.Remove(detallepedido);
            return GuardarDetallePedido();
        }

        public ICollection<DetallePedido> GetDetallePedido()
        {
            return _context.DetallePedido.OrderBy(dp => dp.Id).ToList();
        }

        public DetallePedido GetDetallePedido(int id)
        {
            return _context.DetallePedido.FirstOrDefault(dp => dp.Id == id);
        }

        public bool ExisteDetallePedido(int id)
        {
            bool valor = _context.DetallePedido.Any(dp => dp.Id == id);
            return valor;
        }

        public bool GuardarDetallePedido()
        {
            return _context.SaveChanges() >= 0 ? true : false;
        }
    }
}

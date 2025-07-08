using Proyecto_Api_Polleria.Data;
using Proyecto_Api_Polleria.Modelos;
using Proyecto_Api_Polleria.Repositorio.IRepositorio;

namespace Proyecto_Api_Polleria.Repositorio
{
    public class ProductoRepositorio : IProductoRepositorio
    {
        private readonly ApplicationDbContext _context;

        public ProductoRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool ActualizarProducto(Producto producto)
        {
            producto.FechaRegistro = DateTime.Now;
            var productoExistente = _context.Producto.Find(producto.Id);
            if (productoExistente != null)
            {
                _context.Entry(productoExistente).CurrentValues.SetValues(producto);
            }

            return GuardarProducto();
        }

        public bool CrearProducto(Producto producto)
        {
            producto.FechaRegistro = DateTime.Now;
            _context.Producto.Add(producto);
            return GuardarProducto();
        }

        public bool EliminarProducto(Producto producto)
        {
            _context.Producto.Remove(producto);
            return GuardarProducto();
        }

        public ICollection<Producto> GetProducto()
        {
            return _context.Producto.OrderBy(pr => pr.Nombre).ToList();
        }

        public Producto GetProducto(int id)
        {
            return _context.Producto.FirstOrDefault(pr => pr.Id == id);
        }

        public bool ExisteProducto(int id)
        {
            bool valor = _context.Producto.Any(pr => pr.Id == id);
            return valor;
        }

        public bool ExisteProducto(string nombre)
        {
            bool valor = _context.Producto.Any(pr => pr.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;
        }

        public bool GuardarProducto()
        {
            return _context.SaveChanges() >= 0 ? true : false;
        }
    }
}

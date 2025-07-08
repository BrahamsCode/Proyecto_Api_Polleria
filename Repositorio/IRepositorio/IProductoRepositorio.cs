using Proyecto_Api_Polleria.Modelos;

namespace Proyecto_Api_Polleria.Repositorio.IRepositorio
{
    public interface IProductoRepositorio
    {
        ICollection<Producto> GetProducto();

        Producto GetProducto(int id);
        bool ExisteProducto(int id);
        bool ExisteProducto(string nombre);
        bool CrearProducto(Producto producto);
        bool ActualizarProducto(Producto producto);
        bool EliminarProducto(Producto producto);
        bool GuardarProducto();
    }
}

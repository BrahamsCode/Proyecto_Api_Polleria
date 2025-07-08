using Proyecto_Api_Polleria.Modelos;

namespace Proyecto_Api_Polleria.Repositorio.IRepositorio
{
    public interface ICategoriaRepositorio
    {
        ICollection<Categoria> GetCategoria();

        Categoria GetCategoria(int id);
        bool ExisteCategoria(int id);
        bool ExisteCategoria(string nombre);
        bool CrearCategoria(Categoria categoria);
        bool ActualizarCategoria(Categoria categoria);
        bool EliminarCategoria(Categoria categoria);
        bool GuardarCategoria();
    }
}

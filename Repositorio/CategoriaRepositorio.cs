using Proyecto_Api_Polleria.Data;
using Proyecto_Api_Polleria.Modelos;
using Proyecto_Api_Polleria.Repositorio.IRepositorio;

namespace Proyecto_Api_Polleria.Repositorio
{
    public class CategoriaRepositorio : ICategoriaRepositorio
    {
        private readonly ApplicationDbContext _context;

        public CategoriaRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool ActualizarCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            var categoriaExistente = _context.Categoria.Find(categoria.Id);
            if (categoriaExistente != null)
            {
                _context.Entry(categoriaExistente).CurrentValues.SetValues(categoria);
            }

            return GuardarCategoria();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            categoria.FechaCreacion = DateTime.Now;
            _context.Categoria.Add(categoria);
            return GuardarCategoria();
        }

        public bool EliminarCategoria(Categoria categoria)
        {
            _context.Categoria.Remove(categoria);
            return GuardarCategoria();
        }

        public ICollection<Categoria> GetCategoria()
        {
            return _context.Categoria.OrderBy(c => c.Nombre).ToList();
        }

        public Categoria GetCategoria(int id)
        {
            return _context.Categoria.FirstOrDefault(c => c.Id == id);
        }

        public bool ExisteCategoria(int id)
        {
            bool valor = _context.Categoria.Any(c => c.Id == id);
            return valor;
        }

        public bool ExisteCategoria(string nombre)
        {
            bool valor = _context.Categoria.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            return valor;
        }

        public bool GuardarCategoria()
        {
            return _context.SaveChanges() >= 0 ? true : false;
        }
    }
}

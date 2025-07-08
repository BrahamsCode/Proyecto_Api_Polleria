using Microsoft.EntityFrameworkCore;
using Proyecto_Api_Polleria.Modelos;

namespace Proyecto_Api_Polleria.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<DetallePedido> DetallePedido { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Proyecto_Api_Polleria.Modelos
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Proyecto_Api_Polleria.Modelos.Dtos
{
    public class CategoriaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "El numero maximo es 100")]
        public string Nombre { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}

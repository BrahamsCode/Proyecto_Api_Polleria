using System.ComponentModel.DataAnnotations;

namespace Proyecto_Api_Polleria.Modelos.Dtos
{
    public class CrearProductoDto
    {
        [Required(ErrorMessage = "El campo Nombre es obligatorio")]
        [MaxLength(100)]
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string ImagenUrl { get; set; }
        public int CategoriaId { get; set; }
    }
}

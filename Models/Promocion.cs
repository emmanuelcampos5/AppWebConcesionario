using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppWebConcesionario.Models
{
    public class Promocion
    {
        [Key, ForeignKey("Vehiculo")]
        public int idVehiculo { get; set; }

        [Required(ErrorMessage = "El precio de promocion es obligatorio")]
        [Range(0, Int32.MaxValue, ErrorMessage = "El precio debe ser un numero dentro del rango permitido")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "No se permiten letras en el precio")]
        public double precioPromocion { get; set; }

        [Required(ErrorMessage ="Seleccione el lugar de la promocion")]
        public string lugarPromocion { get; set; }


    }
}

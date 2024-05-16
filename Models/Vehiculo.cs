using System.ComponentModel.DataAnnotations;

namespace AppWebConcesionario.Models
{
    public class Vehiculo
    {
        [Key]
        public int idVehiculo { get; set; }

        [Required(ErrorMessage = "Debe ingresar la Marca del Carro")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string marcaVehiculo { get; set; }

        [Required(ErrorMessage = "Debe ingresar el Model del Carro")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string modeloVehiculo { get; set; }

        [Required(ErrorMessage = "Debe ingresar el tipo de combustible")]
        [DataType(DataType.Text)]
        [StringLength(50)]
        public string tipoCombustible { get; set; }

        [Required (ErrorMessage = "Seleccione el precio del mismo =")]
        [Range(0,Int32.MaxValue)]
        public double precioVehiculo { get; set; }

        [Required(ErrorMessage = "Selecciones un Estado")]
        public bool estadoActivo { get; set; }

        [Required(ErrorMessage = "Seleccione una foto")]
        [DataType(DataType.Text)]
        public string imagenUrl { get; set; }

    }
}

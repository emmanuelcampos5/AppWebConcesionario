using System.ComponentModel.DataAnnotations;

namespace AppWebConcesionario.Models
{
    public class Vehiculo
    {
        [Key]
        public int idVehiculo { get; set; }
        public string marcaVehiculo { get; set; }
        public string modeloVehiculo { get; set; }
        public string tipoCombustible { get; set; }
        public double precioVehiculo { get; set; }
        public bool estadoActivo { get; set; }
    }
}

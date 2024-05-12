using System.ComponentModel.DataAnnotations;

namespace AppWebConcesionario.Models
{
    public class Inventario
    {
        [Key]
        public int idVehiculo { get; set; }
        public int cantidadVehiculos { get; set; }


    }
}

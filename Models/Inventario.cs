using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppWebConcesionario.Models
{
    public class Inventario
    {
        [Key, ForeignKey("Vehiculo")]
        public int idVehiculo { get; set; }
        public int cantidadVehiculos { get; set; }

    }
}

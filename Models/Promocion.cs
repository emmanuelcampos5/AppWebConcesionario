using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppWebConcesionario.Models
{
    public class Promocion
    {
        [Key, ForeignKey("Vehiculo")]
        public int idVehiculo { get; set; }
        public double precioPromocion { get; set; }
        public string lugarPromocion { get; set; }


    }
}

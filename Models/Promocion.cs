using System.ComponentModel.DataAnnotations;

namespace AppWebConcesionario.Models
{
    public class Promocion
    {
        [Key]
        public int idVehiculo { get; set; }
        public double precioPromocion { get; set; }
        public string lugarPromocion { get; set; }

        //dylan
        public Vehiculo Vehiculo { get; set; }

        //dylan
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppWebConcesionario.Models
{
    public class Det_Factura
    {

        [Key]
        public int idDetalle { get; set; }


        [ForeignKey("Factura")]
        public int idFactura { get; set; }

        [ForeignKey("Vehiculo")]
        public int idVehiculo { get; set; }
        public double precioUnitario { get; set; }
        public int cantidad { get; set; }
        public double subtotal { get; set; }
        public double montoDescuento { get; set; }
        public double montoImpuesto { get; set; }

    }
}

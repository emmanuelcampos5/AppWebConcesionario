using System.ComponentModel.DataAnnotations;

namespace AppWebConcesionario.Models
{
    public class Factura
    {
        [Key]
        public int idFactura { get; set; }
        public int idUsuario { get; set; }
        public double subtotal { get; set; }
        public double montoDescuento { get; set; }
        public double montoImpuesto { get; set; }
        public double totalFactura { get; set; }
        public DateTime fechaFactura { get; set; }
        public string tipoPago { get; set; }
    }
}

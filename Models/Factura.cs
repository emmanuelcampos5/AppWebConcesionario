using System.ComponentModel.DataAnnotations;

namespace AppWebConcesionario.Models
{
    public class Factura
    {
        [Key]
        [Required(ErrorMessage = "Debe ingresar el id de la factura")]
        public int idFactura { get; set; }

        [Required(ErrorMessage = "Debe ingresar el id del usuario")]
        public int idUsuario { get; set; }


        public double subtotal { get; set; }
        public double montoDescuento { get; set; }




        public double montoImpuesto { get; set; }
        public double totalFactura { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime fechaFactura { get; set; }

        [Required(ErrorMessage = "Debe ingresar el id del usuario")]
        [DataType(DataType.Text)]
        public string tipoPago { get; set; }
    }
}

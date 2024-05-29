using System.ComponentModel.DataAnnotations;

namespace AppWebConcesionario.Models
{
    public class SeguridadRestablecer
    {
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe ingresar la clave temporal enviada por el email")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Debe ingresar el nuevo password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }


        public string Confirmar { get; set; }



    }//cierre class
}//cierre namespace

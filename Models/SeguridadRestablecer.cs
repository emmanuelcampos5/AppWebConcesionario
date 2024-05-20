using System.ComponentModel.DataAnnotations;

namespace AppWebConcesionario.Models
{
    public class SeguridadRestablecer
    {
        public string correoUsuario { get; set; }

        [Required(ErrorMessage = "Digite la contraseña enviada por email")]
        [DataType(DataType.Password)]
        public string password { get; set; }


        [Required(ErrorMessage = "Digite la nueva contraseña")]
        [DataType(DataType.Password)]
        public string newPassword { get; set; }


        [Required(ErrorMessage = "Confirme la contraseña")]
        [DataType(DataType.Password)]
        public string confirmar { get; set; }

    }
}

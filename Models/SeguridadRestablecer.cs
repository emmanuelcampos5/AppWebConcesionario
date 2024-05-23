using System.ComponentModel.DataAnnotations;

namespace AppWebConcesionario.Models
{
    public class SeguridadRestablecer
    {
<<<<<<< HEAD
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe ingresar la clave temporal enviada por el email")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Debe ingresar el nuevo password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }


        public string Confirmar { get; set; }
=======
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

>>>>>>> f33251f78b72ed1035dfbf46e0ce5e21fc1f19a6
    }
}

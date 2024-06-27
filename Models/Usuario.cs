using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AppWebConcesionario.Models
{
    public class Usuario
    {
        [Key]
        public int idUsuario { get; set; }
        public int idRol { get; set; }

        [Required(ErrorMessage = "Digite un nombre de usuario")]
        [DisplayName("Nombre de Usuario")]
        public string nombreUsuario { get; set; }

        [Required(ErrorMessage = "La cedula es obligatoria")]
        [DisplayName("Cedula")]
        public string cedulaUsuario { get; set; }

        [Required(ErrorMessage = "Por favor digite su numero de telefono")]
        [DisplayName("Telefono")]
        public string telefonoUsuario { get; set; }

        [Required(ErrorMessage = "Por favor digite el correo electronico")]
        [DisplayName("Correo Electronico")]
        [DataType(DataType.EmailAddress)]
        public string correoUsuario { get; set; }

        [Required(ErrorMessage = "El campo Lugar de Residencia es obligatorio.")]
        [DisplayName("Lugar de Residencia")]
        public string lugarResidencia { get; set; }

        [Required(ErrorMessage ="La contra es obligatoria")]
        public string password { get; set; }
        public bool restablecer { get; set; }
        public bool estadoActivo { get; set; }
        public bool estadoSuscripcion { get; set; }
    }
}

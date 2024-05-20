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

        [Required]
        [DisplayName("Nombre de Usuario")]
        public string nombreUsuario { get; set; }

        [Required]
        [DisplayName("Cedula")]
        public string cedulaUsuario { get; set; }

        [Required]
        [DisplayName("Telefono")]
        public string telefonoUsuario { get; set; }

        [Required]
        [DisplayName("Correo Electronico")]
        [DataType(DataType.EmailAddress)]
        public string correoUsuario { get; set; }

        [Required(ErrorMessage = "El campo Lugar de Residencia es obligatorio.")]
        [DisplayName("Lugar de Residencia")]
        public string lugarResidencia { get; set; }
        public string password { get; set; }
        public bool restablecer { get; set; }
        public bool estadoActivo { get; set; }
        public bool estadoSuscripcion { get; set; }
    }
}

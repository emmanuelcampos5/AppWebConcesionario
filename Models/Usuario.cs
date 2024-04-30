using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AppWebConcesionario.Models
{
    public class Usuario
    {
        [Key]
        public int idUsuario { get; set; }
        public int idRol { get; set; }
        public string nombreUsuario { get; set; }
        public string cedulaUsuario { get; set; }
        public string telefonoUsuario { get; set; }
        public string correoUsuario { get; set; }
        public string lugarResidencia { get; set; }
        public string password { get; set; }
        public bool restablecer { get; set; }
        public bool estadoActivo { get; set; }
        public bool estadoSuscripcion { get; set; }
    }
}

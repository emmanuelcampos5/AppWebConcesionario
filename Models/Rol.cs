using System.ComponentModel.DataAnnotations;

namespace AppWebConcesionario.Models
{
    public class Rol
    {
        [Key]
        public int idRol { get; set; }
        public string nombreRol { get; set; }
    }
}

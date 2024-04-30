using System.ComponentModel.DataAnnotations;

namespace AppWebConcesionario.Models
{
    public class RegistroAuditoria
    {
        [Key]
        public int idAuditoria { get; set; }
        public string tablaModificada { get; set; }
        public DateTime fechaModificacion { get; set; }
        public int idUsuarioModificacion { get; set; }
        public string descripcion { get; set; }
    }
}

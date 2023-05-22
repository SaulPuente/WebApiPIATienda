using System.ComponentModel.DataAnnotations;

namespace WebApiPIATienda.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}


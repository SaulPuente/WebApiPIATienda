using System.ComponentModel.DataAnnotations;
using WebApiPIATienda.Validaciones;

namespace WebApiPIATienda.DTOs.MetodoDePago
{
    public class MetodoDePagoCreacionDTO : IValidatableObject
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 16, ErrorMessage = "El campo {0} debe contener 16 caracteres", MinimumLength = 16)]
        [EsNumerico]
        public string Bin { get; set; } = null!;
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 2, ErrorMessage = "El campo {0} debe contener 2 caracteres", MinimumLength = 2)]
        [EsNumerico]
        public string Mes { get; set; } = null!;
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 2, ErrorMessage = "El campo {0} debe contener 2 caracteres", MinimumLength = 2)]
        [EsNumerico]
        public string Año { get; set; } = null!;
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 10, ErrorMessage = "El campo {0} solo puede tener hasta 10 caracteres")]
        public string Tipo { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(Mes))
            {
                var numA = Int32.Parse(Mes);

                if (numA > 12 | numA < 0)
                {
                    yield return new ValidationResult("Mes incorrecto.",
                        new String[] { nameof(Mes) });
                }
            }
            else
            {
                yield return new ValidationResult("Este campo no puede estar vacío.",
                        new String[] { nameof(Mes) });
            }

            if (!string.IsNullOrEmpty(Tipo))
            {
                if (Tipo != "credito" & Tipo != "crédito" & Tipo != "debito")
                {
                    yield return new ValidationResult("Tipo incorrecto.",
                        new String[] { nameof(Tipo) });
                }
            }
            else
            {
                yield return new ValidationResult("Este campo no puede estar vacío.",
                        new String[] { nameof(Tipo) });
            }
        }
    }
}

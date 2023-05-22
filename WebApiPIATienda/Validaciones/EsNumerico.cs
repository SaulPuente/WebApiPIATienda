using System.ComponentModel.DataAnnotations;

namespace WebApiPIATienda.Validaciones
{
    public class EsNumerico : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            long number1 = 0;
            bool canConvert = long.TryParse(value.ToString(), out number1);
            if (canConvert == true)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult("El valor debe ser numérico.");
            }
        }
    }
}


using System.ComponentModel.DataAnnotations;

namespace Csv_Reader.Domain.Validators
{
    public class ExtensionValidator : ValidationAttribute
    {
        private string[] allowedExtensions;
        public ExtensionValidator(string[] allowedExtensions)
        {
            this.allowedExtensions = allowedExtensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            IFormFile file = value as IFormFile;

            if (file is not null)
            {
                var fileExtension = Path.GetExtension(file.FileName);

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return new ValidationResult($"Некорректное расширение! Разрешены только : {string.Join(", ", allowedExtensions)}");
                }
            } 
            else 
            {
                return new ValidationResult("Файл пустой");
            }

            return ValidationResult.Success;
        }
    }
}
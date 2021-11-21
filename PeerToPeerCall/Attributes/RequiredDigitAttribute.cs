using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PeerToPeerCall.Attributes
{
    public class RequiredDigitAttribute : ValidationAttribute
    {
        public RequiredDigitAttribute(int digits)
        {
            Digits = digits;
        }

        private int Digits { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult(string.Empty);

            if (value.ToString().Count(char.IsDigit) >= Digits)
                return ValidationResult.Success;
            else
                return new ValidationResult(string.Empty);
        }
    }
}

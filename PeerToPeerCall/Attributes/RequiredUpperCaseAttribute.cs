using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PeerToPeerCall.Attributes
{
    public class RequiredUpperCaseAttribute : ValidationAttribute
    {
        public RequiredUpperCaseAttribute(int upperCases)
        {
            UpperCases = upperCases;
        }

        private int UpperCases { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult(string.Empty);

            if (value.ToString().Count(char.IsUpper) >= UpperCases)
                return ValidationResult.Success;
            else
                return new ValidationResult(string.Empty);
        }
    }
}

using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PeerToPeerCall.Attributes
{
    public class RequiredLowerCaseAttribute : ValidationAttribute
    {
        public RequiredLowerCaseAttribute(int lowerCases)
        {
            LowerCases = lowerCases;
        }

        private int LowerCases { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult(string.Empty);

            if (value.ToString().Count(char.IsLower) >= LowerCases)
                return ValidationResult.Success;
            else
                return new ValidationResult(string.Empty);
        }
    }
}

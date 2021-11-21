using System.ComponentModel.DataAnnotations;

namespace PeerToPeerCall.Attributes
{
    public class OnlyAllowedCharactersAttribute : ValidationAttribute
    {
        public OnlyAllowedCharactersAttribute(string allowedCharacters)
        {
            AllowedCharacters = allowedCharacters;
        }

        private string AllowedCharacters { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult(string.Empty);

            foreach (var character in value.ToString())
            {
                if (!AllowedCharacters.Contains(character))
                    return new ValidationResult(string.Empty);
            }

            return ValidationResult.Success;
        }
    }
}

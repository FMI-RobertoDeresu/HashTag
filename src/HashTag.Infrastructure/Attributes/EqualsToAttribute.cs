using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace HashTag.Infrastructure.Attributes
{
    public class EqualsToAttribute : ValidationAttribute
    {
        private readonly string _errorMessage;
        private readonly string _otherPropertyName;

        public EqualsToAttribute(string otherPropertyName, string errorMessage)
            : base(errorMessage)
        {
            _otherPropertyName = otherPropertyName;
            _errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherPropertyValue = validationContext.ObjectType
                .GetProperty(_otherPropertyName)
                .GetValue(validationContext.ObjectInstance);

            return Equals(value, otherPropertyValue) ? ValidationResult.Success : new ValidationResult(_errorMessage);
        }
    }
}
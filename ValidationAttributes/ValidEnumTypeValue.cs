using System.ComponentModel.DataAnnotations;

namespace BlogApi.ValidationAttributes {
    public class ValidEnumTypeValue : ValidationAttribute {
        private readonly Type _enumType;

        public ValidEnumTypeValue(Type enumType)
        {
            _enumType = enumType;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
            if (value is not null && Enum.IsDefined(_enumType, value)) return ValidationResult.Success;
            return new ValidationResult(ErrorMessage ?? "Invalid value");
        }
    }
}

using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace BlogApi.ValidationAttributes {
    public class MinListElements : ValidationAttribute {
        private readonly int _minElements;

        public MinListElements(int minElements)
        {
            _minElements = minElements;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
            if (value is IList l && l.Count >= _minElements) return ValidationResult.Success;
            return new ValidationResult(ErrorMessage ?? "Invalid no of elements");
        }

    }
}

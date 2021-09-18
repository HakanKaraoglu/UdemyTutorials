using BandApi.DataTransferObjects;
using System;
using System.ComponentModel.DataAnnotations;

namespace BandApi.CustomValidations
{
    public sealed class TitleAndDescriptionValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = (AlbumModificationsDto)validationContext.ObjectInstance;

            return string.Equals(context.Title.Trim(), context.Description.Trim(), StringComparison.CurrentCultureIgnoreCase)
                ? new ValidationResult($"{nameof(context.Description)} and {nameof(context.Title)} can not be the same")
                : ValidationResult.Success;
        }
    }
}

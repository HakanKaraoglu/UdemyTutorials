using System.ComponentModel.DataAnnotations;
using BandApi.CustomValidations;

namespace BandApi.DataTransferObjects
{
    [TitleAndDescriptionValidation]
    public abstract class AlbumModificationsDto
    {
        [Required(ErrorMessage = "Each album must have a title")]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(400)]
        public virtual string Description { get; set; }
    }
}

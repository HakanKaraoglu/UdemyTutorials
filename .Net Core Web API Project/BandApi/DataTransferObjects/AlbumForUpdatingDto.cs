using System.ComponentModel.DataAnnotations;

namespace BandApi.DataTransferObjects
{
    public class AlbumForUpdatingDto : AlbumModificationsDto
    {
        [Required]
        public override string Description { get; set; }
    }
}

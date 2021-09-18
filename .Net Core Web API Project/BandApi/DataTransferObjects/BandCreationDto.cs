using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BandApi.DataTransferObjects
{
    public class BandCreationDto
    {
        [Required]
        public string Name { get; set; }
        public DateTime? Founded { get; set; }
        [Required]
        public string MainGenre { get; set; }
        public ICollection<AlbumCreationDto> Albums { get; set; } = new List<AlbumCreationDto>();

    }
}

using System;
using System.Collections.Generic;

namespace BandApi.Entities
{
    public class Band
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Founded { get; set; }
        public string MainGenre { get; set; }
        public ICollection<Album> Albums { get; set; } = new List<Album>();
    }
}

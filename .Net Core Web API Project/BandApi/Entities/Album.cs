using System;

namespace BandApi.Entities
{
    public class Album
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Band Band { get; set; }
        public Guid BandId { get; set; }    
    }
}

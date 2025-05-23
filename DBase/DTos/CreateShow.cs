using System.ComponentModel.DataAnnotations;

namespace Dtos{
    public class CreateShow{

        [Required]
        public required string Name { get; set; }
        [Required]
        public long VenueId { get; set; }

        public string? Description { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime startwindow { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime endwindow { get; set; }

        public IFormFile? Photo { get; set; }
        public string? PhotoPath { get; internal set; }
    }
}
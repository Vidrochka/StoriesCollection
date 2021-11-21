using System.ComponentModel.DataAnnotations;

namespace StoriesCollection.Db.Models
{
    public class Button
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public string? SourceStoryPartId { get; set; }
        public StoryPart? SourceStoryPart { get; set; }

        public string? DestioationStoryPartId { get; set; }
        public StoryPart? DestioationStoryPart { get; set; }
    }
}

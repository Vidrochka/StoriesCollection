using System.ComponentModel.DataAnnotations;

namespace StoriesCollection.Db.Models
{
    public class Button
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        public int SourceStoryPartId { get; set; }
        public StoryPart SourceStoryPart { get; set; }

        public int? DestinationStoryPartId { get; set; }
        public StoryPart? DestinationStoryPart { get; set; }
    }
}

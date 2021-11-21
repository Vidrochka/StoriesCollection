using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace StoriesCollection.Db.Models
{
    public class Story
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? FirstStoryPartId { get; set; }

        public List<StoryPart> StoryParts { get; set; } = new List<StoryPart>();
    }
}

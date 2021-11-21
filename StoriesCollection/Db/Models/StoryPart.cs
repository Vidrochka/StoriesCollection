using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoriesCollection.Db.Models
{
    public class StoryPart
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        public int StoryId { get; set; }
        public Story Story { get; set; } = null!;

        [Required]
        public string Text { get; set; } = string.Empty;

        public List<Button> ButtonsNext { get; set; } = new List<Button>();
        public List<Button> ButtonsFrom { get; set; } = new List<Button>();
    }
}

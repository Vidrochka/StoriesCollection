using System.Text.Json.Serialization;

namespace StoriesCollection.Models
{
    public enum ButtonType : short
    {
        StartStory,
        NextPart,
        EndStory
    }

    public class ButtonData
    {
        [JsonPropertyName("t")]
        public ButtonType Type { get; set; }
        [JsonPropertyName("s")]
        public int StoryId { get; set; }
        [JsonPropertyName("b")]
        public int? ButtonId { get; set; }
        [JsonPropertyName("cm")]
        public int? CurrentPartMessageId { get; set; }
        [JsonPropertyName("sm")]
        public int? StartMessageId { get; set; }
    }
}

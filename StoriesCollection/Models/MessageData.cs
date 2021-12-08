using System.Collections.Generic;
using System.Linq;

namespace StoriesCollection.Models
{
    public class MessageData
    {
        public MessageData(string? message)
        {
            Message = message;
        }

        public string? Message { get; set; }
    }

    public class AddStory
    {
        public AddStory(IEnumerable<string> messageParts)
        {
            StoryName = messageParts.ElementAtOrDefault(2);
            if (StoryName is null) IsValid = false;
        }

        public string? StoryName { get; set; }
        public bool IsValid { get; set; } = true;
    }

    public class AddStoryPart
    {
        public AddStoryPart(IEnumerable<string> messageParts)
        {
            var isIdValid = int.TryParse(messageParts.ElementAtOrDefault(2), out var storyId);
            StoryId = isIdValid ? storyId : null;
            StoryPartText = messageParts.ElementAtOrDefault(3);

            if (StoryId is null || StoryPartText is null) IsValid = false;
        }

        public int? StoryId { get; set; }
        public string? StoryPartText { get; set; }
        public bool IsValid { get; set; } = true;
    }

    public class AddButton
    {
        public AddButton(IEnumerable<string> messageParts)
        {
            var isIdFromValid = int.TryParse(messageParts.ElementAtOrDefault(2), out var fromStoryId);
            var isIdToValid = int.TryParse(messageParts.ElementAtOrDefault(3), out var toStoryId);

            StoryPartFromId = isIdFromValid ? fromStoryId : null;
            StoryPartToId = isIdToValid ? toStoryId : null;

            ButtonText = messageParts.ElementAtOrDefault(4);

            if (StoryPartFromId is null || ButtonText is null ||
                (StoryPartToId is null && !string.IsNullOrEmpty(messageParts.ElementAtOrDefault(3)))
            )
                IsValid = false;
        }

        public int? StoryPartFromId { get; set; }
        public int? StoryPartToId { get; set; }
        public string? ButtonText { get; set; }
        public bool IsValid { get; set; } = true;
    }
}

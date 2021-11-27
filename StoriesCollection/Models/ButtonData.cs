using System;
using System.Text.Json.Serialization;

namespace StoriesCollection.Models
{
    public enum ButtonType : short
    {
        StartStory,
        NextPart,
        EndStory,
    }

    //public class ButtonData
    //{
    //    public ButtonData(string data)
    //    {
    //        var splittedData = data.Split(';');
    //        Type = Enum.Parse<ButtonType>(splittedData[0]);
    //        StoryId = int.Parse(splittedData[1]);
    //        ButtonId = string.IsNullOrEmpty(splittedData[2]) ? null : int.Parse(splittedData[2]);
    //        CurrentPartMessageId = string.IsNullOrEmpty(splittedData[3]) ? null : int.Parse(splittedData[3]);
    //        StartMessageId = string.IsNullOrEmpty(splittedData[4]) ? null : int.Parse(splittedData[4]);
    //    }

    //    [JsonPropertyName("t")]
    //    public ButtonType Type { get; set; }
    //    [JsonPropertyName("s")]
    //    public int StoryId { get; set; }
    //    [JsonPropertyName("b")]
    //    public int? ButtonId { get; set; }
    //    [JsonPropertyName("cm")]
    //    public int? CurrentPartMessageId { get; set; }
    //    [JsonPropertyName("sm")]
    //    public int? StartMessageId { get; set; }

    //    public string ToRequest() => $"{(int)Type};{StoryId};{ButtonId?.ToString() ?? ""};{CurrentPartMessageId?.ToString() ?? ""};{StartMessageId?.ToString() ?? ""}";
    //}

    public class StartStoryButton
    {
        public StartStoryButton() { }

        public StartStoryButton(string data)
        {
            var splittedData = data.Split(';');
            Type = Enum.Parse<ButtonType>(splittedData[0]);
            StoryId = int.Parse(splittedData[1]);
            StartMessageId = string.IsNullOrEmpty(splittedData[2]) ? null : int.Parse(splittedData[2]);
        }

        public ButtonType Type { get; set; } = ButtonType.StartStory;
        public int StoryId { get; set; }
        public int? StartMessageId { get; set; }

        public string ToRequest() => $"{(int)Type};{StoryId};{StartMessageId?.ToString() ?? ""}";
    }

    public class NextStoryPartButton
    {
        public NextStoryPartButton() { }

        public NextStoryPartButton(string data)
        {
            var splittedData = data.Split(';');
            Type = Enum.Parse<ButtonType>(splittedData[0]);
            ButtonId = int.Parse(splittedData[1]);
            CurrentPartMessageId = string.IsNullOrEmpty(splittedData[2]) ? null : int.Parse(splittedData[2]);
        }

        public ButtonType Type { get; set; } = ButtonType.NextPart;
        public int ButtonId { get; set; }
        public int? CurrentPartMessageId { get; set; }

        public string ToRequest() => $"{(int)Type};{ButtonId};{CurrentPartMessageId?.ToString() ?? ""}";
    }

    public class EndStoryButton
    {
        public EndStoryButton() { }

        public EndStoryButton(string data)
        {
            var splittedData = data.Split(';');
            Type = Enum.Parse<ButtonType>(splittedData[0]);
            ButtonId = int.Parse(splittedData[1]);
            CurrentPartMessageId = string.IsNullOrEmpty(splittedData[2]) ? null : int.Parse(splittedData[2]);
        }

        public ButtonType Type { get; set; } = ButtonType.NextPart;
        public int ButtonId { get; set; }
        public int? CurrentPartMessageId { get; set; }

        public string ToRequest() => $"{(int)Type};{ButtonId};{CurrentPartMessageId?.ToString() ?? ""}";
    }
}

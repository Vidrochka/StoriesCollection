using StoriesCollection.Models;
using System;
using System.Linq;

namespace StoriesCollection.Helpers
{
    public static class ButtonFactory
    {
        public static object GetButton(string data) => Enum.Parse<ButtonType>(data.Split(";").First()) switch
        {
            ButtonType.StartStory => new StartStoryButton(data),
            ButtonType.NextPart => new NextStoryPartButton(data),
            ButtonType.EndStory => new EndStoryButton(data),
            _ => throw new Exception($"Инвалидный callback [{data}]"),
        };
    }
}

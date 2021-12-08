using StoriesCollection.Models;
using System.Linq;

namespace StoriesCollection.Helpers
{
    public static class MessageFactory
    {
        public static object GetMessage(string? data)
        {
            if (data is null || !data.StartsWith("admin")) return new MessageData (data);

            var parts = data.Split(";").Skip(1);

            var model = parts.ElementAt(0);
            var command = parts.ElementAt(1);

            switch (model, command)
            {
                case ("story", "add"): return new AddStory (parts);
                case ("story part", "add"): return new AddStoryPart(parts);
                case ("button", "add"): return new AddButton(parts);
                default: return new MessageData(data);
            }
        }
    }
}

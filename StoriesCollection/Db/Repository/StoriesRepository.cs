using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoriesCollection.Db.Models;

namespace StoriesCollection.Db.Repository
{
    public class StoriesRepository
    {
        private readonly StoriesDbContext _dbContext;

        public StoriesRepository(StoriesDbContext dbContext)
            => _dbContext = dbContext;

        public async Task<IEnumerable<Story>> GetAllStory()
            => await _dbContext.Stories.ToListAsync();

        public async Task<Story?> GetStoryInfo(int storyId)
            => await _dbContext.Stories.FirstOrDefaultAsync(x => x.Id == storyId);

        public async Task<IEnumerable<StoryPart>?> GetAllStoryParts(string storyName)
            => (await _dbContext.Stories.Where(x => x.Name == storyName).Include(x => x.StoryParts).SingleOrDefaultAsync())?.StoryParts;

        public async Task<StoryPart?> GetStoryPartWithButtons(int storyPartId)
            => await _dbContext.StoryParts.Where(x => x.Id == storyPartId).Include(x => x.ButtonsNext).SingleOrDefaultAsync();

        public async Task<StoryPart?> GetStoryPart(int storyPartId)
            => await _dbContext.StoryParts.SingleOrDefaultAsync(x => x.Id == storyPartId);

        public async Task<Button?> GetButton(int buttonId)
            => await _dbContext.Buttons.SingleOrDefaultAsync(x => x.Id == buttonId);

        public async Task<bool> IsStoryExist(string storyName)
            => await _dbContext.Stories.AnyAsync(x => x.Name == storyName);

        public async Task AddStory(string storyName)
            => await _dbContext.Stories.AddAsync(new Story { Name = storyName });

        public async Task AddStoryPart(Story story, string storyPartText)
            => await _dbContext.StoryParts.AddAsync(new StoryPart
            {
                Text = storyPartText,
                Story = story,
            });

        public async Task AddButton(StoryPart sourceStoryPart, StoryPart? destinationStoryPart, string buttonText)
            => await _dbContext.Buttons.AddAsync(new Button
            {
                SourceStoryPart = sourceStoryPart,
                DestinationStoryPart = destinationStoryPart,
                Text = buttonText,
            });

        public void UpdateStory(Story story)
            => _dbContext.Stories.Update(story);

        public async Task<Story?> GetStory(string storyName)
            => await _dbContext.Stories.FirstOrDefaultAsync(x => x.Name == storyName);

        public async Task<StoryPart?> GetStoryPart(int storyId, string storyPartText)
            => await _dbContext.StoryParts.FirstOrDefaultAsync(x => x.StoryId == storyId && x.Text == storyPartText);

        public async Task SaveChanges() => await _dbContext.SaveChangesAsync();
    }
}

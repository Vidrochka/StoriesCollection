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

        public async Task<IEnumerable<Story>> GetAllStory() => await _dbContext.Stories.ToListAsync();

        public async Task<Story?> GetStoryInfo(int storyId) => await _dbContext.Stories.FirstOrDefaultAsync(x => x.Id == storyId);

        public async Task<IEnumerable<StoryPart>?> GetAllStoryParts(string storyName)
            => (await _dbContext.Stories.Where(x => x.Name == storyName).Include(x => x.StoryParts).SingleOrDefaultAsync())?.StoryParts;

        public async Task<StoryPart?> GetStoryPart(int storyId, string storyPartId)
            => await _dbContext.StoryParts.Where(x => x.Id == storyPartId).Include(x => x.ButtonsNext).SingleOrDefaultAsync();

        public async Task<Button?> GetButton(int buttonId) => await _dbContext.Buttons.SingleOrDefaultAsync(x => x.Id == buttonId);
    }
}

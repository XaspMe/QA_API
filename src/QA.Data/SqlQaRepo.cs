using Microsoft.EntityFrameworkCore;
using QA.Models.Models;

namespace QA.Data
{
    public class SqlQaRepo : IQaRepo
    {
        private readonly QaContext _context;

        public SqlQaRepo(QaContext context)
        {
            _context = context;
        }

        public void CreateCategory(QACategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            _context.Categories.Add(category);
        }

        public QAElement GetElementRandom()
        {
            var collection = _context.Elements.ToList();
            var randomElement = collection[new Random().Next(collection.Count)];
            return randomElement;
        }

        public QAElement GetElementRandomInCategory(int category)
        {
            var collection = _context.Elements.Where(x => x.Category.Id == category).ToList();
            var randomElement = collection[new Random().Next(collection.Count)];
            return randomElement;
        }

        public async Task<QAElement> GetRandomElementFromTelegramUserFavorites(long chatId)
        {
            User user = await _context.Users
                .Include(x => x.FavoriteElements)
                .ThenInclude(x => x.Category)
                .Where(x => x.TelegramChatId == chatId)
                .FirstOrDefaultAsync();

            var elements = user.FavoriteElements;
            if (elements.Any())
                return elements.ToArray()[new Random().Next(elements.Count - 1)];
            return null;
        }

        public void CreateElement(QAElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            _context.Elements.Add(element);
        }

        public async Task CreateElementWithCategoryLoading(QAElement element)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Name == element.Category.Name);
            element.Category = category;
            _context.Elements.Add(element);
            await _context.SaveChangesAsync();
        }

        public void DeleteCategory(QACategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            _context.Categories.Remove(category);
        }

        public void DeleteElement(QAElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            _context.Elements.Remove(element);
        }

        public IEnumerable<string> CategoriesStats()
        {
            var cateories = _context.Categories.ToList();
            foreach (var cat in cateories)
            {
                yield return $"{cat.Name} - {_context.Elements.Count(x => x.Category.Id == cat.Id)} elements";
            }
        }

        public IEnumerable<QACategory> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public IEnumerable<QAElement> GetAllElements()
        {
            var modelElem = _context.Elements.ToList();
            return modelElem;
        }

        public QACategory GetCategoryById(int id)
        {
            return _context.Categories.FirstOrDefault(x => x.Id == id);
        }

        public QACategory GetCategoryByName(string name)
        {
            return _context.Categories.FirstOrDefault(x => x.Name == name);
        }

        public QAElement GetElementInGroupByQuestion(string question, int group)
        {
            return _context.Elements.FirstOrDefault(x => x.Question == question && x.Category.Id == group);
        }

        public async Task СreateTelegramUserIfDoesntExist(long chatId)
        {
            if (!await _context.Users.AnyAsync(x => x.TelegramChatId == chatId))
            {
                await _context.Users.AddAsync(new User() { TelegramChatId = chatId });
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetElementOnCurrentTelegramUser(long chatId, QAElement element)
        {
            var user = await _context.Users.Where(x => x.TelegramChatId == chatId).FirstOrDefaultAsync();
            user.CurrentQuestion = element;
            await _context.SaveChangesAsync();
        }

        public async Task<QAElement> GetElementOnCurrentTelegramUser(long chatId)
        {
            var user = await _context.Users.Where(x => x.TelegramChatId == chatId)
                .Include(userState => userState.CurrentQuestion)
                .Include(x => x.CurrentQuestion.Category).FirstOrDefaultAsync();
            return user.CurrentQuestion;
        }

        public async Task<IEnumerable<QACategory>> GetTelegramUserCategories(long chatId)
        {
            var user = _context.Users.Include(u => u.FavoriteCategories).FirstOrDefault(u => u.TelegramChatId == chatId);
            return user.FavoriteCategories;
        }

        public async Task UpdateTelegramUserFavoriteCategories(long chatId, IEnumerable<QACategory> qaCategories)
        {
            var user = await _context.Users.Include(u => u.FavoriteCategories).FirstOrDefaultAsync(u => u.TelegramChatId == chatId);
            if (user != null)
            {
                if (qaCategories.Any())
                {
                    user.FavoriteCategories.Clear();
                    user.FavoriteCategories = qaCategories.ToList();
                    await _context.SaveChangesAsync();
                }
                else
                {
                    user.FavoriteCategories = _context.Categories.ToList();
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task AddToTelegramUserFavoriteElements(long chatId, QAElement qaElement)
        {
            var user = await _context.Users.Include(u => u.FavoriteElements).FirstOrDefaultAsync(u => u.TelegramChatId == chatId);
            if (user != null && user.FavoriteElements.All(x => x.Id != qaElement.Id))
            {
                user.FavoriteElements.Add(qaElement);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromTelegramUserFavoriteElements(long chatId, QAElement qaElement)
        {
            var user = await _context.Users.Include(u => u.FavoriteElements).FirstOrDefaultAsync(u => u.TelegramChatId == chatId);
            if (user != null && user.FavoriteElements.Contains(qaElement))
            {
                user.FavoriteElements.Remove(qaElement);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsElementTelegramUserFavorite(long chatId, QAElement element)
        {
            var user = await _context.Users.Where(x => x.TelegramChatId == chatId)
                .Include(user => user.FavoriteElements).FirstOrDefaultAsync();
            return user.FavoriteElements.Any(x => x == element);
        }

        public async Task<UserInputMode> GetTelegramUserMode(long chatId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.TelegramChatId == chatId);
            return user.UserInputMode;
        }

        public async Task AddTelegramUserFeedBack(long chatId, string message)
        {
            var user = await _context.Users
                .Where(x => x.TelegramChatId == chatId)
                .Include(x => x.FeedBacks)
                .FirstOrDefaultAsync();
            user.FeedBacks.Add(new FeedBack() { User = user, Message = message});
            await _context.SaveChangesAsync();
        }

        public async Task CreateTelegramUserQaCategory(long chatId, QACategory category)
        {
            var existingCategories = await _context.Categories.Select(x => x.Id).ToListAsync();
            // костыль, это важно для быстрого инита базы данных на новых тачках
            category.Id = existingCategories.Max() + 1;
            var user = await _context.Users.Include(x => x.CategoriesCreated)
                .FirstOrDefaultAsync(x => x.TelegramChatId == chatId);
            user.CategoriesCreated.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeQuestionCategory(QAElement qaElement, QACategory category)
        {
            var element = await _context.Elements.FirstAsync(x => x.Id == qaElement.Id);
            element.Category = await _context.Categories.FirstAsync(x => x.Id == category.Id);
            await _context.SaveChangesAsync();
        }

        public async Task<UserCurrentStep> GetUserCurrentStep(long TelegramChatId)
        {
            var firstAsync = await _context.Users.Where(x => x.TelegramChatId == TelegramChatId).FirstAsync();
            return firstAsync.UserCurrentStep;
        }

        public async Task SetUserCurrentStep(long TelegramChatId, UserCurrentStep step)
        {
            var firstAsync = await _context.Users.Where(x => x.TelegramChatId == TelegramChatId).FirstAsync();
            firstAsync.UserCurrentStep = step;
            await _context.SaveChangesAsync();
        }

        public async Task SetTelegramUserMode(long chatId, UserInputMode mode)
        {
            var user = await _context.Users
                .Where(x => x.TelegramChatId == chatId)
                .Include(x => x.FeedBacks)
                .FirstOrDefaultAsync();
            user.UserInputMode = mode;
            await _context.SaveChangesAsync();
        }

        public QAElement GetElementById(int id)
        {
            return _context.Elements.
                Include(x => x.Category).
                FirstOrDefault(x => x.Id == id);
        }

        public async Task<User> GetTelegramUser(long chatId)
        {
            return await _context.Users.FirstAsync(x => x.TelegramChatId == chatId);
        }

        public async Task<bool> IsTelegramUserAdmin(long chatId)
        {
            var user = await _context.Users.FirstAsync(x => x.TelegramChatId == chatId);
            return user.isAdmin;
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateCategory(QACategory category)
        {
            throw new NotImplementedException();
        }

        public void UpdateElement(QAElement element)
        {
            throw new NotImplementedException();
        }

        public int ElementsCount()
        {
            return _context.Elements.Count();
        }
    }
}
using QA_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace QA_API.Data
{
    public class SqlQaRepo : IQaRepo
    {
        private readonly QAContext _context;

        public SqlQaRepo(QAContext context)
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

        public void CreateElement(QAElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            _context.Elements.Add(element);
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
                .Include(userState => userState.CurrentQuestion).FirstOrDefaultAsync();
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
            if (user != null && user.FavoriteElements.All(x => x.Id != qaElement.Id))
            {
                user.FavoriteElements.Add(qaElement);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsElementTelegramUserFavorite(long chatId, QAElement element)
        {
            var user = await _context.Users.Where(x => x.TelegramChatId == chatId)
                .Include(user => user.FavoriteElements).FirstOrDefaultAsync();
            return user.FavoriteElements.Any(x => x == element);
        }

        public QAElement GetElementById(int id)
        {
            return _context.Elements.FirstOrDefault(x => x.Id == id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateCategory(QACategory category)
        {
            // Nothing
        }

        public void UpdateElement(QAElement element)
        {
            // Nothing
        }

        public int ElementsCount()
        {
            return _context.Elements.Count();
        }
    }
}
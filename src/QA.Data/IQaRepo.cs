using QA.Models.Models;

namespace QA.Common.Data
{
    public interface IQaRepo
    {
        // todo убрать все телеграмм методы
        bool SaveChanges();
        int ElementsCount();
        IEnumerable<string> CategoriesStats();

        IEnumerable<QACategory> GetAllCategories();
        QACategory GetCategoryById(int id);
        QACategory GetCategoryByName(string name);
        void CreateCategory(QACategory category);
        void UpdateCategory(QACategory category);
        void DeleteCategory(QACategory category);

        IEnumerable<QAElement> GetAllElements();
        QAElement GetElementById(int id);
        QAElement GetElementRandom();
        QAElement GetElementRandomInCategory(int category);
        Task<QAElement> GetRandomElementFromTelegramUserFavorites(long chatId);
        void CreateElement(QAElement element);
        void UpdateElement(QAElement element);
        void DeleteElement(QAElement element);
        QAElement GetElementInGroupByQuestion(string question, int group);
        Task СreateTelegramUserIfDoesntExist(long chatId);
        Task SetElementOnCurrentTelegramUser(long chatId, QAElement element);
        Task<QAElement> GetElementOnCurrentTelegramUser(long chatId);
        Task<IEnumerable<QACategory>> GetTelegramUserCategories(long chatId);
        public Task UpdateTelegramUserFavoriteCategories(long chatId, IEnumerable<QACategory> qaCategories);
        public Task AddToTelegramUserFavoriteElements(long chatId, QAElement qaElement);
        public Task RemoveFromTelegramUserFavoriteElements(long chatId, QAElement qaElement);
        public Task<bool> IsElementTelegramUserFavorite(long chatId, QAElement element);
        public Task<UserInputMode> GetTelegramUserMode(long chatId);
        public Task SetTelegramUserMode(long chaId, UserInputMode mode);
        public Task AddTelegramUserFeedBack(long chatId, string message);
        public Task CreateTelegramUserQaCategory(long chatId, QACategory category);
    }
}
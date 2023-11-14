namespace QA.Telegram.Bot.Constants;

public static class TelegramMessages
{
    public const string CATEGORY_CREATED = "Категория успешно создана.";
    public const string REQUEST_CATEGORY_NAME = "Введите имя новой категории.";
    public const string HANDLE_ERROR = "Извините, я не могу обработать ваше сообщение.";
    public const string FEEDBACK_MESSAGE = "Введите текст и отправьте мне обычным сообщением, ваше обращение будет рассмотрено администратором.";
    public const string FEEDBACK_ACCEPTED_MESSAGE = "Спасибо за обратную связь, это помогает проекту стать лучше.";
    public const string CATEGORIES = "Выбери категорию";
    public const string ERROR = "Произошла ошибка :(";
    // todo вынести в конфиг или указать ссылку на гитхаб
    public const string DEVELOPER_CONTACT = "По всем существующим вопросам:\n@comppomosh";
    public const string ADDED_TO_FAVORITES = "Вопрос успешно добавлен в избранное, ваши избранные вопросы могут быть найдены в главном меню.";
    public const string REMOVED_FROM_FAVORITES = "Вопрос был убран из вашего списка избранных.";
    public const string NO_FAVORITES = "В избранном пока ничего нет, добавьте понравившиеся вопросы в избранные, чтобы они появились в этом разделе.";
    public static string HELLO(int qaCount) => $"Привет! Я знаю {qaCount} вопросов, которые могут быть использованы на собеседованиях по следующим темам:";
    public static string MAIN_MENU_SELECTOR(int qaCount) => $"Текущее количество вопросов в базе: {qaCount} \nCейчас вы в главном меню";
    
}
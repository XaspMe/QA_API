// ReSharper disable InconsistentNaming

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QA_API.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace QA_API.Constants;

public static class TelegramCommands
{
    public const string ANSWER_CURRENT_QUESTION = "\u003f Показать ответ";
    public const string NEXT_QUESTION = "\u21d2 Следующий вопрос";
    public const string NEXT_FAVORITE_QUESTION = "\u21d2 Следующий избранный вопрос";
    public const string ADD_TO_FAVORITES = "\u2661 В избранное";
    public const string REMOVE_FROM_FAVORITES = "\u2573 Убрать из избранного";
    public const string EXCLUDE_FROM_FAVORITES = "\u2573 Исключить из избранного";
    public const string CATEGORIES = "\ud83d\udcf0 Вопросы по категориям";
    public const string MENU = "\u2630 Меню";
    public const string DEVELOPER_CONTACTS = "\ud83d\udcf1 Контакты разработчика";
    public const string MY_FAVORITES_QUESTIONS = "\u272d Мои избранные";
    public const string FEEDBACK = "\u002a Я бы хотел улучшить";
    public const string REPORT = "\u275e Замечание";
    public const string RATE = "\u2756 Оценить вопрос или ответ";
    public const string START = "/start";
    public const string ADMIN = "/adm";
    public const string ALL_CATEGORIES = "Все подряд";
    // adm
    public const string EDIT_QUESTION = "Исправить вопрос";
    public const string ADD_QUESTION = "Добавить вопрос";
    public const string DELETE_QUESTION = "Удалить вопрос";
    public const string CHANGE_QUESTION_CATEGORY = "Изменить категорию для вопроса";
    public const string RENAME_CATEGORY = "Переименовать категорию";
    public const string CREATE_CATEGORY = "Добавить категорию";
    public const string DELETE_CATEGORY = "Удалить категорию";
    public const string SHOW_CATEGORIES_STATISTICS = "Показать статистику по категориям";
}

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

public static class TelegramMarkups
{
    public static ReplyKeyboardMarkup QUESTIONS_KEYBOARD(bool isFavorite)
    {
        if (isFavorite)
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { TelegramCommands.ANSWER_CURRENT_QUESTION, TelegramCommands.NEXT_QUESTION },
                new KeyboardButton[] { TelegramCommands.REMOVE_FROM_FAVORITES, TelegramCommands.RATE },
                new KeyboardButton[] { TelegramCommands.REPORT, TelegramCommands.MENU }
            });
        else
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { TelegramCommands.ANSWER_CURRENT_QUESTION, TelegramCommands.NEXT_QUESTION },
                new KeyboardButton[] { TelegramCommands.ADD_TO_FAVORITES, TelegramCommands.RATE },
                new KeyboardButton[] { TelegramCommands.REPORT, TelegramCommands.MENU }
            });
    }
    
    public static ReplyKeyboardMarkup FAVORITE_QUESTIONS_KEYBOARD()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { TelegramCommands.ANSWER_CURRENT_QUESTION, TelegramCommands.NEXT_FAVORITE_QUESTION },
            new KeyboardButton[] { TelegramCommands.EXCLUDE_FROM_FAVORITES, TelegramCommands.RATE },
            new KeyboardButton[] { TelegramCommands.REPORT, TelegramCommands.MENU }
        });
    }

    public static ReplyKeyboardMarkup GO_TO_MENU => new(new[]
    {
        new KeyboardButton[] { TelegramCommands.MENU }
    });

    public static ReplyKeyboardMarkup MAIN_MENU(bool isAdmin)
    {
        if (!isAdmin)
        {
            return new(new[]
            {
                new KeyboardButton[]
                    { TelegramCommands.CATEGORIES, TelegramCommands.MY_FAVORITES_QUESTIONS },
                new KeyboardButton[]
                    { TelegramCommands.DEVELOPER_CONTACTS, TelegramCommands.FEEDBACK }
            });
        }
        
        return new(new[]
        {
            new KeyboardButton[]
                { TelegramCommands.CATEGORIES, TelegramCommands.MY_FAVORITES_QUESTIONS },
            new KeyboardButton[]
                { TelegramCommands.DEVELOPER_CONTACTS, TelegramCommands.FEEDBACK },
            new KeyboardButton[] 
                { TelegramCommands.EDIT_QUESTION, TelegramCommands.ADD_QUESTION },
            new KeyboardButton[] 
                { TelegramCommands.DELETE_QUESTION, TelegramCommands.CHANGE_QUESTION_CATEGORY },
            new KeyboardButton[]
                { TelegramCommands.RENAME_CATEGORY, TelegramCommands.CREATE_CATEGORY },
            new KeyboardButton[]
                { TelegramCommands.DELETE_CATEGORY, TelegramCommands.SHOW_CATEGORIES_STATISTICS}
            
        });
    }

    public static ReplyKeyboardMarkup CATEGORIES_KEYBOARD(IEnumerable<string> Categories)
    {
        // todo переписать
        var keyboardButtonsEnumerable = Categories
            .Select((category, index) => new { Category = category, Index = index })
            .GroupBy(x => x.Index / 3)
            .Select(group => group.Select(x => new KeyboardButton(x.Category)).ToArray())
            .ToList();
        keyboardButtonsEnumerable.Add(new KeyboardButton[] { TelegramCommands.ALL_CATEGORIES });
        keyboardButtonsEnumerable.Add(new KeyboardButton[] { TelegramCommands.MENU });
        return new ReplyKeyboardMarkup(keyboardButtonsEnumerable);
    }
}
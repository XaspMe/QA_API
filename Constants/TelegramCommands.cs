// ReSharper disable InconsistentNaming

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace QA_API.Constants;

public static class TelegramCommands
{
    public const string ANSWER_CURRENT_QUESTION = "Показать ответ";
    public const string NEXT_QUESTION = "Следующий вопрос";
    public const string ADD_TO_FAVORITES = "В избранное";
    public const string CATEGORIES = "Темы по категориям";
    public const string MENU = "Меню";
    public const string CONTACTS = "Контакты разработчика";
    public const string FAVORITES_QUESTIONS = "Мои избранные";
    public const string FEEDBACK = "Я бы хотел улучшить...";
    public const string REPORT = "Замечание";
    public const string RATE = "Оценить вопрос или ответ";
    public const string START = "/start";
    public const string ADMIN = "/adm";
    public const string ALL_CATEGORIES = "Все подряд";
    // adm
    public const string EDIT_QUESTION = "Исправить вопрос";
    public const string ADD_QUESTION = "Добавить вопрос";
    public const string DELETE_QUESTION = "Удалить вопрос";
    public const string CHANGE_QUESTION_CATEGORY = "Изменить категорию для вопроса";
    public const string RENAME_CATEGORY = "Переименовать категорию";
    public const string ADD_CATEGORY = "Добавить категорию";
    public const string DELETE_CATEGORY = "Удалить категорию";
    public const string SHOW_CATEGORIES_STATISTICS = "Показать статистику по категориям";
}

public static class TelegramMessages
{
    public const string HANDLE_ERROR = "Извините, я не могу обработать ваше сообщение.";
    public const string CATEGORIES = "Выбери категорию";
    public const string ERROR = "Произошла ошибка :(";
    public const string ADDED_TO_FAVORITES = "Вопрос успешно добавлен в избранное, ваши избранные вопросы могут быть найдены в главном меню.";
    public static string HELLO(int qaCount) => $"Привет! Я знаю {qaCount} вопросов, которые могут быть использованы на собеседованиях по следующим темам:";
    public static string MAIN_MENU_SELECTOR(int qaCount) => $"Текущее количество вопросов в базе: {qaCount} \nCейчас вы в главном меню";
}

public static class TelegramMarkups
{
    public static readonly ReplyKeyboardMarkup QUESTIONS_KEYBOARD = new(new[]
    {
        new KeyboardButton[] { "\u2611 " + TelegramCommands.ANSWER_CURRENT_QUESTION,  "\U000027a1 " + TelegramCommands.NEXT_QUESTION },
        new KeyboardButton[] { "\U00002b50 " + TelegramCommands.ADD_TO_FAVORITES, TelegramCommands.RATE },
        new KeyboardButton[] { "\U0000270F " + TelegramCommands.REPORT, "\u2630 " + TelegramCommands.MENU }
    });

    public static ReplyKeyboardMarkup MAIN_MENU(bool isAdmin)
    {
        if (!isAdmin)
        {
            return new(new[]
            {
                new KeyboardButton[]
                    { "\ud84d " + TelegramCommands.CATEGORIES, "\U00002b50 " + TelegramCommands.FAVORITES_QUESTIONS },
                new KeyboardButton[]
                    { "\u260f " + TelegramCommands.CONTACTS, "\U0001F48C " + TelegramCommands.FEEDBACK }
            });
        }
        
        return new(new[]
        {
            new KeyboardButton[]
                { "\ud84d " + TelegramCommands.CATEGORIES, "\U00002b50 " + TelegramCommands.FAVORITES_QUESTIONS },
            new KeyboardButton[]
                { "\u260f " + TelegramCommands.CONTACTS, "\U0001F48C " + TelegramCommands.FEEDBACK },
            new KeyboardButton[] 
                { TelegramCommands.EDIT_QUESTION, TelegramCommands.ADD_QUESTION },
            new KeyboardButton[] 
                { TelegramCommands.DELETE_QUESTION, TelegramCommands.CHANGE_QUESTION_CATEGORY },
            new KeyboardButton[]
                { TelegramCommands.RENAME_CATEGORY, TelegramCommands.ADD_CATEGORY },
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
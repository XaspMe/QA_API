using Telegram.Bot.Types.ReplyMarkups;

namespace QA.Telegram.Bot.Constants;

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
                { TelegramCommands.DELETE_CATEGORY, TelegramCommands.SHOW_CATEGORIES_STATISTICS},
            new KeyboardButton[]
                { TelegramCommands.ADD_TEST_DATA }

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
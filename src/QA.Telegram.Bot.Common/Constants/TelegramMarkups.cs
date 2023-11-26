using Telegram.Bot.Types.ReplyMarkups;

namespace QA.Telegram.Bot.Common.Constants;

public static class TelegramMarkups
{
    public static ReplyKeyboardMarkup QUESTIONS_KEYBOARD(bool isFavorite, bool isAdmin)
    {
        if (isAdmin)
        {
            if (isFavorite)
                return new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { TelegramCommands.SHOW_ANSWER, TelegramCommands.NEXT_QUESTION },
                    new KeyboardButton[] { TelegramCommands.REMOVE_FROM_FAVORITES, TelegramCommands.RATE },
                    new KeyboardButton[] { TelegramCommands.REPORT, TelegramCommands.MENU },
                    new KeyboardButton[] { TelegramCommands.EDIT_QUESTION  }
                });

            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { TelegramCommands.SHOW_ANSWER, TelegramCommands.NEXT_QUESTION },
                new KeyboardButton[] { TelegramCommands.ADD_TO_FAVORITES, TelegramCommands.RATE },
                new KeyboardButton[] { TelegramCommands.REPORT, TelegramCommands.MENU },
                new KeyboardButton[] { TelegramCommands.EDIT_QUESTION  }
            });
        }

        if (isFavorite)
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { TelegramCommands.SHOW_ANSWER, TelegramCommands.NEXT_QUESTION },
                new KeyboardButton[] { TelegramCommands.REMOVE_FROM_FAVORITES, TelegramCommands.RATE },
                new KeyboardButton[] { TelegramCommands.REPORT, TelegramCommands.MENU }
            });

        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { TelegramCommands.SHOW_ANSWER, TelegramCommands.NEXT_QUESTION },
            new KeyboardButton[] { TelegramCommands.ADD_TO_FAVORITES, TelegramCommands.RATE },
            new KeyboardButton[] { TelegramCommands.REPORT, TelegramCommands.MENU }
        });
    }

    public static ReplyKeyboardMarkup EDIT_QUESTION_KEYBOARD => new ReplyKeyboardMarkup(new[]
    {
        new KeyboardButton[] { TelegramCommands.EDIT_QUESTION_CONTENT, TelegramCommands.EDIT_ANSWER_CONTENT },
        new KeyboardButton[] { TelegramCommands.CHANGE_QUESTION_CATEGORY, TelegramCommands.DELETE_QUESTION },
        new KeyboardButton[] { TelegramCommands.RETURN}
    });

    public static ReplyKeyboardMarkup SELECTED_QUESTION_KEYBOARD(bool isFavorite, bool isAdmin)
    {
        if (isAdmin)
        {
            if (isFavorite)
                return new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton[] { TelegramCommands.SHOW_ANSWER, TelegramCommands.RETURN },
                    new KeyboardButton[] { TelegramCommands.REMOVE_FROM_FAVORITES, TelegramCommands.RATE },
                    new KeyboardButton[] { TelegramCommands.REPORT, TelegramCommands.MENU },
                    new KeyboardButton[] { TelegramCommands.EDIT_QUESTION  }
                });

            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { TelegramCommands.SHOW_ANSWER, TelegramCommands.RETURN },
                new KeyboardButton[] { TelegramCommands.ADD_TO_FAVORITES, TelegramCommands.RATE },
                new KeyboardButton[] { TelegramCommands.REPORT, TelegramCommands.MENU },
                new KeyboardButton[] { TelegramCommands.EDIT_QUESTION  }
            });
        }

        if (isFavorite)
            return new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { TelegramCommands.SHOW_ANSWER, TelegramCommands.RETURN },
                new KeyboardButton[] { TelegramCommands.REMOVE_FROM_FAVORITES, TelegramCommands.RATE },
                new KeyboardButton[] { TelegramCommands.REPORT, TelegramCommands.MENU }
            });

        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { TelegramCommands.SHOW_ANSWER, TelegramCommands.RETURN },
            new KeyboardButton[] { TelegramCommands.ADD_TO_FAVORITES, TelegramCommands.RATE },
            new KeyboardButton[] { TelegramCommands.REPORT, TelegramCommands.MENU }
        });
    }

    public static ReplyKeyboardMarkup FAVORITE_QUESTIONS_KEYBOARD()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { TelegramCommands.SHOW_ANSWER, TelegramCommands.NEXT_FAVORITE_QUESTION },
            new KeyboardButton[] { TelegramCommands.REMOVE_FROM_FAVORITES, TelegramCommands.RATE },
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
                    { TelegramCommands.DEVELOPER_CONTACTS, TelegramCommands.FEEDBACK },
                new KeyboardButton[]
                    { TelegramCommands.ABOUT }
            });
        }

        return new(new[]
        {
            new KeyboardButton[]
                { TelegramCommands.CATEGORIES, TelegramCommands.MY_FAVORITES_QUESTIONS },
            new KeyboardButton[]
                { TelegramCommands.DEVELOPER_CONTACTS, TelegramCommands.FEEDBACK },
            new KeyboardButton[]
                { TelegramCommands.ABOUT },
            new KeyboardButton[]
                { TelegramCommands.RENAME_CATEGORY, TelegramCommands.CREATE_CATEGORY },
            new KeyboardButton[]
                { TelegramCommands.DELETE_CATEGORY, TelegramCommands.SHOW_CATEGORIES_STATISTICS},
            new KeyboardButton[]
                { TelegramCommands.ADD_TEST_DATA, TelegramCommands.ADD_QUESTION }

        });
    }

    public static ReplyKeyboardMarkup CATEGORIES_WITH_MENU_AND_ALL_SELECTED(IEnumerable<string> Categories)
    {
        //todo переписать
        var keyboardButtonsEnumerable = Categories
            .Select((category, index) => new { Category = category, Index = index })
            .GroupBy(x => x.Index / 3)
            .Select(group => group.Select(x => new KeyboardButton(x.Category)).ToArray())
            .ToList();
        keyboardButtonsEnumerable.Add(new KeyboardButton[] { TelegramCommands.ALL_CATEGORIES });
        keyboardButtonsEnumerable.Add(new KeyboardButton[] { TelegramCommands.MENU });
        return new ReplyKeyboardMarkup(keyboardButtonsEnumerable);
    }

    public static ReplyKeyboardMarkup CATEGORIES_WITH_MENU(IEnumerable<string> Categories)
    {
        var keyboardButtonsEnumerable = Categories
            .Select((category, index) => new { Category = category, Index = index })
            .GroupBy(x => x.Index / 3)
            .Select(group => group.Select(x => new KeyboardButton(x.Category)).ToArray())
            .ToList();
        keyboardButtonsEnumerable.Add(new KeyboardButton[] { TelegramCommands.MENU });
        return new ReplyKeyboardMarkup(keyboardButtonsEnumerable);
    }
}
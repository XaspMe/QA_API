﻿namespace QA.Telegram.Bot.Common.Constants;

public static class TelegramMessages
{
    public const string ABOUT = "Данное приложение содержит функционал для самостоятельной теоретической подготовки к прохождению собеседований/тестов/презентаций, а также их проведению специалистами HR или преподавателями.\n\nНа данный момент контент собран для подготовки к собеседованию по стеку DotNet.\nПодборка подходит для широкого уровня квалификации. Она включает в себя топ самых популярных, взятых с просторов интернета и озвученных на реальных собеседованиях, вопросов.\n\nПосле выбора интересующей вас темы, бот присылает вам случайный вопрос из выбранной категории, а также может отображать ответ.\n\nЕсли вы хотите добавить свои темы и вопросы, свяжитесь с разработчиком для получения прав администратора.";
    public const string CATEGORY_CREATED = "Категория успешно создана.";
    public const string UPDATE_SUCCESS = "Обновление успешно.";
    public const string REQUEST_CATEGORY_NAME = "Введите имя новой категории.";
    public const string REQUEST_CATEGORY_FOR_NEW_QA = "Выберите категорию для нового вопроса.";
    public const string QA_ELEMENT_CREATED = "Вопрос был создан.";
    public const string QA_ELEMENT_MOVED_TO_NEW_CATEGORY = "Категория вопроса была изменена.";
    public const string REQUEST_QUESTION_FOR_NEW_QA = "Введите ваш вопрос и отправьте мне текстом.";
    public const string REQUEST_ANSWER_FOR_NEW_QA = "Введите ответ на вопрос и отправьте мне текстом.";
    public const string HANDLE_ERROR = "Извините, я не могу обработать ваше сообщение.";
    public const string FEEDBACK_MESSAGE = "Введите текст и отправьте мне обычным сообщением, ваше обращение будет рассмотрено администратором.";
    public const string FEEDBACK_ACCEPTED_MESSAGE = "Спасибо за обратную связь, это помогает проекту стать лучше.";
    public const string CATEGORIES = "Выбери категорию";
    public const string ERROR = "Произошла ошибка :(";
    public const string DEVELOPER_CONTACT = "По всем вопросам или предложениям вы можете:\nНаписать мне - @comppomosh\n<a href='https://github.com/XaspMe'>Посетить мой гитхаб</a>\n<a href='https://github.com/XaspMe/QA_API'>Посмотреть исходный код этого бота</a>";
    public const string ADDED_TO_FAVORITES = "Вопрос успешно добавлен в избранное, ваши избранные вопросы могут быть найдены в главном меню.";
    public const string REMOVED_FROM_FAVORITES = "Вопрос был убран из вашего списка избранных.";
    public const string QUESTION_ISNT_FAVORITE = "Вопрос не находится в списке избранных.";
    public const string NO_FAVORITES = "В избранном пока ничего нет, добавьте понравившиеся вопросы в избранные, чтобы они появились в этом разделе.";
    public const string CATEGORY_INVALID = "Данной категории не существует.";
    public const string CONTENT_INVALID = "Неправильное содержимое.";
    public const string UNKNOWN_COMMAND = "Пока я не могу обработать данную команду:(";
    public const string ACCESS_DENIED = "Пока я не могу обработать данную команду или ваш уровень доступа ограничен, свяжитесь с администратором для повышения привелегий:(";
    public const string CATEGORY_IS_EMPTY = "В данной категории пока пусто:(";
    public const string TEST_DATA_APPEND_SUCCESS = "Тестовые данные добавлены";
    public const string SELECTOR = "Выберите пункт меню.";
    public const string REQUEST_NEW_QUESTION = "Введите исправленный вопрос.";
    public const string REQUEST_NEW_ANSWER = "Введите исправленный ответ.";
    public const string DELETE_SUCCESS = "Удаление завершено.";
    public const string ENTER_SEARCH_KEYWORD = "Введите искомую фразу или слово.";
    public static string HELLO(int qaCount) => $"Привет! Я знаю {qaCount} вопросов, которые могут быть использованы на собеседованиях по следующим темам:";
    public static string MAIN_MENU_WITH_COUNT(int qaCount) => $"Текущее количество вопросов в базе: {qaCount} \nCейчас вы в главном меню";

}
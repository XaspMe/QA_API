using QA.Models.Models;

namespace QA.Common.Extensions;

public static class QaElementExtesion
{
    public static string TelegramMarkupShowAsString(this QAElement qaElement)
    {
        return $"Вопрос /{qaElement.Id}\nКатегория:\n{qaElement.Category.Name}\nВопрос:\n{qaElement.Question}\nОтвет:\n{qaElement.Answer}";
    }
}
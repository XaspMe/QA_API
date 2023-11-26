using QA.Models.Models;

namespace QA.Common.Extesions;

public static class QaElementExtesion
{
    public static string TelegramMarkupShowAsString(this QAElement qaElement)
    {
        return $"\nКатегория:\n{qaElement.Category.Name}\nВопрос:\n{qaElement.Question}\nОтвет:\n{qaElement.Answer}";
    }
}
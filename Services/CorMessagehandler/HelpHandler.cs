// namespace QA_API.CorMessagehandler;
//
// public class HelpHandler : MessageHandler
// {
//     public override string HandleMessage(string message)
//     {
//         if (message == "Помощь")
//         {
//             // Обработка сообщения типа "Помощь"
//             return "Вот список доступных команд: ...";
//         }
//         else if (_nextHandler != null)
//         {
//             return _nextHandler.HandleMessage(message);
//         }
//         else
//         {
//             return "Извините, я не могу обработать ваше сообщение.";
//         }
//     }
// }
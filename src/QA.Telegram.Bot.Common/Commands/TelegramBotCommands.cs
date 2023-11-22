// using QA.Models.Models;
// using QA.Telegram.Bot.Common.Constants;
// using Telegram.Bot.Types;
// using Telegram.Bot.Types.Enums;
//
// namespace QA.Telegram.Bot.Common.Commands;
//
// public class TelegramBotCommands
// {
//     private async Task SendNextQuestion(Message message)
//     {
//         await _repo.SetUserCurrentStep(message.Chat.Id, UserCurrentStep.Questions);
//         var curent = await _repo.GetElementOnCurrentTelegramUser(message.Chat.Id);
//         if (curent != null)
//         {
//             await _repo.RemoveFromTelegramUserFavoriteElements(message.Chat.Id, curent);
//             await _telegramBotClient.SendTextMessageAsync(
//                 chatId: message.Chat.Id,
//                 // replace br's for telegram only
//                 text: TelegramMessages.REMOVED_FROM_FAVORITES,
//                 replyMarkup: TelegramMarkups.QUESTIONS_KEYBOARD(
//                     await _repo.IsElementTelegramUserFavorite(message.Chat.Id, curent),
//                     await _repo.IsTelegramUserAdmin(message.Chat.Id)),
//                 cancellationToken: _ct,
//                 parseMode: ParseMode.Html);
//         }
//     }
// }
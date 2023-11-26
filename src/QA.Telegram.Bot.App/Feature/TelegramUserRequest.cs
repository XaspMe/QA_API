using MediatR;
using QA.Telegram.Bot.Models;

namespace QA.Telegram.Bot.App.Feature;

public record TelegramUserRequest(TelegramUserMessage UserMessage, bool RequireAdminAccess = false) : IRequest<QaBotResponse>;
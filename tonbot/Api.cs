using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace tonbot;

public class Api
{
    private string _apiHash;
    private string _apiId;
    private readonly IConfiguration _configuration;
    private string _accessToken;
    private TelegramBotClient _bot;

    public Api(IConfiguration configuration)
    {
        _configuration = configuration;
        _apiHash = _configuration.GetValue<string>("Api_Hash");
        _apiId = _configuration.GetValue<string>("Api_Id");
        _bot = new TelegramBotClient(configuration.GetValue<string>("TELEGRAM_ACCESS_TOKEN"));
    }

    public async Task Function()
    {
        using var cts = new CancellationTokenSource();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { } // receive all update types
        };
        _bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token);

        var me = await _bot.GetMeAsync(cancellationToken: cts.Token);

        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();

// Send cancellation request to stop bot
        cts.Cancel();

    }
    
    async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Type != UpdateType.Message)
            return;
        // Only process text messages
        if (update.Message!.Type != MessageType.Text)
            return;

        var chatId = update.Message.Chat.Id;
        var messageText = update.Message.Text;

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
  
        // Echo received message text
        Message sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "\n " +
                  "С НОВЫМ ГОДОМ, Рыр. По этой ссылке ты активируешь чек, который пополнит твой кошелек на 1 TON. " +
                  "\n " +
                  "TON - крипта Дурова и Телеги, если вкратце." +
                  "\n " +
                  "Главное, что тебе нужно делать с этой монетой, это HODL. " +
                  "\n " +
                  "Ссылка для получения: t.me/CryptoBot?start=CQ5Jpd4q7nCn " +
                  "\n " +
                  "Пароль для активации написан на листочке.",
            cancellationToken: cancellationToken);
    }
    
    Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
    

   
    
    
}
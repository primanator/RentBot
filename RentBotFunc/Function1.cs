using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace RentBotFunc
{
    public static class Function1
    {
        static ITelegramBotClient botClient;

        static Function1()
            {
            botClient = new TelegramBotClient("1327964967:AAEcfo-WnViXPyGGR-VBhnQ6NQxtMSr1BWI");
        }

        [FunctionName("Function1")]
        public static async void Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var body = await req.ReadAsStringAsync();
            var update = JsonConvert.DeserializeObject<Update>(body);

            log.LogInformation("User entered channel message Update Type: " + update.Type);

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                log.LogInformation($"Received a text message in chat {update.Message.Chat.Id}.");

                await botClient.SendTextMessageAsync(
  chatId: update.Message.Chat,
  text: "You said:\n" + update.Message.Text
);
            }


            //var me = botClient.GetMeAsync().Result;
            //log.LogInformation(
            //  $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            //);
        }
    }
}

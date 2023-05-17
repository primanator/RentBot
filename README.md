# 🏠 Airbnb House Renting Telegram Bot 🤖

![Azure Functions](https://img.shields.io/badge/Azure_Functions-blue?style=flat-square&logo=microsoft-azure&logoColor=white)
![Azure Blob Storage](https://img.shields.io/badge/Azure_Blob_Storage-blue?style=flat-square&logo=microsoft-azure&logoColor=white)
![Telegram Bot](https://img.shields.io/badge/Telegram_Bot-blue?style=flat-square&logo=telegram&logoColor=white)
![Postman](https://img.shields.io/badge/Postman-orange?style=flat-square&logo=postman&logoColor=white)
![Telegram API](https://img.shields.io/badge/Telegram_API-blue?style=flat-square&logo=telegram&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-blue?style=flat-square&logo=.net&logoColor=white)

[![.NET](https://github.com/primanator/RentBot/actions/workflows/dotnet_build_and_run_unit_tests.yml/badge.svg)](https://github.com/primanator/RentBot/actions/workflows/dotnet_build_and_run_unit_tests.yml) [![Deploy DotNet project to Azure Function App](https://github.com/primanator/RentBot/actions/workflows/azure-functions-app-dotnet-publish.yml/badge.svg)](https://github.com/primanator/RentBot/actions/workflows/azure-functions-app-dotnet-publish.yml)

Welcome to the Airbnb House Renting Telegram Bot! This project is a Telegram bot that uses webhooks and Azure Functions to assist users in finding and renting the perfect Airbnb property. The bot also leverages Azure Blob Storage to store and manage property data. 🌟

## 🚀 Getting Started

To get started with the project, follow these steps:

1. Clone the repository `git clone https://github.com/primanator/RentBot.git`

2. Create a new Telegram bot by talking to the [BotFather](https://core.telegram.org/bots#how-do-i-create-a-bot) and receive the bot HTTP API token.

3. Set up your Azure Function and Azure Blob Storage accounts. For local testing of the Function, add the `"AzureWebJobsStorage"` value to the `local.settings.json` project file.

4. Set the Azure Function `authorizationLevel` to `function` and trigger to `POST` message.

5. Publish your Azure Function to the cloud.

6. Use Postman to register webhook, make a POST request to `https://api.telegram.org/bot***/setWebhook`, where `***` symbols are the bot HTTP API token. The message body should be raw JSON and include:
- `"url"`: The actual Azure Function URL with token.
- `"allowed_updates"`: An array of messages for the function to receive.
- `"drop_pending_updates": true`.

7. To see the state of the webhook, make a POST request to `https://api.telegram.org/bot***/getWebhookinfo` with an empty body.

8. To delete the webhook, make a POST request to `https://api.telegram.org/bot***/deleteWebhook` with an empty body.

9. Start using the bot on Telegram! 🎉

For more information on this project, refer to the [Wiki page](https://github.com/primanator/RentBot/wiki).

## 💼 Features

- 🔎 Search for Airbnb property by location and see what's around.
- 📷 View property photos and details.
- 🗓️ Check availability and book a property via Airbnb link.
- 💬 Communicate with property owners through the dedicated Telegram channel.
- 📝 Leave reviews and ratings for properties.

## 🛠️ Technologies

- [Azure Functions](https://azure.microsoft.com/en-us/services/functions/) - for serverless backend logic.
- [Azure Blob Storage](https://azure.microsoft.com/en-us/services/storage/blobs/) - for storing user data and property listings.
- [Postman](https://www.postman.com/) - for API testing and development.
- [Telegram API Webhooks](https://core.telegram.org/bots/api#setwebhook) - for real-time bot updates.
- [Telegram.Bot](https://github.com/TelegramBots/telegram.bot) - .NET client for the Telegram Bot API.

## 📃 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙋‍♂️ Support

If you have any questions, feel free to [open an issue](https://github.com/primanator/RentBot/issues/new) or reach out to the maintainers.

Happy house hunting! 🏡

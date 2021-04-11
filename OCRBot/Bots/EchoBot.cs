// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.12.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using OCRBot.Services.AzureStorage;
using OCRBot.Services.ContentModerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OCRBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        private IAzureStorageService _azureStorageService;
        private IContentModeratorService _contentModeratorService;
        public EchoBot(IAzureStorageService azureStorageService, IContentModeratorService contentModeratorService)
        {
            _azureStorageService = azureStorageService;
            _contentModeratorService = contentModeratorService;
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Attachments == null)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Ingresa tu documento de identidad"));
            }
            else
            {
                var image = turnContext.Activity.Attachments[0];
                //await turnContext.SendActivityAsync(MessageFactory.Attachment(image));

                byte[] imageByte = await GetByteImage(image.ContentUrl);
                var urlImage = await _azureStorageService.Execute(imageByte, image.ContentType.Split("/")[1]);
                var result = await _contentModeratorService.processImage(urlImage);

                var modelResult = result.Split("\n");

                string numero = modelResult[3].Trim();
                string primerApellido = modelResult[6].Trim();
                string segundoApellido = modelResult[8].Trim();
                string nombres = modelResult[10].Trim();
                string fechaNacimiento = modelResult[19].Trim();

                await turnContext.SendActivityAsync(MessageFactory.Text($"Número de documento: {numero}{Environment.NewLine}" +
                    $"Nombres: {nombres}{Environment.NewLine}" +
                    $"Fecha de Nacimiento: {fechaNacimiento}"));

                await turnContext.SendActivityAsync(MessageFactory.Text("Son correctos tus datos?"));


            }
        }

        private async Task<byte[]> GetByteImage(string contentUrl)
        {
            using (var httpClient = new HttpClient())
            {
                var uri = new Uri(contentUrl);
                var stream = await httpClient.GetStreamAsync(uri);

                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}

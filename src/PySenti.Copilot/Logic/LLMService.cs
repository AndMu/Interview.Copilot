using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using PySenti.Copilot.Config;
using PySenti.Copilot.Definitions;
using System.Runtime.CompilerServices;

namespace PySenti.Copilot.Logic
{
    public class LlmService : ILlmService
    {
        private readonly IChatCompletionService chatCompletion;

        public LlmService(IChatCompletionService chatCompletion)
        {
            ArgumentNullException.ThrowIfNull(chatCompletion);
            this.chatCompletion = chatCompletion;
        }

        public async IAsyncEnumerable<string> GetAnswer(LlmRequest request, [EnumeratorCancellation] CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(request);

            var history = new ChatHistory();
            history.AddSystemMessage(request.System);

            if (request.ImageData != null)
            {
                history.Add(
                    new()
                    {
                        Role = AuthorRole.User,
                        Items = [
                            new TextContent { Text = request.Question },
                            new ImageContent { Data = BinaryData.FromBytes(request.ImageData), MimeType="image/jpeg" }
                        ]
                    }
                );
            }
            else
            {
                history.Add(
                    new()
                    {
                        Role = AuthorRole.User,
                        Content = request.Question
                    }
                );
            }

            OpenAIPromptExecutionSettings openAiPromptExecutionSettings = new()
            {
            };
            
            var updates = chatCompletion.GetStreamingChatMessageContentsAsync(
                history,
                executionSettings: openAiPromptExecutionSettings,
                cancellationToken: token);

            await foreach (var update in updates)
            {
                yield return update.Content!;                
            }
        }
    }
}
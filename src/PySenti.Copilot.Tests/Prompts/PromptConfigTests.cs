using PySenti.Copilot.Prompts;

namespace PySenti.Copilot.Tests.Prompts
{
    [TestFixture]
    public class PromptConfigTests
    {
        [Test]
        public async Task Load()
        {
            var configurator = CreatePromptConfigurator();
            await configurator.Load();
            Assert.That(configurator.Prompts, Is.Not.Null);
        }

        [Test]
        public async Task Save_SerializesPromptsCorrectly()
        {
            var configurator = CreatePromptConfigurator();
            configurator.Prompts = new AllPrompts
            {
                Definitions =
                [
                    new InterviewPromptDefinition
                    {
                        Role = "TestPrompt",
                        Short = "You are interviewing for a {Role} role. {Prompts}",
                        Long = "You are interviewing for a {Role} role. {Prompts}",
                    }
                ],
            };

            await configurator.Save();
        }

        private InterviewPromptConfigurator CreatePromptConfigurator()
        {
            return new InterviewPromptConfigurator();
        }
    }
}
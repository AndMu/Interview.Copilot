using PySenti.Copilot.Prompts;

namespace PySenti.Copilot.Tests.Prompts
{
    [TestFixture]
    public class InterviewPromptGeneratorTests
    {
        [Test]
        public void GeneratePrompt()
        {
            var promptGenerator = CreatePromptGenerator();
            var request = new InterviewPromptDefinition
            {
                Role = "TestRole",
                Short = "You are interviewing for a ${Role} role.",
                Long = "You are interviewing for a ${Role} role.",
            };

            var result = promptGenerator.GeneratePrompt(request);
            Assert.That(result.Short, Is.SupersetOf("You are interviewing for a TestRole role."));
            Assert.That(result.Long, Is.SupersetOf("You are interviewing for a TestRole role."));
        }

        private InterviewPromptGenerator CreatePromptGenerator()
        {
            return new InterviewPromptGenerator();
        }
    }
}

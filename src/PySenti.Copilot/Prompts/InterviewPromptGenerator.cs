using StringTokenFormatter;
using System.Globalization;

namespace PySenti.Copilot.Prompts
{
    public class InterviewPromptGenerator : IInterviewPromptGenerator
    {
        private readonly StringTokenFormatterSettings settings;

        public InterviewPromptGenerator()
        {
            settings = StringTokenFormatterSettings.Default with
            {
                Syntax = CommonTokenSyntax.DollarCurly,
                FormatProvider = CultureInfo.GetCultureInfo("en-GB")
            };

        }

        public InterviewPromptDefinition GeneratePrompt(InterviewPromptDefinition request)        
        {
            ArgumentNullException.ThrowIfNull(request);           
            var longPrompt = request.Long.FormatFromObject(
                new
                {
                    request.Role
                },
                settings);

            var shortPrompt = request.Short.FormatFromObject(
                new
                {
                    request.Role
                },
                settings);

            return new InterviewPromptDefinition
            {
                Role = request.Role,
                Short = shortPrompt,
                Long = longPrompt
            };
        }
    }
}

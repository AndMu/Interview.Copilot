using PySenti.Copilot.Prompts;

namespace PySenti.Copilot.App.ViewModels.Config
{
    public class PromptConfigViewModel : BaseConfigViewModel<InterviewPromptDefinition>
    {
        private readonly IInterviewPromptConfigurator interviewPromptGenerator;

        public PromptConfigViewModel(IInterviewPromptConfigurator interviewPromptGenerator)
        {
            ArgumentNullException.ThrowIfNull(interviewPromptGenerator);
            this.interviewPromptGenerator = interviewPromptGenerator;
            AvailablePromptTypes = Enum.GetValues<PromptType>();
        }

        public PromptType[] AvailablePromptTypes { get; }

        public override async Task Load()
        {
            Items.Clear();
            await interviewPromptGenerator.Load();
            if (interviewPromptGenerator.Prompts == null)
            {
                return;
            }

            foreach (var prompt in interviewPromptGenerator.Prompts.Definitions)
            {            
                Items.Add(prompt);
            }
        }

        protected override void OnSaved()
        {
            interviewPromptGenerator.Prompts = new AllPrompts
            {
                Definitions = Items.ToList()
            };

            base.OnSaved();
        }

        protected override bool CanSave()
        {
            return EditingItem != null &&
                   !string.IsNullOrWhiteSpace(EditingItem.Role) &&
                   !string.IsNullOrWhiteSpace(EditingItem.Short) &&
                   !string.IsNullOrWhiteSpace(EditingItem.Long);                   
        }

        protected override InterviewPromptDefinition Create()
        {
            return
                new InterviewPromptDefinition
                {
                    Role = "New Prompt Block",
                    Short = "Short description",
                    Long = "Long description"
                };
            ;
        }
    }
}
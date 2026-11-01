using QuizConfigurator.Models;

namespace QuizConfigurator.ViewModels
{
    public class ConfigurationViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        private Question? _selectedQuestion;

        public ConfigurationViewModel(
            MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public QuestionPackViewModel ActivePack
        {
            get => _mainWindowViewModel.ActivePack;
        }

        public Question? SelectedQuestion
        {
            get => _selectedQuestion;
            set => SetProperty(ref _selectedQuestion, value);
        }
    }
}
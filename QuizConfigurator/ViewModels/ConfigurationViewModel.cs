using QuizConfigurator.Commands;
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

            AddQuestionCommand = new RelayCommand(
                _ => AddQuestion());

            RemoveQuestionCommand = new RelayCommand(
                _ => RemoveQuestion(),
                _ => SelectedQuestion is not null);
        }

        public QuestionPackViewModel ActivePack
        {
            get => _mainWindowViewModel.ActivePack;
        }

        public Question? SelectedQuestion
        {
            get => _selectedQuestion;

            set
            {
                if (SetProperty(ref _selectedQuestion, value))
                {
                    RemoveQuestionCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand AddQuestionCommand { get; }

        public RelayCommand RemoveQuestionCommand { get; }

        private void AddQuestion()
        {
            var question = new Question
            {
                Query = "Ny fråga",
                CorrectAnswer = "",
                IncorrectAnswers = new[] { "", "", "" }
            };

            ActivePack.Questions.Add(question);
            SelectedQuestion = question;
        }

        private void RemoveQuestion()
        {
            if (SelectedQuestion is null)
            {
                return;
            }

            var questionToRemove = SelectedQuestion;

            SelectedQuestion = null;

            ActivePack.Questions.Remove(questionToRemove);
        }
    }
}
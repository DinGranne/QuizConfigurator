using System.Collections.Generic;
using System.Linq;
using QuizConfigurator.Commands;
using QuizConfigurator.Models;

namespace QuizConfigurator.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        private List<Question> _questions = new();
        private int _questionIndex = -1;
        private Question? _currentQuestion;
        private string _message = "Välj ett frågepaket och starta quizet.";

        public PlayerViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            StartQuizCommand = new RelayCommand(
                _ => StartQuiz());
        }

        public RelayCommand StartQuizCommand { get; }

        public Question? CurrentQuestion
        {
            get => _currentQuestion;

            private set
            {
                SetProperty(ref _currentQuestion, value);
            }
        }

        public string ProgressText
        {
            get
            {
                if (_questionIndex < 0)
                {
                    return "";
                }

                return $"Fråga {_questionIndex + 1} av {_questions.Count}";
            }
        }

        public string Message
        {
            get => _message;
            private set => SetProperty(ref _message, value);
        }

        private void StartQuiz()
        {
            CurrentQuestion = null;
            _questionIndex = -1;
            _questions.Clear();

            RaisePropertyChanged(nameof(ProgressText));

            var pack = _mainWindowViewModel.ActivePack;

            if (pack is null)
            {
                Message = "Välj ett frågepaket först.";
                return;
            }

            if (pack.Questions.Count == 0)
            {
                Message = "Paketet är tomt. Lägg till frågor i redigeringsläget.";
                return;
            }

            _questions = pack.Questions
                .Select(question => new Question
                {
                    Query = question.Query,
                    CorrectAnswer = question.CorrectAnswer,
                    IncorrectAnswers = question.IncorrectAnswers.ToArray()
                })
                .ToList();

            _questionIndex = 0;
            CurrentQuestion = _questions[_questionIndex];

            Message = "";
            RaisePropertyChanged(nameof(ProgressText));
        }
    }
}
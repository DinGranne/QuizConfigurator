using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private int _score;
        private Question? _currentQuestion;

        private bool _hasAnswered = true;
        private string _message = "Välj ett frågepaket och starta quizet.";
        private string _feedback = "";

        public PlayerViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            StartQuizCommand = new RelayCommand(
                _ => StartQuiz());

            AnswerCommand = new RelayCommand(
                parameter => Answer(parameter),
                parameter => !_hasAnswered
                    && parameter is QuizAnswer answer
                    && Answers.Contains(answer));

            NextQuestionCommand = new RelayCommand(
                _ => NextQuestion(),
                _ => CurrentQuestion is not null && _hasAnswered);
        }

        public RelayCommand StartQuizCommand { get; }

        public RelayCommand AnswerCommand { get; }

        public RelayCommand NextQuestionCommand { get; }

        public ObservableCollection<QuizAnswer> Answers { get; } = new();

        public Question? CurrentQuestion
        {
            get => _currentQuestion;
            private set => SetProperty(ref _currentQuestion, value);
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

        public string ScoreText => $"Antal rätt: {_score}";

        public string NextButtonText
        {
            get
            {
                return CurrentQuestion is not null
                    && _questionIndex == _questions.Count - 1
                    ? "Visa resultat"
                    : "Nästa fråga";
            }
        }

        public string Message
        {
            get => _message;
            private set => SetProperty(ref _message, value);
        }

        public string Feedback
        {
            get => _feedback;
            private set => SetProperty(ref _feedback, value);
        }

        private void StartQuiz()
        {
            _hasAnswered = true;
            CurrentQuestion = null;
            Answers.Clear();

            _questionIndex = -1;
            _score = 0;
            _questions.Clear();

            Feedback = "";
            UpdateGameState();

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

            bool hasIncompleteQuestion = pack.Questions.Any(question =>
                string.IsNullOrWhiteSpace(question.Query)
                || string.IsNullOrWhiteSpace(question.CorrectAnswer)
                || question.IncorrectAnswers.Any(
                    answer => string.IsNullOrWhiteSpace(answer)));

            if (hasIncompleteQuestion)
            {
                Message =
                    "Fyll i frågetext och alla fyra svar för varje fråga innan du spelar.";
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
            Message = "";

            ShowCurrentQuestion();
        }

        private void ShowCurrentQuestion()
        {
            _hasAnswered = true;
            Answers.Clear();
            Feedback = "";

            CurrentQuestion = _questions[_questionIndex];

            Answers.Add(new QuizAnswer
            {
                Text = CurrentQuestion.CorrectAnswer,
                IsCorrect = true
            });

            foreach (var incorrectAnswer in CurrentQuestion.IncorrectAnswers)
            {
                Answers.Add(new QuizAnswer
                {
                    Text = incorrectAnswer,
                    IsCorrect = false
                });
            }

            _hasAnswered = false;
            UpdateGameState();
        }

        private void Answer(object? parameter)
        {
            if (_hasAnswered
                || CurrentQuestion is null
                || parameter is not QuizAnswer answer
                || !Answers.Contains(answer))
            {
                return;
            }

            _hasAnswered = true;

            if (answer.IsCorrect)
            {
                _score++;
            }

            Feedback = answer.IsCorrect
                ? $"Rätt! Det korrekta svaret är: {CurrentQuestion.CorrectAnswer}"
                : $"Fel. Det korrekta svaret är: {CurrentQuestion.CorrectAnswer}";

            UpdateGameState();
        }

        private void NextQuestion()
        {
            if (CurrentQuestion is null || !_hasAnswered)
            {
                return;
            }

            if (_questionIndex == _questions.Count - 1)
            {
                FinishQuiz();
                return;
            }

            _questionIndex++;
            ShowCurrentQuestion();
        }

        private void FinishQuiz()
        {
            Message = $"Quizet är klart! Du fick {_score} av {_questions.Count} rätt.";

            CurrentQuestion = null;
            Answers.Clear();
            Feedback = "";
            _questionIndex = -1;
            _hasAnswered = true;

            UpdateGameState();
        }

        private void UpdateGameState()
        {
            RaisePropertyChanged(nameof(ProgressText));
            RaisePropertyChanged(nameof(ScoreText));
            RaisePropertyChanged(nameof(NextButtonText));

            AnswerCommand.RaiseCanExecuteChanged();
            NextQuestionCommand.RaiseCanExecuteChanged();
        }
    }
}
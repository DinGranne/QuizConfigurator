using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Threading;
using QuizConfigurator.Commands;
using QuizConfigurator.Models;

namespace QuizConfigurator.ViewModels
{
    public class PlayerViewModel : ViewModelBase
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly DispatcherTimer _timer;
        private readonly Stopwatch _stopwatch = new();

        private List<Question> _questions = new();
        private int _questionIndex = -1;
        private int _score;
        private int _timeLimitInSeconds;
        private int _secondsRemaining;

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

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };

            _timer.Tick += OnTimerTick;

            _mainWindowViewModel.PropertyChanged
                += OnMainWindowPropertyChanged;
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

        public int SecondsRemaining
        {
            get => _secondsRemaining;
            private set => SetProperty(ref _secondsRemaining, value);
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
            ClearRound();

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

            if (pack.TimeLimitInSeconds <= 0)
            {
                Message = "Paketets tidsgräns måste vara större än noll.";
                return;
            }

            _timeLimitInSeconds = pack.TimeLimitInSeconds;

            _questions = pack.Questions
                .Select(question => new Question
                {
                    Query = question.Query,
                    CorrectAnswer = question.CorrectAnswer,
                    IncorrectAnswers = question.IncorrectAnswers.ToArray()
                })
                .ToList();

            Shuffle(_questions);

            _questionIndex = 0;
            Message = "";

            ShowCurrentQuestion();
        }

        private void ShowCurrentQuestion()
        {
            StopTimer();

            _hasAnswered = true;
            Answers.Clear();
            Feedback = "";

            CurrentQuestion = _questions[_questionIndex];

            var options = new List<QuizAnswer>
            {
                new QuizAnswer
                {
                    Text = CurrentQuestion.CorrectAnswer,
                    IsCorrect = true
                }
            };

            foreach (var incorrectAnswer in CurrentQuestion.IncorrectAnswers)
            {
                options.Add(new QuizAnswer
                {
                    Text = incorrectAnswer,
                    IsCorrect = false
                });
            }

            Shuffle(options);

            foreach (var option in options)
            {
                Answers.Add(option);
            }

            SecondsRemaining = _timeLimitInSeconds;
            _hasAnswered = false;

            _stopwatch.Restart();
            _timer.Start();

            UpdateGameState();
        }

        private void OnTimerTick(object? sender, EventArgs e)
        {
            if (_hasAnswered || CurrentQuestion is null)
            {
                StopTimer();
                return;
            }

            double remaining =
                _timeLimitInSeconds - _stopwatch.Elapsed.TotalSeconds;

            SecondsRemaining =
                (int)Math.Ceiling(Math.Max(0, remaining));

            if (remaining <= 0)
            {
                HandleTimeExpired();
            }
        }

        private void HandleTimeExpired()
        {
            if (_hasAnswered || CurrentQuestion is null)
            {
                return;
            }

            _hasAnswered = true;
            StopTimer();
            SecondsRemaining = 0;

            Feedback =
                $"Tiden är slut! Det korrekta svaret är: {CurrentQuestion.CorrectAnswer}";

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

            if (_stopwatch.Elapsed.TotalSeconds >= _timeLimitInSeconds)
            {
                HandleTimeExpired();
                return;
            }

            _hasAnswered = true;
            StopTimer();

            SecondsRemaining = (int)Math.Ceiling(Math.Max(
                0,
                _timeLimitInSeconds - _stopwatch.Elapsed.TotalSeconds));

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
            StopTimer();

            Message =
                $"Quizet är klart! Du fick {_score} av {_questions.Count} rätt.";

            CurrentQuestion = null;
            Answers.Clear();
            Feedback = "";

            _questionIndex = -1;
            _hasAnswered = true;
            SecondsRemaining = 0;

            UpdateGameState();
        }

        private void ClearRound()
        {
            StopTimer();

            _hasAnswered = true;
            CurrentQuestion = null;
            Answers.Clear();
            _questions.Clear();

            _questionIndex = -1;
            _score = 0;
            SecondsRemaining = 0;
            Feedback = "";

            UpdateGameState();
        }

        private void StopTimer()
        {
            _timer.Stop();
            _stopwatch.Stop();
        }

        private void OnMainWindowPropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            bool changedPack =
                e.PropertyName == nameof(MainWindowViewModel.ActivePack);

            bool leftPlayMode =
                e.PropertyName == nameof(MainWindowViewModel.IsPlayMode)
                && !_mainWindowViewModel.IsPlayMode;

            if (changedPack || leftPlayMode)
            {
                ClearRound();
                Message = "Välj ett frågepaket och starta quizet.";
            }
        }

        private static void Shuffle<T>(List<T> items)
        {
            for (int i = items.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Shared.Next(i + 1);

                T temporary = items[i];
                items[i] = items[randomIndex];
                items[randomIndex] = temporary;
            }
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
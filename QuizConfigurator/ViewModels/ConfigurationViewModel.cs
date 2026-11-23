using System;
using System.ComponentModel;
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
                _ => AddQuestion(),
                _ => ActivePack is not null);

            RemoveQuestionCommand = new RelayCommand(
                _ => RemoveQuestion(),
                _ => ActivePack is not null
                    && SelectedQuestion is not null);

            OpenPackOptionsCommand = new RelayCommand(
                _ => OpenPackOptions(),
                _ => ActivePack is not null);

            _mainWindowViewModel.PropertyChanged
                += OnMainWindowPropertyChanged;
        }

        public QuestionPackViewModel? ActivePack
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

        public RelayCommand OpenPackOptionsCommand { get; }

        public event Action<PackOptionsViewModel>? PackOptionsRequested;

        private void OnMainWindowPropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.ActivePack))
            {
                SelectedQuestion = null;

                RaisePropertyChanged(nameof(ActivePack));

                AddQuestionCommand.RaiseCanExecuteChanged();
                RemoveQuestionCommand.RaiseCanExecuteChanged();
                OpenPackOptionsCommand.RaiseCanExecuteChanged();
            }
        }

        private void AddQuestion()
        {
            var pack = ActivePack;

            if (pack is null)
            {
                return;
            }

            var question = new Question
            {
                Query = "Ny fråga",
                CorrectAnswer = "",
                IncorrectAnswers = new[] { "", "", "" }
            };

            pack.Questions.Add(question);
            SelectedQuestion = question;
        }

        private void RemoveQuestion()
        {
            var pack = ActivePack;
            var questionToRemove = SelectedQuestion;

            if (pack is null || questionToRemove is null)
            {
                return;
            }

            SelectedQuestion = null;
            pack.Questions.Remove(questionToRemove);
        }

        private void OpenPackOptions()
        {
            var pack = ActivePack;

            if (pack is null)
            {
                return;
            }

            var options = new PackOptionsViewModel(pack);

            PackOptionsRequested?.Invoke(options);
        }
    }
}
using System;
using System.Collections.ObjectModel;
using QuizConfigurator.Commands;
using QuizConfigurator.Models;

namespace QuizConfigurator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private QuestionPackViewModel? _activePack;

        public ObservableCollection<QuestionPackViewModel> Packs
        {
            get;
        } = new();

        public QuestionPackViewModel? ActivePack
        {
            get => _activePack;

            set
            {
                if (SetProperty(ref _activePack, value))
                {
                    RemovePackCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ConfigurationViewModel ConfigurationViewModel { get; }

        public RelayCommand CreateNewPackCommand { get; }

        public RelayCommand RemovePackCommand { get; }

        public event Action<CreateNewPackViewModel>? CreateNewPackRequested;

        public MainWindowViewModel()
        {
            RemovePackCommand = new RelayCommand(
                _ => RemovePack(),
                _ => ActivePack is not null);

            var questionPack = new QuestionPack(
                "Mina frågor",
                Difficulty.Easy);

            questionPack.Questions.Add(new Question
            {
                Query = "Vad heter Sveriges huvudstad?",
                CorrectAnswer = "Stockholm",
                IncorrectAnswers = new[]
                {
                    "Göteborg",
                    "Malmö",
                    "Uppsala"
                }
            });

            questionPack.Questions.Add(new Question
            {
                Query = "Hur många ben har en spindel?",
                CorrectAnswer = "Åtta",
                IncorrectAnswers = new[]
                {
                    "Fyra",
                    "Sex",
                    "Tio"
                }
            });

            _activePack = new QuestionPackViewModel(questionPack);
            Packs.Add(_activePack);

            ConfigurationViewModel = new ConfigurationViewModel(this);

            CreateNewPackCommand = new RelayCommand(
                _ => OpenCreateNewPack());
        }

        private void OpenCreateNewPack()
        {
            var dialogViewModel = new CreateNewPackViewModel();

            dialogViewModel.CreateRequested += () =>
            {
                CreatePack(dialogViewModel.Name);
            };

            CreateNewPackRequested?.Invoke(dialogViewModel);
        }

        private void CreatePack(string name)
        {
            var model = new QuestionPack(
                name.Trim(),
                Difficulty.Easy);

            var pack = new QuestionPackViewModel(model);

            Packs.Add(pack);
            ActivePack = pack;
        }

        private void RemovePack()
        {
            var packToRemove = ActivePack;

            if (packToRemove is null)
            {
                return;
            }

            ActivePack = null;
            Packs.Remove(packToRemove);

            if (Packs.Count > 0)
            {
                ActivePack = Packs[0];
            }
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using QuizConfigurator.Commands;
using QuizConfigurator.Models;
using QuizConfigurator.Services;

namespace QuizConfigurator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly QuizStorageService _storage = new();

        private QuestionPackViewModel? _activePack;
        private bool _isSaving;
        private string _statusMessage = "";

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

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value);
        }

        public ConfigurationViewModel ConfigurationViewModel { get; }

        public RelayCommand CreateNewPackCommand { get; }

        public RelayCommand RemovePackCommand { get; }

        public RelayCommand SaveCommand { get; }

        public event Action<CreateNewPackViewModel>? CreateNewPackRequested;

        public MainWindowViewModel()
        {
            RemovePackCommand = new RelayCommand(
                _ => RemovePack(),
                _ => ActivePack is not null);

            SaveCommand = new RelayCommand(
                async _ => await SaveAsync(),
                _ => !_isSaving);

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

        private async Task SaveAsync()
        {
            if (_isSaving)
            {
                return;
            }

            _isSaving = true;
            SaveCommand.RaiseCanExecuteChanged();

            StatusMessage = "Sparar frågepaketen...";

            try
            {
                var models = Packs
                    .Select(pack => pack.ToModel())
                    .ToList();

                await _storage.SaveAsync(models);

                StatusMessage = "Frågepaketen har sparats.";
            }
            catch (Exception ex)
            {
                StatusMessage =
                    $"Det gick inte att spara frågepaketen: {ex.Message}";
            }
            finally
            {
                _isSaving = false;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
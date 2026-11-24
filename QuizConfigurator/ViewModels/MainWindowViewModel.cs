using System;
using System.Collections.ObjectModel;
using System.IO;
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
        private bool _isBusy = true;
        private bool _loadFailed;
        private bool _isPlayMode;
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

        public bool IsPlayMode
        {
            get => _isPlayMode;
            private set => SetProperty(ref _isPlayMode, value);
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

        public RelayCommand ShowEditCommand { get; }

        public RelayCommand ShowPlayCommand { get; }

        public event Action<CreateNewPackViewModel>? CreateNewPackRequested;

        public MainWindowViewModel()
        {
            RemovePackCommand = new RelayCommand(
                _ => RemovePack(),
                _ => !_isBusy && ActivePack is not null);

            CreateNewPackCommand = new RelayCommand(
                _ => OpenCreateNewPack(),
                _ => !_isBusy);

            SaveCommand = new RelayCommand(
                async _ => await SaveAsync(),
                _ => !_isBusy && !_loadFailed);

            ShowEditCommand = new RelayCommand(
                _ => IsPlayMode = false);

            ShowPlayCommand = new RelayCommand(
                _ => IsPlayMode = true);

            ConfigurationViewModel = new ConfigurationViewModel(this);
        }

        public async Task LoadAsync()
        {
            SetBusy(true);
            _loadFailed = false;
            StatusMessage = "Läser in frågepaketen...";

            try
            {
                var models = await _storage.LoadAsync();

                foreach (var model in models)
                {
                    ValidatePack(model);
                }

                var loadedPacks = models
                    .Select(model => new QuestionPackViewModel(model))
                    .ToList();

                ActivePack = null;
                Packs.Clear();

                foreach (var pack in loadedPacks)
                {
                    Packs.Add(pack);
                }

                ActivePack = Packs.FirstOrDefault();

                StatusMessage = Packs.Count == 0
                    ? "Inga frågepaket finns. Skapa ett via File-menyn."
                    : $"Läste in {Packs.Count} frågepaket.";
            }
            catch (Exception ex)
            {
                _loadFailed = true;

                StatusMessage =
                    $"Kunde inte läsa frågepaketen: {ex.Message} " +
                    "Sparning är blockerad för att skydda filen.";
            }
            finally
            {
                SetBusy(false);
            }
        }

        private static void ValidatePack(QuestionPack? pack)
        {
            if (pack is null
                || string.IsNullOrWhiteSpace(pack.Name)
                || !Enum.IsDefined(typeof(Difficulty), pack.Difficulty)
                || pack.TimeLimitInSeconds <= 0
                || pack.Questions is null)
            {
                throw new InvalidDataException(
                    "Ett frågepaket har ogiltiga inställningar.");
            }

            foreach (var question in pack.Questions)
            {
                if (question is null
                    || question.Query is null
                    || question.CorrectAnswer is null
                    || question.IncorrectAnswers is null
                    || question.IncorrectAnswers.Length != 3
                    || question.IncorrectAnswers.Any(answer => answer is null))
                {
                    throw new InvalidDataException(
                        "En fråga saknar text eller giltiga svarsfält.");
                }
            }
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
            ActivePack = Packs.FirstOrDefault();
        }

        private async Task SaveAsync()
        {
            if (_isBusy || _loadFailed)
            {
                return;
            }

            SetBusy(true);
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
                SetBusy(false);
            }
        }

        private void SetBusy(bool value)
        {
            _isBusy = value;

            SaveCommand.RaiseCanExecuteChanged();
            CreateNewPackCommand.RaiseCanExecuteChanged();
            RemovePackCommand.RaiseCanExecuteChanged();
        }
    }
}
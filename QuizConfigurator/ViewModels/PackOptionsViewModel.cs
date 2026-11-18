using System;
using QuizConfigurator.Commands;
using QuizConfigurator.Models;

namespace QuizConfigurator.ViewModels
{
    public class PackOptionsViewModel : ViewModelBase
    {
        private readonly QuestionPackViewModel _pack;

        private string _name;
        private Difficulty _difficulty;
        private string _timeLimitText;

        public PackOptionsViewModel(QuestionPackViewModel pack)
        {
            _pack = pack;

            _name = pack.Name;
            _difficulty = pack.Difficulty;
            _timeLimitText = pack.TimeLimitInSeconds.ToString();

            SaveCommand = new RelayCommand(
                _ => Save(),
                _ => CanSave());
        }

        public string Name
        {
            get => _name;

            set
            {
                if (SetProperty(ref _name, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public Difficulty Difficulty
        {
            get => _difficulty;
            set => SetProperty(ref _difficulty, value);
        }

        public string TimeLimitText
        {
            get => _timeLimitText;

            set
            {
                if (SetProperty(ref _timeLimitText, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public Difficulty[] Difficulties { get; } =
            Enum.GetValues<Difficulty>();

        public RelayCommand SaveCommand { get; }

        public event Action? SaveRequested;

        public void ApplyChanges()
        {
            if (!CanSave())
            {
                return;
            }

            _pack.Name = Name.Trim();
            _pack.Difficulty = Difficulty;
            _pack.TimeLimitInSeconds = int.Parse(TimeLimitText);
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Name)
                && int.TryParse(TimeLimitText, out int seconds)
                && seconds > 0;
        }

        private void Save()
        {
            if (!CanSave())
            {
                return;
            }

            SaveRequested?.Invoke();
        }
    }
}
using System;
using QuizConfigurator.Commands;

namespace QuizConfigurator.ViewModels
{
    public class CreateNewPackViewModel : ViewModelBase
    {
        private string _name = "";

        public CreateNewPackViewModel()
        {
            CreateCommand = new RelayCommand(
                _ => Create(),
                _ => !string.IsNullOrWhiteSpace(Name));
        }

        public string Name
        {
            get => _name;

            set
            {
                if (SetProperty(ref _name, value))
                {
                    CreateCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand CreateCommand { get; }

        public event Action? CreateRequested;

        private void Create()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                return;
            }

            CreateRequested?.Invoke();
        }
    }
}
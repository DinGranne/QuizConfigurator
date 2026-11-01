using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace QuizConfigurator.Models
{
    public class Question : INotifyPropertyChanged
    {
        private string _query = "";

        public string Query
        {
            get => _query;

            set
            {
                if (_query == value)
                {
                    return;
                }

                _query = value;
                OnPropertyChanged();
            }
        }

        public string CorrectAnswer { get; set; } = "";

        public string[] IncorrectAnswers { get; set; } = new string[3];

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
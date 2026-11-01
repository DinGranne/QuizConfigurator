using System.Collections.ObjectModel;
using QuizConfigurator.Models;

namespace QuizConfigurator.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private QuestionPackViewModel _activePack;

        public ObservableCollection<QuestionPackViewModel> Packs
        {
            get;
        } = new();

        public QuestionPackViewModel ActivePack
        {
            get => _activePack;
            set => SetProperty(ref _activePack, value);
        }

        public ConfigurationViewModel ConfigurationViewModel { get; }

        public MainWindowViewModel()
        {
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
        }
    }
}
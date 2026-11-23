using System.Windows;
using QuizConfigurator.Dialogs;
using QuizConfigurator.ViewModels;

namespace QuizConfigurator
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();

            _viewModel.ConfigurationViewModel.PackOptionsRequested
                += OpenPackOptionsDialog;

            _viewModel.CreateNewPackRequested
                += OpenCreateNewPackDialog;

            DataContext = _viewModel;

            Loaded += OnWindowLoaded;
        }

        private async void OnWindowLoaded(
            object sender,
            RoutedEventArgs e)
        {
            Loaded -= OnWindowLoaded;

            await _viewModel.LoadAsync();
        }

        private void OpenPackOptionsDialog(
            PackOptionsViewModel options)
        {
            var dialog = new PackOptionsDialog(options)
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true)
            {
                options.ApplyChanges();
            }
        }

        private void OpenCreateNewPackDialog(
            CreateNewPackViewModel options)
        {
            var dialog = new CreateNewPackDialog(options)
            {
                Owner = this
            };

            dialog.ShowDialog();
        }
    }
}
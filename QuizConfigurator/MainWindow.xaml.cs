using System.Windows;
using QuizConfigurator.Dialogs;
using QuizConfigurator.ViewModels;

namespace QuizConfigurator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new MainWindowViewModel();

            viewModel.ConfigurationViewModel.PackOptionsRequested
                += OpenPackOptionsDialog;

            viewModel.CreateNewPackRequested
                += OpenCreateNewPackDialog;

            DataContext = viewModel;
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
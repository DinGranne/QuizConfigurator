using System.Windows;
using QuizConfigurator.ViewModels;

namespace QuizConfigurator.Dialogs
{
    public partial class PackOptionsDialog : Window
    {
        public PackOptionsDialog()
        {
            InitializeComponent();
        }

        public PackOptionsDialog(PackOptionsViewModel viewModel)
            : this()
        {
            DataContext = viewModel;

            viewModel.SaveRequested += OnSaveRequested;

            Closed += (_, _) =>
                viewModel.SaveRequested -= OnSaveRequested;
        }

        private void OnSaveRequested()
        {
            DialogResult = true;
        }
    }
}
using System.Windows;
using QuizConfigurator.ViewModels;

namespace QuizConfigurator.Dialogs
{
    public partial class CreateNewPackDialog : Window
    {
        public CreateNewPackDialog()
        {
            InitializeComponent();
        }

        public CreateNewPackDialog(CreateNewPackViewModel viewModel)
            : this()
        {
            DataContext = viewModel;

            viewModel.CreateRequested += OnCreateRequested;

            Closed += (_, _) =>
                viewModel.CreateRequested -= OnCreateRequested;
        }

        private void OnCreateRequested()
        {
            DialogResult = true;
        }
    }
}
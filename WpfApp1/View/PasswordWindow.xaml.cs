using System.Windows;
using WpfApp1.Models;
using WpfApp1.ViewModel;

namespace WpfApp1.View
{
    public partial class PasswordWindow : Window
    {
        public PasswordWindow()
        {
            InitializeComponent();
            PasswordViewModel viewModel = new();

            DataContext = viewModel;

            viewModel.PasswordAccepted += OnPasswordAccepted;
        }

        private void OnPasswordAccepted(Teacher teacher)
        {
            new MainWindow(teacher).Show();
            Close();
        }
    }
}

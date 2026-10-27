using System.ComponentModel;
using System.Windows;
using WpfApp1.Models;
using WpfApp1.ViewModel;

namespace WpfApp1.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow(Teacher teacher)
        {
            InitializeComponent();
            _viewModel = new MainViewModel(teacher);
            DataContext = _viewModel;
        }

        // не даём закрыть окно с несохранённым рейтингом без вопроса
        protected override void OnClosing(CancelEventArgs e)
        {
            if (!_viewModel.ConfirmClose()) e.Cancel = true;
            base.OnClosing(e);
        }
    }
}

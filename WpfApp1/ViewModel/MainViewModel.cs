using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp1.Models;
using WpfApp1.Services;

namespace WpfApp1.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly FileDataService _dataService = new();

        // дисциплина, чьи студенты сейчас в таблице (нужна, чтобы сохранить её при переходе к другой)
        private Discipline? _loadedDiscipline;

        public Teacher Teacher { get; }

        public ObservableCollection<Discipline> Disciplines { get; }

        [ObservableProperty]
        private ObservableCollection<StudentRating> _students = new();

        [ObservableProperty]
        private Discipline? _selectedDiscipline;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private bool _hasUnsavedChanges;

        [ObservableProperty]
        private string _statusMessage = "";

        public MainViewModel(Teacher teacher)
        {
            Teacher = teacher;
            Disciplines = new ObservableCollection<Discipline>(_dataService.LoadDisciplines(teacher.Login));

            StatusMessage = Disciplines.Count == 0
                ? "За вами не закреплено ни одной дисциплины"
                : "Выберите дисциплину слева";
        }

        // вызывается автоматически при выборе дисциплины в списке
        partial void OnSelectedDisciplineChanged(Discipline? value)
        {
            AskToSaveChanges("Сохранить рейтинг перед переходом к другой дисциплине?");

            foreach (StudentRating s in Students) s.PropertyChanged -= OnStudentChanged;

            Students = value == null
                ? new ObservableCollection<StudentRating>()
                : new ObservableCollection<StudentRating>(_dataService.LoadRatings(value));

            foreach (StudentRating s in Students) s.PropertyChanged += OnStudentChanged;

            _loadedDiscipline = value;
            HasUnsavedChanges = false;

            if (value == null) StatusMessage = "";
            else if (Students.Count == 0) StatusMessage = $"Не найден список студентов группы {value.Group}";
            else StatusMessage = $"Студентов в группе: {Students.Count}";
        }

        // любая правка балла — появились несохранённые изменения
        private void OnStudentChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StudentRating.Total)) return;

            HasUnsavedChanges = true;
            StatusMessage = "Есть несохранённые изменения";
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private void Save()
        {
            if (_loadedDiscipline == null) return;

            try
            {
                _dataService.SaveRatings(_loadedDiscipline, Students);
                HasUnsavedChanges = false;
                StatusMessage = $"Рейтинг сохранён ({DateTime.Now:HH:mm:ss})";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось сохранить файл:\n{ex.Message}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanSave() => _loadedDiscipline != null && HasUnsavedChanges;

        /// <summary>
        /// Вызывается окном при закрытии: предлагает сохранить, false — закрытие отменено
        /// </summary>
        public bool ConfirmClose()
        {
            if (!HasUnsavedChanges) return true;

            MessageBoxResult answer = MessageBox.Show("Рейтинг не сохранён. Сохранить перед выходом?",
                "Несохранённые изменения", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

            if (answer == MessageBoxResult.Cancel) return false;
            if (answer == MessageBoxResult.Yes) Save();
            return true;
        }

        private void AskToSaveChanges(string question)
        {
            if (!HasUnsavedChanges || _loadedDiscipline == null) return;

            MessageBoxResult answer = MessageBox.Show(question, "Несохранённые изменения",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (answer == MessageBoxResult.Yes) Save();
        }
    }
}

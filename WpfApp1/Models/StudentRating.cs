using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp1.Models
{
    /// <summary>
    /// Одна строка таблицы рейтинга: студент и баллы по трём контрольным точкам
    /// </summary>
    public partial class StudentRating : ObservableObject
    {
        public const int MaxScore = 100;

        public string FullName { get; set; } = "";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Total))]
        private int _kt1;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Total))]
        private int _kt2;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Total))]
        private int _kt3;

        public int Total => Kt1 + Kt2 + Kt3;

        // баллы ограничены диапазоном 0..MaxScore
        partial void OnKt1Changed(int value) => Kt1 = Clamp(value);
        partial void OnKt2Changed(int value) => Kt2 = Clamp(value);
        partial void OnKt3Changed(int value) => Kt3 = Clamp(value);

        private static int Clamp(int value) => Math.Clamp(value, 0, MaxScore);
    }
}

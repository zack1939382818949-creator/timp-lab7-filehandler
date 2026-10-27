using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using WpfApp1.Models;

namespace WpfApp1.Services
{
    /// <summary>
    /// Вся работа с текстовыми файлами из папки Data
    /// </summary>
    public class FileDataService
    {
        private readonly string _dataFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private readonly Encoding _enc = new UTF8Encoding(false);

        public bool TeachersFileExists => File.Exists(Path.Combine(_dataFolder, "teachers.txt"));

        /// <summary>
        /// Читает teachers.txt: логин;пароль;ФИО;кафедра
        /// </summary>
        public List<Teacher> LoadTeachers()
        {
            List<Teacher> teachers = new();
            string teachersPath = Path.Combine(_dataFolder, "teachers.txt");
            if (!File.Exists(teachersPath)) { return teachers; }

            using (StreamReader reader = new StreamReader(teachersPath, _enc))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split(";");
                    if (parts.Length < 4) { continue; }

                    teachers.Add(new Teacher()
                    {
                        Login = parts[0].Trim(),
                        Password = parts[1].Trim(),
                        FullName = parts[2].Trim(),
                        Department = parts[3].Trim()
                    });
                }
            }
            return teachers;
        }

        /// <summary>
        /// Читает disciplines.txt: логин;код;название;группа;КТ1|КТ2|КТ3 — только дисциплины указанного преподавателя
        /// </summary>
        public List<Discipline> LoadDisciplines(string teacherLogin)
        {
            List<Discipline> disciplines = new();
            string disciplinesPath = Path.Combine(_dataFolder, "disciplines.txt");
            if (!File.Exists(disciplinesPath)) { return disciplines; }

            using (StreamReader reader = new StreamReader(disciplinesPath, _enc))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split(";");
                    if (parts.Length < 5) { continue; }
                    if (!string.Equals(parts[0].Trim(), teacherLogin, StringComparison.OrdinalIgnoreCase)) { continue; }

                    disciplines.Add(new Discipline()
                    {
                        TeacherLogin = parts[0].Trim(),
                        Code = parts[1].Trim(),
                        Name = parts[2].Trim(),
                        Group = parts[3].Trim(),
                        ControlPoints = parts[4].Split("|").Select(cp => cp.Trim()).ToList()
                    });
                }
            }
            return disciplines;
        }

        /// <summary>
        /// Студенты группы из students_[группа].txt + ранее сохранённые баллы из ratings_[код].txt
        /// </summary>
        public List<StudentRating> LoadRatings(Discipline discipline)
        {
            List<StudentRating> students = new();
            string studentsPath = Path.Combine(_dataFolder, $"students_{discipline.Group}.txt");
            if (!File.Exists(studentsPath)) { return students; }

            using (StreamReader reader = new StreamReader(studentsPath, _enc))
            {
                // сначала считаем строки, чтобы создать список нужной ёмкости
                int count = 0;
                while (reader.ReadLine() != null) count++;

                reader.BaseStream.Position = 0;   // возврат курсора в начало файла
                reader.DiscardBufferedData();     // очищаем буфер StreamReader, иначе прочитаем старые данные

                students = new List<StudentRating>(count);
                while (!reader.EndOfStream)
                {
                    string? name = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(name)) continue;
                    students.Add(new StudentRating() { FullName = name.Trim() });
                }
            }

            string ratingsPath = GetRatingsPath(discipline);
            if (!File.Exists(ratingsPath)) { return students; }

            using (StreamReader reader = new StreamReader(ratingsPath, _enc))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] parts = line.Split(";");
                    if (parts.Length < 4) { continue; }

                    StudentRating? student = students.FirstOrDefault(s => s.FullName == parts[0].Trim());
                    if (student == null) { continue; }

                    student.Kt1 = int.TryParse(parts[1], out int kt1) ? kt1 : 0;
                    student.Kt2 = int.TryParse(parts[2], out int kt2) ? kt2 : 0;
                    student.Kt3 = int.TryParse(parts[3], out int kt3) ? kt3 : 0;
                }
            }
            return students;
        }

        /// <summary>
        /// Записывает рейтинг в ratings_[код].txt: ФИО;КТ1;КТ2;КТ3 (файл перезаписывается)
        /// </summary>
        public void SaveRatings(Discipline discipline, IEnumerable<StudentRating> students)
        {
            Directory.CreateDirectory(_dataFolder);

            using (StreamWriter writer = new StreamWriter(GetRatingsPath(discipline), false, _enc))
            {
                foreach (StudentRating s in students)
                    writer.WriteLine($"{s.FullName};{s.Kt1};{s.Kt2};{s.Kt3}");
            }
        }

        private string GetRatingsPath(Discipline discipline) =>
            Path.Combine(_dataFolder, $"ratings_{discipline.Code}.txt");
    }
}

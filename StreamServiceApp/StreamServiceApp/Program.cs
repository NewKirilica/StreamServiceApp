using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;  
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}: Начало работы");

        // Создае студентов
        var students = CreateStudents(14);

        // Выводим информацию о студентах и их баллах
        Console.WriteLine("Список студентов:");
        foreach (var student in students)
        {
            Console.WriteLine($"ID: {student.Id}, Имя: {student.Name}, Средний балл: {student.AverageGrade:F2}");
        }

        var progress = new Progress<string>(message => Console.WriteLine(message));

        var streamService = new StreamService<Student>();

        // Используем MemoryStream
        using (var memoryStream = new MemoryStream())
        {
            // Запуск WriteToStreamAsync
            var writeTask = Task.Run(() => streamService.WriteToStreamAsync(memoryStream, students, progress));
            // Задержка перед запуском CopyFromStreamAsync
            await Task.Delay(150);  // Задержка от 100 до 200 мс

            // Запуск CopyFromStreamAsync
            var copyTask = Task.Run(() => streamService.CopyFromStreamAsync(memoryStream, "students.dat", progress));

            // Ожидание завершения записи и копирования
            await Task.WhenAll(writeTask, copyTask);
        }

        // Получаем статистику (количество студентов с средним баллом больше 9)
        var studentsList = students.AsParallel().Where(s => s.AverageGrade > 9).ToList();  // Преобразуем в List и фильтруем
        var statistics = studentsList.Count();  //  работает

        Console.WriteLine($"Статистика: количество студентов с баллом больше 9: {statistics}");
    }

    // Метод для создания коллекции студентов
    static List<Student> CreateStudents(int count)
    {
        var students = new List<Student>();
        var random = new Random();

        for (int i = 0; i < count; i++)
        {
            students.Add(new Student(
                i,
                $"Student {i}",
                random.NextDouble() * 10  // Генерируем случайный балл от 0 до 10
            ));
        }

        return students;
    }
}

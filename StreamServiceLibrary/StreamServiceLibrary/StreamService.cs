using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class StreamService<T>
{
    private static readonly object _syncLock = new object();

    public async Task WriteToStreamAsync(Stream stream, IEnumerable<T> data, IProgress<string> progress)
    {
        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}: Начало записи");

        var totalItems = 0;
        foreach (var item in data)
        {
            var itemBytes = System.Text.Encoding.UTF8.GetBytes(item?.ToString() ?? string.Empty);

            await stream.WriteAsync(itemBytes, 0, itemBytes.Length);

            totalItems++;
            progress?.Report($"Записано {totalItems} элементов");

            await Task.Delay(300);  // Задержка для медленной записи
        }

        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}: Конец записи");
    }

    public async Task CopyFromStreamAsync(Stream stream, string filename, IProgress<string> progress)
    {
        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}: Начало копирования");

        using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
        {
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(fileStream);
        }

        Console.WriteLine($"Поток {Thread.CurrentThread.ManagedThreadId}: Конец копирования");
    }

    public async Task<int> GetStatisticsAsync(string fileName, Func<T, bool> filter)
    {
        var count = 0;
        var items = await ReadItemsFromFileAsync(fileName);

        foreach (var item in items)
        {
            if (filter(item))
            {
                count++;
            }
        }

        return count;
    }

    private async Task<List<T>> ReadItemsFromFileAsync(string fileName)
    {
        var items = new List<T>();
        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                // Преобразуем строку в объект типа T (Student)
                var item = ParseStudent(line);
                if (item != null)
                {
                    items.Add((T)item);
                }
            }
        }
        return items;
    }

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Spectre.Console;

namespace ExcelReaderApp.Controller;

internal static class Utilities
{
    static readonly string[] LoadingIcons = { " |", " /", "--", " \\", " *" };
    internal static void LoadingScreen(string text="loading...")
    {
        foreach(string icon in LoadingIcons)
        {
            System.Console.Write($"\r{text} {icon}");
            Thread.Sleep(120);
        }
        System.Console.Write($"\r{new String(' ', Console.BufferWidth)}");
    }

    internal static void HandleTask(Task task, string text, bool disposeTask=true)
    {
        do
        {
            LoadingScreen(text);
        }while(!task.IsCompleted);
        
        if (disposeTask) task.Dispose();
    }

    internal static void DisplayTable<T>(List<T> data, string[] headers)
    {
        System.Console.Clear();
        Spectre.Console.Table table = new();
        foreach (string header in headers)
            table.AddColumn(header);

        foreach(T item in data)
        {
            List<string> rows = new();
            foreach(string header in headers)
            {
                var property = typeof(T).GetProperty(header);
                rows.Add(property.GetValue(item).ToString() ?? "N/A");
            }
            table.AddRow(rows.ToArray());
        }

        AnsiConsole.Write(table);
        System.Console.WriteLine();
    }
}

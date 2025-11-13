using System;

namespace DocumentParser;

/// <summary>
/// Provides console output utilities with colored output, log levels, and progress indicators.
/// </summary>
public static class ConsoleHelper
{
    /// <summary>
    /// Defines the log levels for console output.
    /// </summary>
    public enum LogLevel
    {
        Info,
        Success,
        Warning,
        Error,
        Verbose
    }

    /// <summary>
    /// Gets or sets whether verbose mode is enabled.
    /// </summary>
    public static bool VerboseMode { get; set; } = false;

    /// <summary>
    /// Gets or sets whether quiet mode is enabled (suppresses all output except errors).
    /// </summary>
    public static bool QuietMode { get; set; } = false;

    /// <summary>
    /// Writes a formatted message to the console with appropriate color and prefix based on log level.
    /// </summary>
    /// <param name="level">The log level of the message.</param>
    /// <param name="message">The message format string.</param>
    /// <param name="args">Optional arguments for the message format string.</param>
    public static void WriteLine(LogLevel level, string message, params object[] args)
    {
        if (QuietMode && level != LogLevel.Error) return;
        if (!VerboseMode && level == LogLevel.Verbose) return;

        var formattedMessage = args.Length > 0 ? string.Format(message, args) : message;
        var originalColor = Console.ForegroundColor;

        Console.ForegroundColor = level switch
        {
            LogLevel.Success => ConsoleColor.Green,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Verbose => ConsoleColor.DarkGray,
            _ => ConsoleColor.White
        };

        var prefix = level switch
        {
            LogLevel.Success => "✓",
            LogLevel.Warning => "⚠",
            LogLevel.Error => "✗",
            LogLevel.Verbose => "→",
            _ => "ℹ"
        };

        Console.WriteLine($"{prefix} {formattedMessage}");
        Console.ForegroundColor = originalColor;
    }

    /// <summary>
    /// Writes a progress message to the console (overwrites the current line).
    /// </summary>
    /// <param name="message">The progress message to display.</param>
    public static void WriteProgress(string message)
    {
        if (QuietMode) return;
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write($"\r{message}");
        Console.ForegroundColor = originalColor;
    }

    /// <summary>
    /// Clears the current progress line.
    /// </summary>
    public static void ClearProgress()
    {
        if (QuietMode) return;
        Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
    }
}


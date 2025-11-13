namespace DocumentParser;

/// <summary>
/// Defines exit codes used by the application.
/// </summary>
public static class ExitCodes
{
    /// <summary>
    /// Exit code indicating successful execution.
    /// </summary>
    public const int Success = 0;

    /// <summary>
    /// Exit code indicating a license file error.
    /// </summary>
    public const int LicenseError = 1;

    /// <summary>
    /// Exit code indicating a parsing error.
    /// </summary>
    public const int ParsingError = 3;

    /// <summary>
    /// Exit code indicating an I/O error.
    /// </summary>
    public const int IoError = 4;
}


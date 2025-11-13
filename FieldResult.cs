namespace DocumentParser;

/// <summary>
/// Represents a field result extracted from a document.
/// </summary>
public class FieldResult
{
    /// <summary>
    /// Gets or sets the name of the field.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the page index where the field was found.
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// Gets or sets the extracted value of the field.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of the field (Text, Table, Barcode, etc.).
    /// </summary>
    public string Type { get; set; } = string.Empty;
}


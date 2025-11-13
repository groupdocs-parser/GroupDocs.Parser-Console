
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;

using GroupDocs.Parser;
using GroupDocs.Parser.Data;
using GroupDocs.Parser.Options;
using GroupDocs.Parser.Templates;
using DocumentParser;

try
{
    var config = new ConfigurationBuilder()
        .AddJsonFile("config.json", optional: true)
        .Build();

    string? licensePath = null;

    // First, check if LicensePath from config.json exists
    var configLicensePath = config["LicensePath"];
    if (!string.IsNullOrEmpty(configLicensePath) && File.Exists(configLicensePath))
    {
        licensePath = configLicensePath;
    }
    else
    {
        // If LicensePath doesn't exist, check for LicenseEnv environment variable
        var licenseEnv = config["LicenseEnv"];
        if (!string.IsNullOrEmpty(licenseEnv))
        {
            var envPath = Environment.GetEnvironmentVariable(licenseEnv);
            if (!string.IsNullOrEmpty(envPath))
            {
                // Use the environment variable path and append the license filename
                var envLicensePath = Path.Combine(envPath, "GroupDocs.Parser.NET.lic");
                if (File.Exists(envLicensePath))
                {
                    licensePath = envLicensePath;
                }
            }
        }
    }

    // Fallback to searching for .lic files in current directory
    if (licensePath == null)
    {
        licensePath = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.lic").FirstOrDefault();
    }

    if (licensePath == null || !File.Exists(licensePath))
    {
        ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Error, "License file not found.");
        ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Info, "Please ensure a license file exists in the current directory, configure 'LicensePath' in config.json, or set 'LicenseEnv' environment variable pointing to the directory containing GroupDocs.Parser.NET.lic");
        return ExitCodes.LicenseError;
    }

    ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Verbose, "Loading license from: {0}", licensePath);
    new License().SetLicense(licensePath);
    ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Success, "License loaded successfully");

    var rootCommand = new RootCommand("DocumentParser CLI - Extract structured data from documents using GroupDocs.Parser for .NET")
    {
        Name = "documentparser"
    };

    var documentOption = new Option<string>(["-i", "--input"], 
        description: "Path to the input document file (PDF, TIFF, etc.)")
    { 
        IsRequired = true 
    };
    documentOption.AddValidator(result =>
    {
        var value = result.GetValueOrDefault<string>();
        if (string.IsNullOrWhiteSpace(value))
        {
            result.ErrorMessage = "Input file path cannot be empty";
            return;
        }
        if (!File.Exists(value))
        {
            result.ErrorMessage = $"Input file not found: {value}";
        }
    });

    var templateOption = new Option<List<string>>(["-t", "--template"], 
        description: "Path(s) to template file(s) (XML format). Multiple templates can be specified.")
    { 
        IsRequired = true, 
        Arity = ArgumentArity.OneOrMore 
    };
    templateOption.AddValidator(result =>
    {
        var paths = result.GetValueOrDefault<List<string>>();
        if (paths == null || paths.Count == 0)
        {
            result.ErrorMessage = "At least one template file must be specified";
            return;
        }
        foreach (var path in paths)
        {
            if (!File.Exists(path))
            {
                result.ErrorMessage = $"Template file not found: {path}";
                return;
            }
        }
    });

    var outputOption = new Option<string>(["-o", "--output"], 
        description: "Path to the output file where extracted data will be written")
    { 
        IsRequired = true 
    };

    var pageIndexOption = new Option<int>(["-p", "--page"], 
        getDefaultValue: () => 0, 
        description: "Zero-based page index to parse (default: 0)")
    {
        Arity = ArgumentArity.ZeroOrOne
    };
    pageIndexOption.AddValidator(result =>
    {
        int value = result.GetValueOrDefault<int>();
        if (value < 0)
        {
            result.ErrorMessage = $"Page index must be non-negative, but is set to {value}";
        }
    });

    var useOcrOption = new Option<bool>("--ocr", 
        getDefaultValue: () => false, 
        description: "Enable OCR (Optical Character Recognition) for scanned documents")
    {
        Arity = ArgumentArity.ZeroOrOne
    };

    var dpiOption = new Option<int>("--dpi", 
        getDefaultValue: () => 288, 
        description: "DPI (dots per inch) for image rendering and OCR (default: 288, range: 1-10000)")
    {
        Arity = ArgumentArity.ZeroOrOne
    };
    dpiOption.AddValidator(result =>
    {
        int value = result.GetValueOrDefault<int>();
        if (value < 1 || value > 10000)
        {
            result.ErrorMessage = $"DPI must be in the range from 1 to 10000, but is set to {value}";
        }
    });

    var verboseOption = new Option<bool>(["-v", "--verbose"], 
        getDefaultValue: () => false, 
        description: "Enable verbose output with detailed progress information")
    {
        Arity = ArgumentArity.ZeroOrOne
    };

    var quietOption = new Option<bool>(["-q", "--quiet"], 
        getDefaultValue: () => false, 
        description: "Suppress all output except errors")
    {
        Arity = ArgumentArity.ZeroOrOne
    };

    var jsonOutputOption = new Option<bool>("--json", 
        getDefaultValue: () => false, 
        description: "Output results in JSON format instead of plain text")
    {
        Arity = ArgumentArity.ZeroOrOne
    };

    rootCommand.AddOption(documentOption);
    rootCommand.AddOption(templateOption);
    rootCommand.AddOption(outputOption);
    rootCommand.AddOption(pageIndexOption);
    rootCommand.AddOption(useOcrOption);
    rootCommand.AddOption(dpiOption);
    rootCommand.AddOption(verboseOption);
    rootCommand.AddOption(quietOption);
    rootCommand.AddOption(jsonOutputOption);

    rootCommand.SetHandler((InvocationContext context) =>
    {
        var documentPath = context.ParseResult.GetValueForOption(documentOption)!;
        var templatePaths = context.ParseResult.GetValueForOption(templateOption)!;
        var outputPath = context.ParseResult.GetValueForOption(outputOption)!;
        var pageIndex = context.ParseResult.GetValueForOption(pageIndexOption);
        var useOcr = context.ParseResult.GetValueForOption(useOcrOption);
        var dpi = context.ParseResult.GetValueForOption(dpiOption);
        var verbose = context.ParseResult.GetValueForOption(verboseOption);
        var quiet = context.ParseResult.GetValueForOption(quietOption);
        var jsonOutput = context.ParseResult.GetValueForOption(jsonOutputOption);

        ConsoleHelper.VerboseMode = verbose;
        ConsoleHelper.QuietMode = quiet;

        try
        {
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Info, "Starting document parsing...");
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Verbose, "Input document: {0}", documentPath);
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Verbose, "Templates: {0}", string.Join(", ", templatePaths));
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Verbose, "Output file: {0}", outputPath);
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Verbose, "Page index: {0}", pageIndex);
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Verbose, "OCR enabled: {0}", useOcr);
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Verbose, "DPI: {0}", dpi);

            var stopwatch = Stopwatch.StartNew();

            // Load templates
            ConsoleHelper.WriteProgress("Loading templates...");
            var templates = new TemplateCollection();
            foreach (var templatePath in templatePaths)
            {
                try
                {
                    var template = Template.Load(templatePath);
                    templates.Add(template);
                    ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Verbose, "Loaded template: {0} ({1} fields)", 
                        Path.GetFileName(templatePath), template.Count);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.ClearProgress();
                    ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Error, "Failed to load template '{0}': {1}", 
                        templatePath, ex.Message);
                    context.ExitCode = ExitCodes.ParsingError;
                    return;
                }
            }
            ConsoleHelper.ClearProgress();
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Success, "Loaded {0} template(s)", templates.Count);
            var templateLoadTime = stopwatch.Elapsed;

            // Initialize parser
            ConsoleHelper.WriteProgress("Initializing parser...");
            Parser parser;
            try
            {
                parser = new Parser(documentPath);
            }
            catch (Exception ex)
            {
                ConsoleHelper.ClearProgress();
                ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Error, "Failed to initialize parser: {0}", ex.Message);
                context.ExitCode = ExitCodes.ParsingError;
                return;
            }

            var ocrOptions = documentPath.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase)
                ? new OcrOptions(new PagePreviewOptions(dpi))
                : null;
            var options = new ParseByTemplateOptions(pageIndex, useOcr, ocrOptions);
            ConsoleHelper.ClearProgress();
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Success, "Parser initialized in {0:F2}s", 
                (stopwatch.Elapsed - templateLoadTime).TotalSeconds);

            // Parse document
            ConsoleHelper.WriteProgress("Parsing document...");
            DocumentData data;
            try
            {
                using (parser)
                {
                    data = parser.ParseByTemplate(templates, options);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.ClearProgress();
                ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Error, "Parsing failed: {0}", ex.Message);
                ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Verbose, "Exception details: {0}", ex);
                context.ExitCode = ExitCodes.ParsingError;
                return;
            }
            var parseTime = stopwatch.Elapsed - templateLoadTime;
            ConsoleHelper.ClearProgress();
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Success, "Document parsed in {0:F2}s", parseTime.TotalSeconds);

            // Process results
            ConsoleHelper.WriteProgress("Processing results...");
            var results = new List<FieldResult>();
            int fieldsFound = 0;
            int fieldsMatched = 0;

            foreach (var field in data)
            {
                fieldsFound++;
                var templateItem = data.Template.FirstOrDefault(f => f.Name == field.Name);
                if (templateItem != null && field.PageIndex == pageIndex)
                {
                    fieldsMatched++;
                    var result = new FieldResult
                    {
                        Name = field.Name,
                        PageIndex = field.PageIndex,
                        Value = field.PageArea switch
                        {
                            PageTextArea text => text.Text,
                            PageTableArea table => string.Join('\t', table.Cells.Select(c => c.Text)),
                            PageBarcodeArea barcode => barcode.Value,
                            _ => string.Empty
                        },
                        Type = field.PageArea switch
                        {
                            PageTextArea => "Text",
                            PageTableArea => "Table",
                            PageBarcodeArea => "Barcode",
                            _ => "Unknown"
                        }
                    };
                    results.Add(result);
                }
            }
            ConsoleHelper.ClearProgress();

            // Generate output
            string outputContent;
            if (jsonOutput)
            {
                var jsonResults = new
                {
                    document = Path.GetFileName(documentPath),
                    pageIndex = pageIndex,
                    timestamp = DateTime.UtcNow,
                    statistics = new
                    {
                        totalFieldsFound = fieldsFound,
                        fieldsMatched = fieldsMatched,
                        parseTimeSeconds = parseTime.TotalSeconds
                    },
                    fields = results.Select(r => new { r.Name, r.Type, r.Value })
                };
                outputContent = JsonSerializer.Serialize(jsonResults, new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                });
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("=".PadRight(60, '='));
                sb.AppendLine($"Document: {Path.GetFileName(documentPath)}");
                sb.AppendLine($"Page: {pageIndex + 1}");
                sb.AppendLine($"Parsed: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine("=".PadRight(60, '='));
                sb.AppendLine();

                if (results.Count == 0)
                {
                    sb.AppendLine("No fields matched on the specified page.");
                }
                else
                {
                    foreach (var result in results)
                    {
                        sb.AppendLine();
                        sb.AppendLine($"Field: {result.Name} ({result.Type})");
                        sb.AppendLine("-".PadRight(40, '-'));
                        sb.AppendLine(result.Value);
                    }
                }

                sb.AppendLine();
                sb.AppendLine("=".PadRight(60, '='));
                sb.AppendLine($"Statistics: {fieldsMatched} of {fieldsFound} fields matched");
                sb.AppendLine($"Parse time: {parseTime.TotalSeconds:F2} seconds");
                outputContent = sb.ToString();
            }

            // Write output
            try
            {
                var outputDir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                File.WriteAllText(outputPath, outputContent);
                ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Success, "Results written to: {0}", outputPath);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Error, "Failed to write output file: {0}", ex.Message);
                context.ExitCode = ExitCodes.IoError;
                return;
            }

            stopwatch.Stop();

            // Summary
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Info, "");
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Success, "Parsing completed successfully!");
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Info, "  Fields matched: {0} of {1}", fieldsMatched, fieldsFound);
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Info, "  Total time: {0:F2}s", stopwatch.Elapsed.TotalSeconds);
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Info, "  Output format: {0}", jsonOutput ? "JSON" : "Text");

            context.ExitCode = ExitCodes.Success;
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Error, "Unexpected error: {0}", ex.Message);
            ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Verbose, "Stack trace: {0}", ex.StackTrace);
            context.ExitCode = ExitCodes.ParsingError;
        }
    });

    var exitCode = await rootCommand.InvokeAsync(args);
    return exitCode;
}
catch (Exception ex)
{
    ConsoleHelper.WriteLine(ConsoleHelper.LogLevel.Error, "Fatal error: {0}", ex.Message);
    return ExitCodes.ParsingError;
}

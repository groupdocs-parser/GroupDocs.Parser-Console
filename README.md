# GroupDocs.Parser Console - Command Line Document Parser

[![Product Page](https://img.shields.io/badge/Product%20Page-2865E0?style=for-the-badge&logo=appveyor&logoColor=white)](https://products.groupdocs.com/parser/net/) 
[![Docs](https://img.shields.io/badge/Docs-2865E0?style=for-the-badge&logo=Hugo&logoColor=white)](https://docs.groupdocs.com/parser/net/) 
[![Demos](https://img.shields.io/badge/Demos-2865E0?style=for-the-badge&logo=appveyor&logoColor=white)](https://products.groupdocs.app/parser/total) 
[![API](https://img.shields.io/badge/API%20Reference-2865E0?style=for-the-badge&logo=html5&logoColor=white)](https://reference.groupdocs.com/parser/net/) 
[![Blog](https://img.shields.io/badge/Blog-2865E0?style=for-the-badge&logo=WordPress&logoColor=white)](https://blog.groupdocs.com/category/parser/) 
[![Support](https://img.shields.io/badge/Free%20Support-2865E0?style=for-the-badge&logo=Discourse&logoColor=white)](https://forum.groupdocs.com/c/parser) 
[![Temp License](https://img.shields.io/badge/Temporary%20License-2865E0?style=for-the-badge&logo=rocket&logoColor=white)](https://purchase.groupdocs.com/temp-license/100308)


A powerful **command-line tool** for extracting structured data from documents using [GroupDocs.Parser for .NET](https://products.groupdocs.com/parser/net/). Parse text, parse tables, parse barcodes, and parse images from PDFs, TIFFs, and other document formats using XML-based templates.

---

## 📖 Overview

**GroupDocs.Parser Console** is a cross-platform CLI application that enables automated document parsing and data extraction. It supports:

- **Parse text** from documents using template-based extraction
- **Parse tables** with structured data extraction
- **Parse barcodes** from documents and images
- **Parse images** with OCR support for scanned documents
- Batch processing capabilities
- JSON and text output formats
- Verbose logging and progress indicators

Perfect for automation scripts, CI/CD pipelines, and server-side document processing.

---

## ⚙️ Features

### Core Parsing Capabilities

- **Parse Text** – Extract text fields from documents using visual templates
- **Parse Tables** – Extract structured table data with cell-level precision
- **Parse Barcodes** – Recognize and extract barcode values (QR codes, Code128, etc.)
- **Parse Images** – Process scanned documents and images with OCR support

### Advanced Features

- **Template-Based Extraction** – Use XML templates to define extraction regions
- **Multi-Template Support** – Apply multiple templates to a single document
- **OCR Integration** – Enable OCR for scanned PDFs and TIFF images
- **Page-Specific Parsing** – Target specific pages for extraction
- **Flexible Output** – Generate results in JSON or human-readable text format
- **Progress Indicators** – Real-time progress feedback during parsing
- **Verbose Logging** – Detailed logging for debugging and monitoring
- **Error Handling** – Comprehensive error reporting with exit codes

### Supported Document Formats

- PDF (text-based and scanned)
- TIFF images
- Other formats supported by GroupDocs.Parser

---

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Valid GroupDocs.Parser license file

### Installation

#### Option 1: Build from Source

```bash
git clone https://github.com/groupdocs-parser/groupdocs-parser-console.git
cd groupdocs-parser-console
dotnet build
```

#### Option 2: Use Precompiled Binary

Download the latest release from the [Releases](https://github.com/groupdocs-parser/groupdocs-parser-console/releases) page.

### Set License

The application supports multiple ways to configure the license file:

#### Option 1: Direct Path in config.json

Create or update `config.json` with your GroupDocs.Parser license path:

```json
{
  "LicensePath": "D:\\Licenses\\GroupDocs.Parser.NET.lic"
}
```

#### Option 2: Environment Variable via config.json

If `LicensePath` doesn't exist, the application will check for `LicenseEnv` in `config.json` and use it as an environment variable name that points to the directory containing `GroupDocs.Parser.NET.lic`:

```json
{
  "LicenseEnv": "LIC_PATH"
}
```

Then set the environment variable to the directory path:
```bash
# Windows (PowerShell)
$env:LIC_PATH = "D:\Licenses"

# Windows (CMD)
set LIC_PATH=D:\Licenses

# Linux/Mac
export LIC_PATH=/path/to/licenses
```

The application will look for `GroupDocs.Parser.NET.lic` in the directory specified by the environment variable.

#### Option 3: Place License in Application Directory

Alternatively, place a `.lic` file in the application directory.

👉 **Don't have a license?** Request a free temporary license:  
[Get Temporary License](https://purchase.groupdocs.com/temporary-license/)

---

## 📖 Usage

### Basic Syntax

```bash
documentparser -i <input-file> -t <template-file> -o <output-file> [options]
```

### Command Options

| Option | Short | Required | Description |
|--------|-------|----------|-------------|
| `--input` | `-i` | Yes | Path to the input document file (PDF, TIFF, etc.) |
| `--template` | `-t` | Yes | Path(s) to template file(s) (XML format). Multiple templates can be specified. |
| `--output` | `-o` | Yes | Path to the output file where extracted data will be written |
| `--page` | `-p` | No | Zero-based page index to parse (default: 0) |
| `--ocr` | | No | Enable OCR (Optical Character Recognition) for scanned documents |
| `--dpi` | | No | DPI for image rendering and OCR (default: 288, range: 1-10000) |
| `--verbose` | `-v` | No | Enable verbose output with detailed progress information |
| `--quiet` | `-q` | No | Suppress all output except errors |
| `--json` | | No | Output results in JSON format instead of plain text |

---

## 💡 Examples

### Example 1: Parse Text from a PDF Document

Extract text fields from the first page of a PDF:

```bash
documentparser -i invoice.pdf -t invoice-template.xml -o output.txt
```

**Output:**
```
ℹ Starting document parsing...
✓ License loaded successfully
✓ Loaded 1 template(s)
✓ Parser initialized in 0.15s
✓ Document parsed in 1.23s
✓ Results written to: output.txt

✓ Parsing completed successfully!
  Fields matched: 5 of 5
  Total time: 1.45s
  Output format: Text
```

**Output File Content:**
```
============================================================
Document: invoice.pdf
Page: 1
Parsed: 2024-01-15 14:30:25
============================================================

Field: InvoiceNumber (Text)
----------------------------------------
INV-2024-001

Field: Date (Text)
----------------------------------------
2024-01-15

Field: Total (Text)
----------------------------------------
$1,250.00

Field: CustomerName (Text)
----------------------------------------
Acme Corporation

Field: Tax (Text)
----------------------------------------
$125.00

============================================================
Statistics: 5 of 5 fields matched
Parse time: 1.23 seconds
```

### Example 2: Parse Tables with Multiple Templates

Extract data using multiple templates:

```bash
documentparser -i report.pdf -t header-template.xml -t table-template.xml -o results.txt -p 0
```

**Output:**
```
ℹ Starting document parsing...
✓ License loaded successfully
→ Loaded template: header-template.xml (3 fields)
→ Loaded template: table-template.xml (1 fields)
✓ Loaded 2 template(s)
✓ Parser initialized in 0.18s
✓ Document parsed in 2.45s
✓ Results written to: results.txt
```

### Example 3: Parse Barcodes from Scanned Document

Extract barcodes from a scanned PDF with OCR enabled:

```bash
documentparser -i scanned-invoice.pdf -t barcode-template.xml -o barcodes.txt --ocr --dpi 300
```

**Output:**
```
ℹ Starting document parsing...
✓ License loaded successfully
✓ Loaded 1 template(s)
✓ Parser initialized in 0.22s
✓ Document parsed in 3.67s
✓ Results written to: barcodes.txt

✓ Parsing completed successfully!
  Fields matched: 2 of 2
  Total time: 3.92s
  Output format: Text
```

**Output File Content:**
```
============================================================
Document: scanned-invoice.pdf
Page: 1
Parsed: 2024-01-15 14:35:10
============================================================

Field: QRCode (Barcode)
----------------------------------------
https://example.com/invoice/12345

Field: ProductBarcode (Barcode)
----------------------------------------
1234567890123

============================================================
Statistics: 2 of 2 fields matched
Parse time: 3.67 seconds
```

### Example 4: Parse Images with OCR

Process a scanned TIFF image with OCR:

```bash
documentparser -i document.tiff -t text-template.xml -o extracted.txt --ocr --dpi 288
```

### Example 5: Verbose Mode

Enable detailed logging:

```bash
documentparser -i document.pdf -t template.xml -o output.txt --verbose
```

**Output:**
```
ℹ Starting document parsing...
→ Input document: C:\Documents\invoice.pdf
→ Templates: C:\Templates\invoice-template.xml
→ Output file: C:\Output\output.txt
→ Page index: 0
→ OCR enabled: False
→ DPI: 288
✓ License loaded successfully
→ Loaded template: invoice-template.xml (5 fields)
✓ Loaded 1 template(s)
✓ Parser initialized in 0.15s
✓ Document parsed in 1.23s
✓ Results written to: C:\Output\output.txt

✓ Parsing completed successfully!
  Fields matched: 5 of 5
  Total time: 1.45s
  Output format: Text
```

### Example 6: Quiet Mode

Suppress all output except errors (useful for scripts):

```bash
documentparser -i document.pdf -t template.xml -o output.txt --quiet
```

### Example 8: Parse Specific Page

Extract data from page 2 (zero-based index):

```bash
documentparser -i multi-page.pdf -t template.xml -o page2.txt -p 1
```

### Example 9: High-Resolution OCR

Process scanned document with high DPI for better OCR accuracy:

```bash
documentparser -i scanned.pdf -t template.xml -o output.txt --ocr --dpi 600
```

### Example 10: Parse Tables

Extract table data from a document:

```bash
documentparser -i report.pdf -t table-template.xml -o table-data.txt
```

**Output File Content:**
```
============================================================
Document: report.pdf
Page: 1
Parsed: 2024-01-15 14:40:00
============================================================

Field: ProductTable (Table)
----------------------------------------
Product Name	Quantity	Price	Total
Widget A	10	$5.00	$50.00
Widget B	5	$10.00	$50.00
Widget C	3	$15.00	$45.00

============================================================
Statistics: 1 of 1 fields matched
Parse time: 0.87 seconds
```

---

## 📋 Exit Codes

The application returns the following exit codes:

| Code | Description |
|------|-------------|
| `0` | Success |
| `1` | License file error |
| `3` | Parsing error |
| `4` | I/O error (file read/write) |

Use these codes in automation scripts to handle errors appropriately.

---

## 🎯 Use Cases

### Parse Text Use Cases

- Extract invoice numbers, dates, and customer information
- Parse form data from PDFs
- Extract metadata from documents
- Automated data entry from scanned forms

### Parse Tables Use Cases

- Extract financial data from reports
- Parse product catalogs
- Extract tabular data for database import
- Process structured reports automatically

### Parse Barcodes Use Cases

- Extract QR codes from documents
- Read product barcodes from invoices
- Process shipping labels
- Extract tracking numbers

### Parse Images Use Cases

- OCR text from scanned documents
- Extract data from image-based forms
- Process TIFF files with text recognition
- Convert scanned documents to structured data

---

## 🛠 Template Creation

Templates define the regions where data should be extracted. Create templates using:

1. **GroupDocs.Parser GUI** – Visual template editor (see [README-GUI.md](README-GUI.md))
2. **Manual XML Creation** – Define templates in XML format

### Template Structure

Templates are XML files that define:
- Field positions and sizes
- Field types (Text, Table, Barcode)
- Field names for extracted data

Example template structure:
```xml
<Template>
  <Field Name="InvoiceNumber" Rectangle="100,100,200,120" />
  <Field Name="Date" Rectangle="100,130,200,150" />
  <Table Name="Items" Rectangle="50,200,500,400" />
  <Barcode Name="QRCode" Rectangle="400,100,500,200" />
</Template>
```

---

## 📌 Limitations

### Supported Documents
- PDFs with text
- Scanned PDFs & TIFF images (with OCR enabled)

### Supported Field Types
- **Text field** – Extract text from specified regions
- **Table field** – Extract structured table data
- **Barcode field** – Extract barcode values

### Template Scope
- Templates work **per page** (can be reused across pages with the same structure)

---

## 🔧 Troubleshooting

### License File Not Found

**Error:**
```
✗ License file not found.
ℹ Please ensure a license file exists in the current directory or configure 'LicensePath' in config.json
```

**Solution:**
1. Place a `.lic` file in the application directory, or
2. Update `config.json` with the correct `LicensePath`, or
3. Configure `LicenseEnv` in `config.json` and set the corresponding environment variable to point to the directory containing `GroupDocs.Parser.NET.lic`

### Template File Not Found

**Error:**
```
✗ Template file not found: template.xml
```

**Solution:**
- Verify the template file path is correct
- Use absolute paths if relative paths fail
- Check file permissions

### Parsing Failed

**Error:**
```
✗ Parsing failed: [error message]
```

**Solution:**
- Enable verbose mode (`--verbose`) for detailed error information
- Verify the document format is supported
- Check template compatibility with the document structure
- For scanned documents, enable OCR with `--ocr`

### Low OCR Accuracy

**Solution:**
- Increase DPI setting: `--dpi 600`
- Ensure good image quality
- Use appropriate DPI for the document type (300-600 recommended)

---

## 🤝 Contributing

This project is open-source. We welcome contributions!

- Suggest new features
- Submit pull requests
- Report issues
- Improve documentation

---

## 📚 Additional Resources

- [GroupDocs.Parser Product Page](https://products.groupdocs.com/parser/net/)
- [Documentation](https://docs.groupdocs.com/parser/net/)
- [API Reference](https://reference.groupdocs.com/parser/net/)
- [Live Demos](https://products.groupdocs.app/parser/total)
- [Forum Support](https://forum.groupdocs.com/c/parser)
- [Blog](https://blog.groupdocs.com/category/parser/)

---

## 📜 License

This tool is provided for **customer convenience** under open-source terms.  
For core parsing functionality, a [GroupDocs.Parser for .NET](https://products.groupdocs.com/parser/net/) license is required.

---

## 🔮 Roadmap

- Automatic detection of scanned vs text-based documents
- Enhanced table parsing support
- Batch processing multiple documents
- Template validation and preview
- Performance optimizations

---

**Keywords:** Parse text, parse tables, parse barcodes, parse images, document parser, PDF parser, OCR, data extraction, template-based parsing, command-line parser, GroupDocs.Parser


using System.Globalization;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace DustInTheWind.PeerBerry.Toolkit.Xlsx;

internal class XlsxTransactionsDocument : IDisposable
{
	private readonly Stream stream;
	private readonly SpreadsheetDocument spreadsheetDocument;

	public XlsxTransactionsDocument(Stream stream)
	{
		this.stream = stream ?? throw new ArgumentNullException(nameof(stream));

		spreadsheetDocument = SpreadsheetDocument.Open(stream, isEditable: false);
	}

	public TransactionsSection GetTransactionsSection()
	{
		if (spreadsheetDocument.WorkbookPart == null)
			throw new DocumentLoadException("The spreadsheet document has no workbook part.");

		try
		{
			string[] sharedStrings = LoadSharedStrings(spreadsheetDocument.WorkbookPart);

			WorksheetPart worksheetPart = GetTransactionsWorksheetPart();
			return ParseTransactionsSection(worksheetPart.Worksheet, sharedStrings);
		}
		catch (DocumentLoadException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException(ex);
		}
	}

	private static string[] LoadSharedStrings(WorkbookPart workbookPart)
	{
		SharedStringTablePart sharedStringTablePart = workbookPart.SharedStringTablePart;

		if (sharedStringTablePart == null)
			return [];

		return sharedStringTablePart.SharedStringTable
			.Elements<SharedStringItem>()
			.Select(x => x.InnerText)
			.ToArray();
	}

	private WorksheetPart GetTransactionsWorksheetPart()
	{
		WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
		Regex regex = new Regex(@"^Investor\s(\d+)\stransactions$", RegexOptions.IgnoreCase);

		Sheet sheet = workbookPart.Workbook?.Sheets?.Elements<Sheet>()
			.FirstOrDefault(x => x.Name?.Value != null && regex.IsMatch(x.Name.Value));

		if (sheet == null)
			throw new DocumentLoadException("Transactions sheet not found in the spreadsheet document.");

		return (WorksheetPart)workbookPart.GetPartById(sheet.Id!);
	}

	private static TransactionsSection ParseTransactionsSection(Worksheet worksheet, string[] sharedStrings)
	{
		TransactionsSection section = new();
		
		SheetData sheetData = worksheet.GetFirstChild<SheetData>()
			?? throw new DocumentLoadException("The Transactions sheet contains no data.");
		
		IEnumerable<Row> rows = sheetData.Elements<Row>().Skip(1);

		foreach (Row row in rows)
		{
			if (row.Elements<Cell>().All(x => string.IsNullOrEmpty(x.InnerText)))
				continue;

			TransactionRecord transactionRecord = new()
			{
				Id = GetStringValue(FindCell(row, "A"), sharedStrings),
				Date = GetDateValue(FindCell(row, "B")),
				Type = GetStringValue(FindCell(row, "C"), sharedStrings),
				Amount = GetDecimalValue(FindCell(row, "D")),
				Currency = GetStringValue(FindCell(row, "E"), sharedStrings),
				LoanId = GetStringValue(FindCell(row, "F"), sharedStrings),
				Country = GetStringValue(FindCell(row, "G"), sharedStrings),
				LoanStatus = GetStringValue(FindCell(row, "H"), sharedStrings)
			};

			section.Transactions.Add(transactionRecord);
		}
		
		return section;
	}

	private static Cell FindCell(Row row, string column)
	{
		string cellReference = column + row.RowIndex;

		return row.Elements<Cell>()
			.FirstOrDefault(x => x.CellReference?.Value == cellReference);
	}

	private static string GetStringValue(Cell cell, string[] sharedStrings)
	{
		if (cell == null || string.IsNullOrEmpty(cell.InnerText))
			return null;

		if (cell.DataType?.Value == CellValues.SharedString)
		{
			int index = int.Parse(cell.InnerText);
			return index < sharedStrings.Length ? sharedStrings[index] : null;
		}

		return cell.InnerText;
	}

	private static DateTime GetDateValue(Cell cell)
	{
		if (cell == null || string.IsNullOrEmpty(cell.InnerText))
			return default;

		double oaDate = double.Parse(cell.InnerText, CultureInfo.InvariantCulture);
		return DateTime.FromOADate(oaDate);
	}

	private static decimal GetDecimalValue(Cell cell)
	{
		if (cell == null || string.IsNullOrEmpty(cell.InnerText))
			return 0m;

		return decimal.Parse(cell.InnerText, CultureInfo.InvariantCulture);
	}

	public void Dispose()
	{
		spreadsheetDocument?.Dispose();
		stream?.Dispose();
	}
}
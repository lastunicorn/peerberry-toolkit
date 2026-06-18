using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace DustInTheWind.PeerBerry.Toolkit.Xlsx;

internal class XlsxTransactionsDocument : IDisposable
{
	private readonly Stream stream;
	private readonly SpreadsheetDocument spreadsheetDocument;
	private readonly Lazy<string[]> sharedStrings;

	public XlsxTransactionsDocument(Stream stream)
	{
		this.stream = stream ?? throw new ArgumentNullException(nameof(stream));

		spreadsheetDocument = SpreadsheetDocument.Open(stream, isEditable: false);
		sharedStrings = new Lazy<string[]>(LoadSharedStrings);
	}

	public XlsxTransactionsSheet GetTransactionsSheet()
	{
		if (spreadsheetDocument.WorkbookPart == null)
			throw new DocumentLoadException("The spreadsheet document has no workbook part.");

		try
		{
			WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
			Regex regex = new Regex(@"^Investor\s(\d+)\stransactions$", RegexOptions.IgnoreCase);

			IEnumerable<Sheet> sheets = workbookPart.Workbook?.Sheets?.Elements<Sheet>()
				.Where(x => x.Name?.Value != null);

			foreach (Sheet sheet in sheets)
			{
				Match match = regex.Match(sheet.Name.Value);

				if (match.Success)
				{
					WorksheetPart worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id!);
					string investorId = match.Groups[1].Value;
					return new XlsxTransactionsSheet(worksheetPart.Worksheet, investorId, sharedStrings.Value);
				}
			}

			throw new DocumentLoadException("Transactions sheet not found in the spreadsheet document.");
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

	private string[] LoadSharedStrings()
	{
		SharedStringTablePart sharedStringTablePart = spreadsheetDocument.WorkbookPart?.SharedStringTablePart;

		if (sharedStringTablePart == null)
			return [];

		return sharedStringTablePart.SharedStringTable
			.Elements<SharedStringItem>()
			.Select(x => x.InnerText)
			.ToArray();
	}

	public void Dispose()
	{
		spreadsheetDocument?.Dispose();
		stream?.Dispose();
	}
}
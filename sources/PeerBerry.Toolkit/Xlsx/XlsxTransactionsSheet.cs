using System.Collections;
using System.Globalization;
using DocumentFormat.OpenXml.Spreadsheet;

namespace DustInTheWind.PeerBerry.Toolkit.Xlsx;

internal class XlsxTransactionsSheet : IEnumerable<TransactionRecord>
{
	private readonly Worksheet worksheet;
	private readonly string[] sharedStrings;

	public string InvestorId { get; }

	public XlsxTransactionsSheet(Worksheet worksheet, string investorId, string[] sharedStrings)
	{
		this.worksheet = worksheet ?? throw new ArgumentNullException(nameof(worksheet));

		InvestorId = investorId ?? throw new ArgumentNullException(nameof(investorId));
		this.sharedStrings = sharedStrings ?? throw new ArgumentNullException(nameof(sharedStrings));
	}

	public IEnumerator<TransactionRecord> GetEnumerator()
	{
		SheetData sheetData = worksheet.GetFirstChild<SheetData>()
			?? throw new DocumentLoadException("The Transactions sheet contains no data.");

		foreach (Row row in sheetData.Elements<Row>().Skip(1))
		{
			if (row.Elements<Cell>().All(x => string.IsNullOrEmpty(x.InnerText)))
				continue;

			TransactionRecord transactionRecord = new()
			{
				Id = GetStringValue(FindCell(row, "A")),
				Date = GetDateValue(FindCell(row, "B")),
				Type = GetStringValue(FindCell(row, "C")),
				Amount = GetDecimalValue(FindCell(row, "D")),
				Currency = GetStringValue(FindCell(row, "E")),
				LoanId = GetStringValue(FindCell(row, "F")),
				Country = GetStringValue(FindCell(row, "G")),
				LoanStatus = GetStringValue(FindCell(row, "H"))
			};

			yield return transactionRecord;
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	private static Cell FindCell(Row row, string column)
	{
		string cellReference = column + row.RowIndex;

		return row.Elements<Cell>()
			.FirstOrDefault(x => x.CellReference?.Value == cellReference);
	}

	private string GetStringValue(Cell cell)
	{
		if (cell == null || string.IsNullOrEmpty(cell.InnerText))
			return null;

		if (cell.DataType?.Value == CellValues.SharedString)
		{
			int index = int.Parse(cell.InnerText);

			return index < sharedStrings.Length
				? sharedStrings[index]
				: null;
		}

		return cell.InnerText;
	}

	private DateTime GetDateValue(Cell cell)
	{
		if (cell == null || string.IsNullOrEmpty(cell.InnerText))
			return default;

		if (cell.DataType?.Value == CellValues.SharedString)
		{
			int index = int.Parse(cell.InnerText);
			string text = index < sharedStrings.Length
				? sharedStrings[index]
				: null;

			return text == null
				? default
				: DateTime.ParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
		}

		double oaDate = double.Parse(cell.InnerText, CultureInfo.InvariantCulture);
		return DateTime.FromOADate(oaDate);
	}

	private static decimal GetDecimalValue(Cell cell)
	{
		if (cell == null || string.IsNullOrEmpty(cell.InnerText))
			return 0m;

		return decimal.Parse(cell.InnerText, CultureInfo.InvariantCulture);
	}
}
using System.Globalization;
using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.PeerBerry.Toolkit.Demo;

internal static class Program
{
	public static async Task Main(string[] args)
	{
		const string fileName = "transactions.xlsx";

		try
		{
			TransactionsDocument document = await TransactionsDocument.LoadFromFileAsync(fileName);

			Display(document.TransactionsSection);
		}
		catch (DocumentLoadException ex)
		{
			await Console.Error.WriteLineAsync($"Failed to read '{fileName}': {ex.Message}");
			Environment.ExitCode = 1;
		}
		catch (Exception ex)
		{
			await Console.Error.WriteLineAsync($"Unexpected error: {ex.Message}");
			Environment.ExitCode = 1;
		}
	}

	private static void Display(TransactionsSection document)
	{
		DataGrid dataGrid = new()
		{
			Title = new[]
			{
				$"Transactions for investor {document.InvestorId}"
			},
			BorderTemplate = BorderTemplate.PlusMinusBorderTemplate,
			Footer = new[]
			{
				$"Count: {document.Transactions.Count}"
			}
		};

		dataGrid.Columns.Add("Id");
		dataGrid.Columns.Add("Date");
		dataGrid.Columns.Add("Type");
		dataGrid.Columns.Add("Amount", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Currency");
		dataGrid.Columns.Add("Loan ID");
		dataGrid.Columns.Add("Country");
		dataGrid.Columns.Add("Loan Status");

		foreach (TransactionRecord transactionRecord in document.Transactions)
			dataGrid.Rows.Add(
				transactionRecord.Id,
				transactionRecord.Date.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
				transactionRecord.Type,
				transactionRecord.Amount.ToString(CultureInfo.CurrentCulture),
				transactionRecord.Currency,
				transactionRecord.LoanId,
				transactionRecord.Country,
				transactionRecord.LoanStatus);

		dataGrid.Display();
	}
}
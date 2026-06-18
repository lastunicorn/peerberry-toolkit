using DustInTheWind.PeerBerry.Toolkit.Xlsx;

namespace DustInTheWind.PeerBerry.Toolkit;

/// <summary>
/// Represents an XTB report document parsed from an .xlsx export file.
/// </summary>
public class TransactionsDocument
{
	public TransactionsSection TransactionsSection { get; set; }

	/// <summary>
	/// Loads a report document from a file path.
	/// </summary>
	public static async Task<TransactionsDocument> LoadFromFileAsync(string filePath, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		try
		{
			await using Stream stream = File.OpenRead(filePath);
			return await LoadInternalAsync(stream, cancellationToken);
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

	/// <summary>
	/// Loads a report document from a stream.
	/// </summary>
	public static async Task<TransactionsDocument> LoadAsync(Stream stream, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(stream);

		try
		{
			return await LoadInternalAsync(stream, cancellationToken);
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

	/// <summary>Loads a report document from a <see cref="FileInfo"/>.</summary>
	public static async Task<TransactionsDocument> LoadAsync(FileInfo fileInfo, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(fileInfo);

		try
		{
			await using Stream stream = fileInfo.OpenRead();
			return await LoadInternalAsync(stream, cancellationToken);
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

	private static Task<TransactionsDocument> LoadInternalAsync(Stream stream, CancellationToken cancellationToken)
	{
		try
		{
			TransactionsDocument transactionsDocument = new();

			using XlsxTransactionsDocument xlsxTransactionsDocument = new(stream);
			cancellationToken.ThrowIfCancellationRequested();

			XlsxTransactionsSheet transactionsSheet = xlsxTransactionsDocument.GetTransactionsSheet();
			TransactionsSection section = new()
			{
				InvestorId = transactionsSheet.InvestorId
			};

			foreach (TransactionRecord transactionRecord in transactionsSheet)
				section.Transactions.Add(transactionRecord);

			transactionsDocument.TransactionsSection = section;

			return Task.FromResult(transactionsDocument);
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
}
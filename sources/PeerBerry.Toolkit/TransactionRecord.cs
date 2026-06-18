using DustInTheWind.Mintos.Toolkit;

namespace DustInTheWind.PeerBerry.Toolkit;

public class TransactionRecord
{
	public string Id { get; set; }

	public DateTime Date { get; set; }

	public TransactionType Type { get; set; }

	public decimal Amount { get; set; }

	public Currency Currency { get; set; }

	public string LoanId { get; set; }

	public string Country { get; set; }

	public LoanStatus LoanStatus { get; set; }
}
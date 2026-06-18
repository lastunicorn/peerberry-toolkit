namespace DustInTheWind.PeerBerry.Toolkit;

public class TransactionsSection
{
	public string InvestorId { get; set; }

	public List<TransactionRecord> Transactions { get; } = [];
}
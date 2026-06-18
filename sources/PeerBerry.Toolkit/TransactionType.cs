namespace DustInTheWind.PeerBerry.Toolkit;

public sealed record class TransactionType
{
	public static readonly TransactionType Investment = new("INVESTMENT");
	public static readonly TransactionType RepaymentInterest = new("REPAYMENT_INTEREST");
	public static readonly TransactionType RepaymentPrincipal = new("REPAYMENT_PRINCIPAL");
	public static readonly TransactionType BuybackInterest = new("BUYBACK_INTEREST");
	public static readonly TransactionType BuybackPrincipal = new("BUYBACK_PRINCIPAL");
	public static readonly TransactionType Withdrawal = new("WITHDRAWAL");

	public static readonly IReadOnlyCollection<TransactionType> KnownValues =
	[
		Investment,
		RepaymentInterest,
		RepaymentPrincipal,
		BuybackInterest,
		BuybackPrincipal,
		Withdrawal
	];
	
	public string Value { get; }

	public TransactionType(string value)
	{
		Value = value ?? throw new ArgumentNullException(nameof(value));
	}

	public override string ToString()
	{
		return Value;
	}

	public static implicit operator TransactionType(string value)
	{
		return value == null
			? null
			: new TransactionType(value);
	}

	public static implicit operator string(TransactionType transactionType)
	{
		return transactionType?.Value;
	}
}
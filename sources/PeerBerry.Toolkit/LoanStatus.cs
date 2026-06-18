namespace DustInTheWind.PeerBerry.Toolkit;

public sealed record class LoanStatus
{
	public static readonly LoanStatus Current = new("CURRENT");
	public static readonly LoanStatus Finished = new("FINISHED");

	public static readonly IReadOnlyCollection<LoanStatus> KnownValues =
	[
		Current,
		Finished
	];

	public string Value { get; }

	public LoanStatus(string value)
	{
		Value = value ?? throw new ArgumentNullException(nameof(value));
	}

	public override string ToString()
	{
		return Value;
	}

	public static implicit operator LoanStatus(string value)
	{
		return value == null
			? null
			: new LoanStatus(value);
	}

	public static implicit operator string(LoanStatus loanStatus)
	{
		return loanStatus?.Value;
	}
}
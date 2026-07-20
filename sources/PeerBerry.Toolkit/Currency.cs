namespace DustInTheWind.PeerBerry.Toolkit;

/// <summary>
/// Value object that represents a transaction currency (ISO 4217 currency code).
/// </summary>
public sealed record class Currency
{
	public static readonly Currency RON = new("RON");
	public static readonly Currency EUR = new("EUR");
	public static readonly Currency USD = new("USD");
	public static readonly Currency GBP = new("GBP");

	public static readonly IReadOnlyCollection<Currency> KnownValues =
	[
		RON,
		EUR,
		USD,
		GBP
	];

	public string Value { get; }

	public Currency(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			throw new ArgumentNullException(nameof(value));

		if (value.Length != 3)
			throw new ArgumentException("Currency code must be exactly 3 characters long.", nameof(value));

		Value = value.ToUpperInvariant();
	}

	public override string ToString()
	{
		return Value;
	}

	public static implicit operator Currency(string value)
	{
		return value == null
			? null
			: new Currency(value);
	}

	public static implicit operator string(Currency currency)
	{
		return currency?.Value;
	}
}
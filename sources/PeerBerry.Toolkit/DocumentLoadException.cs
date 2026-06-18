namespace DustInTheWind.PeerBerry.Toolkit;

public class DocumentLoadException : Exception
{
	private static readonly string DefaultMessage = "Failed to load PeerBerry transactions document.";

	public DocumentLoadException()
		: base(DefaultMessage)
	{
	}

	public DocumentLoadException(Exception innerException)
		: base(DefaultMessage, innerException)
	{
	}


	public DocumentLoadException(string message)
		: base(message)
	{
	}

	public DocumentLoadException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
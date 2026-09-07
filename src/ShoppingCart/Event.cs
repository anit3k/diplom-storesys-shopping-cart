namespace ShoppingCart;

public record Event(long SequenceNumber, string EventName, object Content);

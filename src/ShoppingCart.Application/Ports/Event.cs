namespace ShoppingCart.Application.Ports;

public record Event(long SequenceNumber, string EventName, object Content);
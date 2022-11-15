namespace Answer.King.Api.RequestModels;

public record MakePayment
{
    public double Amount { get; init; }

    public long OrderId { get; init; }
}
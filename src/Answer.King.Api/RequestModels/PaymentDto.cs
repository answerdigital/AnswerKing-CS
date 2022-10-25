namespace Answer.King.Api.RequestModels;

public record MakePayment
{
    public double Amount { get; init; }

    public Guid OrderId { get; init; }
}
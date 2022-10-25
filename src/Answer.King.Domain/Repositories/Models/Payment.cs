namespace Answer.King.Domain.Repositories.Models;

public partial class Payment
{
    public Payment(Guid orderId, double amount, double orderTotal)
    {
        Domain.Guard.AgainstDefaultValue(nameof(orderId), orderId);
        Domain.Guard.AgainstNegativeValue(nameof(amount), amount);
        Domain.Guard.AgainstNegativeValue(nameof(orderTotal), orderTotal);
        Guard.AgainstUnderPayment(amount, orderTotal);

        this.Id = Guid.NewGuid();
        this.OrderId = orderId;
        this.Amount = amount;
        this.OrderTotal = orderTotal;
        this.Date = DateTime.UtcNow;
    }

    private Payment(Guid id, Guid orderId, double amount, double orderTotal, DateTime date)
    {
        Domain.Guard.AgainstDefaultValue(nameof(id), id);
        Domain.Guard.AgainstDefaultValue(nameof(orderId), orderId);
        Domain.Guard.AgainstNegativeValue(nameof(amount), amount);
        Domain.Guard.AgainstNegativeValue(nameof(orderTotal), orderTotal);
        Domain.Guard.AgainstDefaultValue(nameof(date), date);
        Guard.AgainstUnderPayment(amount, orderTotal);

        this.Id = id;
        this.OrderId = orderId;
        this.Amount = amount;
        this.OrderTotal = orderTotal;
        this.Date = date;
    }

    public Guid Id { get; }

    public Guid OrderId { get; }

    public double Amount { get; }

    public double OrderTotal { get; }

    public double Change => this.Amount - this.OrderTotal;

    public DateTime Date { get; }

    public static class Guard
    {
        public static void AgainstUnderPayment(double paymentAmount, double orderTotal)
        {
            if (paymentAmount < orderTotal)
            {
                throw new PaymentException($"Payment amount '{paymentAmount}' does not cover Order Total '{orderTotal}'.");
            }
        }
    }
}

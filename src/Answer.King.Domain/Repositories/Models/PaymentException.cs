namespace Answer.King.Domain.Repositories.Models;

[Serializable]
public class PaymentException : Exception
{
    public PaymentException(string message) : base(message)
    {
    }
}

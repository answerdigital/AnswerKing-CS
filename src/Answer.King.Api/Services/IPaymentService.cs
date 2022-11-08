using Answer.King.Api.RequestModels;
using Answer.King.Domain.Repositories.Models;

namespace Answer.King.Api.Services;

public interface IPaymentService
{
    Task<Payment?> GetPayment(long paymentId);
    Task<IEnumerable<Payment>> GetPayments();
    Task<Payment> MakePayment(MakePayment makePayment);
}

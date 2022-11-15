namespace Answer.King.Domain;

public interface IAggregateRepository<T> where T : IAggregateRoot
{
    Task<IEnumerable<T>> Get();

    Task<T?> Get(long id);

    Task Save(T item);
}

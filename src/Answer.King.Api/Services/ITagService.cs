using Answer.King.Domain.Inventory;

namespace Answer.King.Api.Services;

public interface ITagService
{
    Task<Tag> CreateTag(RequestModels.Tag createTag);

    Task<IEnumerable<Tag>> GetTags();

    Task<Tag?> GetTag(long tagId);

    Task<Tag?> GetTagByName(string name);

    Task<Tag?> RetireTag(long tagId);

    Task<Tag?> UnretireTag(long tagId);

    Task<Tag?> UpdateTag(long tagId, RequestModels.Tag updateTag);
}

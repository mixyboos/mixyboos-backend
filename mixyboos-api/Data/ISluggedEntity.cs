namespace MixyBoos.Api.Data
{
    public interface ISluggedEntity : IUniqueFieldEntity {
        string Slug { get; set; }
    }
}

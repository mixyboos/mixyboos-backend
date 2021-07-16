namespace MixyBoos.Api.Data.Utils {
    public interface ISluggedEntity : IUniqueFieldEntity {
        string Slug { get; set; }
    }
}

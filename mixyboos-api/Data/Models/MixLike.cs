#nullable enable
namespace MixyBoos.Api.Data.Models;

public class MixLike : BaseEntity {
    public Mix Mix { get; set; }
    public MixyBoosUser? User { get; set; }
}

#nullable enable
namespace MixyBoos.Api.Data.Models;

public class MixPlay : BaseEntity {
    public Mix Mix { get; set; }
    public MixyBoosUser? User { get; set; }
}

#nullable enable
namespace MixyBoos.Api.Data.Models;

public class MixDownload : BaseEntity {
    public Mix Mix { get; set; }
    public MixyBoosUser? User { get; set; }
}

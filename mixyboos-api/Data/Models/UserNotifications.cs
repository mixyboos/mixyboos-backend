namespace MixyBoos.Api.Data.Models;

public class UserNotifications : BaseEntity {
    public MixyBoosUser User { get; set; }
    public BaseEntity Entity { get; set; }

    public string NotificationText { get; set; }
    public virtual MixyBoosUser SourceUser { get; set; }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MixyBoos.Api.Data;

public class IdentityUserStore : UserStore<MixyBoosUser> {
    public IdentityUserStore(MixyBoosContext context, IdentityErrorDescriber describer = null) : base(context,
        describer) {
        this.AutoSaveChanges = true;
    }
}

using Microsoft.AspNetCore.Identity;

namespace MixyBoos.Api.Data {
    public class ApplicationUser : IdentityUser {
        public string DisplayName { get; set; }
        public string Image { get; set; }
        [SlugField]
        public string Slug { get; set; }
    }
}

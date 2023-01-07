using System.Collections.Generic;

namespace MixyBoos.Api.Data.DTO {
    public class FollowDTO {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ProfileDTO {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Image { get; set; }
        public string Slug { get; set; }
        public string DisplayName { get; set; }

        public List<FollowDTO> Followers { get; set; }
        public List<FollowDTO> Following { get; set; }
    }
}

using System.Collections.Generic;

namespace MixyBoos.Api.Data.DTO {
    public class FollowDTO {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ProfileDTO {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ProfileImage { get; set; }
        public string HeaderImage { get; set; }
        public string Slug { get; set; }
        public string DisplayName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Biography { get; set; }
        public string PhoneNumber { get; set; }

        public List<FollowDTO> Followers { get; set; }
        public List<FollowDTO> Following { get; set; }

    }
}

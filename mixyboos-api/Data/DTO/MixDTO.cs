using System;

namespace MixyBoos.Api.Data.DTO {
    public record MixDTO(
        string Id,
        string Slug,
        string Title,
        string Description,
        DateTime DateUploaded,
        string Image,
        string AudioUrl,
        bool IsProcessed,
        ProfileDTO User
    );
}

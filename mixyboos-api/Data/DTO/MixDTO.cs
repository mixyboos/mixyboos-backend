namespace MixyBoos.Api.Data.DTO {
    public record MixDTO(
        string Id,
        string Slug,
        string Title,
        string Description,
        string Image,
        string AudioUrl,
        UserDTO User
    );
}

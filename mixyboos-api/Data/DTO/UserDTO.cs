namespace MixyBoos.Api.Data.DTO {
    public record UserDTO(
        string Id,
        string Slug,
        string DisplayName,
        string Image
    );
}

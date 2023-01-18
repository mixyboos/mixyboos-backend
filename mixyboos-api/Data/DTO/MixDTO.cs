using System;

namespace MixyBoos.Api.Data.DTO; 

public record MixDTO(
    string Id,
    string Slug,
    string Title,
    string Description,
    DateTime DateUploaded,
    string Image,
    bool IsProcessed,
    ProfileDTO User,
    int PlayCount
);

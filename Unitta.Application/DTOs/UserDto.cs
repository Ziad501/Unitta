﻿namespace Unitta.Application.DTOs;

public record UserDto
(
    string Id,
    string Name,
    string Email,
    DateTimeOffset CreatedAt
);

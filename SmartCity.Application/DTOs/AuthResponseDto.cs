namespace SmartCity.Application.DTOs
{
    public record AuthResponseDto(string Token, string Email, string FullName, string UserId);
}

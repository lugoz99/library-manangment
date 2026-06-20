namespace LibraryManagment.Helpers.Errors
{
    public record ErrorResponseDto(
    string Message,
    string ErrorCode,
    Dictionary<string, object>? Metadata = null
);
}

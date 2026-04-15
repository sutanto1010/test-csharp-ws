using System.ComponentModel.DataAnnotations;

namespace RealtimeDemo.Api.Contracts;

public sealed class CreateNoteRequest : IValidatableObject
{
    [Required]
    [MaxLength(120)]
    public string Title { get; init; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Content { get; init; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            yield return new ValidationResult(
                "Title is required.",
                [nameof(Title)]);
        }

        if (string.IsNullOrWhiteSpace(Content))
        {
            yield return new ValidationResult(
                "Content is required.",
                [nameof(Content)]);
        }
    }
}

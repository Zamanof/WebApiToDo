namespace TO_DO.DTOs;

public class ToDoItemDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool Iscompleted { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

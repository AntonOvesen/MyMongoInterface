namespace MyMongoInterface.Models.DTOs;

public class StudentDTO
{
    public string Name { get; set; } = string.Empty;

    public bool IsGraduated { get; set; }

    public string Gender { get; set; } = string.Empty;

    public int Age { get; set; }
}

namespace BookingPlatform.Domain.Entities;

public class Role
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }

    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    private Role() { }

    public Role(string name, string? description = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
    }
}

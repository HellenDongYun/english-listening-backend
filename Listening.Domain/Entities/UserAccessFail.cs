namespace Listening.Domain.Entities;

public class UserAccessFail
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    private bool _lockOut;
    public DateTime? LockOutEnd { get; private set; }
    public int AccessFailCount { get; private set; }

    private UserAccessFail() { }

    internal UserAccessFail(Guid userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
    }
}
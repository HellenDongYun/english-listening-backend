using Zack.Commons;

namespace Listening.Domain.Entities;

public class User:IAggregateRoot
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    private string? _passwordHash;
    
    private UserAccessFail _accessFail;
    public UserAccessFail AccessFail => _accessFail;

    private User()
    {
    }

    public User(string email)
    {
        Id = Guid.NewGuid();
        Email = email;
        _accessFail = new UserAccessFail(Id);
    }
    public void SetPassword(string plain)
    {
        // hash
        _passwordHash = HashHelper.ComputeMd5Hash(plain);
    }

    public bool CheckPassword(string plain) =>
        _passwordHash == HashHelper.ComputeMd5Hash(plain);

}
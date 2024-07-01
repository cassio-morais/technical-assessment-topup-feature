namespace Backend.FakeUser
{
    public static class FakeUserService
    {
        public static readonly List<User> Users = new()
        {
            new User(Guid.Parse("C30CF3C7-C738-435D-AC77-FA19B6018924"), true),
            new User(Guid.Parse("29C0F3B9-75C1-4A80-8530-BA295A612B67"), false),
        };
    }

    public sealed record User(Guid UserId, bool IsVerified);
}

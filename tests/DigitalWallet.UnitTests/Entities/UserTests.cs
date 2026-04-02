using DigitalWallet.Domain.Entities;
using DigitalWallet.Domain.Enums;

namespace DigitalWallet.UnitTests.Entities
{
    public class UserTests
    {
        [Fact]
        public void Constructor_ShouldSetAllProperties()
        {
            var user = new User("Aykut", "aykut@test.com", "hashedpw", UserRole.User);

            Assert.Equal("Aykut", user.FullName);
            Assert.Equal("aykut@test.com", user.Email);
            Assert.Equal("hashedpw", user.PasswordHash);
            Assert.Equal(UserRole.User, user.Role);
            Assert.NotEqual(Guid.Empty, user.Id);
        }

        [Fact]
        public void Constructor_ShouldGenerateUniqueIds()
        {
            var user1 = new User("User1", "a@test.com", "hash1", UserRole.User);
            var user2 = new User("User2", "b@test.com", "hash2", UserRole.User);

            Assert.NotEqual(user1.Id, user2.Id);
        }

        [Fact]
        public void Constructor_ShouldSetCreatedAtToUtcNow()
        {
            var before = DateTime.UtcNow;
            var user = new User("Aykut", "aykut@test.com", "hashedpw", UserRole.User);
            var after = DateTime.UtcNow;

            Assert.InRange(user.CreatedAt, before, after);
        }
    }
}

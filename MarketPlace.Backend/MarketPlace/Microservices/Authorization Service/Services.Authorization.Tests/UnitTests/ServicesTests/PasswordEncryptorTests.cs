using Bogus;
using FluentAssertions;

namespace AuthorizationService
{
    public class PasswordEncryptorTests
    {
        private readonly PasswordEncryptor _passwordEncryptor;

        public PasswordEncryptorTests()
        {
            _passwordEncryptor = new PasswordEncryptor();
        }

        [Fact]
        public void GenerateEncryptedPassword_ShouldReturnEncryptedPassword()
        {
            // Arrange
            var faker = new Faker();
            var password = faker.Internet.Password();

            // Act
            var encryptedPassword = _passwordEncryptor.GenerateEncryptedPassword(password);

            // Assert
            encryptedPassword.Should().NotBeNullOrEmpty();
            // SHA512 возвращает 64 байта, в шестнадцатеричном представлении это 128 символов
            encryptedPassword.Should().HaveLength(128); 
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            var faker = new Faker();
            var password = faker.Internet.Password(); 
            var encryptedPassword = _passwordEncryptor.GenerateEncryptedPassword(password);

            // Act
            var result = _passwordEncryptor.VerifyPassword(encryptedPassword, password);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            var faker = new Faker();
            var password = faker.Internet.Password();
            var wrongPassword = password + "sasf";
            var encryptedPassword = _passwordEncryptor.GenerateEncryptedPassword(password);

            // Act
            var result = _passwordEncryptor.VerifyPassword(encryptedPassword, wrongPassword);

            // Assert
            result.Should().BeFalse();
        }
    }
}

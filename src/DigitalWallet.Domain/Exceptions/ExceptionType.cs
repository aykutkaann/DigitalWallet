using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalWallet.Domain.Exceptions
{
    public class KeyNotFoundException(string message) : Exception(message);
    public class ArgumentException(string message) : Exception(message);
    public class InvalidOperationException(string message) : Exception(message);
    public class UnauthorizedAccessException(string message) : Exception(message);

}

using Isopoh.Cryptography.Argon2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Utilities
{
    public class PasswordHasher
    {
        public static string HashPassword(string password) => Argon2.Hash(password);

        public static bool VerifyPassword(string password, string hashedPassword)
            => Argon2.Verify(hashedPassword, password);

    }
}

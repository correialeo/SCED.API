using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace SCED.API
{
    public static class Settings
    {
        public static readonly string Secret = "qz6q${a);#rhdm{be,{n)$~}7};!a9x|";
        public static IConfiguration _configuration { get; private set; }

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GetEnvVars(string env)
        {
            return _configuration.GetValue<string>(env);
        }

        public static string encodePassword(string password)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(Secret);

            string hashed = Convert.ToBase64String(KeyDerivation
                .Pbkdf2(
                    password: password,
                    salt: bytes,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8
                )
            );

            return hashed;
        }

        public static string GetConnectionString()
        {
            string databaseServer = Environment.GetEnvironmentVariable("DATABASE__SERVER");
            string databaseName = Environment.GetEnvironmentVariable("DATABASE__NAME");
            string databasePort = Environment.GetEnvironmentVariable("DATABASE__PORT");
            string databaseUser = Environment.GetEnvironmentVariable("DATABASE__USER");
            string databasePass = Environment.GetEnvironmentVariable("DATABASE__PASSWORD");

            string conectString = _configuration.GetConnectionString("sqlServer");
            conectString = string.Format(conectString, databaseServer, databasePort, databaseName, databaseUser, databasePass);

            return conectString;
        }
    }
}

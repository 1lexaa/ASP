using ASP.Services.Hash;

namespace ASP.Services.Kdf
{
    public class HashKdfService : IKdfService
    {
        private readonly IHashService _hash_service;

        public HashKdfService(IHashService hash_service)
        {
            _hash_service = hash_service;
        }

        public string GetDerivedKey(string password, string salt) { return _hash_service.Hash(salt + password); }
    }
}

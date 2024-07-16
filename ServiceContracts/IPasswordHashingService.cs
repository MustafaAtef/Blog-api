namespace BlogApi.ServiceContracts {
    public interface IPasswordHashingService {
        string Hash(string password);
        bool Compare(string hashed, string password);
    }
}

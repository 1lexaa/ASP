namespace ASP.Services.Random
{
    public interface IRandomService
    {
        String RandomString(int lenght);
        String ConfirmCode(int lenght);
        String RandomFileName(int length);
    }
}

namespace ASP.Services.Random
{
    public class RandomServiceV1 : IRandomService
    {
        private readonly System.Random _random = new System.Random();
        private readonly String _code_chars = "abcdefghijklmnopqrstuvwxyz0123456789_-=";
        private readonly String _safe_chars = new String(Enumerable.Range(20, 107).Select(x => (char)x).ToArray());

        private String _MakeString(String source_string, int lenght)
        {
            char[] chars = new char[lenght];

            for (int i = 0; i < chars.Length; i++)
                chars[i] = source_string[_random.Next(source_string.Length)];
            return new string(chars);
        }

        string IRandomService.RandomString(int lenght) { return _MakeString(_safe_chars, lenght); }
        string IRandomService.ConfirmCode(int lenght) { return _MakeString(_code_chars, lenght); }
        string IRandomService.RandomFileName(int length) { return _MakeString(_code_chars, length); }
    }
}

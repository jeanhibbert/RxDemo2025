namespace RxAspireApp.Domain.Strategy.Services;

public static class GeneralExtensions
{
    public static bool IsValidTicker(this string str)
    {
        if (string.IsNullOrEmpty(str) || str.Length < 3 || str.Length > 5)
            return false;

        return str.ToCharArray().All(c => char.IsLetter(c) || char.IsNumber(c) || char.IsUpper(c));
    }
}

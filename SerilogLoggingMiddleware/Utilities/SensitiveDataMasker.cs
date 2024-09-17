using System.Text.RegularExpressions;

namespace SerilogLoggingMiddleware.Utilities;

public static class SensitiveDataMasker
{
    public static string MaskSensitiveData(string data)
    {
        if (string.IsNullOrEmpty(data)) return data;

        // TC Kimlik numarası maskeleme
        data = Regex.Replace(data, @"\b(\d{2})(\d{5})(\d{4})\b", "$1*****$3");

        // Telefon numarası maskeleme
        data = Regex.Replace(data, @"\b(\d{4})(\d{3})(\d{4})\b", "$1***$3");

        // Kredi kartı numarası maskeleme
        data = Regex.Replace(data, @"\b(\d{4})(\d{8})(\d{4})\b", "$1********$3");

        // Parola maskeleme
        data = Regex.Replace(data, @"(?i)(""password"":\s?""[^""]*"")", "\"password\":\"********\"");

        return data;
    }
}

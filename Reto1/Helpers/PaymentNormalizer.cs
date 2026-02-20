using System.Text.RegularExpressions;

namespace Reto1.Helpers
{
    public static class PaymentNormalizer
    {
        public static string NormalizeMerchant(string merchant)
        {
            var m = (merchant ?? string.Empty).Trim().ToLowerInvariant();
            m = Regex.Replace(m, @"\s+", " ");
            return m;
        }
    }
}

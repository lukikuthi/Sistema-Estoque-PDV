using System.Globalization;

namespace SistemaEstoquePDV.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = cpf.Replace(".", "").Replace("-", "");
            
            if (cpf.Length != 11)
                return false;

            if (cpf.All(c => c == cpf[0]))
                return false;

            // Validação do CPF
            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }

        public static bool IsValidDecimal(string value, out decimal result)
        {
            return decimal.TryParse(value, NumberStyles.Currency, CultureInfo.CurrentCulture, out result) && result >= 0;
        }

        public static bool IsValidInteger(string value, out int result)
        {
            return int.TryParse(value, out result) && result >= 0;
        }

        public static bool IsValidDate(string value, out DateTime result)
        {
            return DateTime.TryParseExact(value, "dd/MM/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out result);
        }

        public static string FormatCurrency(decimal value)
        {
            return value.ToString("C", CultureInfo.CurrentCulture);
        }

        public static string FormatDate(DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }

        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}
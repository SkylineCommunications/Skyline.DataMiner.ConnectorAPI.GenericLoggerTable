namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable
{
    using System;
    using System.Text;

    /// <summary>
    /// Contains methods to convert Unicode strings to hexadecimal strings and vice versa.
    /// </summary>
    public static class HexConverter
    {
        /// <summary>
        /// Converts a string to a hexadecimal string using Unicode encoding.
        /// </summary>
        /// <param name="str">String to convert to hexadecimal string.</param>
        /// <returns>Hexadecimal string.</returns>
        public static string ToHexString(string str)
        {
            if (String.IsNullOrEmpty(str)) return null;

            var sb = new StringBuilder();
            var bytes = Encoding.Unicode.GetBytes(str);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts a hexadecimal string to a Unicode encoded string.
        /// </summary>
        /// <param name="str">Hexadecimal string.</param>
        /// <returns>Unicode string.</returns>
        public static string FromHexString(string hexString)
        {
            if (String.IsNullOrEmpty(hexString)) return null;

            var bytes = new byte[hexString.Length / 2];
            try
            {
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }
            }
            catch (FormatException)
            {
                return hexString;
            }


            return Encoding.Unicode.GetString(bytes);
        }
    }
}

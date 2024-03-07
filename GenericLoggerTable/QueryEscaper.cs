namespace Skyline.DataMiner.ConnectorAPI.GenericLoggerTable
{
    using System;
    using System.Text;

    /// <summary>
    /// Contains methods to convert Unicode strings to hexadecimal strings and vice versa.
    /// </summary>
    public static class QueryEscaper
    {
        /// <summary>
        /// Escapes the ' character in the data.
        /// </summary>
        /// <param name="data">String to be escaped.</param>
        /// <returns>Escaped string.</returns>
        public static string Escape(string data)
        {
            return data?.Replace("'", "''");
        }

        /// <summary>
        /// Unescapes the escaped ' characters.
        /// </summary>
        /// <param name="data">String to be unescaped.</param>
        /// <returns>Unescaped string.</returns>
        public static string Unescape(string data)
        {
            return data?.Replace("''", "'");
        }
    }
}

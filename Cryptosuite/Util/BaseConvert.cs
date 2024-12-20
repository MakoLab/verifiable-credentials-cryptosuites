namespace Cryptosuite.Core.Util
{
    public static class BaseConvert
    {
        public static string ToBase64UrlNoPadding(byte[] data)
        {
            string base64 = Convert.ToBase64String(data);
            Span<char> buffer = stackalloc char[base64.Length];
            int index = 0;

            foreach (char c in base64)
            {
                buffer[index++] = c switch
                {
                    '+' => '-',
                    '/' => '_',
                    _ => c
                };
            }

            // Trim padding characters ('=')
            int end = index;
            while (end > 0 && buffer[end - 1] == '=') end--;

            return new string(buffer[..end]);
        }

        public static byte[] FromBase64UrlNoPadding(string data)
        {
            int padding = (4 - (data.Length % 4)) % 4;
            Span<char> charSpan = stackalloc char[data.Length + padding];
            for (int i = 0; i < data.Length; i++)
            {
                char c = data[i];
                charSpan[i] = c switch
                {
                    '-' => '+',
                    '_' => '/',
                    _ => c
                };
            }
            for (int i = data.Length; i < data.Length + padding; i++)
            {
                charSpan[i] = '=';
            }

            return Convert.FromBase64String(new string(charSpan));
        }
    }
}

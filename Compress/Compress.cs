using System;
using System.IO;
using System.IO.Compression;

namespace Compress
{
    public static class Algorithm
    {
        public static class RLE
        {
            public static byte[] Encode(string s)
            {
                int val = 0;
                char current = s[0];
                byte count = 0x81;
                for (int i = 1; i < s.Length; i++)
                {
                    if (current != s[i]) { current = s[i]; val++; }
                }
                byte[] res = new byte[val * 2 + 2];

                val = 0;
                current = s[0];
                for (int i = 1; i < s.Length; i++)
                {
                    if (current == s[i])
                    {
                        count += 0x01;
                    }
                    else
                    {

                        res[val] = count;
                        res[val + 1] = (byte)current;
                        val += 2;
                        count = 0x81;
                        current = s[i];
                    }
                }

                res[val] = count;
                res[val + 1] = (byte)current;

                return res;
            }
            public static string Decode(byte[] g)
            {
                string res = "";
                int val = 0;
                char s = '0';
                MemoryStream ms = new MemoryStream(g);
                for (int i = 0; i < ms.Length; i += 2)
                {
                    val = ms.ReadByte() - 0x80;
                    s = (char)ms.ReadByte();
                    for (int j = 0; j < val; j++)
                    {
                        res += s;
                    }
                }
                ms.Close();
                ms.Dispose();
                return res;
            }
        }
        public static class RLEM
        {
            public static byte[] Encode(string s)
            {
                int val = 0;
                char current = s[0];
                byte count = 0x81;
                for (int i = 1; i < s.Length; i++)
                {
                    if (current != s[i]) { current = s[i]; val++; }
                }
                byte[] res = new byte[val * 2 + 2];

                val = 0;
                current = s[0];
                for (int i = 1; i < s.Length; i++)
                {
                    if (current == s[i])
                    {
                        count += 0x01;
                    }
                    else
                    {

                        res[val] = count;
                        res[val + 1] = (byte)current;
                        val += 2;
                        count = 0x81;
                        current = s[i];
                    }
                }

                res[val] = count;
                res[val + 1] = (byte)current;

                MemoryStream mw = new MemoryStream();
                mw.WriteByte(res[0]);
                mw.WriteByte(res[1]);
                for (int i = 0; i < res.Length - 2; i += 2)
                {
                    if (res[i] == res[i + 2])
                    {
                        mw.WriteByte(res[i + 3]);
                    }
                    else
                    {
                        mw.WriteByte(res[i + 2]);
                        mw.WriteByte(res[i + 3]);
                    }
                }

                return mw.ToArray();
            }
            public static string Decode(byte[] g)
            {
                string res = "";
                byte buf = 0;
                int val = 0;

                MemoryStream ms = new MemoryStream(g);
                buf = (byte)ms.ReadByte();
                for (int i = 0; i < ms.Length; i++)
                {
                    if (buf > 0x80)
                    {
                        val = buf - 0x80;

                        for (;;)
                        {
                            buf = (byte)ms.ReadByte();
                            if (buf > 0x80) break;
                            for (int j = 0; j < val; j++)
                            {
                                res += (char)buf;
                            }
                        }
                    }
                }
                ms.Close();
                ms.Dispose();
                return res;
            }
        }
        public static class RLEM2
        {
            public static byte[] Encode(string s)
            {
                int val = 0;
                char current = s[0];
                byte count = 0x81;
                MemoryStream mw = new MemoryStream();

                for (int i = 1; i < s.Length; i++) if (current != s[i]) { current = s[i]; val++; }
                byte[] res = new byte[val * 2 + 2];

                val = 0;
                current = s[0];
                for (int i = 1; i < s.Length; i++)
                {
                    if (current == s[i]) count += 0x01;
                    else
                    {
                        res[val] = count;
                        res[val + 1] = (byte)current;
                        val += 2;
                        count = 0x81;
                        current = s[i];
                    }
                }

                res[val] = count;
                res[val + 1] = (byte)current;

                mw.WriteByte(res[0]);
                mw.WriteByte(res[1]);
                for (int i = 0; i < res.Length - 2; i += 2)
                {
                    if (res[i] == res[i + 2])
                    {
                        mw.WriteByte(res[i + 3]);
                    }
                    else
                    {
                        mw.WriteByte(res[i + 2]);
                        mw.WriteByte(res[i + 3]);
                    }
                }
                res = mw.ToArray();

                mw = new MemoryStream();
                foreach (var item in res) if (item != 0x81) mw.WriteByte(item);

                return mw.ToArray();
            }
            public static string Decode(byte[] g)
            {
                string res = null;
                int chng;

                for (int i = 0; i < g.Length; i++)
                {
                    if (g[i] > 0x80)
                    {
                        chng = g[i] - 0x80;
                        i++;
                        for (int j = 0; j < chng; j++) res += (char)g[i];
                    }
                    else res += (char)g[i];
                }

                return res;
            }
        }
        public static class RLEM3
        {
            public static byte[] Encode(string s)
            {
                MemoryStream ms = new MemoryStream();
                char sub = s[0];
                int val = 1;
                for (int i = 1; i < s.Length; i++)
                {
                    if (sub == s[i])
                    {
                        if (val >= 0x7F)
                        {
                            for (int k = val; k >= 0x7F; k -= 0x7F)
                            {
                                val += 0x80;
                                ms.WriteByte((byte)val);
                                ms.WriteByte((byte)sub);
                            }
                            val = 1;
                        }
                        val++;
                    }
                    else
                    {
                        if (val == 1) ms.WriteByte((byte)sub);
                        else
                        {
                            val += 0x80;
                            ms.WriteByte((byte)val);
                            ms.WriteByte((byte)sub);
                            val = 1;
                        }
                        sub = s[i];
                    }
                }
                if (val > 1)
                {
                    val += 0x80;
                    ms.WriteByte((byte)val);
                    ms.WriteByte((byte)sub);
                }
                else ms.WriteByte((byte)sub);
                val = 1;

                return ms.ToArray();
            }
            public static string Decode(byte[] g)
            {
                string res = null;
                int chng;

                for (int i = 0; i < g.Length; i++)
                {
                    if (g[i] > 0x80)
                    {
                        chng = g[i] - 0x80;
                        i++;
                        for (int j = 0; j < chng; j++) res += (char)g[i];
                    }
                    else res += (char)g[i];
                }

                return res;
            }
        }
        public static class Deflate
        {
            public static byte[] Encode(string s)
            {
                using (MemoryStream output = new MemoryStream())
                {
                    using (DeflateStream gzip =
                      new DeflateStream(output, CompressionMode.Compress))
                    {
                        using (StreamWriter writer =
                          new StreamWriter(gzip, System.Text.Encoding.UTF8))
                        {
                            writer.Write(s);
                        }
                    }

                    return output.ToArray();
                }
            }
            public static string Decode(byte[] g)
            {
                using (MemoryStream inputStream = new MemoryStream(g))
                {
                    using (DeflateStream gzip =
                      new DeflateStream(inputStream, CompressionMode.Decompress))
                    {
                        using (StreamReader reader =
                          new StreamReader(gzip, System.Text.Encoding.UTF8))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }
        public static class BWT
        {
            private static string getShift(string s, int offset)
            {
                if (offset > s.Length) return null;
                string res = null;
                for (int i = s.Length - offset; i < s.Length; i++) res += s[i];
                for (int i = 0; i < s.Length - offset; i++) res += s[i];
                return res;
            }
            private static string[] getALL(string s)
            {
                string[] res = new string[s.Length];
                for (int i = 0; i < s.Length; i++)
                {
                    res[i] = getShift(s, i);
                }
                return res;
            }
            public static string getSort(string s)
            {
                string res = null;
                string[] buf = getALL(s);
                Array.Sort(buf);
                for (int i = 0; i < buf.Length; i++) res += buf[i][s.Length - 1];
                return res;
            }
            public static string deSort(string s)
            {
                string[] res = new string[s.Length];
                string[] buf = new string[s.Length];
                string[] fst = new string[s.Length];
                int val = 0;

                for (int i = 0; i < s.Length; i++) fst[i] += s[i];

                for (int i = 0; i < s.Length; i++)
                {
                    for (int g = 0; g < s.Length; g++) buf[g] = res[g];
                    Array.Sort(buf);
                    for (int j = 0; j < s.Length; j++)
                    {
                        res[j] = fst[j];
                        res[j] += buf[j];
                    }
                }
                for (int i = 0; i < res.Length; i++)
                {
                    if (res[i][res.Length - 1] == '\n')
                    {
                        val = i;
                        break;
                    }
                }
                return res[val];
            }
            public static byte[] Encode(string s)
            {
                byte[] res = RLEM3.Encode(getSort(s));
                return res;
            }
            public static string Decode(byte[] g)
            {
                string buf = RLEM3.Decode(g);
                return deSort(buf);
            }
        }
        public static class LZMA
        {
            public static byte[] Encode(string s)
            {
                byte[] result;
                byte[] str = new byte[s.Length];

                int v = 0;
                foreach (var item in s)
                {
                    str[v] = (byte)item;
                    v++;
                }

                MemoryStream ins = new MemoryStream(str);
                MemoryStream outs = new MemoryStream();

                SevenZip.Compression.LZMA.Encoder lzma = new SevenZip.Compression.LZMA.Encoder();

                lzma.WriteCoderProperties(outs);
                outs.Write(BitConverter.GetBytes(ins.Length), 0, 8);
                lzma.Code(ins, outs, -1, -1, null);
                result = outs.ToArray();

                //outs.Flush();
                outs.Close();
                outs.Dispose();

                ins.Close();
                ins.Dispose();

                return result;
            }
            public static string Decode(byte[] str)
            {
                string buffer = null;

                MemoryStream ins = new MemoryStream(str);
                MemoryStream outs = new MemoryStream();

                SevenZip.Compression.LZMA.Decoder lzma = new SevenZip.Compression.LZMA.Decoder();

                byte[] properties = new byte[5];
                ins.Read(properties, 0, 5);
                byte[] fileLengthBytes = new byte[8];
                ins.Read(fileLengthBytes, 0, 8);
                long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

                lzma.SetDecoderProperties(properties);
                lzma.Code(ins, outs, ins.Length, fileLength, null);

                foreach (var item in outs.ToArray()) buffer += (char)item;

                //outs.Flush();
                outs.Close();
                outs.Dispose();

                ins.Close();
                ins.Dispose();

                return buffer;
            }
        }
    }
}

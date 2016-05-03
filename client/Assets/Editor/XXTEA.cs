using System;
using System.Collections;
using System.Collections.Generic;

class XXTEA
{
    private static byte[] Key = System.Text.Encoding.UTF8.GetBytes("123456789123456789");

    public static byte[] Encrypt(byte[] Data)
    { 
        if (Data.Length == 0)
        { 
            return Data; 
        } 
        return ToByteArray(Encrypt(TouintArray(Data, true), TouintArray(Key, false)), false); 
    }

    public static byte[] Decrypt(byte[] Data)
    { 
        if (Data.Length == 0)
        { 
            return Data; 
        } 
        return ToByteArray(Decrypt(TouintArray(Data, false), TouintArray(Key, false)), true); 
    }

    public static string EncryptToString(string Data)
    {
        return System.Convert.ToBase64String(Encrypt(System.Text.Encoding.UTF8.GetBytes(Data)));
    }

    public static string DecryptFromString(string Data)
    {
        return System.Text.Encoding.UTF8.GetString(Decrypt(System.Convert.FromBase64String(Data)));
    }

    public static uint[] Encrypt(uint[] v, uint[] k)
    { 
        int n = v.Length - 1; 
        if (n < 1)
        { 
            return v; 
        } 
        if (k.Length < 4)
        { 
            uint[] Key = new uint[4]; 
            k.CopyTo(Key, 0); 
            k = Key; 
        } 
        uint z = v[n], y = v[0], delta = 0x9E3779B9, sum = 0, e; 
        int p, q = 6 + 52 / (n + 1); 
        while (q-- > 0)
        { 
            sum = unchecked(sum + delta); 
            e = sum >> 2 & 3; 
            for (p = 0; p < n; p++)
            { 
                y = v[p + 1]; 
                z = unchecked(v[p] += (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z)); 
            } 
            y = v[0]; 
            z = unchecked(v[n] += (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z)); 
        } 
        return v; 
    }

    public static uint[] Decrypt(uint[] v, uint[] k)
    { 
        int n = v.Length - 1; 
        if (n < 1)
        { 
            return v; 
        } 
        if (k.Length < 4)
        { 
            uint[] Key = new uint[4]; 
            k.CopyTo(Key, 0); 
            k = Key; 
        } 
        uint z = v[n], y = v[0], delta = 0x9E3779B9, sum, e; 
        int p, q = 6 + 52 / (n + 1); 
        sum = unchecked((uint)(q * delta)); 
        while (sum != 0)
        { 
            e = sum >> 2 & 3; 
            for (p = n; p > 0; p--)
            { 
                z = v[p - 1]; 
                y = unchecked(v[p] -= (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z)); 
            } 
            z = v[n]; 
            y = unchecked(v[0] -= (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z)); 
            sum = unchecked(sum - delta); 
        } 
        return v; 
    }

    private static uint[] TouintArray(byte[] Data, bool IncludeLength)
    { 
        int n = (((Data.Length & 3) == 0) ? (Data.Length >> 2) : ((Data.Length >> 2) + 1)); 
        uint[] Result; 
        if (IncludeLength)
        { 
            Result = new uint[n + 1]; 
            Result[n] = (uint)Data.Length; 
        } else
        { 
            Result = new uint[n]; 
        } 
        n = Data.Length; 
        for (int i = 0; i < n; i++)
        { 
            Result[i >> 2] |= (uint)Data[i] << ((i & 3) << 3); 
        } 
        return Result; 
    }

    private static byte[] ToByteArray(uint[] Data, Boolean IncludeLength)
    { 
        int n; 
        if (IncludeLength)
        { 
            n = (int)Data[Data.Length - 1]; 
        } else
        { 
            n = Data.Length << 2; 
        } 
        byte[] Result = new byte[n]; 
        for (int i = 0; i < n; i++)
        { 
            Result[i] = (byte)(Data[i >> 2] >> ((i & 3) << 3)); 
        } 
        return Result; 
    }
} 
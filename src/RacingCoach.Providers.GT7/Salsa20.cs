namespace RacingCoach.Providers.GT7;

internal class Salsa20
{
    private readonly uint[] _key;
    private readonly uint[] _nonce;

    public Salsa20(byte[] key, byte[] nonce)
    {
        if (key.Length != 32)
            throw new ArgumentException("Key must be 32 bytes");
        if (nonce.Length != 8)
            throw new ArgumentException("Nonce must be 8 bytes");

        _key = new uint[8];
        for (int i = 0; i < 8; i++)
            _key[i] = BitConverter.ToUInt32(key, i * 4);

        _nonce = new uint[2];
        for (int i = 0; i < 2; i++)
            _nonce[i] = BitConverter.ToUInt32(nonce, i * 4);
    }

    public byte[] Encrypt(byte[] data)
    {
        return Process(data);
    }

    public byte[] Decrypt(byte[] data)
    {
        return Process(data);
    }

    private byte[] Process(byte[] data)
    {
        byte[] result = new byte[data.Length];
        int blocks = (data.Length + 63) / 64;

        for (int block = 0; block < blocks; block++)
        {
            uint[] counter = new uint[2];
            counter[0] = (uint)block;
            counter[1] = 0;

            uint[] keystream = GenerateBlock(counter);
            int offset = block * 64;
            int length = Math.Min(64, data.Length - offset);

            for (int i = 0; i < length; i++)
            {
                byte keystreamByte = (byte)(keystream[i / 4] >> ((i % 4) * 8));
                result[offset + i] = (byte)(data[offset + i] ^ keystreamByte);
            }
        }

        return result;
    }

    private uint[] GenerateBlock(uint[] counter)
    {
        uint[] state = new uint[16];

        state[0] = 0x61707865;
        state[5] = 0x3320646e;
        state[10] = 0x79622d32;
        state[15] = 0x6b206574;

        state[1] = _key[0];
        state[2] = _key[1];
        state[3] = _key[2];
        state[4] = _key[3];
        state[11] = _key[4];
        state[12] = _key[5];
        state[13] = _key[6];
        state[14] = _key[7];

        state[6] = _nonce[0];
        state[7] = _nonce[1];

        state[8] = counter[0];
        state[9] = counter[1];

        uint[] working = (uint[])state.Clone();

        for (int i = 0; i < 10; i++)
        {
            QuarterRound(ref working[0], ref working[4], ref working[8], ref working[12]);
            QuarterRound(ref working[5], ref working[9], ref working[13], ref working[1]);
            QuarterRound(ref working[10], ref working[14], ref working[2], ref working[6]);
            QuarterRound(ref working[15], ref working[3], ref working[7], ref working[11]);

            QuarterRound(ref working[0], ref working[1], ref working[2], ref working[3]);
            QuarterRound(ref working[5], ref working[6], ref working[7], ref working[4]);
            QuarterRound(ref working[10], ref working[11], ref working[8], ref working[9]);
            QuarterRound(ref working[15], ref working[12], ref working[13], ref working[14]);
        }

        for (int i = 0; i < 16; i++)
            working[i] += state[i];

        return working;
    }

    private static void QuarterRound(ref uint a, ref uint b, ref uint c, ref uint d)
    {
        b ^= RotateLeft(a + d, 7);
        c ^= RotateLeft(b + a, 9);
        d ^= RotateLeft(c + b, 13);
        a ^= RotateLeft(d + c, 18);
    }

    private static uint RotateLeft(uint value, int bits)
    {
        return (value << bits) | (value >> (32 - bits));
    }
}

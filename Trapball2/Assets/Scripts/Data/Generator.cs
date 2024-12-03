using System.Text;

public static class Generator
{
    private static byte[] hidden = { 0x74, 0x69, 0x61, 0x6E, 0x54, 0x69, 0x6F, 0x77, 0x6A };
    private static byte[] mask = { 0x75, 0x59, 0x69, 0x65, 0x6C, 0x61, 0x6F, 0x33 };


    public static string GetKey()
    {
        byte[] revealedKey = new byte[hidden.Length];
        for (int i = 0; i < hidden.Length; i++)
        {
            revealedKey[i] = (byte)(hidden[i] ^ mask[i]);
        }
        return Encoding.UTF8.GetString(revealedKey).PadRight(32, '0');
    }
}

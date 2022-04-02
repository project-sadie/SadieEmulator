using System.Text;
using System.Text.RegularExpressions;

namespace Sadie.Networking;

public static class WebSocketHelpers
{
    public static byte[] UnmaskData(byte[] data)
    {
        var usingMask = (data[1] & 0b10000000) != 0;

        if (!usingMask)
        {
            return Array.Empty<byte>();
        }
        
        int messageLength = data[1] - 128;
        var offset = 2;

        switch (messageLength)
        {
            case 126:
                messageLength = BitConverter.ToUInt16(new[] { data[3], data[2] }, 0);
                offset = 4;
                break;
        }

        if (messageLength == 0)
        {
            return Array.Empty<byte>();
        }
        
        var decoded = new byte[messageLength];
        var masks = new[] {data[offset], data[offset + 1], data[offset + 2], data[offset + 3]};
        
        offset += 4;

        for (var i = 0; i < messageLength; ++i)
        {
            decoded[i] = (byte) (data[offset + i] ^ masks[i % 4]);
        }

        return decoded;
    }
    
    public static byte[] AddFramingToOutput(byte[] bytes)
    {
        var frame = new byte[10];

        long indexStartRawData;
        var length = (long)bytes.Length;

        var Opcode = 2;

        frame[0] = (byte)(128 + Opcode);
        switch (length)
        {
            case <= 125:
                frame[1] = (byte)length;
                indexStartRawData = 2;
                break;
            case >= 126 and <= 65535:
                frame[1] = 126;
                frame[2] = (byte)((length >> 8) & 255);
                frame[3] = (byte)(length & 255);
                indexStartRawData = 4;
                break;
            default:
                frame[1] = 127;
                frame[2] = (byte)((length >> 56) & 255);
                frame[3] = (byte)((length >> 48) & 255);
                frame[4] = (byte)((length >> 40) & 255);
                frame[5] = (byte)((length >> 32) & 255);
                frame[6] = (byte)((length >> 24) & 255);
                frame[7] = (byte)((length >> 16) & 255);
                frame[8] = (byte)((length >> 8) & 255);
                frame[9] = (byte)(length & 255);

                indexStartRawData = 10;
                break;
        }

        var response = new byte[indexStartRawData + length];

        long i, reponseIdx = 0;

        for (i = 0; i < indexStartRawData; i++)
        {
            response[reponseIdx] = frame[i];
            reponseIdx++;
        }

        for (i = 0; i < length; i++)
        {
            response[reponseIdx] = bytes[i];
            reponseIdx++;
        }

        return response;
    }

    public static byte[] GetHandshakeResponseBytes(string dataString)
    {
        var swk = Regex.Match(dataString, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim();
        var swka = swk + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        var swkaSha1 = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(swka));
        var swkaSha1Base64 = Convert.ToBase64String(swkaSha1);

        return Encoding.UTF8.GetBytes(
            "HTTP/1.1 101 Switching Protocols\r\n" +
            "Connection: Upgrade\r\n" +
            "Upgrade: websocket\r\n" +
            "Sec-WebSocket-Accept: " + swkaSha1Base64 + "\r\n\r\n");
    }
}
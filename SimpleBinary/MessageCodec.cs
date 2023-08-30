
using System.Text;

public class Message
{
    public Dictionary<string, string>? headers;
    public byte[]? payload;
}

public interface MessageCodec
{
    byte[] Encode(Message message);
    Message Decode(byte[] data);
}

public class SimpleMessageCodec : MessageCodec
{
    private const int MaxHeaderSize = 1023;
    private const int MaxHeaders = 63;
    private const int MaxPayloadSize = 256 * 1024; //256 KiB

    public byte[] Encode(Message message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        ValidateHeaders(message.headers);

        if (message.payload == null)
            throw new ArgumentNullException(nameof(message.payload));

        if (message.payload.Length > MaxPayloadSize)
            throw new ArgumentException("Payload size exceeds the maximum limit.");
        

        List<byte> encodedMessage = new List<byte>();

        //Encode the headers count
        encodedMessage.Add((byte)Math.Min(message.headers.Count, MaxHeaders));

        //Encode headers, for each header the name and values are encoded, length is limited by MaxHeadSize
        foreach (var header in message.headers)
        {
            byte[] nameBytes = Encoding.ASCII.GetBytes(header.Key);
            byte[] valueBytes = Encoding.ASCII.GetBytes(header.Value);

            encodedMessage.Add((byte)Math.Min(nameBytes.Length, MaxHeaderSize));
            encodedMessage.AddRange(nameBytes);

            encodedMessage.Add((byte)Math.Min(valueBytes.Length, MaxHeaderSize));
            encodedMessage.AddRange(valueBytes);
        }

        //Encodethe payload size
        encodedMessage.AddRange(BitConverter.GetBytes(message.payload.Length));

        //Encode payload
        encodedMessage.AddRange(message.payload);

        return encodedMessage.ToArray();
    }

    public Message Decode(byte[] data)
    {
        if (data == null || data.Length < 1)
            throw new ArgumentException("Invalid data");

        int offset = 0;
        var message = new Message();
        message.headers = new Dictionary<string, string>();

        //Enabling Error handling 
        try
        {

        
        //Decode headers count
        int headerCount = data[offset++];

        //Decode headers for each header 
        for (int i = 0; i < headerCount; i++)
        {
            int nameLength = data[offset++];
            string name = Encoding.ASCII.GetString(data, offset, nameLength);
            offset += nameLength;

            int valueLength = data[offset++];
            string value = Encoding.ASCII.GetString(data, offset, valueLength);
            offset += valueLength;

            message.headers[name] = value;
        }

        //Decodnig payload size
        int payloadSize = BitConverter.ToInt32(data, offset);
        offset += sizeof(int);

        //Decode the actual payload
        message.payload = new byte[payloadSize];
        Array.Copy(data, offset, message.payload, 0, payloadSize);
        }
        catch (Exception ex)
        {

            throw new Exception("Error decoding the message: " + ex.Message);
        }
        return message;
    }

    private void ValidateHeaders(Dictionary<string, string> headers)
    {
        if (headers != null)
        {
            if (headers.Count > MaxHeaders)
                throw new ArgumentException("There are too many headers");

            foreach (var header in headers)
            {
                if (header.Key.Length > MaxHeaderSize || header.Value.Length > MaxHeaderSize)
                    throw new ArgumentException("Headers name or value too long");
            }
        }
    }
}

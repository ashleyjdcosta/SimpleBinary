
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        //Create an instance of SimpleMessageCodec
        SimpleMessageCodec codec = new SimpleMessageCodec();

        //Creating a sample message
        var message = new Message
        {
            headers = new Dictionary<string, string> {
                { "Header1", "Dcosta" },
                { "Header2", "AshleyJeff" }
            },
            payload = Encoding.ASCII.GetBytes("Test payload data.")
        };

        //Encode the message
        byte[] encodedData = codec.Encode(message);

        //Decode the encoded data
        Message decodedMessage = codec.Decode(encodedData);

        //Display the decoded message
        //Here we are just displaying our encoded message, and displaying the decoded message from our functions. 
        Console.WriteLine("Decoded Message Headers:");
        foreach (var header in decodedMessage.headers)
        {
            Console.WriteLine($"{header.Key}: {header.Value}");
        }
        Console.WriteLine("Decoded Message Payload: " + Encoding.ASCII.GetString(decodedMessage.payload));

        Console.WriteLine("Encoding and decoding test completed.");
    }
}

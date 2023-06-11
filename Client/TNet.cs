using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public static class TNet
    {
        public static string Connect(String server, String message)
        {
            try
            {
                Int32 port = 3000;
                TcpClient client = new TcpClient(server, port);
                
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                
                NetworkStream stream = client.GetStream();
                
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message);
                
                data = new Byte[256];
                
                String responseData = String.Empty;
                
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                // Close everything.
                stream.Close();
                client.Close();
                return responseData;
            }
            catch (ArgumentNullException e)
            {
                return e.ToString();
            }
            catch (SocketException e)
            {
                return e.ToString();
            }
        }
    }
}
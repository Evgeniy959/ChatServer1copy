using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;

namespace ChatServer1
{
    class Program
    {
        static TcpListener listener;
        static List<TcpClient> clients;
        static List<string> Names;
        static void Main(string[] args)
        {
            clients = new List<TcpClient>();
            Names = new List<string>();
            WaitNewClients();
        }
        static void ClientListner(object Client)
        {
            TcpClient client = (TcpClient)Client;
           
            while (true)
            {
                try
                { 
                    NetworkStream networkStream = client.GetStream();
                    byte[] buffer = new byte[255];
                    networkStream.Read(buffer, 0, 255);

                    string message = Encoding.Default.GetString(buffer);
                    message = message.Remove(message.IndexOf("\0"));
                  
                        Console.WriteLine(message); 
                    if (message.IndexOf("<NAME>")==0)//<NAME>aaa
                    {
                        int index = clients.FindIndex((x) => x == client);
                        Names[index] = message.Remove(0, 6);
                        sendList();
                    } 
                    else  if (message.IndexOf("<LIST>")==-1)

                    foreach (TcpClient cl in clients)
                    {
                        NetworkStream stream = cl.GetStream();
                        stream.Write(buffer, 0, 255);
                        stream.Flush();
                    }
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    clients.Remove(client);
                    break;
                }
            }
        }

         static void sendList()
        {
            string message = JsonConvert.SerializeObject(Names);
            byte[] buffer = Encoding.Default.GetBytes("<LIST>" + message);
            foreach (TcpClient cl in clients)
            {
                NetworkStream stream = cl.GetStream();
                stream.Write(buffer, 0, message.Length+6);
                stream.Flush();
            }
        }

        static void WaitNewClients()
        {
            TcpClient client;
            listener = new TcpListener(IPAddress.Any, 8888);
            listener.Start();
            while (true)
            {
                client = listener.AcceptTcpClient();
                clients.Add(client);
                Names.Add("NoName");
                Console.WriteLine("У нас новый посетитель!");
                Thread thread = new Thread(new ParameterizedThreadStart(ClientListner));
                thread.Start(client);
            }

        }
    }
}

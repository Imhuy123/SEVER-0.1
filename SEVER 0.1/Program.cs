using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace SocketServer
{
    class Program
    {
        private static Dictionary<string, Socket> _clients = new Dictionary<string, Socket>();
        private static List<ChatLog> _messageHistory = new List<ChatLog>();

        static void Main(string[] args)
        {
            Console.WriteLine("Socket Server is running...");

            string serverIp = "192.168.0.101";
            int portToRun = 8081;

            Console.WriteLine($"Server IP: {serverIp}");
            Console.WriteLine($"Port: {portToRun}");

            LoadMessageHistory();
            RunAsServer(serverIp, portToRun);

            Console.ReadLine();
        }

        static void RunAsServer(string ipAddress, int port)
        {
            try
            {
                IPAddress ipAddr = IPAddress.Parse(ipAddress);
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

                Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(10);

                Console.WriteLine("Waiting for connections...");

                while (true)
                {
                    Socket clientSocket = listener.Accept();
                    Thread clientThread = new Thread(() => HandleClient(clientSocket));
                    clientThread.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        static void HandleClient(Socket clientSocket)
        {
            string userName = null;
            StringBuilder messageBuffer = new StringBuilder(); // Bộ đệm để lưu dữ liệu nhận được

            try
            {
                byte[] buffer = new byte[1024];
                int numByte;

                // Nhận username của client
                numByte = clientSocket.Receive(buffer);
                userName = Encoding.UTF8.GetString(buffer, 0, numByte).Replace("<EOF>", "");

                lock (_clients)
                {
                    if (_clients.ContainsKey(userName))
                    {
                        _clients[userName].Close();
                        _clients.Remove(userName);
                    }
                    _clients.Add(userName, clientSocket);
                }

                Console.WriteLine($"Client connected: {userName}");
                SendUserListToAll();
                SendChatHistoryToUser(userName);

                while (clientSocket.Connected)
                {
                    try
                    {
                        numByte = clientSocket.Receive(buffer);
                        if (numByte > 0)
                        {
                            string receivedData = Encoding.UTF8.GetString(buffer, 0, numByte);
                            messageBuffer.Append(receivedData);

                            // Xử lý tin nhắn trong bộ đệm nếu phát hiện <EOF>
                            while (messageBuffer.ToString().Contains("<EOF>"))
                            {
                                string completeMessage = messageBuffer.ToString();
                                int eofIndex = completeMessage.IndexOf("<EOF>");
                                string message = completeMessage.Substring(0, eofIndex);
                                messageBuffer.Remove(0, eofIndex + "<EOF>".Length); // Xóa tin nhắn đã xử lý khỏi bộ đệm

                                Console.WriteLine($"Received message from {userName}: {message}");
                                ProcessMessage(userName, message.Trim());
                            }
                        }
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine($"{userName} has disconnected unexpectedly: {ex.Message}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }
            finally
            {
                CleanupClient(userName, clientSocket);
            }
        }

        static void CleanupClient(string userName, Socket clientSocket)
        {
            lock (_clients)
            {
                if (userName != null && _clients.ContainsKey(userName))
                {
                    _clients.Remove(userName);
                }
            }

            if (clientSocket != null && clientSocket.Connected)
            {
                try
                {
                    clientSocket.Shutdown(SocketShutdown.Both);
                }
                catch (SocketException) { }
                finally
                {
                    clientSocket.Close();
                }
            }

            Console.WriteLine($"Client {userName} has been cleaned up.");
            SendUserListToAll();
        }

        static void ProcessMessage(string fromUser, string message)
        {
            var splitMessage = message.Split(new[] { '>', ':' }, 3);
            if (splitMessage.Length == 3)
            {
                string toUser = splitMessage[1].Trim();
                string content = splitMessage[2].Trim();
                string fullMessage = $"{fromUser} -> {toUser} : {content}<EOF>";

                // Gửi tin nhắn đến người gửi và người nhận với định dạng chuẩn
                SendMessageToUser(fromUser, fullMessage);
                SendMessageToUser(toUser, fullMessage);
            }
            else
            {
                Console.WriteLine($"Received message in wrong format: {message}");
            }
        }

        static void SendMessageToUser(string user, string message)
        {
            if (_clients.ContainsKey(user))
            {
                try
                {
                    Console.WriteLine($"[Debug] Sending message to {user}: {message}");
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    _clients[user].Send(messageBytes);
                    Console.WriteLine($"Message sent to {user}: {message}");
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Error sending message to {user}: {ex.Message}");
                    CleanupClient(user, _clients[user]);
                }
            }
        } 

        static void SendUserListToAll()
        {
            string userList = "UserList:" + string.Join(",", _clients.Keys) + "<EOF>";
            byte[] userListBytes = Encoding.UTF8.GetBytes(userList);

            lock (_clients)
            {
                foreach (var client in _clients.Values)
                {
                    try
                    {
                        client.Send(userListBytes);
                    }
                    catch (SocketException) { }
                }
            }
        }

        static void SendChatHistoryToUser(string userName)
        {
            if (_clients.ContainsKey(userName))
            {
                var history = _messageHistory
                    .Where(log => log.FromUser == userName || log.ToUser == userName)
                    .Select(log => $"{log.FromUser}->{log.ToUser}:{log.Message}<EOF>")
                    .ToList();

                foreach (var message in history)
                {
                    SendMessageToUser(userName, message);
                    Thread.Sleep(50);
                }
            }
        }

        static void SaveMessageHistory()
        {
            File.WriteAllText("messageHistory.json", JsonConvert.SerializeObject(_messageHistory, Formatting.Indented));
        }

        static void LoadMessageHistory()
        {
            if (File.Exists("messageHistory.json"))
            {
                var json = File.ReadAllText("messageHistory.json");
                _messageHistory = JsonConvert.DeserializeObject<List<ChatLog>>(json) ?? new List<ChatLog>();
            }
        }

        class ChatLog
        {
            public string FromUser { get; set; }
            public string ToUser { get; set; }
            public string Message { get; set; }
        }
    }
}
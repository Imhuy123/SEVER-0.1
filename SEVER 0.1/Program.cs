using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Socket Server is running...");

            // Thiết lập IP và cổng tự động
            string serverIp = "192.168.0.101";
            int portToRun = 8081;

            // Hiển thị thông tin máy chủ
            Console.WriteLine($"Server IP: {serverIp}");
            Console.WriteLine($"Port: {portToRun}");

            // Chạy server
            runAsServer(serverIp, portToRun);

            Console.ReadLine();
        }

        static void runAsServer(string ipAddress, int port)
        {
            try
            {
                // Tạo địa chỉ IP cho server
                IPAddress ipAddr = IPAddress.Parse(ipAddress);

                // Tạo IPEndPoint với IP và cổng
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, port);

                // Tạo socket TCP/IP
                Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Liên kết socket với địa chỉ IP và cổng
                listener.Bind(localEndPoint);

                // Đặt chế độ lắng nghe, với tối đa 10 kết nối
                listener.Listen(10);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Waiting for connection...");

                // Chấp nhận kết nối từ client
                Socket clientSocket = listener.Accept();

                while (true)
                {
                    // Buffer để lưu dữ liệu nhận được
                    byte[] bytes = new Byte[1024];
                    string data = null;

                    // Nhận dữ liệu từ client
                    while (true)
                    {
                        int numByte = clientSocket.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, numByte);

                        // Nếu nhận được kết thúc chuỗi <EOF>, thoát vòng lặp
                        if (data.IndexOf("<EOF>") > -1)
                            break;
                    }

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Text received -> {0} ", data);

                    // Gửi phản hồi tới client
                    Console.WriteLine("Enter message to send back to client:");
                    byte[] message = Encoding.ASCII.GetBytes(Console.ReadLine() + " <EOF>");
                    clientSocket.Send(message);
                }

                // Đóng kết nối với client
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}

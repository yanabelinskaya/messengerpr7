using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfAppprac7
{
    public partial class Serverchik : Window
    {
        private List<Socket> users = new List<Socket>();
        private Socket socket;
        private string userName;

        public Serverchik(string userName)
        {
            InitializeComponent();
            this.userName = userName;

            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, 3000);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(ipPoint);
            socket.Listen(100);
            ListenToClients();
        }

        private async void ListenToClients()
        {
            while (true)
            {
                var client = await socket.AcceptAsync();
                users.Add(client);
                ReceiveMessages(client);
            }
        }

        private async void ReceiveMessages(Socket client)
        {
            while (true)
            {
                byte[] b = new byte[1024];
                int byteCount = await client.ReceiveAsync(b, SocketFlags.None);
                if (byteCount == 0)
                {
                    users.Remove(client);
                    Dispatcher.Invoke(() => ListNameS.Items.Add($"{client.RemoteEndPoint} отключился."));
                    LogChat($"{client.RemoteEndPoint} отключился.");
                    client.Close();
                    break;
                }

                string message = Encoding.UTF8.GetString(b, 0, byteCount);

                if (message.Trim() == "/disconnect")
                {
                    users.Remove(client);
                    Dispatcher.Invoke(() => ListNameS.Items.Add($"{client.RemoteEndPoint} отключился по команде."));
                    LogChat($"{client.RemoteEndPoint} отключился по команде.");
                    client.Close();
                    break;
                }

                Dispatcher.Invoke(() => ListNameS.Items.Add($"[Сообщение от {client.RemoteEndPoint}]: {message}"));
                LogChat($"[Сообщение от {client.RemoteEndPoint}]: {message}");

                foreach (var user in users)
                {
                    SendMessages(user, message);
                }
            }
        }

        private async void SendMessages(Socket client, string message)
        {
            if (message.Trim() == "/disconnect")
            {
                client.Close();
                return;
            }

            byte[] b = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(b, SocketFlags.None);
        }

        private void LogChat(string message)
        {
            string logFilePath = "chat_logs.txt";
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }

        private void SendS_Click(object sender, RoutedEventArgs e)
        {
            string message = TextBoxNameS.Text.Trim();
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Пожалуйста, введите сообщение.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string fullMessage = $"[{DateTime.Now}] [{userName}]: {message}";

            ListNameS.Items.Add(fullMessage);
            LogChat(fullMessage);

            foreach (var user in users)
            {
                SendMessages(user, fullMessage);
            }

            TextBoxNameS.Clear();
        }


        private void Logs_Click(object sender, RoutedEventArgs e)
        {
            ChatLogs logsWindow = new ChatLogs();
            logsWindow.Show();
        }

        private void BackS_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
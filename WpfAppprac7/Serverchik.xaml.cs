using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfAppprac7
{
    public partial class Serverchik : Window
    {
        private List<Socket> users = new List<Socket>();
        private Dictionary<Socket, string> userNames = new Dictionary<Socket, string>();
        private Socket socket;
        private string serverName;

        public Serverchik(string serverName)
        {
            InitializeComponent();
            this.serverName = serverName;

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
                userNames[client] = serverName;
                UpdateUserList();
                ReceiveMessages(client);
            }
        }

        private async void ReceiveMessages(Socket client)
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                int byteCount = await client.ReceiveAsync(buffer, SocketFlags.None);
                if (byteCount == 0)
                {
                    if (userNames.TryGetValue(client, out string userName))
                    {
                        string disconnectMessage = $"{userName} отключился.";
                        Dispatcher.Invoke(() => ListNameS.Items.Add(disconnectMessage));
                        LogChat(disconnectMessage);
                        userNames.Remove(client);
                        UpdateUserList();
                    }
                    users.Remove(client);
                    client.Close();
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, byteCount);

                if (message.Trim() == "/disconnect")
                {
                    foreach (var user in users)
                    {
                        if (user != client)
                        {
                            SendMessages(user, "/disconnect");
                            user.Close();
                        }
                    }
                    Dispatcher.Invoke(() => ListNameS.Items.Add("Мессенджер завершен по команде от сервера."));
                    socket.Close();
                    BackToMain();
                    break;
                }

                Dispatcher.Invoke(() => ListNameS.Items.Add(message));
                LogChat(message);

                foreach (var user in users)
                {
                    if (user != client)
                    {
                        SendMessages(user, message);
                    }
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

        private void BackToMain()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void UpdateUserList()
        {
            List<string> userNamesList = new List<string>();
            foreach (var entry in userNames)
            {
                userNamesList.Add(entry.Value);
            }
            Users.ItemsSource = userNamesList;
        }

        private void SendS_Click(object sender, RoutedEventArgs e)
        {
            string message = TextBoxNameS.Text.Trim();
            if (string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show("Пожалуйста, введите сообщение.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ListNameS.Items.Add($"[Сообщение от сервера]: {message}");
            LogChat($"[Сообщение от сервера]: {message}");

            foreach (var user in users)
            {
                SendMessages(user, message);
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

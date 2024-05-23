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


        private bool isRunning = true;

        private async void ListenToClients()
        {
            while (isRunning)
            {
                var client = await socket.AcceptAsync();
                users.Add(client);
                userNames[client] = serverName; // Добавляем имя сервера в список пользователей
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
                        SendMessageToAllUsers("/updateUsers " + string.Join(",", userNames.Values));
                    }
                    users.Remove(client);
                    client.Close();
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, byteCount);

                if (message.Trim() == "/disconnect")
                {
                    if (userNames.TryGetValue(client, out string userName))
                    {
                        string disconnectMessage = $"{userName} отключился.";
                        Dispatcher.Invoke(() => ListNameS.Items.Add(disconnectMessage));
                        LogChat(disconnectMessage);
                        userNames.Remove(client);
                        UpdateUserList();
                        SendMessageToAllUsers("/updateUsers " + string.Join(",", userNames.Values));
                    }
                    users.Remove(client);
                    client.Close();
                    break;
                }

                if (message.Contains("подключился к чату"))
                {
                    string userName = message.Split(' ')[0];
                    userNames[client] = userName;
                    UpdateUserList();
                    SendMessageToAllUsers("/updateUsers " + string.Join(",", userNames.Values));
                    string connectMessage = $"{userName} подключился.";
                    Dispatcher.Invoke(() => ListNameS.Items.Add(connectMessage));
                    LogChat(connectMessage);
                }
                else
                {
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
                userNamesList.Add(serverName);
                userNamesList.Add(entry.Value);
                
            }
            
            Users.ItemsSource = userNamesList;
        }

        private async void SendMessageToAllUsers(string message)
        {
            byte[] b = Encoding.UTF8.GetBytes(message);
            foreach (var user in users)
            {
                await user.SendAsync(b, SocketFlags.None);
            }
        }

        private async void BackS_Click(object sender, RoutedEventArgs e)
        {
            isRunning = false; // Установка флага для завершения цикла
            foreach (var user in users)
            {
                SendMessages(user, "/disconnect"); // Отправляем сообщение о выходе всем клиентам
                user.Close(); // Закрываем соединение с каждым клиентом
            }
            socket.Close(); // Закрываем сокет сервера
            BackToMain(); // Переходим на главную страницу
        }


        private async void SendS_Click(object sender, RoutedEventArgs e)
        {
            string message = TextBoxNameS.Text.Trim();
            if (!string.IsNullOrWhiteSpace(message))
            {
                string fullMessage = $"{DateTime.Now} [{serverName}]: {message}";
                Dispatcher.Invoke(() => ListNameS.Items.Add(fullMessage));
                LogChat(fullMessage);

                foreach (var user in users)
                {
                    SendMessages(user, fullMessage);
                }

                TextBoxNameS.Clear();
            }
        }

        private void Logs_Click(object sender, RoutedEventArgs e)
        {
            string logFilePath = "chat_logs.txt";
            if (System.IO.File.Exists(logFilePath))
            {
                System.Diagnostics.Process.Start("notepad.exe", logFilePath);
            }
            else
            {
                MessageBox.Show("Log file not found.");
            }
        }
    }
}
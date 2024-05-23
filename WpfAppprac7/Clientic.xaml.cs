using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfAppprac7
{
    public partial class Clientic : Window
    {
        private Socket socket;
        private string userName;
        private string ipAddress;

        public Clientic(string userName, string ipAddress)
        {
            InitializeComponent();
            this.userName = userName;
            this.ipAddress = ipAddress;

            ConnectToServer();
            GetMessages();
        }

        private void ConnectToServer()
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipAddress, 3000);
                SendMessages($"{userName} подключился к чату.");

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к серверу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                BackToMain();
            }
        }

        private async void SendMessages(string message)
        {
            byte[] b = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(b, SocketFlags.None);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string message = TextBoxName.Text.Trim();

            if (message == "/disconnect")
            {
                SendMessages($"{userName} отключился от чата.");
                socket.Close();
                BackToMain();
                return;
            }

            string fullMessage = $"{DateTime.Now} [{userName}]: {message}";
            SendMessages(fullMessage);
            Dispatcher.Invoke(() => ListName.Items.Add(fullMessage));
            TextBoxName.Clear();
        }

        private async void GetMessages()
        {
            while (true)
            {
                byte[] buffer = new byte[1024];
                int byteCount = await socket.ReceiveAsync(buffer, SocketFlags.None);
                if (byteCount == 0)
                {
                    Dispatcher.Invoke(() => ListName.Items.Add("Отключен от сервера."));
                    socket.Close();
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, byteCount);

                if (message.StartsWith("/updateUsers"))
                {
                    string[] users = message.Substring("/updateUsers".Length).Trim().Split(',');
                    Dispatcher.Invoke(() => Users.ItemsSource = users);
                }
                else
                {
                    Dispatcher.Invoke(() => ListName.Items.Add(message));
                }
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            SendMessages($"{userName} отключился от чата.");
            socket?.Close(); // Закрываем сокет клиента
            BackToMain(); // Переходим на главную страницу
        }

        private void BackToMain()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
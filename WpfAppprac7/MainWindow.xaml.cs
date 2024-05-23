using System;
using System.Net.Sockets;
using System.Windows;

namespace WpfAppprac7
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Создание нового чата с новым IP-адресом
        private void NewChat_Click(object sender, RoutedEventArgs e)
        {
            string userName = UserName.Text.Trim();

            if (string.IsNullOrWhiteSpace(userName))
            {
                MessageBox.Show("Пожалуйста, введите имя пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Serverchik serverWindow = new Serverchik(userName);
            serverWindow.Show();
            this.Close();
        }

        // Подключение к чату
        private void Сonnect_Click(object sender, RoutedEventArgs e)
        {
            string userName = UserName.Text.Trim();
            string ipAddress = IP.Text.Trim();

            // Проверяем валидность введенного имени пользователя и IP-адреса
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(ipAddress))
            {
                MessageBox.Show("Пожалуйста, введите имя пользователя и IP-адрес.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Clientic clientWindow = new Clientic(userName, ipAddress);
            clientWindow.Show();
            this.Close();
        }
    }
}
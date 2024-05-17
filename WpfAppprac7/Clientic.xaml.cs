using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfAppprac7
{
    /// <summary>
    /// Логика взаимодействия для Clientic.xaml
    /// </summary>
    public partial class Clientic : Window
    {
        Socket socket;
        public Clientic()
        {
            InitializeComponent();
            socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("26.131.153.105", 3000);

            GetMessages();
        }

        private async void SendMessages(string message)
        {
            byte[] b = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(b, SocketFlags.None);
        }

        //отправка сообщения 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendMessages(TextBoxName.Text);
        }

        private async void GetMessages()
        {
            while (true)
            {
                byte[] b = new byte[1024];
                await socket.ReceiveAsync(b, SocketFlags.None);
                string message = Encoding.UTF8.GetString(b);

                ListName.Items.Add(message);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}

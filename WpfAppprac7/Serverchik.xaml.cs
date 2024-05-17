using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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
    /// Логика взаимодействия для Serverchik.xaml
    /// </summary>
    public partial class Serverchik : Window
    {
        List<Socket> users = new List<Socket>();
        Socket socket;
        public Serverchik()
        {
            InitializeComponent();
            socket = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream,ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 3000));
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
                await client.ReceiveAsync(b, SocketFlags.None);
                string message = Encoding.UTF8.GetString(b);


                ListNameS.Items.Add(message);

                foreach(var item in users)
                {
                    SendMessage(item, message);
                }
            }
        }
        private async void SendMessage(Socket client, string message)
        {
            byte[] b = Encoding.UTF8.GetBytes(message);
            await client.SendAsync(b, SocketFlags.None);
        }

        private void BackS_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}

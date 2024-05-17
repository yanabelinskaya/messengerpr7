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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfAppprac7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket socket;
        public MainWindow()
        {
            InitializeComponent();
            

        }
        //Создание нового чата с новым ip адресом
        private void NewChat_Click(object sender, RoutedEventArgs e)
        {

        }
        //Подключение к чату
        private void Сonnect_Click(object sender, RoutedEventArgs e)
        {
            Clientic serv = new Clientic();
            serv.Show();
            this.Close();
        }
    }
}

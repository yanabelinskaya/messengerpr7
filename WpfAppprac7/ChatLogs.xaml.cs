using System.IO;
using System.Windows;

namespace WpfAppprac7
{
    public partial class ChatLogs : Window
    {
        public ChatLogs()
        {
            InitializeComponent();
            LoadLogs();
        }

        private void LoadLogs()
        {
            string logFilePath = "chat_logs.txt";
            if (File.Exists(logFilePath))
            {
                LogTextBox.Text = File.ReadAllText(logFilePath);
            }
            else
            {
                LogTextBox.Text = "Логи чата отсутствуют.";
            }
        }
    }
}
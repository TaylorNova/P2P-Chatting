using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MyQQ
{
    /// <summary>
    /// AddMessBox.xaml 的交互逻辑
    /// </summary>
    public partial class AddMessBox : Window
    {
        string mess;
        public string receiveOrNot = null;
        public AddMessBox(string tempmess)
        {
            InitializeComponent();
            mess = tempmess;
            messege.Text = mess;
        }

        private void RefuseButton_Click(object sender, RoutedEventArgs e)
        {
            receiveOrNot = "NO";
            this.Close();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            receiveOrNot = "OK";
            this.Close();
        }
    }
}

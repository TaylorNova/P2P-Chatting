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
    /// chooseWindow.xaml 的交互逻辑
    /// </summary>
    public partial class chooseWindow : Window
    {
        public string[] Choose;//选择的好友
        public int done = 0;
        public chooseWindow(string[] friendList)
        {
            InitializeComponent();
            int friendNum = friendList.Length;
            for (int i = 0; i < friendNum; i++)
            {
                ListViewItem newItem = new ListViewItem();//控件
                CheckBox che = new CheckBox();
                newItem.Content = che;
                Style myStyle = (Style)this.FindResource("MaterialDesignUserForegroundCheckBox");
                che.Style = myStyle;
                che.FontSize = 10;
                che.Width = 130;
                che.Content = friendList[i];
                che.Margin = new Thickness(16,4,16,0);
                ChooseList.Items.Add(newItem);
            }
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> strList = new List<string>();
            foreach (ListViewItem tempItem in ChooseList.Items)
            {
                string reStr = tempItem.Content.ToString();
                string[] reStr_list = reStr.Split(new char[2] { ' ', ':' });
                if (reStr_list[4] == "True")
                {
                    strList.Add(reStr_list[2]);
                }
            }
            Choose = strList.ToArray();
            done = 1;
            this.Close();
        }
    }
}

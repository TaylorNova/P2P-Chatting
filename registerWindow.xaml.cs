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
using System.Text.RegularExpressions;
using System.Data.SQLite;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Data;
using System.IO;

namespace MyQQ
{
    /// <summary>
    /// registerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class registerWindow : Window
    {
        public registerWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void MinimizeWindow_Exec(object sender, ExecutedRoutedEventArgs e)//最小化窗口
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)//窗口移动
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)//关闭窗口按钮
        {
            this.Close();
        }

        private void NameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)//用户名输入正则表达式
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)//注册
        {
            //打开数据库并建立/打开表
            string dbFilename = "myQQdb.db";
            SQLiteConnection dbCore = new SQLiteConnection("data source=" + dbFilename);
            dbCore.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS users(ID CHAR(10) NOT NULL, PASSWORD VARCHAR(20) NOT NULL);";
            SQLiteCommand cmd = new SQLiteCommand(commandText, dbCore);
            int result = cmd.ExecuteNonQuery();

            string tempID = this.NameTextBox.Text;
            string tempPassword = this.PasswordBox.Text;
            string tempPasswordCheck = this.PasswordCheckBox.Text;
            if (!string.IsNullOrEmpty(tempID) && !string.IsNullOrEmpty(tempPassword) && !string.IsNullOrEmpty(tempPasswordCheck))//判断输入是否为空
            {
                if (tempPassword != tempPasswordCheck)//判断两次输入密码是否相同
                {
                    messegebox warning = new messegebox("两次密码输入不相同");
                    warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    warning.Show();
                }
                else
                {
                    //向数据库中添加信息
                    cmd.CommandText = "INSERT INTO users(ID,PASSWORD) VALUES(@ID, @PASSWORD)";//添加数据
                    cmd.Parameters.Add("ID", DbType.String).Value = tempID;
                    cmd.Parameters.Add("PASSWORD", DbType.String).Value = tempPassword;
                    result = cmd.ExecuteNonQuery();
                    if (result == 1)
                    {
                        messegebox confirm = new messegebox("注册成功");
                        confirm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        confirm.Show();
                        dbCore.Close();
                        this.Close();
                    }
                }
            }
            else
            {
                //错误提示
                messegebox warning2 = new messegebox("用户名或密码不能为空");
                warning2.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                warning2.Show();
            }
        }
    }
}

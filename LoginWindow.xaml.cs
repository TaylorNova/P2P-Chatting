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
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            //test();//意外退出后的辅助下线
            //this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        private void MinimizeWindow_Exec(object sender, ExecutedRoutedEventArgs e)//窗口最小化
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)//关闭按钮
        {
            test();
            System.Windows.Application.Current.Shutdown();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)//登录按钮
        {
            string tempName = this.NameTextBox.GetLineText(0);//当前输入的用户名和密码
            string tempPasw = this.PasswordsBox.Password;
            if (!string.IsNullOrEmpty(tempName) && !string.IsNullOrEmpty(tempPasw))//判断是否为空
            {
                //数据库加载
                string dbFilename = "myQQdb.db";
                SQLiteConnection dbCore = new SQLiteConnection("data source=" + dbFilename);
                dbCore.Open();
                string commandText = "select * from users;";
                SQLiteCommand cmd = new SQLiteCommand(commandText, dbCore);
                int result = cmd.ExecuteNonQuery();
                SQLiteDataReader reader = cmd.ExecuteReader();
                bool signforlogin = false;
                while (reader.Read())
                {
                    //检查账号密码是否正确
                    string checkID = (string)reader["ID"];
                    string checkPassword = (string)reader["PASSWORD"];
                    if (checkID == tempName && checkPassword == tempPasw)
                    {
                        signforlogin = true;
                    }
                }
                if (signforlogin == true)
                {
                    string strRe = null;
                    string sendMe = "q" + tempName;
                    ServerConnect tempSC = new ServerConnect();
                    strRe = tempSC.ServerQuery(sendMe);
                    Console.WriteLine(strRe);
                    if (strRe == "n")
                    {
                        //登录，告知服务器上线信息
                        string strReceive = null;
                        string sendMess = tempName + "_net2019";
                        ServerConnect tempsev = new ServerConnect();
                        strReceive = tempsev.ServerQuery(sendMess);
                        if (strReceive == "lol")
                        {
                            MainWindow a = new MainWindow(tempName);
                            a.Show();
                            this.Close();
                        }
                        else
                        {
                            //错误提示
                            messegebox warning = new messegebox("登录失败：未知错误");
                            warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            warning.Show();
                        }
                    }
                    else
                    {
                        //错误提示
                        messegebox warning = new messegebox("账号已在线，无法登陆");
                        warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        warning.Show();
                    }
                }
                else
                {
                    //错误提示
                    messegebox warning = new messegebox("用户名或密码错误");
                    warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    warning.Show();
                }
            }
            else
            {
                //错误提示
                messegebox warning = new messegebox("用户名或密码为空");
                warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                warning.Show();
            }
        }

        private void NameTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)//用户名输入正则表达式，只能输入数字
        {
            Regex re = new Regex("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)//注册按钮
        {
            registerWindow r1 = new registerWindow();
            r1.Show();
        }

        public void test()
        {
            string Name = "2017011540";
            string strReceive = null;
            string sendMess = "logout" + Name;
            ServerConnect tempsev = new ServerConnect();
            strReceive = tempsev.ServerQuery(sendMess);

            string Name2 = "2017011565";
            string strReceive2 = null;
            string sendMess2 = "logout" + Name2;
            ServerConnect tempsev2 = new ServerConnect();
            strReceive2 = tempsev2.ServerQuery(sendMess2);

            string Name3 = "2016011889";
            string strReceive3 = null;
            string sendMess3 = "logout" + Name3;
            ServerConnect tempsev3 = new ServerConnect();
            strReceive3 = tempsev3.ServerQuery(sendMess3);

            string Name4 = "2017011574";
            string strReceive4 = null;
            string sendMess4 = "logout" + Name4;
            ServerConnect tempsev4 = new ServerConnect();
            strReceive4 = tempsev4.ServerQuery(sendMess4);
        }
    }
}

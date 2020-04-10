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
using System.Threading;
using System.Windows.Threading;
//using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Data;
using System.IO;
using System.Data.SQLite;
using System.Text.RegularExpressions;



namespace MyQQ
{
    /// <summary>
    /// MessegeList.xaml 的交互逻辑
    /// </summary>
    public partial class MessegeList : Window
    {
        public struct Messeges
        {
            public string sendID;
            public string content;
            public string type;
        }
        public MessegeList(Messeges[] inputMes,int messegeNum)
        {
            InitializeComponent();
            for (int k = 1; k < messegeNum; k++)
            {
                if (inputMes[k].type == "ADDF")
                {
                    if (k == 1)
                    {
                        expander1.Header = "添加好友信息";
                        textblock1.Text = inputMes[k].sendID + " 想添加您为好友!";
                        intextblock1.Text = "请问您同意吗？";
                    }
                }
            }
        }
    }
}

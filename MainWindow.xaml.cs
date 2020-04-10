using System.Windows;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
using Microsoft.Win32;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Media;
using Netframe.Event;
using Netframe.Model;
using Netframe.Tool;
using Netframe.Core;

namespace MyQQ
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public struct Records//聊天记录
        {
            public string sendID;//发送者ID
            public string receiveID;//接收者ID
            public string SRtime;//发送/接收时间
            public string content;//内容
        };

        public struct GroupRecords//群聊记录
        {
            public string groupName;//群聊名称
            public string sendID;//发送者ID
            public string SRtime;//发送接收时间
            public string content;//内容
        };

        public struct ChatGroup//群聊
        {
            public string groupName;//群聊名称
            public string[] memberList;//成员名单
            public int memberNum;//成员数目
        };

        public class ChatRecord//聊天记录存储类
        {
            public Records[] myChatRecord;
            public int myChatRecordNum;
            string UserID = "";
            public ChatRecord(string tempUserID)
            {
                UserID = tempUserID;
                string dbFilename = "myQQdb.db";
                SQLiteConnection dbCore = new SQLiteConnection("data source=" + dbFilename);
                dbCore.Open();
                string commandText = "select * from chatrecords;";
                SQLiteCommand cmd = new SQLiteCommand(commandText, dbCore);
                int result = cmd.ExecuteNonQuery();
                SQLiteDataReader reader = cmd.ExecuteReader();
                List<Records> chatRecord_list = new List<Records>();
                while (reader.Read())
                {
                    //读取聊天记录信息
                    string checkSendID = (string)reader["SENDID"];
                    string checkReceiveID = (string)reader["RECEIVEID"];
                    string checkSRtime = (string)reader["SRTIME"];
                    string checkContent = (string)reader["CONTENT"];
                    if (checkSendID == UserID || checkReceiveID == UserID)
                    {
                        Records tempre = new Records();
                        tempre.sendID = checkSendID;
                        tempre.receiveID = checkReceiveID;
                        tempre.SRtime = checkSRtime;
                        tempre.content = checkContent;
                        chatRecord_list.Add(tempre);
                    }
                    myChatRecord = chatRecord_list.ToArray();
                    myChatRecordNum = myChatRecord.Length;
                }
            }
        }

        public class ChatGroupRecord//群聊记录存储类
        {
            public GroupRecords[] myChatGroupRecord;
            public int myChatGroupRecordNum;
            string UserID = "";
            public ChatGroupRecord(string tempUserID)
            {
                UserID = tempUserID;
                string dbFilename = "myQQdb.db";
                SQLiteConnection dbCore = new SQLiteConnection("data source=" + dbFilename);
                dbCore.Open();
                string commandText = "select * from chatgrouprecords;";
                SQLiteCommand cmd = new SQLiteCommand(commandText, dbCore);
                int result = cmd.ExecuteNonQuery();
                SQLiteDataReader reader = cmd.ExecuteReader();
                List<GroupRecords> chatGroupRecord_list = new List<GroupRecords>();
                while (reader.Read())
                {
                    //读取聊天记录信息
                    string checkGroupName = (string)reader["GROUPNAME"];
                    string checkSendID = (string)reader["SENDID"];
                    string checkSRtime = (string)reader["SRTIME"];
                    string checkContent = (string)reader["CONTENT"];
                    GroupRecords tempre = new GroupRecords();
                    tempre.groupName = checkGroupName;
                    tempre.sendID = checkSendID;
                    tempre.SRtime = checkSRtime;
                    tempre.content = checkContent;
                    chatGroupRecord_list.Add(tempre);
                    myChatGroupRecord = chatGroupRecord_list.ToArray();
                    myChatGroupRecordNum = myChatGroupRecord.Length;
                }
            }
        }

        public class Friend//好友信息存储类
        {
            public string[] friendList;
            public int friendNum;
            string UserID = "";
            public Friend(string tempUserID)
            {
                UserID = tempUserID;
                string dbFilename = "myQQdb.db";
                SQLiteConnection dbCore = new SQLiteConnection("data source=" + dbFilename);
                dbCore.Open();
                string commandText = "select * from friends;";
                SQLiteCommand cmd = new SQLiteCommand(commandText, dbCore);
                int result = cmd.ExecuteNonQuery();
                SQLiteDataReader reader = cmd.ExecuteReader();
                List<string> friend_list = new List<string>();
                while (reader.Read())
                {
                    //读取好友信息
                    string checkID = (string)reader["ID"];
                    string checkFriendID = (string)reader["FRIENDID"];
                    if (checkID == UserID)
                    {
                        friend_list.Add(checkFriendID);
                    }
                }
                friendList = friend_list.ToArray();
                friendNum = friendList.Length;
            }

            public void addFriend(string newFriend)
            {
                List<string> friend_list = new List<string>(friendList);
                friend_list.Add(newFriend);
                friendList = friend_list.ToArray();
                friendNum = friendList.Length;
            }
        }

        public class ChatGroups//群聊信息存储类
        {
            public ChatGroup[] chatGroupList;
            public int chatGroupNum = 0;
            string UserID = "";
            public ChatGroups(string tempUserID)
            {
                UserID = tempUserID;
                string dbFilename = "myQQdb.db";
                SQLiteConnection dbCore = new SQLiteConnection("data source=" + dbFilename);
                dbCore.Open();
                //读取群聊名称
                string commandText = "select * from chatgroups;";
                SQLiteCommand cmd = new SQLiteCommand(commandText, dbCore);
                int result = cmd.ExecuteNonQuery();
                SQLiteDataReader reader = cmd.ExecuteReader();
                List<ChatGroup> chatGroup_list = new List<ChatGroup>();
                while (reader.Read())
                {
                    //读取群聊信息
                    string checkgroupName = (string)reader["GROUPNAME"];
                    bool sign = false;
                    for (int k = 0; k < chatGroupNum; k++)
                    {
                        if (chatGroup_list[k].groupName == checkgroupName)
                        {
                            sign = true;
                            break;
                        }
                    }
                    if (sign == false)
                    {
                        ChatGroup tempgro = new ChatGroup();
                        tempgro.groupName = checkgroupName;
                        tempgro.memberList = new string[] { };
                        tempgro.memberNum = 0;
                        chatGroup_list.Add(tempgro);
                    }
                    chatGroupList = chatGroup_list.ToArray();
                    chatGroupNum = chatGroupList.Length;
                }
                //读取群聊中的成员
                string commandText2 = "select * from chatgroups;";
                SQLiteCommand cmd2 = new SQLiteCommand(commandText2, dbCore);
                int result2 = cmd2.ExecuteNonQuery();
                SQLiteDataReader reader2 = cmd2.ExecuteReader();
                while (reader2.Read())
                {
                    //读取好友信息
                    string checkgroupName = (string)reader2["GROUPNAME"];
                    string checkMember = (string)reader2["MEMBERID"];
                    int findgroup = 0;
                    for (int k = 0; k < chatGroupNum; k++)
                    {
                        if (chatGroupList[k].groupName == checkgroupName)
                        {
                            findgroup = k;
                            break;
                        }
                    }
                    List<string> member_list = new List<string>(chatGroupList[findgroup].memberList);
                    string tempstr = checkMember;
                    member_list.Add(tempstr);
                    chatGroupList[findgroup].memberList = member_list.ToArray();
                    chatGroupList[findgroup].memberNum = chatGroupList[findgroup].memberList.Length;
                }
            }

            public void addGroup(string newGroup)
            {
                int findgroup = -1;
                for (int k = 0; k < chatGroupNum; k++)
                {
                    if (chatGroupList[k].groupName == newGroup)
                    {
                        findgroup = k;
                        break;
                    }
                }
                if (findgroup == -1)
                {
                    List<ChatGroup> chatGroup_List = new List<ChatGroup>(chatGroupList);
                    ChatGroup tempgro = new ChatGroup();
                    tempgro.groupName = newGroup;
                    tempgro.memberList = new string[] { };
                    tempgro.memberNum = 0;
                    chatGroup_List.Add(tempgro);
                    chatGroupList = chatGroup_List.ToArray();
                    chatGroupNum = chatGroupList.Length;
                }
            }

            public void addMember(string setGroupName, string newMember)
            {
                int findgroup = 0;
                for (int k = 0; k < chatGroupNum; k++)
                {
                    if (chatGroupList[k].groupName == setGroupName)
                    {
                        findgroup = k;
                        break;
                    }
                }
                int findMember = -1;
                for (int k = 0; k < chatGroupList[findgroup].memberNum; k++)
                {
                    if (chatGroupList[findgroup].memberList[k] == newMember)
                    {
                        findMember = k;
                        break;
                    }
                }
                if (findMember == -1)
                {
                    List<string> member_list = new List<string>(chatGroupList[findgroup].memberList);
                    string tempstr = newMember;
                    member_list.Add(tempstr);
                    chatGroupList[findgroup].memberList = member_list.ToArray();
                    chatGroupList[findgroup].memberNum = chatGroupList[findgroup].memberList.Length;
                }
            }
        }

        string UserName = "";
        Friend myFriend;
        ChatGroups myChatGroup;
        ChatRecord myRecord;
        ChatGroupRecord myGroupRecord;
        public bool appRun = true;//当appRun为false时，程序结束
        public string curInparam;//当前的Inparam指令
        public string MyIP;//我的IP
        private static Listener li;//一个静态的Listener对象
        private string inparam = "start";//初始化指令为监听
        private string precase = null;//前一个状态
        private string[] aInput;//数组aInput用于接受用户输入的信息
        public string[] sendIDList;//文件群发人物列表
        public string receAddID;//接收到添加好友确认信息的ID
        public string receAddGroup;//接收到邀请加入的群聊
        public string receAddState;//接收到添加好友确认信息的状态
        public string receAddIP;//接收到添加好友信息的IP
        public string inviteID;//发起语音聊天的ID
        public string cheVoiceState;//接收语音聊天的状态
        public string preChoose = "";//上次的聊天选择
        public string curChoose = "";//本次的聊天选择
        public bool[] FriendsOl;//好友在线信息
        public bool chooseChange;//聊天选择改变信息
        public string VoiceObject;//语音聊天对象
        public string VoiceObjIP;//语音聊天对象IP
        public string SendVoice;//发送语音的文件名
        public string ReceVoice;//接收语音的文件名
        public string ReVoiceSign = "N";//语音接收标志
        public MsgTranslator tran = null;//语音发送相关类
        public AddMessBox tips;//好友添加确认窗口
        public AddMessBox voicetips;//语音聊天确认窗口
        public chooseWindow chw;//选择群发对象窗口
        public VoiceWaiting myWait;//等待语音聊天接收界面
        public bool signForSend = false;//发送标志
        public string[] groupMemberList;//选择的群组名单
        public string chooseGroup;//选择的群聊
        public int chatstate;//选择状态：群聊or单聊
        public string chosenFile;//选择的文件
        public string simpleFileName;//选择文件的名字
        public string sendFileState = "mes";//发送状态—消息or文件
        public byte[] receAllByte;//接收到的所有数据byte
        public string chosenFilePath;//选择的文件保存路径
        public Thread th2;//处理线程
        public Thread th3;//等待添加好友按钮确认线程
        public Thread th4;//查询好友是否在线的线程
        public Thread th5;//监测选中好友情况
        public Thread th6;//更新treeview的情况
        public Thread th7;//等待群聊邀请确认线程
        public Thread th8;//保存文件的线程
        public Thread th9;//等待确认按钮的线程
        public Thread th10;//等待确认按钮的线程
        public Thread th11;//等待对方接收语音聊天与否的线程
        public Thread th12;//发送语音的线程
        public Thread th13;//接收语音的线程
        public int currentPort = 0;
        public int targetPort = 0;

        public MainWindow(string tempUserName)
        {
            InitializeComponent();
            UserName = tempUserName;
            currentPort = 4000 + int.Parse(UserName.Substring(UserName.Length - 4));//当前端口
            string hostName = Dns.GetHostName();
            MyIP = Dns.GetHostEntry(hostName).AddressList.FirstOrDefault(d => d.AddressFamily.ToString().Equals("InterNetwork")).ToString();//获取本机IP
            IdNum.Text = UserName;
            myFriend = new Friend(tempUserName);
            myChatGroup = new ChatGroups(tempUserName);
            myRecord = new ChatRecord(tempUserName);
            myGroupRecord = new ChatGroupRecord(tempUserName);
            //好友列表初始化
            for (int k = 0; k < myFriend.friendNum; k++)
            {
                TreeViewItem TVI = new TreeViewItem();
                TVI.Header = myFriend.friendList[k];
                friendTreelist1.Items.Add(TVI);
            }
            for (int k = 0; k < myChatGroup.chatGroupNum; k++)
            {
                for (int i = 0; i < myChatGroup.chatGroupList[k].memberNum; i++)
                {
                    if (myChatGroup.chatGroupList[k].memberList[i] == UserName)
                    {
                        TreeViewItem TVI2 = new TreeViewItem();
                        TVI2.Header = myChatGroup.chatGroupList[k].groupName;
                        chatGroup.Items.Add(TVI2);
                    }
                }
            }
            th2 = new Thread(new ThreadStart(process));//新建一个用于处理各类事件的线程
            th2.Start();//打开新线程
            th4 = new Thread(new ThreadStart(checkForOL));//新建线程
            th4.Start();//打开新线程
            th5 = new Thread(new ThreadStart(friendChat));//新建线程
            th5.Start();//打开新线程
            th6 = new Thread(new ThreadStart(refreshTree));//新建线程
            th6.Start();//打开新线程
        }

        public void preserveFile()//保存文件
        {
            Thread.Sleep(1);
            if (!string.IsNullOrEmpty(chosenFilePath))
            {
                FileStream fs = new FileStream(chosenFilePath, FileMode.Create);
                fs.Write(receAllByte, 0, receAllByte.Length);
                fs.Close();
            }
        }

        public void refreshTree()//时刻更新聊天树
        {
            while (true)
            {
                Thread.Sleep(1);
                if (chooseChange == true)
                {
                    App.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        ChatList.Items.Clear();
                    }));
                    if (chatstate == 1)
                    {
                        //聊天记录初始化
                        for (int k = 0; k < myRecord.myChatRecordNum; k++)
                        {
                            if (myRecord.myChatRecord[k].sendID == UserName && myRecord.myChatRecord[k].receiveID == curChoose)
                            {
                                App.Current.Dispatcher.Invoke((Action)(() =>
                                {
                                    //聊天记录显示
                                    ListViewItem newItem = new ListViewItem();//控件
                                    ListViewItem newItem2 = new ListViewItem();
                                    Card newCard = new Card();
                                    TextBlock newTextBlock = new TextBlock();
                                    TextBlock newTextBlock2 = new TextBlock();
                                    newItem2.Content = newTextBlock2;//嵌套关系
                                    newItem.Content = newCard;
                                    newCard.Content = newTextBlock;
                                    //样式
                                    newItem.HorizontalContentAlignment = HorizontalAlignment.Right;
                                    newItem.Width = 571;
                                    newItem2.HorizontalContentAlignment = HorizontalAlignment.Center;
                                    newItem2.Width = 571;
                                    SolidColorBrush newBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8490C3"));
                                    Thickness newThick = new Thickness(8);
                                    newCard.Background = newBrush;
                                    newCard.Width = 200;
                                    newCard.Padding = newThick;
                                    newCard.UniformCornerRadius = 6;
                                    newCard.HorizontalContentAlignment = HorizontalAlignment.Right;
                                    SolidColorBrush newBrush2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                                    newTextBlock.TextWrapping = TextWrapping.Wrap;
                                    newTextBlock.Foreground = newBrush2;
                                    newTextBlock.Text = myRecord.myChatRecord[k].content;
                                    newTextBlock2.Text = "------ " + myRecord.myChatRecord[k].SRtime + " ------";
                                    newTextBlock2.VerticalAlignment = VerticalAlignment.Center;
                                    newTextBlock2.HorizontalAlignment = HorizontalAlignment.Center;
                                    ChatList.Items.Add(newItem2);//添加聊天记录并显示
                                    ChatList.Items.Add(newItem);
                                }));
                            }
                            else if (myRecord.myChatRecord[k].receiveID == UserName && myRecord.myChatRecord[k].sendID == curChoose)
                            {
                                App.Current.Dispatcher.Invoke((Action)(() =>
                                {
                                    //聊天记录显示
                                    ListViewItem newItem = new ListViewItem();//控件
                                    ListViewItem newItem2 = new ListViewItem();//控件
                                    Card newCard = new Card();
                                    TextBlock newTextBlock = new TextBlock();
                                    TextBlock newTextBlock2 = new TextBlock();
                                    newItem2.Content = newTextBlock2;//嵌套关系
                                    newItem.Content = newCard;
                                    newCard.Content = newTextBlock;
                                    //样式
                                    newItem.HorizontalContentAlignment = HorizontalAlignment.Left;
                                    newItem.Width = 571;
                                    newItem2.HorizontalContentAlignment = HorizontalAlignment.Center;
                                    newItem2.Width = 571;
                                    SolidColorBrush newBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFb08bbc"));
                                    Thickness newThick = new Thickness(8);
                                    newCard.Background = newBrush;
                                    newCard.Width = 200;
                                    newCard.Padding = newThick;
                                    newCard.UniformCornerRadius = 6;
                                    newCard.HorizontalContentAlignment = HorizontalAlignment.Right;
                                    SolidColorBrush newBrush2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                                    newTextBlock.TextWrapping = TextWrapping.Wrap;
                                    newTextBlock.Foreground = newBrush2;
                                    newTextBlock.Text = myRecord.myChatRecord[k].content;
                                    newTextBlock2.Text = "------ " + myRecord.myChatRecord[k].SRtime + " ------";
                                    newTextBlock2.VerticalAlignment = VerticalAlignment.Center;
                                    newTextBlock2.HorizontalAlignment = HorizontalAlignment.Center;
                                    ChatList.Items.Add(newItem2);//添加聊天记录并显示
                                    ChatList.Items.Add(newItem);
                                }));
                            }
                        }
                    }
                    else
                    {
                        for (int k = 0; k < myGroupRecord.myChatGroupRecordNum; k++)
                        {
                            if (myGroupRecord.myChatGroupRecord[k].groupName == curChoose)
                            {
                                App.Current.Dispatcher.Invoke((Action)(() =>
                                {
                                    //聊天记录显示
                                    ListViewItem newItem = new ListViewItem();//控件
                                    ListViewItem newItem2 = new ListViewItem();
                                    ListViewItem newItem3 = new ListViewItem();
                                    Card newCard = new Card();
                                    TextBlock newTextBlock = new TextBlock();
                                    TextBlock newTextBlock2 = new TextBlock();
                                    TextBlock newTextBlock3 = new TextBlock();
                                    newItem2.Content = newTextBlock2;//嵌套关系
                                    newItem3.Content = newTextBlock3;
                                    newItem.Content = newCard;
                                    newCard.Content = newTextBlock;
                                    //样式
                                    if (myGroupRecord.myChatGroupRecord[k].sendID == UserName)
                                    {
                                        newItem.HorizontalContentAlignment = HorizontalAlignment.Right;
                                        newItem3.HorizontalContentAlignment = HorizontalAlignment.Right;
                                    }
                                    else
                                    {
                                        newItem.HorizontalContentAlignment = HorizontalAlignment.Left;
                                        newItem3.HorizontalContentAlignment = HorizontalAlignment.Left;
                                    }
                                    newItem.Width = 571;
                                    newItem2.HorizontalContentAlignment = HorizontalAlignment.Center;
                                    newItem2.Width = 571;
                                    newItem3.Width = 571;
                                    SolidColorBrush newBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8490C3"));
                                    Thickness newThick = new Thickness(8);
                                    newCard.Background = newBrush;
                                    newCard.Width = 200;
                                    newCard.Padding = newThick;
                                    newCard.UniformCornerRadius = 6;
                                    newCard.HorizontalContentAlignment = HorizontalAlignment.Right;
                                    SolidColorBrush newBrush2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                                    newTextBlock.TextWrapping = TextWrapping.Wrap;
                                    newTextBlock.Foreground = newBrush2;
                                    newTextBlock.Text = myGroupRecord.myChatGroupRecord[k].content;
                                    newTextBlock2.Text = "------ " + myGroupRecord.myChatGroupRecord[k].SRtime + " ------";
                                    newTextBlock2.VerticalAlignment = VerticalAlignment.Center;
                                    newTextBlock2.HorizontalAlignment = HorizontalAlignment.Center;
                                    newTextBlock3.Text = "------ " + myGroupRecord.myChatGroupRecord[k].sendID;
                                    newTextBlock3.VerticalAlignment = VerticalAlignment.Center;
                                    newTextBlock3.HorizontalAlignment = HorizontalAlignment.Right;
                                    ChatList.Items.Add(newItem2);//添加聊天记录并显示
                                    ChatList.Items.Add(newItem3);
                                    ChatList.Items.Add(newItem);
                                }));
                            }
                        }
                    }
                }
            }
        }

        public void friendChat()//监测选中好友/群聊情况
        {
            while (true)
            {
                Thread.Sleep(1);
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    foreach (TreeViewItem tempItem in friendTreelist1.Items)
                    {
                        if (tempItem.IsSelected == true)
                        {
                            chatstate = 1;
                            curChoose = tempItem.Header.ToString();
                        }
                    }
                    foreach (TreeViewItem tempItem in chatGroup.Items)
                    {
                        if (tempItem.IsSelected == true)
                        {
                            chatstate = 2;
                            curChoose = tempItem.Header.ToString();
                        }
                    }
                }));
                if (curChoose == preChoose)
                {
                    chooseChange = false;
                }
                else
                {
                    chooseChange = true;
                    //Thread.Sleep(1000);
                }
                preChoose = curChoose;
            }
        }

        public void checkForOL()//查看好友在线情况
        {
            while (true)
            {
                Thread.Sleep(2000);//每过2秒查询一次
                FriendsOl = new bool[myFriend.friendNum];//好友在线的记录
                for (int k = 0; k < myFriend.friendNum; k++)
                {
                    //查询好友是否在线
                    string tempID = myFriend.friendList[k];
                    string strReceive = null;
                    string sendMess = "q" + tempID;
                    ServerConnect tempsev = new ServerConnect();
                    strReceive = tempsev.ServerQuery(sendMess);
                    if (strReceive != "n")
                    {
                        FriendsOl[k] = true;
                    }
                    else
                    {
                        FriendsOl[k] = false;
                    }
                }
                //更新UI以显示好友在线情况
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    foreach (TreeViewItem tempItem in friendTreelist1.Items)
                    {
                        string chosenID = tempItem.Header.ToString();
                        bool chosenstate = false;
                        for (int k = 0; k < myFriend.friendNum; k++)
                        {
                            if (chosenID == myFriend.friendList[k])
                            {
                                chosenstate = FriendsOl[k];
                                break;
                            }
                        }
                        if (chosenstate == true)
                        {
                            SolidColorBrush nowBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                            tempItem.Foreground = nowBrush;
                        }
                        else
                        {
                            SolidColorBrush nowBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF000000"));
                            tempItem.Foreground = nowBrush;
                        }
                    }
                }));
            }
        }

        public void process()//各类事件的处理函数
        {
            while (appRun)
            {
                Thread.Sleep(1);
                if (!string.IsNullOrEmpty(inparam))
                {
                    aInput = inparam.Split(' ');
                    //将inparam分割的目的是为了获得字符串中的第一个字，从而执行以下不同的命令
                    switch (aInput[0])
                    {
                        case "send"://如果是"send"，则新建一个Sender对象并发送信息
                            if (signForSend == true)
                            {
                                Sender se = new Sender();
                                se.Send(aInput, targetPort);
                                signForSend = false;
                                inparam = "start";
                            }
                            break;
                        case "groupsend"://如果是"groupsend"，则新建一个GroupSender对象并发送信息
                            if (signForSend == true)
                            {
                                GroupSender se = new GroupSender();
                                se.Send(aInput, groupMemberList, groupMemberList.Length, UserName, chooseGroup);
                                signForSend = false;
                                inparam = "start";
                            }
                            break;
                        case "start"://如果是"start"，则新的开始监听
                            try
                            {
                                if (string.IsNullOrEmpty(precase))
                                {
                                    li = new Listener(MyIP, currentPort);
                                }
                                else if (precase != "start")
                                {
                                    li.listenerRun = false;
                                    li.Stop();
                                    li = new Listener(MyIP, currentPort);
                                }
                            }
                            catch (NullReferenceException)
                            {
                                ;
                            }
                            finally
                            {
                                App.Current.Dispatcher.Invoke((Action)(() =>
                                {
                                    if (li.signForReceive == true)
                                    {
                                        string currentIP = li.senderIP;
                                        string receMes = li.receiveMes;
                                        receAllByte = li.receFileAll;
                                        string[] divRece = receMes.Split(' ');
                                        string[] divCuIP = currentIP.Split(':');
                                        if (divRece[0] == "ADDF")
                                        {
                                            Messtip.Badge = 1;
                                            receAddState = divRece[0];
                                            receAddID = divRece[1];
                                            receAddIP = divCuIP[0];
                                        }
                                        else if (divRece[0] == "ADDG")
                                        {
                                            Messtip.Badge = 1;
                                            receAddState = divRece[0];
                                            receAddGroup = divRece[1];
                                            receAddID = divRece[2];
                                            receAddIP = divCuIP[0];
                                        }
                                        else if (divRece[0] == "CHE")
                                        {
                                            Messtip.Badge = 1;
                                            receAddState = divRece[0];
                                            receAddID = divRece[1];
                                        }
                                        else if (divRece[0] == "CHENO")
                                        {
                                            Messtip.Badge = 1;
                                            receAddState = divRece[0];
                                            receAddID = divRece[1];
                                        }
                                        else if (divRece[0] == "CHEG")
                                        {
                                            Messtip.Badge = 1;
                                            receAddState = divRece[0];
                                            receAddGroup = divRece[1];
                                            receAddID = divRece[2];
                                        }
                                        else if (divRece[0] == "CHEGNO")
                                        {
                                            Messtip.Badge = 1;
                                            receAddState = divRece[0];
                                            receAddGroup = divRece[1];
                                            receAddID = divRece[2];
                                        }
                                        else if (divRece[0] == "VCHAT")
                                        {
                                            inviteID = divRece[1];
                                            receAddIP = divCuIP[0];
                                            voicetips = new AddMessBox(inviteID + " 邀请您语音聊天");
                                            voicetips.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                            voicetips.Show();
                                            th10 = new Thread(new ThreadStart(waitForVoiceButton));//新建一个用于处理各类事件的线程
                                            th10.Start();//打开新线程
                                        }
                                        else if (divRece[0] == "CHEV")
                                        {
                                            receAddID = divRece[1];
                                            VoiceObject = receAddID;
                                            cheVoiceState = "R";
                                        }
                                        else if (divRece[0] == "CHEVNO")
                                        {
                                            receAddID = divRece[1];
                                            VoiceObject = receAddID;
                                            cheVoiceState = "N";
                                        }
                                        else if (divRece[0] == "GROUP")
                                        {
                                            string sedGroup = divRece[1];//发来的信息所在的群聊
                                            string stream = "";//内容
                                            string sedID = "";//发送者ID
                                            int chosenGroup = 0;//选择的群聊索引
                                            for (int i = 2; i < divRece.Length; i++)
                                            {
                                                stream += divRece[i] + " ";
                                            }
                                            string selectedGroup = "";//所选群组
                                            foreach (TreeViewItem tempItem in chatGroup.Items)
                                            {
                                                if (tempItem.IsSelected == true)
                                                {
                                                    selectedGroup = tempItem.Header.ToString();
                                                }
                                            }
                                            for (int k = 0; k < myChatGroup.chatGroupNum; k++)
                                            {
                                                if (myChatGroup.chatGroupList[k].groupName == sedGroup)
                                                {
                                                    chosenGroup = k;
                                                }
                                            }
                                            for (int k = 0; k < myChatGroup.chatGroupList[chosenGroup].memberNum; k++)
                                            {
                                                if (myChatGroup.chatGroupList[chosenGroup].memberList[k] != UserName)
                                                {
                                                    string tempID = myChatGroup.chatGroupList[chosenGroup].memberList[k];//查询好友是否在线
                                                    string strReceive = null;
                                                    string sendMess = "q" + tempID;
                                                    ServerConnect tempsev = new ServerConnect();
                                                    strReceive = tempsev.ServerQuery(sendMess);
                                                    if (strReceive == divCuIP[0])
                                                    {
                                                        sedID = tempID;
                                                    }
                                                }
                                            }
                                            if (selectedGroup == sedGroup)
                                            {
                                                string currentTime = DateTime.Now.ToString();
                                                //聊天记录存储
                                                List<GroupRecords> chatGroupRecord_list = new List<GroupRecords>(myGroupRecord.myChatGroupRecord);
                                                GroupRecords tempre = new GroupRecords();
                                                tempre.groupName = sedGroup;
                                                tempre.sendID = sedID;
                                                tempre.SRtime = currentTime;
                                                tempre.content = stream;
                                                chatGroupRecord_list.Add(tempre);
                                                myGroupRecord.myChatGroupRecord = chatGroupRecord_list.ToArray();
                                                myGroupRecord.myChatGroupRecordNum = myGroupRecord.myChatGroupRecord.Length;
                                                //聊天记录显示
                                                ListViewItem newItem = new ListViewItem();//控件
                                                ListViewItem newItem2 = new ListViewItem();
                                                ListViewItem newItem3 = new ListViewItem();
                                                Card newCard = new Card();
                                                TextBlock newTextBlock = new TextBlock();
                                                TextBlock newTextBlock2 = new TextBlock();
                                                TextBlock newTextBlock3 = new TextBlock();
                                                newItem2.Content = newTextBlock2;//嵌套关系
                                                newItem3.Content = newTextBlock3;
                                                newItem.Content = newCard;
                                                newCard.Content = newTextBlock;
                                                //样式
                                                newItem.HorizontalContentAlignment = HorizontalAlignment.Left;
                                                newItem.Width = 571;
                                                newItem2.HorizontalContentAlignment = HorizontalAlignment.Center;
                                                newItem2.Width = 571;
                                                newItem3.HorizontalContentAlignment = HorizontalAlignment.Left;
                                                newItem3.Width = 571;
                                                SolidColorBrush newBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8490C3"));
                                                Thickness newThick = new Thickness(8);
                                                newCard.Background = newBrush;
                                                newCard.Width = 200;
                                                newCard.Padding = newThick;
                                                newCard.UniformCornerRadius = 6;
                                                newCard.HorizontalContentAlignment = HorizontalAlignment.Right;
                                                SolidColorBrush newBrush2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                                                newTextBlock.TextWrapping = TextWrapping.Wrap;
                                                newTextBlock.Foreground = newBrush2;
                                                newTextBlock.Text = stream;
                                                newTextBlock2.Text = "------ " + currentTime + " ------";
                                                newTextBlock2.VerticalAlignment = VerticalAlignment.Center;
                                                newTextBlock2.HorizontalAlignment = HorizontalAlignment.Center;
                                                newTextBlock3.Text = "------ " + sedID;
                                                newTextBlock3.VerticalAlignment = VerticalAlignment.Center;
                                                newTextBlock3.HorizontalAlignment = HorizontalAlignment.Right;
                                                ChatList.Items.Add(newItem2);//添加聊天记录并显示
                                                ChatList.Items.Add(newItem3);
                                                ChatList.Items.Add(newItem);
                                            }
                                            else
                                            {
                                                string currentTime = DateTime.Now.ToString();
                                                //聊天记录只存储，不显示
                                                List<GroupRecords> chatGroupRecord_list = new List<GroupRecords>(myGroupRecord.myChatGroupRecord);
                                                GroupRecords tempre = new GroupRecords();
                                                tempre.groupName = sedGroup;
                                                tempre.sendID = sedID;
                                                tempre.SRtime = currentTime;
                                                tempre.content = stream;
                                                chatGroupRecord_list.Add(tempre);
                                                myGroupRecord.myChatGroupRecord = chatGroupRecord_list.ToArray();
                                                myGroupRecord.myChatGroupRecordNum = myGroupRecord.myChatGroupRecord.Length;
                                            }
                                        }
                                        else
                                        {
                                            string sedID = "";
                                            foreach (TreeViewItem tempItem in friendTreelist1.Items)
                                            {
                                                string tempID = tempItem.Header.ToString();//查询好友是否在线
                                                string strReceive = null;
                                                string sendMess = "q" + tempID;
                                                ServerConnect tempsev = new ServerConnect();
                                                strReceive = tempsev.ServerQuery(sendMess);
                                                if (strReceive == divCuIP[0])
                                                {
                                                    sedID = tempID;
                                                }
                                            }
                                            string selectedID = "";
                                            foreach (TreeViewItem tempItem in friendTreelist1.Items)
                                            {
                                                if (tempItem.IsSelected == true)
                                                {
                                                    selectedID = tempItem.Header.ToString();
                                                }
                                            }
                                            if (selectedID == sedID)
                                            {
                                                string currentTime = DateTime.Now.ToString();
                                                //聊天记录存储
                                                List<Records> chatRecord_list = new List<Records>(myRecord.myChatRecord);
                                                Records tempre = new Records();
                                                tempre.sendID = sedID;
                                                tempre.receiveID = UserName;
                                                tempre.SRtime = currentTime;
                                                tempre.content = receMes;
                                                chatRecord_list.Add(tempre);
                                                myRecord.myChatRecord = chatRecord_list.ToArray();
                                                myRecord.myChatRecordNum = myRecord.myChatRecord.Length;
                                                //聊天记录显示
                                                ListViewItem newItem = new ListViewItem();//控件
                                                ListViewItem newItem2 = new ListViewItem();//控件
                                                Card newCard = new Card();
                                                TextBlock newTextBlock = new TextBlock();
                                                TextBlock newTextBlock2 = new TextBlock();
                                                newItem2.Content = newTextBlock2;//嵌套关系
                                                newItem.Content = newCard;
                                                newCard.Content = newTextBlock;
                                                //样式
                                                newItem.HorizontalContentAlignment = HorizontalAlignment.Left;
                                                newItem.Width = 571;
                                                newItem2.HorizontalContentAlignment = HorizontalAlignment.Center;
                                                newItem2.Width = 571;
                                                SolidColorBrush newBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFb08bbc"));
                                                Thickness newThick = new Thickness(8);
                                                newCard.Background = newBrush;
                                                newCard.Width = 200;
                                                newCard.Padding = newThick;
                                                newCard.UniformCornerRadius = 6;
                                                newCard.HorizontalContentAlignment = HorizontalAlignment.Right;
                                                SolidColorBrush newBrush2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                                                newTextBlock.TextWrapping = TextWrapping.Wrap;
                                                newTextBlock.Foreground = newBrush2;
                                                newTextBlock.Text = receMes;
                                                newTextBlock2.Text = "------ " + currentTime + " ------";
                                                newTextBlock2.VerticalAlignment = VerticalAlignment.Center;
                                                newTextBlock2.HorizontalAlignment = HorizontalAlignment.Center;
                                                ChatList.Items.Add(newItem2);//添加聊天记录并显示
                                                ChatList.Items.Add(newItem);
                                            }
                                            else
                                            {
                                                string currentTime = DateTime.Now.ToString();
                                                //聊天记录只存储，不显示
                                                List<Records> chatRecord_list = new List<Records>(myRecord.myChatRecord);
                                                Records tempre = new Records();
                                                tempre.sendID = sedID;
                                                tempre.receiveID = UserName;
                                                tempre.SRtime = currentTime;
                                                tempre.content = receMes;
                                                chatRecord_list.Add(tempre);
                                                myRecord.myChatRecord = chatRecord_list.ToArray();
                                                myRecord.myChatRecordNum = myRecord.myChatRecord.Length;
                                            }
                                        }
                                        li.signForReceive = false;
                                    }
                                }));
                            }
                            break;
                        case "stop"://如果是"stop"，则停止监听
                            try
                            {
                                li.listenerRun = false;
                                li.Stop();
                            }
                            catch (NullReferenceException)
                            {
                                ;
                            }
                            break;
                        case "exit"://退出程序
                            try
                            {
                                li.listenerRun = false;
                                li.Stop();
                            }
                            catch (NullReferenceException)
                            {
                                ;
                            }
                            finally
                            {
                                appRun = false;
                            }
                            break;
                        default:
                            Console.WriteLine("Invalid command");
                            break;
                    }
                    precase = aInput[0];
                }
            }
        }

        public void waitForButton()//等待好友添加询问窗口的确认
        {
            while (true)
            {
                Thread.Sleep(1);
                if (!string.IsNullOrEmpty(tips.receiveOrNot))
                {
                    if (tips.receiveOrNot == "OK")
                    {
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            myFriend.addFriend(receAddID);
                            TreeViewItem TVI2 = new TreeViewItem();
                            TVI2.Header = receAddID;
                            friendTreelist1.Items.Add(TVI2);
                        }));
                        inparam = "send" + " " + receAddIP + " " + "CHE" + " " + UserName;
                        targetPort = 4000 + int.Parse(receAddID.Substring(UserName.Length - 4));//当前端口
                        Console.WriteLine(receAddIP);
                        Console.WriteLine(targetPort);
                        signForSend = true;
                        th3.Abort();
                    }
                    else if (tips.receiveOrNot == "NO")
                    {
                        inparam = "send" + " " + receAddIP + " " + "CHENO" + " " + UserName;
                        targetPort = 4000 + int.Parse(receAddID.Substring(UserName.Length - 4));//当前端口
                        signForSend = true;
                        th3.Abort();
                    }
                    break;
                }
            }
        }

        public void waitForBtn()//等待好友加入群聊询问窗口的确认
        {
            while (true)
            {
                Thread.Sleep(1);
                if (!string.IsNullOrEmpty(tips.receiveOrNot))
                {
                    if (tips.receiveOrNot == "OK")
                    {
                        myChatGroup.addGroup(receAddGroup);
                        myChatGroup.addMember(receAddGroup, receAddID);
                        myChatGroup.addMember(receAddGroup, UserName);
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            TreeViewItem TVI2 = new TreeViewItem();
                            TVI2.Header = receAddGroup;
                            chatGroup.Items.Add(TVI2);
                        }));
                        inparam = "send" + " " + receAddIP + " " + "CHEG" + " " + receAddGroup + " " + UserName;
                        targetPort = 4000 + int.Parse(receAddID.Substring(UserName.Length - 4));//当前端口
                        signForSend = true;
                        th7.Abort();
                    }
                    else if (tips.receiveOrNot == "NO")
                    {
                        inparam = "send" + " " + receAddIP + " " + "CHEGNO" + " " + receAddGroup + " " + UserName;
                        targetPort = 4000 + int.Parse(receAddID.Substring(UserName.Length - 4));//当前端口
                        signForSend = true;
                        th7.Abort();
                    }
                    break;
                }
            }
        }

        public void waitForVoiceButton()//等待接受语音聊天窗口的确认
        {
            while (true)
            {
                Thread.Sleep(1);
                if (!string.IsNullOrEmpty(voicetips.receiveOrNot))
                {
                    if (voicetips.receiveOrNot == "OK")
                    {
                        inparam = "send" + " " + receAddIP + " " + "CHEV" + " " + UserName;
                        targetPort = 4000 + int.Parse(inviteID.Substring(UserName.Length - 4));//当前端口
                        signForSend = true;
                        VoiceObject = inviteID;
                        Console.WriteLine("getingin");
                        th12 = new Thread(new ThreadStart(sendVoice));//新建线程
                        th12.Start();//打开新线程
                        th13 = new Thread(new ThreadStart(receiveVoice));//新建线程
                        th13.Start();//打开新线程
                        th10.Abort();
                    }
                    else
                    {
                        inparam = "send" + " " + receAddIP + " " + "CHEVNO" + " " + UserName;
                        targetPort = 4000 + int.Parse(inviteID.Substring(UserName.Length - 4));//当前端口
                        signForSend = true;
                        th10.Abort();
                    }
                }
            }
        }

        public void waitForVoiceCheck()//等待对方接受语音聊天的确认
        {
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                myWait = new VoiceWaiting();
                myWait.Show();
            }));
            while (true)
            {
                Thread.Sleep(1);
                if (!string.IsNullOrEmpty(cheVoiceState))
                {
                    if (cheVoiceState == "R")
                    {
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            myWait.Close();
                            //提示
                            messegebox mes = new messegebox("对方接受，语音聊天开始");
                            mes.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            mes.Show();
                        }));
                        cheVoiceState = "";
                        Console.WriteLine("getin");
                        th12 = new Thread(new ThreadStart(sendVoice));//新建线程
                        th12.Start();//打开新线程
                        th13 = new Thread(new ThreadStart(receiveVoice));//新建线程
                        th13.Start();//打开新线程
                        th11.Abort();
                    }
                    else if (cheVoiceState == "N")
                    {
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            myWait.Close();
                            //提示
                            messegebox mes = new messegebox("对方拒绝了您的邀请！");
                            mes.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            mes.Show();
                        }));
                        cheVoiceState = "";
                        th11.Abort();
                    }
                }
            }
        }

        public void sendVoice()//发送语音
        {
            try
            {
                SendVoice = "d:\\send" + UserName + ".wav";
                ReceVoice = "d:\\receive" + UserName + ".wav";
                string strReceive = null;
                string sendMess = "q" + VoiceObject;
                ServerConnect tempsev = new ServerConnect();
                strReceive = tempsev.ServerQuery(sendMess);
                VoiceObjIP = strReceive;

                Config cfg = Netframe.SeiClient.GetDefaultConfig();
                cfg.Port = 4000 + int.Parse(UserName.Substring(UserName.Length - 4));//当前端口
                UDPThread udp = new UDPThread(cfg);
                tran = new MsgTranslator(udp, cfg);
                tran.MessageReceived += tran_MessageReceived;

                int Port = 4000 + int.Parse(VoiceObject.Substring(UserName.Length - 4));//当前端口
                string ip = VoiceObjIP;
                string msg = "语音消息";
                while (true)
                {
                    RecordController record = new RecordController();
                    record.StartRecord(SendVoice);
                    while (true)
                    {
                        if (record.signFinish == 1)
                        {
                            IPEndPoint remote = new IPEndPoint(IPAddress.Parse(ip), Port);
                            Msg m = new Msg(remote, "zz", " ", Commands.SendMsg, msg, "Come From A");
                            m.IsRequireReceive = true;
                            m.ExtendMessageBytes = FileContent(SendVoice);
                            m.PackageNo = Msg.GetRandomNumber();
                            m.Type = Consts.MESSAGE_BINARY;
                            tran.Send(m);
                            record.StopRecord();
                            break;
                        }
                    }
                }
            }
            catch (ThreadAbortException ex)
            {

            }
        }

        void tran_MessageReceived(object sender, MessageEventArgs e)//接收语音
        {
            Msg msg = e.msg;
 
            if (msg.Type == Consts.MESSAGE_BINARY)
            {
                Console.WriteLine("getinreceive");
                if (File.Exists(ReceVoice))
                {
                    File.Delete(ReceVoice);
                }
                FileStream fs = new FileStream(ReceVoice, FileMode.Create, FileAccess.Write);
                fs.Write(msg.ExtendMessageBytes, 0, msg.ExtendMessageBytes.Length);
                fs.Close();
                ReVoiceSign = "Y";
            }
        }

        private byte[] FileContent(string fileName)//文件读取
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            try
            {
                byte[] buffur = new byte[fs.Length];
                fs.Read(buffur, 0, (int)fs.Length);

                return buffur;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (fs != null)
                {

                    //关闭资源
                    fs.Close();
                }
            }
        }

        public void receiveVoice()//接收语音
        {
            try
            {
                while (true)
                {
                    if (ReVoiceSign == "Y")
                    {
                        SoundPlayer player = new SoundPlayer();
                        player.SoundLocation = ReceVoice;//读取音频文件
                        player.LoadAsync();
                        player.PlaySync();
                        player.Stop();//停止播放
                        player.Dispose();
                        ReVoiceSign = "N";
                    }
                }
            }
            catch (ThreadAbortException ex)
            {

            }
        }

        private void setUserName(string tempUserName)//用户名获取
        {
            UserName = tempUserName;
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

        private void ChatList_MouseDoubleClick(object sender, MouseButtonEventArgs e)//双击聊天信息触发事件
        {
            object o = ChatList.SelectedItem;
            if (o != null)
            {
                ListViewItem item = o as ListViewItem;
                string curText = item.Content.ToString();
                string[] divText = curText.Split(':');
                if (divText[1] == " File")
                {
                    string receiveFileName = "";
                    SaveFileDialog sfd = new SaveFileDialog();
                    if (sfd.ShowDialog() == true && sfd.FileName.Length > 0)
                    {
                        receiveFileName = sfd.FileName;
                        chosenFilePath = receiveFileName;
                        //保存文件提示
                        messegebox warning = new messegebox("文件已保存");
                        warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        warning.Show();
                    }
                    th8 = new Thread(new ThreadStart(preserveFile));//新建线程
                    th8.Start();//打开新线程
                }
            }
        }

        private void AddMessButton_Click(object sender, RoutedEventArgs e)//添加好友信息处理按钮
        {
            Messtip.Badge = 0;
            if (receAddState == "ADDF")
            {
                //提示选择信息
                tips = new AddMessBox(receAddID + " 请求加您为好友");
                tips.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                tips.Show();
                th3 = new Thread(new ThreadStart(waitForButton));//新建一个用于处理各类事件的线程
                th3.Start();//打开新线程
            }
            else if (receAddState == "ADDG")
            {
                //提示选择信息
                tips = new AddMessBox(receAddID + "邀请您加入群聊： " + receAddGroup);
                tips.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                tips.Show();
                th7 = new Thread(new ThreadStart(waitForBtn));//新建一个用于处理各类事件的线程
                th7.Start();//打开新线程
            }
            else if (receAddState == "CHE")
            {
                //提示
                messegebox tips = new messegebox("添加成功: " + receAddID);
                tips.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                tips.Show();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    myFriend.addFriend(receAddID);
                    TreeViewItem TVI = new TreeViewItem();
                    TVI.Header = receAddID;
                    friendTreelist1.Items.Add(TVI);
                }));
            }
            else if (receAddState == "CHENO")
            {
                //提示
                messegebox tipsno = new messegebox(receAddID + "对方拒绝添加您为好友!");
                tipsno.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                tipsno.Show();
            }
            else if (receAddState == "CHEG")
            {
                //提示
                messegebox tips = new messegebox(receAddID + "已成功加入群聊：" + receAddGroup);
                tips.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                tips.Show();
                myChatGroup.addMember(receAddGroup, receAddID);
            }
            else if (receAddState == "CHEGNO")
            {
                //提示
                messegebox tipsno = new messegebox(receAddID + "对方拒绝加入群聊：" + receAddGroup);
                tipsno.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                tipsno.Show();
            }
        }

        private void addFriendCheckButton_Click(object sender, RoutedEventArgs e)//添加好友确认按钮
        {
            string tempFindFriend = FindFriendBox.Text;
            if (!string.IsNullOrEmpty(tempFindFriend))
            {
                string [] divTempF = tempFindFriend.Split(' ');
                if (divTempF[0] == "G")
                {
                    string currentGroup = divTempF[1];//要加人的群聊
                    string tempFriend = divTempF[2];//要添加的人ID
                    int curGroup = -1;//要加人群聊的索引
                    bool signForExist = false;
                    for (int k = 0; k < myChatGroup.chatGroupNum; k++)
                    {
                        if (myChatGroup.chatGroupList[k].groupName == currentGroup)
                        {
                            curGroup = k;
                            break;
                        }
                    }
                    if (curGroup != -1)
                    {
                        for (int k = 0; k < myChatGroup.chatGroupList[curGroup].memberNum; k++)
                        {
                            if (myChatGroup.chatGroupList[curGroup].memberList[k] == tempFriend)
                            {
                                signForExist = true;
                                break;
                            }
                        }
                        if (signForExist == false)
                        {
                            // 查看用户是否在线
                            string strReceive = null;
                            string sendMess = "q" + tempFriend;
                            ServerConnect tempsev = new ServerConnect();
                            strReceive = tempsev.ServerQuery(sendMess);
                            if (strReceive != "n")
                            {
                                string addMess = "ADDG";
                                inparam = "send" + " " + strReceive + " " + addMess + ' ' + currentGroup + ' ' + UserName;//通讯
                                targetPort = 4000 + int.Parse(tempFriend.Substring(UserName.Length - 4));//当前端口
                                signForSend = true;
                                //添加提示
                                messegebox tipmess = new messegebox("邀请已发送");
                                tipmess.Owner = this;//设置层级关系，子窗口显示在父之上
                                tipmess.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                tipmess.Show();
                            }
                            else
                            {
                                //错误提示
                                messegebox warning = new messegebox("对方现在不在线，无法加入群聊");
                                warning.Owner = this;//设置层级关系，子窗口显示在父之上
                                warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                warning.Show();
                            }
                        }
                        else
                        {
                            //错误提示
                            messegebox warning = new messegebox("此用户已在该群聊中!");
                            warning.Owner = this;
                            warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            warning.Show();
                        }
                    }
                    else
                    {
                        //提示
                        messegebox tips = new messegebox("群聊不存在，将就此创建");
                        tips.Owner = this;
                        tips.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        tips.Show();
                        //创建新群聊
                        myChatGroup.addGroup(currentGroup);
                        myChatGroup.addMember(currentGroup, UserName);
                        TreeViewItem TVI2 = new TreeViewItem();
                        TVI2.Header = currentGroup;
                        chatGroup.Items.Add(TVI2);
                        // 查看用户是否在线
                        string strReceive = null;
                        string sendMess = "q" + tempFriend;
                        ServerConnect tempsev = new ServerConnect();
                        strReceive = tempsev.ServerQuery(sendMess);
                        if (strReceive != "n")
                        {
                            string addMess = "ADDG";
                            inparam = "send" + " " + strReceive + " " + addMess + ' ' + currentGroup + ' ' + UserName;//通讯
                            targetPort = 4000 + int.Parse(tempFriend.Substring(UserName.Length - 4));//当前端口
                            signForSend = true;
                            //添加提示
                            messegebox tipmess = new messegebox("邀请已发送");
                            tipmess.Owner = this;//设置层级关系，子窗口显示在父之上
                            tipmess.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            tipmess.Show();
                        }
                        else
                        {
                            //错误提示
                            messegebox warning = new messegebox("对方现在不在线，无法加入群聊");
                            warning.Owner = this;//设置层级关系，子窗口显示在父之上
                            warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            warning.Show();
                        }
                    }
                }
                else
                {
                    bool signForExist = false;
                    for (int k = 0; k < myFriend.friendNum; k++)
                    {
                        if (myFriend.friendList[k] == tempFindFriend)
                        {
                            signForExist = true;
                            break;
                        }
                    }
                    if (signForExist == false)
                    {
                        // 查看用户是否在线
                        string strReceive = null;
                        string sendMess = "q" + tempFindFriend;
                        ServerConnect tempsev = new ServerConnect();
                        strReceive = tempsev.ServerQuery(sendMess);
                        if (strReceive != "n")
                        {
                            string addMess = "ADDF";
                            inparam = "send" + " " + strReceive + " " + addMess + ' ' + UserName;//通讯
                            targetPort = 4000 + int.Parse(tempFindFriend.Substring(UserName.Length - 4));//当前端口
                            signForSend = true;
                            //添加提示
                            messegebox tipmess = new messegebox("添加请求已发送");
                            tipmess.Owner = this;//设置层级关系，子窗口显示在父之上
                            tipmess.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            tipmess.Show();
                        }
                        else
                        {
                            //添加提示
                            messegebox warning = new messegebox("对方现在不在线，无法添加好友");
                            warning.Owner = this;//设置层级关系，子窗口显示在父之上
                            warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            warning.Show();
                        }
                    }
                    else
                    {
                        //错误提示
                        messegebox warning = new messegebox("此用户已是您的好友!");
                        warning.Owner = this;
                        warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        warning.Show();
                    }
                }
            }
            else
            {
                //错误提示
                messegebox warning = new messegebox("用户名输入为空");
                warning.Owner = this;
                warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                warning.Show();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)//关闭窗口
        {
            string strReceive = null;
            string sendMess = "logout" + UserName;
            ServerConnect tempsev = new ServerConnect();
            strReceive = tempsev.ServerQuery(sendMess);
            if (strReceive == "loo")
            {
                //数据库更新
                string dbFilename = "myQQdb.db";
                SQLiteConnection dbCore = new SQLiteConnection("data source=" + dbFilename);
                dbCore.Open();
                //好友信息更新
                string commandText = "CREATE TABLE IF NOT EXISTS friends(ID CHAR(10) NOT NULL, FRIENDID CHAR(10) NOT NULL);";
                SQLiteCommand cmd = new SQLiteCommand(commandText, dbCore);
                int result = cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM friends WHERE ID=" + UserName;//删除原始数据
                result = cmd.ExecuteNonQuery();
                for (int k = 0;k < myFriend.friendNum;k++)
                {
                    cmd.CommandText = "INSERT INTO friends(ID,FRIENDID) VALUES(@ID, @FRIENDID)";//添加数据
                    cmd.Parameters.Add("ID", DbType.String).Value = UserName;
                    cmd.Parameters.Add("FRIENDID", DbType.String).Value = myFriend.friendList[k];
                    result = cmd.ExecuteNonQuery();
                }
                //群聊信息更新
                string commandText4 = "CREATE TABLE IF NOT EXISTS chatgroups(GROUPNAME CHAR(20) NOT NULL, MEMBERID CHAR(10) NOT NULL);";
                SQLiteCommand cmd4 = new SQLiteCommand(commandText4, dbCore);
                int result4 = cmd4.ExecuteNonQuery();
                for (int k = 0; k < myChatGroup.chatGroupNum; k++)//删除原始数据
                {
                    string tempname = myChatGroup.chatGroupList[k].groupName;
                    cmd4.CommandText = "DELETE FROM chatgroups WHERE GROUPNAME=" + "'" + tempname + "'";
                    result4 = cmd4.ExecuteNonQuery();
                }
                for (int k = 0; k < myChatGroup.chatGroupNum; k++)
                {
                    for (int m = 0; m < myChatGroup.chatGroupList[k].memberNum; m++)
                    {
                        cmd4.CommandText = "INSERT INTO chatgroups(GROUPNAME,MEMBERID) VALUES(@GROUPNAME,@MEMBERID)";//添加数据
                        cmd4.Parameters.Add("GROUPNAME", DbType.String).Value = myChatGroup.chatGroupList[k].groupName;
                        cmd4.Parameters.Add("MEMBERID", DbType.String).Value = myChatGroup.chatGroupList[k].memberList[m];
                        result = cmd4.ExecuteNonQuery();
                    }
                }
                //聊天记录更新
                string commandText2 = "CREATE TABLE IF NOT EXISTS chatrecords(SENDID CHAR(10) NOT NULL, RECEIVEID CHAR(10) NOT NULL, SRTIME CHAR(10) NOT NULL, CONTENT CHAR(200) NOT NULL);";
                SQLiteCommand cmd2 = new SQLiteCommand(commandText2, dbCore);
                int result2 = cmd2.ExecuteNonQuery();
                cmd2.CommandText = "DELETE FROM chatrecords WHERE SENDID=" + UserName;//删除原始数据
                result2 = cmd2.ExecuteNonQuery();
                cmd2.CommandText = "DELETE FROM chatrecords WHERE RECEIVEID=" + UserName;
                result2 = cmd2.ExecuteNonQuery();
                for (int k = 0; k < myRecord.myChatRecordNum; k++)
                {
                    cmd2.CommandText = "INSERT INTO chatrecords(SENDID,RECEIVEID,SRTIME,CONTENT) VALUES(@SENDID,@RECEIVEID,@SRTIME,@CONTENT)";//添加数据
                    cmd2.Parameters.Add("SENDID", DbType.String).Value = myRecord.myChatRecord[k].sendID;
                    cmd2.Parameters.Add("RECEIVEID", DbType.String).Value = myRecord.myChatRecord[k].receiveID;
                    cmd2.Parameters.Add("SRTIME", DbType.String).Value = myRecord.myChatRecord[k].SRtime;
                    cmd2.Parameters.Add("CONTENT", DbType.String).Value = myRecord.myChatRecord[k].content;
                    result2 = cmd2.ExecuteNonQuery();
                }
                //群聊记录更新
                string commandText3 = "CREATE TABLE IF NOT EXISTS chatgrouprecords(GROUPNAME CHAR(20) NOT NULL, SENDID CHAR(10) NOT NULL, SRTIME CHAR(10) NOT NULL, CONTENT CHAR(200) NOT NULL);";
                SQLiteCommand cmd3 = new SQLiteCommand(commandText3, dbCore);
                int result3 = cmd3.ExecuteNonQuery();
                for (int k = 0; k < myGroupRecord.myChatGroupRecordNum; k++)//删除原始数据
                {
                    string tempname = myGroupRecord.myChatGroupRecord[k].groupName;
                    cmd3.CommandText = "DELETE FROM chatgrouprecords WHERE GROUPNAME=" + "'" + tempname + "'";
                    result3 = cmd3.ExecuteNonQuery();
                }
                for (int k = 0; k < myGroupRecord.myChatGroupRecordNum; k++)
                {
                    cmd3.CommandText = "INSERT INTO chatgrouprecords(GROUPNAME,SENDID,SRTIME,CONTENT) VALUES(@GROUPNAME,@SENDID,@SRTIME,@CONTENT)";//添加数据
                    cmd3.Parameters.Add("GROUPNAME", DbType.String).Value = myGroupRecord.myChatGroupRecord[k].groupName;
                    cmd3.Parameters.Add("SENDID", DbType.String).Value = myGroupRecord.myChatGroupRecord[k].sendID;
                    cmd3.Parameters.Add("SRTIME", DbType.String).Value = myGroupRecord.myChatGroupRecord[k].SRtime;
                    cmd3.Parameters.Add("CONTENT", DbType.String).Value = myGroupRecord.myChatGroupRecord[k].content;
                    result3 = cmd3.ExecuteNonQuery();
                }
                //关闭程序
                inparam = "exit";
                th4.Abort();
                th5.Abort();
                th6.Abort();
                Application.Current.Shutdown();
            }
            else
            {
                //错误提示
                messegebox warning = new messegebox("从服务器下线失败：未知错误");
                warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                warning.Show();
            }
        }

        private void FindFriendBox_PreviewTextInput(object sender, TextCompositionEventArgs e)//用户名输入正则表达式,没有要求
        {
            
        }

        private void StickerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FilesendButton_Click(object sender, RoutedEventArgs e)//选择要发送的文件
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                chosenFile = ofd.FileName;
                simpleFileName = ofd.SafeFileName;
                MesEdit.Text = "已选择文件: \n " + chosenFile;
                sendFileState = "file";
            }
        }

        private void MessButton_Click(object sender, RoutedEventArgs e)//文件群发按钮
        {
            chw = new chooseWindow(myFriend.friendList);
            chw.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            chw.Show();
            th9 = new Thread(new ThreadStart(fileSendGroup));//新建线程
            th9.Start();//打开新线程
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)//语音聊天暂停按钮
        {
            th12.Abort();
            th13.Abort();
        }

        public void fileSendGroup()//群发文件
        {
            while (true)
            {
                Thread.Sleep(1);
                if (chw.done == 1)
                {
                    sendIDList = chw.Choose;
                    int IdNum = sendIDList.Length;
                    if (IdNum != 0)
                    {
                        string selectedID = "";
                        App.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            foreach (TreeViewItem tempItem in friendTreelist1.Items)
                            {
                                if (tempItem.IsSelected == true)
                                {
                                    selectedID = tempItem.Header.ToString();
                                }
                            }
                            for (int k = 0; k < IdNum; k++)
                            {
                                string willSendID = sendIDList[k];
                                try
                                {
                                    string strReceive = null;
                                    string sendMess = "q" + willSendID;
                                    ServerConnect tempsev = new ServerConnect();
                                    strReceive = tempsev.ServerQuery(sendMess);
                                    if (strReceive != "n")
                                    {
                                        IPAddress curIP = IPAddress.Parse(strReceive);
                                        targetPort = 4000 + int.Parse(willSendID.Substring(UserName.Length - 4));//当前端口
                                        IPEndPoint ipEndPoint = new IPEndPoint(curIP, targetPort);//目的IP与端口
                                        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);// 创建socket
                                        client.Connect(ipEndPoint);// 连接到目的端口
                                        string[] divChosen = simpleFileName.Split('.');//划分文件名
                                        string fileType = divChosen[1];//文件类型
                                                                       // 文件发送前的数据
                                        string string1 = String.Format("File begin.$" + fileType + "$" + simpleFileName + '$');
                                        byte[] preBuf = Encoding.ASCII.GetBytes(string1);
                                        // 发送文件
                                        Console.WriteLine("Sending {0} with buffers to the host.{1}", chosenFile, Environment.NewLine);
                                        client.SendFile(chosenFile, preBuf, null, TransmitFileOptions.UseDefaultWorkerThread);
                                        // 关闭socket
                                        client.Shutdown(SocketShutdown.Both);
                                        client.Close();
                                    }
                                    else
                                    {
                                        //错误提示
                                        messegebox warning = new messegebox("有账号不在线，无法发送！");
                                        warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                        warning.Show();
                                    }
                                }
                                catch (NotSupportedException)
                                {
                                    Console.WriteLine("出错，无法访问远程主机");
                                }
                                string currentTime = DateTime.Now.ToString();//当前时间
                                                                             //聊天记录存储
                                List<Records> chatRecord_list = new List<Records>(myRecord.myChatRecord);
                                Records tempre = new Records();
                                tempre.sendID = UserName;
                                tempre.receiveID = willSendID;
                                tempre.SRtime = currentTime;
                                tempre.content = "File: " + simpleFileName;
                                chatRecord_list.Add(tempre);
                                myRecord.myChatRecord = chatRecord_list.ToArray();
                                myRecord.myChatRecordNum = myRecord.myChatRecord.Length;
                                if (selectedID == willSendID)
                                {
                                    //聊天记录显示
                                    ListViewItem newItem = new ListViewItem();//控件
                                    ListViewItem newItem2 = new ListViewItem();
                                    Card newCard = new Card();
                                    TextBlock newTextBlock = new TextBlock();
                                    TextBlock newTextBlock2 = new TextBlock();
                                    newItem2.Content = newTextBlock2;//嵌套关系
                                    newItem.Content = newCard;
                                    newCard.Content = newTextBlock;
                                    //样式
                                    newItem.HorizontalContentAlignment = HorizontalAlignment.Right;
                                    newItem.Width = 571;
                                    newItem2.HorizontalContentAlignment = HorizontalAlignment.Center;
                                    newItem2.Width = 571;
                                    SolidColorBrush newBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8490C3"));
                                    Thickness newThick = new Thickness(8);
                                    newCard.Background = newBrush;
                                    newCard.Width = 200;
                                    newCard.Padding = newThick;
                                    newCard.UniformCornerRadius = 6;
                                    newCard.HorizontalContentAlignment = HorizontalAlignment.Right;
                                    SolidColorBrush newBrush2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                                    newTextBlock.TextWrapping = TextWrapping.Wrap;
                                    newTextBlock.Foreground = newBrush2;
                                    newTextBlock.Text = "File: " + simpleFileName;
                                    FontFamily curset = new FontFamily("方正经黑简体");
                                    newTextBlock.FontFamily = curset;
                                    newTextBlock2.Text = "------ " + currentTime + " ------";
                                    newTextBlock2.VerticalAlignment = VerticalAlignment.Center;
                                    newTextBlock2.HorizontalAlignment = HorizontalAlignment.Center;
                                    ChatList.Items.Add(newItem2);//添加聊天记录并显示
                                    ChatList.Items.Add(newItem);
                                    MesEdit.Clear();//输入框清空
                                }
                            }
                            //提示
                            messegebox mes = new messegebox("文件群发已完成！");
                            mes.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            mes.Show();
                            sendFileState = "mes";
                        }));
                    }
                    th9.Abort();
                }
            }
        }

        private void SnackMess_ActionClick(object sender, RoutedEventArgs e)
        {

        }

        private void VoiceButton_Click(object sender, RoutedEventArgs e)//语音聊天发起
        {
            string selectedID = "";
            foreach (TreeViewItem tempItem in friendTreelist1.Items)
            {
                if (tempItem.IsSelected == true)
                {
                    selectedID = tempItem.Header.ToString();
                }
            }
            if (!string.IsNullOrEmpty(selectedID))
            {
                string strReceive = null;
                string sendMess = "q" + selectedID;
                ServerConnect tempsev = new ServerConnect();
                strReceive = tempsev.ServerQuery(sendMess);
                if (strReceive != "n")
                {
                    string addMess = "VCHAT";
                    inparam = "send" + " " + strReceive + " " + addMess + ' ' + UserName;//通讯
                    targetPort = 4000 + int.Parse(selectedID.Substring(UserName.Length - 4));//当前端口
                    signForSend = true;
                    th11 = new Thread(new ThreadStart(waitForVoiceCheck));//新建一个用于处理各类事件的线程
                    th11.Start();//打开新线程
                }
                else
                {
                    //错误提示
                    messegebox warning = new messegebox("对方不在线，无法语音聊天");
                    warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    warning.Show();
                }
            }
            else
            {
                //错误提示
                messegebox warning = new messegebox("您未选择聊天对象！");
                warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                warning.Show();
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)//发送消息按钮
        {
            int choosesign = 0;
            foreach (TreeViewItem tempItem in friendTreelist1.Items)
            {
                if (tempItem.IsSelected == true)
                {
                    choosesign = 1;
                }
            }
            if (choosesign == 1)
            {
                string selectedID = "";
                foreach (TreeViewItem tempItem in friendTreelist1.Items)
                {
                    if (tempItem.IsSelected == true)
                    {
                        selectedID = tempItem.Header.ToString();
                    }
                }
                string strReceive = null;
                string sendMess = "q" + selectedID;
                ServerConnect tempsev = new ServerConnect();
                strReceive = tempsev.ServerQuery(sendMess);
                if (strReceive != "n")
                {
                    if (!string.IsNullOrEmpty(selectedID))
                    {
                        if (sendFileState != "file")
                        {
                            targetPort = 4000 + int.Parse(selectedID.Substring(UserName.Length - 4));//当前端口
                            string waitMes = MesEdit.Text;//待发送的消息
                            string currentTime = DateTime.Now.ToString();//当前时间
                            inparam = "send" + " " + strReceive + " " + waitMes;//通讯
                            signForSend = true;
                            //聊天记录存储
                            List<Records> chatRecord_list = new List<Records>(myRecord.myChatRecord);
                            Records tempre = new Records();
                            tempre.sendID = UserName;
                            tempre.receiveID = selectedID;
                            tempre.SRtime = currentTime;
                            tempre.content = waitMes;
                            chatRecord_list.Add(tempre);
                            myRecord.myChatRecord = chatRecord_list.ToArray();
                            myRecord.myChatRecordNum = myRecord.myChatRecord.Length;
                            //聊天记录显示
                            ListViewItem newItem = new ListViewItem();//控件
                            ListViewItem newItem2 = new ListViewItem();
                            Card newCard = new Card();
                            TextBlock newTextBlock = new TextBlock();
                            TextBlock newTextBlock2 = new TextBlock();
                            newItem2.Content = newTextBlock2;//嵌套关系
                            newItem.Content = newCard;
                            newCard.Content = newTextBlock;
                            //样式
                            newItem.HorizontalContentAlignment = HorizontalAlignment.Right;
                            newItem.Width = 571;
                            newItem2.HorizontalContentAlignment = HorizontalAlignment.Center;
                            newItem2.Width = 571;
                            SolidColorBrush newBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8490C3"));
                            Thickness newThick = new Thickness(8);
                            newCard.Background = newBrush;
                            newCard.Width = 200;
                            newCard.Padding = newThick;
                            newCard.UniformCornerRadius = 6;
                            newCard.HorizontalContentAlignment = HorizontalAlignment.Right;
                            SolidColorBrush newBrush2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                            newTextBlock.TextWrapping = TextWrapping.Wrap;
                            newTextBlock.Foreground = newBrush2;
                            newTextBlock.Text = waitMes;
                            newTextBlock2.Text = "------ " + currentTime + " ------";
                            newTextBlock2.VerticalAlignment = VerticalAlignment.Center;
                            newTextBlock2.HorizontalAlignment = HorizontalAlignment.Center;
                            ChatList.Items.Add(newItem2);//添加聊天记录并显示
                            ChatList.Items.Add(newItem);
                            MesEdit.Clear();//输入框清空
                        }
                        else
                        {
                            try
                            {
                                IPAddress curIP = IPAddress.Parse(strReceive);
                                targetPort = 4000 + int.Parse(selectedID.Substring(UserName.Length - 4));//当前端口
                                IPEndPoint ipEndPoint = new IPEndPoint(curIP, targetPort);//目的IP与端口
                                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);// 创建socket
                                client.Connect(ipEndPoint);// 连接到目的端口
                                string[] divChosen = simpleFileName.Split('.');//划分文件名
                                string fileType = divChosen[1];//文件类型
                                // 文件发送前的数据
                                string string1 = String.Format("File begin.$" + fileType + "$" + simpleFileName + '$');
                                byte[] preBuf = Encoding.ASCII.GetBytes(string1);
                                // 发送文件
                                Console.WriteLine("Sending {0} with buffers to the host.{1}", chosenFile, Environment.NewLine);
                                client.SendFile(chosenFile, preBuf, null, TransmitFileOptions.UseDefaultWorkerThread);
                                // 关闭socket
                                client.Shutdown(SocketShutdown.Both);
                                client.Close();
                            }
                            catch(NotSupportedException)
                            {
                                Console.WriteLine("出错，无法访问远程主机");
                            }
                            sendFileState = "mes";
                            string currentTime = DateTime.Now.ToString();//当前时间
                            //聊天记录存储
                            List<Records> chatRecord_list = new List<Records>(myRecord.myChatRecord);
                            Records tempre = new Records();
                            tempre.sendID = UserName;
                            tempre.receiveID = selectedID;
                            tempre.SRtime = currentTime;
                            tempre.content = "File: " + simpleFileName;
                            chatRecord_list.Add(tempre);
                            myRecord.myChatRecord = chatRecord_list.ToArray();
                            myRecord.myChatRecordNum = myRecord.myChatRecord.Length;
                            //聊天记录显示
                            ListViewItem newItem = new ListViewItem();//控件
                            ListViewItem newItem2 = new ListViewItem();
                            Card newCard = new Card();
                            TextBlock newTextBlock = new TextBlock();
                            TextBlock newTextBlock2 = new TextBlock();
                            newItem2.Content = newTextBlock2;//嵌套关系
                            newItem.Content = newCard;
                            newCard.Content = newTextBlock;
                            //样式
                            newItem.HorizontalContentAlignment = HorizontalAlignment.Right;
                            newItem.Width = 571;
                            newItem2.HorizontalContentAlignment = HorizontalAlignment.Center;
                            newItem2.Width = 571;
                            SolidColorBrush newBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8490C3"));
                            Thickness newThick = new Thickness(8);
                            newCard.Background = newBrush;
                            newCard.Width = 200;
                            newCard.Padding = newThick;
                            newCard.UniformCornerRadius = 6;
                            newCard.HorizontalContentAlignment = HorizontalAlignment.Right;
                            SolidColorBrush newBrush2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                            newTextBlock.TextWrapping = TextWrapping.Wrap;
                            newTextBlock.Foreground = newBrush2;
                            newTextBlock.Text = "File: " + simpleFileName;
                            FontFamily curset = new FontFamily("方正经黑简体");
                            newTextBlock.FontFamily = curset;
                            newTextBlock2.Text = "------ " + currentTime + " ------";
                            newTextBlock2.VerticalAlignment = VerticalAlignment.Center;
                            newTextBlock2.HorizontalAlignment = HorizontalAlignment.Center;
                            ChatList.Items.Add(newItem2);//添加聊天记录并显示
                            ChatList.Items.Add(newItem);
                            MesEdit.Clear();//输入框清空
                        }
                    }
                    else
                    {
                        //错误提示
                        messegebox warning = new messegebox("未选择账号，未知发送目标！");
                        warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        warning.Show();
                    }
                }
                else
                {
                    //错误提示
                    messegebox warning = new messegebox("账号不在线，无法发送！");
                    warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    warning.Show();
                }
            }
            else
            {
                string selectedGroup = "";
                foreach (TreeViewItem tempItem in chatGroup.Items)
                {
                    if (tempItem.IsSelected == true)
                    {
                        selectedGroup = tempItem.Header.ToString();
                    }
                }
                if (!string.IsNullOrEmpty(selectedGroup))
                {
                    string waitMes = MesEdit.Text;//待发送的消息
                    string currentTime = DateTime.Now.ToString();//当前时间
                    inparam = "groupsend" + " " + "12345" + " " + waitMes;//通讯
                    for (int k = 0; k < myChatGroup.chatGroupNum; k++)
                    {
                        if(myChatGroup.chatGroupList[k].groupName == selectedGroup)
                        {
                            groupMemberList = myChatGroup.chatGroupList[k].memberList;
                            chooseGroup = selectedGroup;
                        }
                    }
                    signForSend = true;
                    //聊天记录存储
                    List<GroupRecords> chatGroupRecord_list = new List<GroupRecords>(myGroupRecord.myChatGroupRecord);
                    GroupRecords tempre = new GroupRecords();
                    tempre.groupName = selectedGroup;
                    tempre.sendID = UserName;
                    tempre.SRtime = currentTime;
                    tempre.content = waitMes;
                    chatGroupRecord_list.Add(tempre);
                    myGroupRecord.myChatGroupRecord = chatGroupRecord_list.ToArray();
                    myGroupRecord.myChatGroupRecordNum = myGroupRecord.myChatGroupRecord.Length;
                    //聊天记录显示
                    ListViewItem newItem = new ListViewItem();//控件
                    ListViewItem newItem2 = new ListViewItem();
                    ListViewItem newItem3 = new ListViewItem();
                    Card newCard = new Card();
                    TextBlock newTextBlock = new TextBlock();
                    TextBlock newTextBlock2 = new TextBlock();
                    TextBlock newTextBlock3 = new TextBlock();
                    newItem2.Content = newTextBlock2;//嵌套关系
                    newItem3.Content = newTextBlock3;
                    newItem.Content = newCard;
                    newCard.Content = newTextBlock;
                    //样式
                    newItem.HorizontalContentAlignment = HorizontalAlignment.Right;
                    newItem.Width = 571;
                    newItem2.HorizontalContentAlignment = HorizontalAlignment.Center;
                    newItem2.Width = 571;
                    newItem3.HorizontalContentAlignment = HorizontalAlignment.Right;
                    newItem3.Width = 571;
                    SolidColorBrush newBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF8490C3"));
                    Thickness newThick = new Thickness(8);
                    newCard.Background = newBrush;
                    newCard.Width = 200;
                    newCard.Padding = newThick;
                    newCard.UniformCornerRadius = 6;
                    newCard.HorizontalContentAlignment = HorizontalAlignment.Right;
                    SolidColorBrush newBrush2 = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
                    newTextBlock.TextWrapping = TextWrapping.Wrap;
                    newTextBlock.Foreground = newBrush2;
                    newTextBlock.Text = waitMes;
                    newTextBlock2.Text = "------ " + currentTime + " ------";
                    newTextBlock2.VerticalAlignment = VerticalAlignment.Center;
                    newTextBlock2.HorizontalAlignment = HorizontalAlignment.Center;
                    newTextBlock3.Text = "------ " + UserName;
                    newTextBlock3.VerticalAlignment = VerticalAlignment.Center;
                    newTextBlock3.HorizontalAlignment = HorizontalAlignment.Right;
                    ChatList.Items.Add(newItem2);//添加聊天记录并显示
                    ChatList.Items.Add(newItem3);
                    ChatList.Items.Add(newItem);
                    MesEdit.Clear();//输入框清空
                }
                else
                {
                    //错误提示
                    messegebox warning = new messegebox("未选择群聊，未知发送目标！");
                    warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    warning.Show();
                }
            }
        }

        public void test()
        {
            string dbFilename = "myQQdb.db";
            SQLiteConnection dbCore = new SQLiteConnection("data source=" + dbFilename);
            dbCore.Open();
            string commandText = "CREATE TABLE IF NOT EXISTS chatgroups(GROUPNAME CHAR(20) NOT NULL, MEMBERID CHAR(10) NOT NULL);";
            SQLiteCommand cmd = new SQLiteCommand(commandText, dbCore);
            int result = cmd.ExecuteNonQuery();
            cmd.CommandText = "INSERT INTO chatgroups(GROUPNAME,MEMBERID) VALUES(@GROUPNAME,@MEMBERID)";//添加数据
            cmd.Parameters.Add("GROUPNAME", DbType.String).Value = "421B";
            cmd.Parameters.Add("MEMBERID", DbType.String).Value = "2017011540";
            result = cmd.ExecuteNonQuery();
        }

    }

    public class Listener//监听消息类
    {
        private Thread th;
        private TcpListener tcpl;
        public bool listenerRun = true;
        IPAddress ListenerIP;
        public string receiveMes = "";
        public string senderIP = "";
        public bool signForReceive = false;
        public bool receiveFile = false;
        public Byte[] receFileAll;
        public long receFileAllNum = 0;
        int currentPort = 5656;
        //listenerRun为true，表示可以接受连接请求，false则为结束程序

        public Listener(string MyIP,int myPort)//构造函数
        {
            th = new Thread(new ThreadStart(Listen));//新建一个用于监听的线程
            th.Start();//打开新线程
            currentPort = myPort;
            if (!string.IsNullOrEmpty(MyIP))
                ListenerIP = IPAddress.Parse(MyIP);
        }

        public void Stop()
        {
            tcpl.Stop();
            th.Abort();//终止线程
        }

        private void Listen()
        {
            try
            {
                tcpl = new TcpListener(ListenerIP, currentPort);//在currentPort端口新建一个TcpListener对象
                tcpl.Start();
                Console.WriteLine("started listening..");

                while (listenerRun)//开始监听
                {
                    Socket s = tcpl.AcceptSocket();
                    string remote = s.RemoteEndPoint.ToString();
                    Byte[] stream = new Byte[80];
                    int i = s.Receive(stream);//接受连接请求的字节流
                    string temoRece = System.Text.Encoding.UTF8.GetString(stream);
                    string msg = "<" + remote + ">" + System.Text.Encoding.UTF8.GetString(stream);
                    senderIP = remote;
                    receiveMes = System.Text.Encoding.UTF8.GetString(stream);
                    string []tempstr = receiveMes.Split('$');
                    if (tempstr[0] == "File begin.")
                    {
                        string filetype = tempstr[1];
                        string filename = tempstr[2];
                        receiveMes = "File: " + filename;
                        Console.WriteLine("hhhh" + receiveMes);
                        receiveFile = true;
                        //前一次接收剩下的bytes
                        byte[] leftBytes;//剩余byte
                        byte[] countlenbyte;
                        string countlenstr = "File begin.$" + filetype + '$' + filename + '$';
                        countlenbyte = Encoding.UTF8.GetBytes(countlenstr);
                        List<byte> byteList = new List<byte>();//bytelist
                        for (int k = countlenbyte.Length; k < 80; k++)
                        {
                            byteList.Add(stream[k]);
                        }
                        leftBytes = byteList.ToArray();
                        List<byte> byteSource = new List<byte>();//bytelist
                        byteSource.AddRange(leftBytes);
                        //循环接收文件
                        Byte[] buffer = new Byte[512];
                        int size = 0;
                        string fileSavePath = @"D:\文件";//默认路径
                        if (Directory.Exists(fileSavePath) == false)//如果不存在就创建文件夹
                        {
                            Directory.CreateDirectory(fileSavePath);
                        }
                        string fileName = fileSavePath + "\\" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + "." + filetype;//默认名称
                        //创建文件流，然后让文件流来根据路径创建一个文件
                        FileStream fs = new FileStream(fileName, FileMode.Create);
                        fs.Write(leftBytes, 0, leftBytes.Length);
                        while ((size = s.Receive(buffer, 0, buffer.Length, SocketFlags.None)) > 0)
                        {
                            byteSource.AddRange(buffer);
                            fs.Write(buffer, 0, size);
                        }
                        fs.Close();
                        receFileAll = byteSource.ToArray();
                        receFileAllNum = receFileAll.Length;
                    }
                    signForReceive = true;
                }
            }
            catch (System.Security.SecurityException)
            {
                Console.WriteLine("firewall says no no to application - application cries..");
            }
            catch (Exception)
            {
                Console.WriteLine("stoped listening..");
            }
        }
    }

    public class Sender//发送消息类
    {
        public void Send(string[] aInput,int targetPort)
        {
            string stream = "";
            //获得要发送的信息
            for (int i = 2; i < aInput.Length; i++)
            {
                stream += aInput[i] + " ";
            }

            try
            {
                TcpClient tcpc = new TcpClient(aInput[1], targetPort);
                //在myPort端口新建一个TcpClient对象
                NetworkStream tcpStream = tcpc.GetStream();

                StreamWriter reqStreamW = new StreamWriter(tcpStream);
                reqStreamW.Write(stream);
                reqStreamW.Flush();//发送信息
                tcpStream.Close();
                tcpc.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("connection refused by target computer");
            }
        }
    }

    public class GroupSender//发送消息类
    {
        public void Send(string[] aInput, string[] memberlist, int memnum, string Username, string groupName)
        {
            string stream = "GROUP" + " " + groupName + " ";
            //获得要发送的信息
            for (int i = 2; i < aInput.Length; i++)
            {
                stream += aInput[i] + " ";
            }

            for (int k = 0; k < memnum; k++)
            {
                if (Username != memberlist[k])
                {
                    try
                    {
                        int targetPort = 4000 + int.Parse(memberlist[k].Substring(memberlist[k].Length - 4));//当前端口
                        string tempID = memberlist[k];//查询好友是否在线
                        string strReceive = null;
                        string sendMess = "q" + tempID;
                        ServerConnect tempsev = new ServerConnect();
                        strReceive = tempsev.ServerQuery(sendMess);
                        if (strReceive != "n")
                        {
                            TcpClient tcpc = new TcpClient(strReceive, targetPort);
                            //在myPort端口新建一个TcpClient对象
                            NetworkStream tcpStream = tcpc.GetStream();
                            StreamWriter reqStreamW = new StreamWriter(tcpStream);
                            reqStreamW.Write(stream);
                            reqStreamW.Flush();//发送信息
                            tcpStream.Close();
                            tcpc.Close();
                        }
                        else
                        {
                            //错误提示
                            messegebox warning = new messegebox("账号不在线，无法发送");
                            warning.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            warning.Show();
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("connection refused by target computer");
                    }
                }
            }
        }
    }

    public class RecordController//录音类
    {
        public WaveInEvent mWavIn;
        public WaveFileWriter mWavWriter;
        public int signFinish = 0;

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="filePath"></param>
        public void StartRecord(string filePath)
        {
            signFinish = 0;
            mWavIn = new WaveInEvent();
            mWavIn.DataAvailable += MWavIn_DataAvailable;
            mWavWriter = new WaveFileWriter(filePath, mWavIn.WaveFormat);
            mWavIn.StartRecording();
        }

        /// <summary>
        /// 停止录音
        /// </summary>
        public void StopRecord()
        {
            mWavIn?.StopRecording();
            mWavIn?.Dispose();
            mWavIn = null;
            mWavWriter?.Close();
            mWavWriter = null;
        }

        private void MWavIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            mWavWriter.Write(e.Buffer, 0, e.BytesRecorded);
            int secondsRecorded = (int)mWavWriter.Length / mWavWriter.WaveFormat.AverageBytesPerSecond;
            if (secondsRecorded >= 3)//最大3s
            {
                StopRecord();
                signFinish = 1;
            }
        }
    }
}

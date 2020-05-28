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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;//引用数据库相关的库

namespace myMovieSystem
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string Conn = "Server=localhost;User Id=root;password=zsz141807;Database=moviedb;Charset=utf8";
        //创建一个字符串，用来存储数据库连接所需要的信息（服务器、用户名、密码、数据库名、编码方式）

        public MainWindow()
        {
            InitializeComponent();
        }

        private void login_Click(object sender, RoutedEventArgs e)
        {
            String userName = nameBox.Text;
            String userPassword = passwordBox.Password;
            if (userName == "" || userPassword == "")
            {
                MessageBox.Show("请填写用户名和密码！");
                return;
            }
            try
            {
                MySqlConnection connection = new MySqlConnection(Conn);
                connection.Open();
                string sql = "select * from db_user where name ='" + userName + "'";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())//运行reader
                {                    
                    string dbpassword = reader["password"].ToString();//存储密码

                    if (userPassword != dbpassword)//若密码不匹配则显示密码错误
                    {
                        MessageBox.Show("密码错误！");
                        return;
                    }
                    else
                    {
                        homePage hp = new homePage();
                        hp.Show();
                        this.Close();
                    }
                    reader.Close();//读取完之后需要将reader关闭
                }
                else
                {
                    MessageBox.Show("该用户不存在！");
                    return;
                }
                connection.Close();//连接打开之后也要记得关闭

            }
            catch
            {
                MessageBox.Show("连接数据库失败");
                return;
            }
        }

        private void register_Click(object sender, RoutedEventArgs e)
        {
            register reg = new register();
            reg.Show();
            this.Close();
        }
    }
}

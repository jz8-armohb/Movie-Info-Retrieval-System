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
using MySql.Data.MySqlClient;

namespace myMovieSystem
{
    /// <summary>
    /// register.xaml 的交互逻辑
    /// </summary>
    public partial class register : Window
    {
        public static string Conn = "Server=localhost;User Id=root;password=zsz141807;Database=movieDB;Charset=utf8";

        public register()
        {
            InitializeComponent();
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            String userName = nameBox.Text; //填写文本框传递过来的用户名
            String userPassword = passwordBox.Text; //填写文本框传递过来的密码
            String email = emailBox.Text;    //填写文本框传递过来的邮箱

            if (userName == "" || userPassword == "" || email == "")
            {
                MessageBox.Show("用户名、密码、邮箱均需填写！");
                return;
            }
            else if (!email.Contains("@") || !email.Contains("."))//判断email中是否包含@和.
            {
                MessageBox.Show("请填写正确的邮箱格式！");
                return;
            }
            else
            {               
                try
                {
                    MySqlConnection connection = new MySqlConnection(Conn);
                    connection.Open();             
                    string sql = "insert into db_user(name,password,email) value('" + userName + "','" + userPassword + "','" + email + "')";   // 插入新用户元组的SQL语句
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("注册成功！");

                    /* 填写跳转到登录页面的代码 */
                    homePage hp = new homePage();
                    hp.Show();
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("连接数据库失败");
                    return;
                }
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            /* 填写跳转到登录页面的代码 */
            homePage hp = new homePage();
            hp.Show();
            this.Close();
        }
    }
}

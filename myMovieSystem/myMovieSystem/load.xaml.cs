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
using MySql.Data.MySqlClient;//引用数据库相关的库
using System.Windows.Forms;

namespace myMovieSystem
{
    /// <summary>
    /// load.xaml 的交互逻辑
    /// </summary>
    public partial class load : Window
    {
        public static string Conn = "Server=localhost;User Id=root;password=zsz141807;Database=moviedb;Charset=utf8";
        // 创建一个字符串，存储的是数据库连接的信息，包括服务器、用户名、密码、数据库名、编码方式
        public load()
        {
            InitializeComponent();
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            insertInfo();
        }
        //复制文件到指定路径，并返回新路径
        private string[] copyPM()
        {
            if (!System.IO.Directory.Exists("...\\poster"))
            {   // 如果路径不存在，建立路径
                System.IO.Directory.CreateDirectory("...\\poster");
            }
            if (!System.IO.Directory.Exists("...\\movie"))
            {   // 如果路径不存在，建立路径
                System.IO.Directory.CreateDirectory("...\\movie");
            }
            string[] newPath =new string[2];
            System.IO.DirectoryInfo topDir = System.IO.Directory.GetParent(System.Environment.CurrentDirectory);
            // 获取该工程运行目录的父目录（到bin）

            newPath[0]= topDir + "\\poster\\" + System.IO.Path.GetFileName(this.posterBox.Text);
            //获取海报文件的完整路径（新路径）
            System.IO.File.Copy(posterBox.Text, newPath[0],true);//true表示可以覆盖原有文件
                                                                 //将海报文件复制到指定路径下

            /* 添加代码，要求将电影文件的新路径返回到newPath[1]中，并复制文件到新路径中 */
            newPath[1] = topDir + "\\movie\\" + System.IO.Path.GetFileName(this.movieBox.Text);
            //获取视频文件的完整路径（新路径）
            System.IO.File.Copy(movieBox.Text, newPath[1], true);//true表示可以覆盖原有文件
            //将电影文件复制到指定路径下


            newPath[0] = newPath[0].Replace("\\", "/");//将路径中的\\换成/
            newPath[1] = newPath[1].Replace("\\", "/");
            return newPath;
        }
        private void insertInfo()
        {
            try
            {
                string movieName = nameBox.Text;        // 添加获取电影名称的代码
                string startTime = timeBox.Text;        // 添加获取上映时间的代码
                string director = directorBox.Text;     // 添加获取导演的代码
                string actors = actorBox.Text;          // 添加获取演员的代码
                string introduction = introBox.Text;    // 添加获取简介的代码

                string[] newPath = new string[2];
                newPath = copyPM(); // 将上载的文件复制到指定路径下，并返回路径

                /* 预留接口，为上载图像信息做准备 */
                imageSearch imgSch = new imageSearch();
                string imgInfo = imgSch.getGray(newPath[0]); // 获取图像的灰度信息

                MySqlConnection conn = new MySqlConnection(Conn);
                conn.Open();
                string sql = "insert into movieinfo(moviename, time, director, actor, content, posterpath, moviepath, imginfo) " +
                    "value('" + movieName + "', '" + startTime + "', '" + director + "', '" + actors + "', '" + introduction + "', '" + newPath[0] + "', '" + newPath[1] + "', '" + imgInfo + "')";  // 插入新信息的SQL
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                System.Windows.MessageBox.Show("上载成功！");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "插入数据库失败");
            }
        }
       
        //打开文件对话框
        public string OpenDialog(String filt)//参数为文件过滤类型说明
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = filt;
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                return filePath;
            }
            else
                return null;
        }
        private void posterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String filt = "图片文件|*.jpg;*.jpeg;*.bmp;";
                this.posterBox.Text = OpenDialog(filt);
            }
            catch
            {
                return;//防止未选择文件就关闭窗口
            }
        }
        private void movieButton_Click(object sender, RoutedEventArgs e)
        {
            /* 添加打开电影文件选择框代码，要求能打开mp4\avi\rmvb格式的电影 */
            try
            {
                String filt = "视频文件|*.mp4;*.avi;*.rmvb;";
                this.movieBox.Text = OpenDialog(filt);
            }
            catch
            {
                return;//防止未选择文件就关闭窗口
            }

        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            homePage hp = new homePage();
            hp.Show();
            this.Close();
        }
    }
}

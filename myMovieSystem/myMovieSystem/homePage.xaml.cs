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
using MySql.Data.MySqlClient;
using System.Windows.Shapes;
using System.Drawing;

namespace myMovieSystem
{
    /// <summary>
    /// homePage.xaml 的交互逻辑
    /// </summary>
    public partial class homePage : Window
    {
        public static string Conn = "Server=localhost;User Id=root;password=zsz141807;Database=moviedb;Charset=utf8";
        int count = 0;//统计数据库中电影总数
        int n = 0; //电影在数据库中的排序
        string[,] getMovieInfo ;//定义二维数组，用于存储电影信息

        public homePage()
        {
            InitializeComponent();
            storeInfo("select * from movieinfo");    //添加参数，要求查找到movieinfo表中的所有信息
            showInfo();
            upButton.IsEnabled = false;
            if (count <= 2)
                /* 若电影数目小于等于2，则不需要翻页 */
                downButton.IsEnabled = false;
        }

        private void storeInfo(string sql)
        {
            try
            {
                count = 0; n = 0;
                getMovieInfo = new string[100, 7];//初始化数组
                MySqlConnection conn = new MySqlConnection(Conn);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //添加代码，请将查询到的电影名称、上映时间、导演、
                    //演员、简介、海报路径、电影路径按顺序存储到数组中(参考登录页面)              
                    getMovieInfo[count, 0] = reader["moviename"].ToString();
                    getMovieInfo[count, 1] = reader["time"].ToString();
                    getMovieInfo[count, 2] = reader["director"].ToString();
                    getMovieInfo[count, 3] = reader["actor"].ToString();
                    getMovieInfo[count, 4] = reader["content"].ToString();
                    getMovieInfo[count, 5] = reader["posterpath"].ToString();
                    getMovieInfo[count, 6] = reader["moviepath"].ToString();
                    count++;
                }
                reader.Close();
                conn.Close();
            }
            catch (Exception)
            {
                return;
            }
        }
        private void showInfo()
        {
            if (getMovieInfo[n, 0] != null)
            {
                BitmapImage img = new BitmapImage(new Uri(getMovieInfo[n, 5], UriKind.Absolute));
                image1.Source = img;
                textBox1.Text = "电影名称：" + getMovieInfo[n, 0] + "\n上映时间：" + getMovieInfo[n, 1] + "\n导演：" + getMovieInfo[n, 2] + "\n演员：" + getMovieInfo[n, 3] + "\n简介：" + getMovieInfo[n, 4];
            }
            else
            {
                image1.Source = null;
                textBox1.Text = "";
            }
            n++;   // 指向数据库中的下一幅海报图片（还未显示)

            if (getMovieInfo[n, 0] != null)
            {
                BitmapImage img = new BitmapImage(new Uri(getMovieInfo[n, 5], UriKind.Absolute));
                image2.Source = img;
                textBox2.Text = "电影名称：" + getMovieInfo[n, 0] + "\n上映时间：" + getMovieInfo[n, 1] + "\n导演：" + getMovieInfo[n, 2] + "\n演员：" + getMovieInfo[n, 3] + "\n简介：" + getMovieInfo[n, 4];
            }
            else
            {
                image2.Source = null;
                textBox2.Text = "";
            }
            n++;
        }
    
        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            upButton.IsEnabled = true;
            showInfo();
            if (n >= count)
                downButton.IsEnabled = false;
        }
       
        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            downButton.IsEnabled = true;
            
            /* n初始值为0，假设现在处于第2页，已经显示完4张海报，那么n = 4；
             * 现要回到第1页，并开始显示第1张海报，即n = 0
             * 故综合上面，n -= 4 */
            n = n - 4;
            if (n <= 0)
                upButton.IsEnabled = false;
            showInfo();           
        }
     
        private void load_Click(object sender, RoutedEventArgs e)
        {
            load load = new load();
            load.Show();
            this.Close();
        }

        private void image1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ClickCount == 2)//如果鼠标点击了两次
                {
                    getMovieInfo[n - 2, 6] = getMovieInfo[n - 2, 6].Replace("/", @"\");
                    System.Diagnostics.Process.Start(getMovieInfo[n - 2, 6]);   // 假设现在在第2页，n = 4，要播放第3张海报，即指向第2张已显示完毕的状态，n = 2，故n -= 2
                }
            }
            catch(Exception)
            {
                MessageBox.Show("哎呀，播放视频失败了！");
            }
        }

        private void image2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ClickCount == 2)
                {
                    getMovieInfo[n - 1, 6] = getMovieInfo[n - 1, 6].Replace("/", @"\");
                    System.Diagnostics.Process.Start(getMovieInfo[n - 1, 6]);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("哎呀，播放视频失败了！");
            }
        }

        private void textSearch_Click(object sender, RoutedEventArgs e)
        {
            string content = text.Text; //添加代码，将查询框中的字符传输进来
            string textSql = "select * from movieinfo where moviename like '%" + content + "%'or time like '%" + content + "%' or director like '%" + content + "%' or actor like '%" + content + "%' or content like '%" + content + "%' or posterpath like '%" + content + "%' or moviepath like '%" + content + "%'";   //添加模糊查询语句，要求查询到包含查询框中文字的所有电影信息
            storeInfo(textSql);
            showInfo();
            if (count <= 2)
                downButton.IsEnabled = false;
            else
                downButton.IsEnabled = true;
            upButton.IsEnabled = false;
        }

        private void openDialog_Click(object sender, RoutedEventArgs e)
        {
            load load = new load();
            String filt = "图片文件|*.jpg;*.jpeg;*.bmp;";           
            image.Text = load.OpenDialog(filt);
        }

        private void imageSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                imageSearch imgSearch = new imageSearch();
                string upGray = imgSearch.getGray(image.Text);  // 获取上载图像的灰度值
                int[] imgOrder = imgSearch.searchResult(upGray);    // 获取与数据库中图像的比较结果
                count = 0; n = 0;
                string sql = "select * from movieinfo";
                getMovieInfo = new string[100, 7];//初始化数组
                MySqlConnection conn = new MySqlConnection(Conn);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //查找数组中的索引，需要理解一下
                    getMovieInfo[Array.IndexOf(imgOrder, count), 0] = reader["moviename"].ToString();
                    getMovieInfo[Array.IndexOf(imgOrder, count), 1] = reader["time"].ToString();
                    getMovieInfo[Array.IndexOf(imgOrder, count), 2] = reader["director"].ToString();
                    getMovieInfo[Array.IndexOf(imgOrder, count), 3] = reader["actor"].ToString();
                    getMovieInfo[Array.IndexOf(imgOrder, count), 4] = reader["content"].ToString();
                    getMovieInfo[Array.IndexOf(imgOrder, count), 5] = reader["posterpath"].ToString();
                    getMovieInfo[Array.IndexOf(imgOrder, count), 6] = reader["moviepath"].ToString();
                    count++;
                }
                for (int i = 3; i < count; i++)//只取前3位
                    getMovieInfo[i, 0] = null;
                count = 3;
                showInfo();
                downButton.IsEnabled = true;
                upButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "查询失败");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MySql.Data.MySqlClient;

namespace myMovieSystem
{
    class imageSearch
    {
        public static string Conn = "Server=localhost;User Id=root;password=123456;Database=dbclass;Charset=utf8";
        //获取图像灰度值
        public string getGray(string filename)
        {
            Bitmap curBitmap = (Bitmap)Image.FromFile(filename);
            int width = curBitmap.Width;
            int height = curBitmap.Height;
            Color curColor;
            int[] gray = new int[256];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    curColor = curBitmap.GetPixel(i,j);//获取当前像素的颜色值
                    //转化为灰度值
                    int m = (int)(curColor.R * 0.299 + curColor.G * 0.587 + curColor.B * 0.114);
                    if (m > 255) m = 255;
                    if (m < 0) m = 0;
                    gray[m]++;//添加注释，具体解释该数组中存放的对象
                }
            }
            string g = string.Join(",", gray);//将数组转化为字符串存储
            return g;
        }
        //比较灰度直方图
        public int compare(string sqlImageGray, string upImageGray)
        {
            int count = 0;
            string [] sqlGray = sqlImageGray.Split(',');
            string[] upGray = upImageGray.Split(',');
            for (int i = 0; i < 256; i++)
                if (sqlGray[i] == upGray[i])
                    count++;//添加注释，解释比较的方法以及该函数返回值的意义
            return count;
        }
        //搜索数据库，按相似度顺序返回比较结果
        public int[] searchResult(string upGray)
        {
            int[] imgOrder = new int[100];
            int n = 0;
            string sql = "select imginfo from db_movieinfo";
            MySqlConnection conn = new MySqlConnection(Conn);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                imgOrder[n++] = compare(reader["imginfo"].ToString(), upGray);
                //添加注释，理解imgOrder数组中存放的对象
            }
            reader.Close();
            int[] index = sortGray(imgOrder);//调用自定义排序方法
            return index;
        }

        //排序，索引不变排序法
        private int[] sortGray(int[] imgOrder)
        {
            int len = imgOrder.Length;
            int[] index = new int[len];
            int temp ;
            for (int i = 0; i < len; i++)
                index[i] = i;//用于存储下标
            for (int j = 0; j < len; j++)
                for (int i = 0; i < len - j - 1; i++)//降序排序
                {
                    if (imgOrder[i] < imgOrder[i + 1])
                    {
                        //交换数值
                        temp = imgOrder[i];
                        imgOrder[i] = imgOrder[i + 1];
                        imgOrder[i + 1] = temp;
                        //交换下标
                        temp = index[i];
                        index[i] = index[i + 1];
                        index[i + 1] = temp;
                    }
                }
            return index;
        }
    }
}

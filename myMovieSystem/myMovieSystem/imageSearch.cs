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
        /* 获取图像灰度值 */
        public static string Conn = "Server=localhost;User Id=root;password=zsz141807;Database=moviedb;Charset=utf8";
        public string getGray(string filename)
        {
            Bitmap curBitmap = (Bitmap)Image.FromFile(filename);
            int width = curBitmap.Width;    // 获取图像宽
            int height = curBitmap.Height;  // 获取图像高
            Color curColor;
            int[] gray = new int[256];  // 存储每个灰度值的频次
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    curColor = curBitmap.GetPixel(i,j); // 获取当前像素的颜色值
                    /* 转化为灰度值 */
                    int m = (int)(curColor.R * 0.299 + curColor.G * 0.587 + curColor.B * 0.114);    // Y分量
                    if (m > 255) m = 255;   // 溢出处理
                    if (m < 0) m = 0;
                    gray[m]++;  // 对应灰度值的count + 1
                }
            }
            string g = string.Join(",", gray);  // 将数组转化为字符串存储
            return g;
        }


        /* 比较灰度直方图 */
        public int compare(string sqlImageGray, string upImageGray)
        {
            int count = 0;
            string [] sqlGray = sqlImageGray.Split(',');
            string[] upGray = upImageGray.Split(',');
            for (int i = 0; i < 256; i++)
                if (sqlGray[i] == upGray[i])
                    count++;    // 灰度值相同，则count + 1
            return count;   // 返回灰度值相同的count
        }


        /* 搜索数据库，按相似度顺序返回比较结果 */
        public int[] searchResult(string upGray)
        {
            int[] imgOrder = new int[100];
            int n = 0;
            string sql = "select imginfo from moviedb";
            MySqlConnection conn = new MySqlConnection(Conn);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read() != 0)
            {
                imgOrder[n++] = compare(reader["imginfo"].ToString(), upGray);  // imgOrder数组中存放灰度值相同的count
            }
            reader.Close();
            int[] index = sortGray(imgOrder);   // 调用自定义排序方法
            return index;
        }


        /* 排序，索引不变排序法 */
        private int[] sortGray(int[] imgOrder)
        {
            int len = imgOrder.Length;
            int[] index = new int[len]; // index[0]中存储相似度最高的图片序号
            int temp ;

            for (int i = 0; i < len; i++)
                index[i] = i;   // 用于存储下标
            for (int j = 0; j < len; j++)
                for (int i = 0; i < len - j - 1; i++)   // 降序排序
                {
                    if (imgOrder[i] < imgOrder[i + 1])
                    {
                        // 交换数值
                        temp = imgOrder[i];
                        imgOrder[i] = imgOrder[i + 1];
                        imgOrder[i + 1] = temp;

                        // 交换下标
                        temp = index[i];
                        index[i] = index[i + 1];
                        index[i + 1] = temp;
                    }
                }
            return index;
        }
    }
}

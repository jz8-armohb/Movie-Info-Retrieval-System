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
            string sql = "select imginfo from movieinfo";
            MySqlConnection conn = new MySqlConnection(Conn);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
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


        /***************** dHash (added by S.Z.Zheng) *****************/
        public string dHashFingerprint(string filename)
        {
            Bitmap currentBitmap = (Bitmap)Image.FromFile(filename);
            int width = currentBitmap.Width;    // 获取图像宽
            int height = currentBitmap.Height;  // 获取图像高
            Color pxColour;
            byte[] grey = new byte[width * height]; // 存储图像灰度值
            //int grey_avr = 0;   // 图像平均灰度值
            double sampFactorW = width / 8;    // 水平方向下采样因子
            double sampFactorH = height / 9;   // 垂直方向下采样因子
            int[] thumb = new int[9 * 8];   // 9x8的缩略图
            int[] dMatrix = new int[8 * 8]; // 差分矩阵
            byte[] fingerprint = new byte[8 * 8];

            /* 将图像转换为灰度图像 */
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    pxColour = currentBitmap.GetPixel(j, i); // 获取当前像素的颜色值（注意是j,i不是i,j）
                    int pxValue = (int)(pxColour.R * 0.299 + pxColour.G * 0.587 + pxColour.B * 0.114);    // Y分量
                    if (pxValue > 255)  // 溢出处理
                        pxValue = 255;
                    if (pxValue < 0) 
                        pxValue = 0;
                    grey[i * width + j] = (byte)pxValue;
                    //grey_avr += pxValue;
                }
            }
            //grey_avr /= (width * height);

            /* 生成缩略图 */
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int topEdge = (int)(sampFactorH * i);   // 上边界
                    int bottomEdge = (int)(sampFactorH * (i + 1));  // 下边界
                    int leftEdge = (int)(sampFactorW * j);  // 左边界
                    int rightEdge = (int)(sampFactorW * (j + 1)); // 右边界
                    int count = 0;

                    for (int ii = topEdge; ii < bottomEdge; ii++)
                    {
                        for (int jj = leftEdge; jj < rightEdge; jj++)
                        {
                            thumb[i * 8 + j] += grey[ii * width + jj];     // 亮度累积
                            count++;
                        }
                    }
                    thumb[i * 8 + j] /= count;    // 得出每块的平均亮度
                }
            }

            /* 生成差分矩阵 */
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    dMatrix[i * 8 + j] = thumb[(i + 1) * 8 + j] - thumb[i * 8 + j];
                }
            }

            /* 生成fingerprint */
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (dMatrix[i * 8 + j] >= 0)
                    {
                        fingerprint[i * 8 + j] = 1;
                    }
                    else
                    {
                        fingerprint[i * 8 + j] = 0;
                    }
                }
            }

            string fpStr = string.Join(",", fingerprint);   // 将fingerprint以逗号作为分隔符，排列成字符串
            return fpStr;
        }

        public int HashFpSimilarity(string dbImgHashFp, string upImgHashFp)
        {
            int hammingDist = 0;
            string[] dbHashFp = dbImgHashFp.Split(',');
            string[] upHashFp = upImgHashFp.Split(',');
            for (int i = 0; i < 64; i++)
            {
                if (dbHashFp[i] == upHashFp[i])
                {
                    hammingDist++;
                }
            }

            return hammingDist;
        }

        public int[] HashSearchResult(string upHashFp)
        {
            int[] imgOrder = new int[100];  // 最多比较100张图
            int n = 0;
            string sql = "select imginfo from movieinfo";
            MySqlConnection conn = new MySqlConnection(Conn);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                imgOrder[n++] = HashFpSimilarity(reader["imginfo"].ToString(), upHashFp);  // imgOrder数组中存放上载图像与数据库中各个图像间的汉明距离
            }
            reader.Close();
            int[] index = SortByHashSimilarity(imgOrder);   // 调用自定义排序方法
            return index;
        }

        private int[] SortByHashSimilarity(int[] imgOrder)
        {
            int imgNum = imgOrder.Length;
            int[] index = new int[imgNum]; // index[0]中存储相似度最高的图片序号
            int temp;

            for (int i = 0; i < imgNum; i++)
            {
                index[i] = i;   // 用于存储下标
            }

            for (int i = 0; i < imgNum; i++)
                for (int j = 0; j < imgNum - i - 1; j++)   // 降序排序
                {
                    if (imgOrder[j] < imgOrder[j + 1])
                    {
                        // 交换数值
                        temp = imgOrder[j];
                        imgOrder[j] = imgOrder[j + 1];
                        imgOrder[j + 1] = temp;

                        // 交换下标
                        temp = index[j];
                        index[j] = index[j + 1];
                        index[j + 1] = temp;
                    }
                }
            return index;
        }
    }
}

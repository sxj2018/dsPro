using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Picture
{
  public  class ImageHelper
    {

        //将BMP图象转换为JPEG图象  
        public static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            return null;
        }
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param>   
        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            using (System.Drawing.Image originalImage = System.Drawing.Image.FromFile(originalImagePath))
            {
                int towidth = width;
                int toheight = height;
                int x = 0;
                int y = 0;
                int ow = originalImage.Width;
                int oh = originalImage.Height;
                if (originalImage.Width <= width && originalImage.Height <= height && mode.ToUpper() != "CUT" && mode.ToUpper() != "CUTA")
                {
                    File.Copy(originalImagePath, thumbnailPath, true);
                    return;
                }
                switch (mode.ToUpper())
                {
                    case "HW"://指定高宽缩放（可能变形）
                        break;
                    case "W"://指定宽，高按比例
                        toheight = originalImage.Height * width / originalImage.Width;
                        break;
                    case "H"://指定高，宽按比例
                        towidth = originalImage.Width * height / originalImage.Height;
                        break;
                    case "CUTA"://指定高宽裁减（不变形）
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        else
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        break;
                    case "CUT"://指定高宽裁减（不变形）自定义
                               //if (ow <= towidth && oh <= toheight)
                               //{
                               //    x = -(towidth - ow) / 2;
                               //    y = -(toheight - oh) / 2;
                               //    ow = towidth;
                               //    oh = toheight;
                               //}
                               //else
                               //{
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            ow = originalImage.Width;
                            oh = originalImage.Width * height / towidth;
                            x = 0;
                            y = (originalImage.Height - oh) / 2;
                        }
                        else
                        {
                            oh = originalImage.Height;
                            ow = originalImage.Height * towidth / toheight;
                            y = 0;
                            x = (originalImage.Width - ow) / 2;
                        }
                        //if (ow > oh)//宽大于高
                        //{
                        //    x = 0;
                        //    y = -(ow - oh) / 2;
                        //    oh = ow;
                        //}
                        //else//高大于宽
                        //{
                        //    y = 0;
                        //    x = -(oh - ow) / 2;
                        //    ow = oh;
                        //}
                        //}
                        break;
                    case "CUT1":
                        if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                        {
                            toheight = originalImage.Height * width / originalImage.Width;
                        }
                        else
                        {
                            towidth = originalImage.Width * height / originalImage.Height;
                        }
                        break;
                    default:
                        break;
                }
                //新建一个bmp图片
                System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);
                //新建一个画板
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);
                //设置高质量插值法
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //清空画布并以透明背景色填充
                //g.Clear(System.Drawing.Color.Transparent);
                g.Clear(System.Drawing.Color.White);
                //在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
                    new System.Drawing.Rectangle(x, y, ow, oh),
                    System.Drawing.GraphicsUnit.Pixel);
                try
                {
                    //string fileExtension = Path.GetExtension(originalImagePath).ToLower();
                    //switch (fileExtension)
                    //{
                    //    case ".jpg":
                    //        //以jpg格式保存缩略图
                    //        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg); break;
                    //    case ".jpeg":
                    //        //以jpg格式保存缩略图
                    //        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg); break;
                    //    case ".gif":
                    //        //以jpg格式保存缩略图
                    //        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Gif); break;
                    //    case ".png":
                    //        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Png); break;
                    //}
                    //以jpg格式保存缩略图
                    //bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ImageCodecInfo ici;
                    System.Drawing.Imaging.Encoder enc;
                    EncoderParameter ep;
                    EncoderParameters epa;
                    //   Initialize   the   necessary   objects  
                    ici = GetEncoderInfo("image/jpeg");
                    enc = System.Drawing.Imaging.Encoder.Quality;//设置保存质量  
                    epa = new EncoderParameters(1);
                    //   Set   the   compression   level  
                    ep = new EncoderParameter(enc, 90L);//质量等级为90%
                    epa.Param[0] = ep;
                    bitmap.Save(thumbnailPath, ici, epa);
                }
                catch (System.Exception e)
                {
                    throw e;
                }
                finally
                {
                    originalImage.Dispose();
                    bitmap.Dispose();
                    g.Dispose();
                }
            }
        }
        /// <summary>
        /// 得到需要的ImageURL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sufix"></param>
        /// <returns></returns>
        public static string GetImageUrl(string url, string sufix)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            return GetImageUrl(url + sufix);
        }
        /// <summary>
        /// 得到需要的ImageURL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sufix"></param>
        /// <returns></returns>
        public static string GetImageUrl(string url)
        {
            if (url.StartsWith("/"))
            {
                url = "~" + url;
            }
            return url;
        }

    }
}

using Gabriel.Cat.S.Extension;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Xamarin.Forms;
using Image = Xamarin.Forms.Image;

namespace Gabriel.Cat.Extension
{
    public static class ExtensionImage
    {
        public static void SetImage(this Image img,byte[] imgData)
        {
            img.Source = ImageSource.FromStream(() => new System.IO.MemoryStream(imgData));
        }
        public static void SetImage(this Image img,Bitmap bmp)
        {
            img.SetImage(bmp.GetBytes());
        }
    }
}

using Gabriel.Cat.S.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Gabriel.Cat.Extension
{
    public static class ExtensionImageSource
    {
        public static FileImageSource ToFileImageSource(this ImageSource img)
        {
            string pathAleatorio = System.IO.Path.GetRandomFileName();
            FileImageSource imgFile = new FileImageSource();
            FileInfo file = new FileInfo(pathAleatorio);
           
            file.WriteAllBytes(img.ToStream());
      
            imgFile.File = pathAleatorio;

            return imgFile;
        }

        public static Stream ToStream(this ImageSource img)
        {
            StreamImageSource streamImageSource = (StreamImageSource)img;
            System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None;
            Task<Stream> task = streamImageSource.Stream(cancellationToken);
            return task.Result;
        }


    }
}

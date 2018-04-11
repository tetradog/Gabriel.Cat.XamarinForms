using System.IO;
using Xamarin.Forms;

namespace Gabriel.Cat.Extension
{
    public static class ExtensionButton
    {
        public static void SetImage(this Button btn, byte[] img)
        {
            btn.SetImage(new MemoryStream(img));
        }
        public static void SetImage(this Button btn, Stream img)
        {
            btn.Image = FileImageSource.FromStream(() => { return img; }).ToFileImageSource();
        }
    }
}

using ImageProcessor;
using ImageProcessor.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSEngine
{
    /// <summary>
    /// reports the over all results of a run of the VSEngine
    /// </summary>
    public class PostReporting
    {
        // out put finial json file of reutls
        // out images in the right format
        public void WriteIamges(Dictionary<string, byte[]> results)
        {
            int imageIndex = 0;
            foreach(KeyValuePair<string, byte[]> currentResult in results)
            {
                using (MemoryStream ms = new MemoryStream(currentResult.Value))
                {
                    TextLayer text = new TextLayer();
                    text.Text = currentResult.Key;
                    text.FontColor = Color.OrangeRed;
                    text.Opacity = 30;
                    text.DropShadow = true;
                    Image currentImage = Image.FromStream(ms);
                    using (ImageFactory constuctImage = new ImageFactory())
                    {
                        constuctImage.Load(currentResult.Value)
                            .Watermark(text)
                            .Save(Directory.GetCurrentDirectory() + "\\" + imageIndex + ".png");
                    }
                    //currentImage.Save(imageIndex + ".png");
                }

                imageIndex++;
            }
        }
        // zip up results for archive
    }
}

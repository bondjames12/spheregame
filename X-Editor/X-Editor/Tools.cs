using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;

namespace X_Editor
{
    public class Tools
    {
        public static void copyDirectory(string Src,string Dst)
        {
            String[] Files;

            if(Dst[Dst.Length-1]!=Path.DirectorySeparatorChar) 
                Dst+=Path.DirectorySeparatorChar;

            if(!Directory.Exists(Dst)) Directory.CreateDirectory(Dst);
            
            Files=Directory.GetFileSystemEntries(Src);

            foreach(string Element in Files){
                if(Directory.Exists(Element)) 
                    copyDirectory(Element,Dst+Path.GetFileName(Element));
                else 
                    File.Copy(Element,Dst+Path.GetFileName(Element),true);
            }
        }

        public static GridItem GetPropertyAtPoint(Point point, PropertyGrid properties)
        {
            GridItemCollection items = properties.SelectedGridItem.Parent.GridItems;

            for (int i = 0; i < items.Count; i++)
            {
                int top = 27 + (15 * i) + i;
                int bottom = 27 + (15 * (i + 1)) + i;

                if (point.Y < bottom && point.Y > top)
                    return items[i];
            }

            return null;
        }

        public unsafe static Image LoadImage(Texture2D tex)
        {

            uint[] d = new uint[tex.Width * tex.Height];
            tex.GetData<uint>(d);

            Bitmap bmp = new Bitmap(tex.Width,
                            tex.Height,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            System.Drawing.Imaging.BitmapData bmpd =
                    bmp.LockBits(
                        new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                        System.Drawing.Imaging.ImageLockMode.WriteOnly,
                        System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            uint* ptr = (uint*)bmpd.Scan0.ToPointer();

            for (int x = 0; x < tex.Width; x++)
                for (int y = 0; y < tex.Height; y++)
                {
                    ptr[x + y * tex.Width] = d[x + y * tex.Width];
                }

            bmp.UnlockBits(bmpd);

            return bmp;
        }

    }
}

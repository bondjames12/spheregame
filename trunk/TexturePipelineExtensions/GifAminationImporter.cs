/*
	                  XNA Gif Animation Library
    Copyright (C) 2007-2008 Mahdi Khodadadi Fard (mahdi3466@yahoo.com)

    License:
    Permission is hereby granted, free of charge, to any person obtaining a copy of
    this software and associated documentation files (the "Software"), to deal in
    the Software without restriction, including without limitation the rights to
    use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
    the Software, and to permit persons to whom the Software is furnished to do so,
    subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software or in "About" menu.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
    FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
    COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
    IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
    CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace XNAGifAnimationLibrary.Pipeline
{
    [ContentImporter(".gif", DisplayName = "Gif Animation Importer", DefaultProcessor = "Gif Animation Processor")]
    internal class GifAminationImporter : ContentImporter<GifAnimationContent>
    {
        public override GifAnimationContent Import(string filename, ContentImporterContext context)
        {
            GifAnimationContent content = new GifAnimationContent();

            Image MasterImage = Image.FromFile(filename);

            FrameDimension oDimension = new FrameDimension(MasterImage.FrameDimensionsList[0]);

            int FrameCount = MasterImage.GetFrameCount(oDimension);
            //MessageBox.Show(FrameCount.ToString());
            content.Frames = new TextureData[FrameCount];

            for (int i = 0; i < FrameCount; i++)
            {
                MasterImage.SelectActiveFrame(oDimension, i);

                byte[] quantized = Quantizer.Quantize(MasterImage);
                //MessageBox.Show(quantized[9999].ToString());
                content.Frames[i].__1__SurfaceFormat = (int)SurfaceFormat.Color;
                content.Frames[i].__2__Width = MasterImage.Width;
                content.Frames[i].__3__Height = MasterImage.Height;
                content.Frames[i].__4__Levels = 1;
                content.Frames[i].Data = quantized;
            }

            MasterImage.Dispose();

            return content;
        }
    }
}

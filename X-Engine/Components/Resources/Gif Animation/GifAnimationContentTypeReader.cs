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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XEngine
{
    public class GifAnimationContentTypeReader : ContentTypeReader<XTextureSequence>
    {
        protected override XTextureSequence Read(ContentReader input, XTextureSequence existingInstance)
        {
            int l = input.ReadInt32();
            
            Texture2D[] textures = new Texture2D[l];

            IGraphicsDeviceService service = (IGraphicsDeviceService)input.ContentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
            if (service == null)
            {
                throw new ContentLoadException();
            }
            GraphicsDevice graphicsDevice = service.GraphicsDevice;
            if (graphicsDevice == null)
            {
                throw new ContentLoadException();
            }

            for (int i = 0; i < l; i++)
            {
                SurfaceFormat format = (SurfaceFormat)input.ReadInt32();
                int width = input.ReadInt32();
                int height = input.ReadInt32();
                int numberLevels = input.ReadInt32();
                textures[i] = new Texture2D(graphicsDevice, width, height, numberLevels, TextureUsage.None, format);
                
                for (int j = 0; j < numberLevels; j++)
                {
                    int count = input.ReadInt32();
                    byte[] data = input.ReadBytes(count);
                    Rectangle? rect = null;
                    textures[i].SetData<byte>(j, rect, data, 0, data.Length, SetDataOptions.None);
                }

            }
            input.Close();

            return new XTextureSequence(textures);
        }
    }
}

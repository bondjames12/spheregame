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

namespace XNAGifAnimationLibrary.Pipeline
{
    struct TextureData
    {
        public int __1__SurfaceFormat;
        public int __2__Width;
        public int __3__Height;
        public int __4__Levels;

        public byte[] Data;
        //SurfaceFormat format = (SurfaceFormat)input.ReadInt32();
        //int width = input.ReadInt32();
        //int height = input.ReadInt32();
        //int numberLevels = input.ReadInt32();
        //Texture2D textured = new Texture2D(input.GraphicsDevice, width, height, numberLevels, ResourceUsage.None, format, ResourceManagementMode.Automatic);
        //for (int i = 0; i < numberLevels; i++)
        //{
        //    int count = input.ReadInt32();
        //    byte[] data = input.ReadBytes(count);
        //    Rectangle? rect = null;
        //    textured.SetData<byte>(i, rect, data, 0, data.Length, SetDataOptions.None);
        //}
        //return textured;
    }
}

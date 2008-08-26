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
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using XEngine;

namespace XNAGifAnimationLibrary.Pipeline
{
    [ContentTypeWriter]
    class GifAnimationContentTypeWriter : ContentTypeWriter<GifAnimationContent>
    {
        protected override void Write(ContentWriter output, GifAnimationContent value)
        {
            output.Write(value.Frames.Length);

            for (int i = 0; i < value.Frames.Length; i++)
            {
                output.Write(value.Frames[i].__1__SurfaceFormat);
                output.Write(value.Frames[i].__2__Width);
                output.Write(value.Frames[i].__3__Height);
                output.Write(value.Frames[i].__4__Levels);
                output.Write(value.Frames[i].Data.Length);
                output.Write(value.Frames[i].Data);
                output.Flush();
            }
            
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(GifAnimationContentTypeReader).AssemblyQualifiedName;
        }
    }
}

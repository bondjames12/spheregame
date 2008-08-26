#region File Description
//-----------------------------------------------------------------------------
// InstancedModelContentWriter.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
#endregion

namespace PipelineExtensions
{
    /// <summary>
    /// Content pipeline support class for saving out InstancedModelContent objects.
    /// </summary>
    [ContentTypeWriter]
    public class InstancedModelContentWriter : ContentTypeWriter<InstancedModelContent>
    {
        /// <summary>
        /// Saves instanced model data into an XNB file.
        /// </summary>
        protected override void Write(ContentWriter output, InstancedModelContent value)
        {
            value.Write(output);
        }


        /// <summary>
        /// Tells the content pipeline what CLR type the instanced
        /// model data will be loaded into at runtime.
        /// </summary>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "InstancedModelSample.InstancedModel, " +
                   "InstancedModelSample, Version=1.0.0.0, Culture=neutral";
        }


        /// <summary>
        /// Tells the content pipeline what worker type
        /// will be used to load the instanced model data.
        /// </summary>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "InstancedModelSample.InstancedModelReader, " +
                   "InstancedModelSample, Version=1.0.0.0, Culture=neutral";
        }
    }
}

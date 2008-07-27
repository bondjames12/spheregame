#region File Description
//-----------------------------------------------------------------------------
// SkinnedModelProcessor.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using SkinnedModel;
#endregion

namespace ContentModel
{
    /// <summary>
    /// Custom processor extends the builtin framework ModelProcessor class,
    /// adding animation support.
    /// </summary>
    public class SkinnedModelProcessor
    {
        // Maximum number of bone matrices we can render using shader 2.0
        // in a single pass. If you change this, update SkinnedModelfx to match.
        const int MaxBones = 59;

    
        /// <summary>
        /// The main Process method converts an intermediate format content pipeline
        /// NodeContent tree to a ModelContent object with embedded animation data.
        /// </summary>
        public SkinningData Process(NodeContent input,
                                             ContentProcessorContext context)
        {
            SkinnedModelHelper.ValidateMesh(input, context, null);

            // Find the skeleton.
            BoneContent skeleton = MeshHelper.FindSkeleton(input);

            if (skeleton == null)
                return null;

            // We don't want to have to worry about different parts of the model being
            // in different local coordinate systems, so let's just bake everything.
            SkinnedModelHelper.FlattenTransforms(input, skeleton);

            // Read the bind pose and skeleton hierarchy data.
            IList<BoneContent> bones = MeshHelper.FlattenSkeleton(skeleton);

            if (bones.Count > MaxBones)
            {
                throw new InvalidContentException(string.Format(
                    "Skeleton has {0} bones, but the maximum supported is {1}.",
                    bones.Count, MaxBones));
            }

            List<Matrix> bindPose = new List<Matrix>();
            List<Matrix> inverseBindPose = new List<Matrix>();
            List<int> skeletonHierarchy = new List<int>();

            foreach (BoneContent bone in bones)
            {
                bindPose.Add(bone.Transform);
                inverseBindPose.Add(Matrix.Invert(bone.AbsoluteTransform));
                skeletonHierarchy.Add(bones.IndexOf(bone.Parent as BoneContent));
            }

            // Convert animation data to our runtime format.
            Dictionary<string, AnimationClip> animationClips;
            animationClips = SkinnedModelHelper.ProcessAnimations(skeleton.Animations, bones);

            // Store our custom animation data in the Tag property of the model.
            return new SkinningData(animationClips, bindPose,
                                         inverseBindPose, skeletonHierarchy);
        }

        /// <summary>
        /// Changes all the materials to use our skinned model effect.
        /// </summary>
        /*protected override MaterialContent ConvertMaterial(MaterialContent material,
                                                        ContentProcessorContext context)
        {
            BasicMaterialContent basicMaterial = material as BasicMaterialContent;

            if (basicMaterial == null)
            {
                throw new InvalidContentException(string.Format(
                    "SkinnedModelProcessor only supports BasicMaterialContent, " +
                    "but input mesh uses {0}.", material.GetType()));
            }

            EffectMaterialContent effectMaterial = new EffectMaterialContent();

            // Store a reference to our skinned mesh effect.
            string effectPath = GetType().Assembly.CodeBase.Remove(0, 8).Replace("SkinnedModelProcessor.dll", "").Replace(@"SkinnedModelProcessor/SkinnedModelProcessor", "X-Engine").Replace("/bin", "").Replace("/Debug", "").Replace("/x86", "").Replace("/Xbox 360", "") + "Content\\XEngine\\Effects\\Model.fx";

            effectMaterial.Effect = new ExternalReference<EffectContent>(effectPath);

            // Copy texture settings from the input
            // BasicMaterialContent over to our new material.
            if (basicMaterial.Texture != null)
                effectMaterial.Textures.Add("Texture", basicMaterial.Texture);

            // Chain to the base ModelProcessor converter.
            return base.ConvertMaterial(effectMaterial, context);
        }*/
    }
}

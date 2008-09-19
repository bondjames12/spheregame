/* 
 * Copyright (c) 2007 Asger Feldthaus
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
 * and associated documentation files (the "Software"), to deal in the Software without restriction, 
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:  
 * 
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
 * WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, 
 * ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//TODO Move tree generation code into here have somekind of selection
namespace XEngine
{
    /// <summary>
    /// Holds the mesh of a tree's trunk, its leaves, and its bounding box.
    /// </summary>
    public class XTreeModel : XComponent
    {
        string Profile;
        string BarkTexture;
        string LeafTexture;

        int Seed;
        int RadialSegments;
        bool AddLeaves;
        int CuttoffLevel;

        #region Editor Properties
        public string XMLProfile
        {
            get { return Profile; }
            set { Profile = value; }
        }

        public string barkTexture
        {
            get { return BarkTexture; }
            set { BarkTexture = value; }
        }

        public string leafTexture
        {
            get { return LeafTexture; }
            set { LeafTexture = value; }
        }

        public int seed
        {
            get { return Seed; }
            set { Seed = value; }
        }

        public int radialSegments
        {
            get { return RadialSegments; }
            set { RadialSegments = value; }
        }

        public int cuttoffLevel
        {
            get { return CuttoffLevel; }
            set { CuttoffLevel = value; }
        }

        public bool addLeaves
        {
            get { return AddLeaves; }
            set { AddLeaves = value; }
        } 
        #endregion

        /// <summary>
        /// Holds the trunk's mesh. You need this to draw the tree's trunk.
        /// </summary>
        public Mesh Trunk;

        /// <summary>
        /// Holds the leaves on the tree, as a particle cloud. You need this to draw the leaves.
        /// </summary>
        public ParticleCloud Leaves;

        /// <summary>
        /// A bounding box containing all the vertices and leaves in the tree.
        /// </summary>
        public BoundingBox boundingBox;

        public XTreeModel(ref XMain X, string XMLProfileFile, string BarkTextureFile, string LeafTextureFile) : base(ref X)
        {
            //generate some random values as default for a new TreeModel
            Random rand = new Random();

            Seed = rand.Next();
            RadialSegments = rand.Next(8,12);
            AddLeaves = true;
            CuttoffLevel = rand.Next(0, 2);

            this.Profile = XMLProfileFile;
            this.LeafTexture = LeafTextureFile;
            this.BarkTexture = BarkTextureFile;
        }

        public override void Load(Microsoft.Xna.Framework.Content.ContentManager Content)
        {
            TreeGenerator generator = new TreeGenerator(X.GraphicsDevice, X.cloudSystem, X.Content.Load<Effect>("Content/Effects/Lambert"));
            generator.LoadFromFile(Profile);
            generator.TrunkEffect.Parameters["AmbientMap"].SetValue(X.Content.Load<Texture2D>(BarkTexture));
            generator.LeafTexture = X.Content.Load<Texture>(LeafTexture);
            
            generator.GenerateTreeMesh(ref X, Seed, RadialSegments, AddLeaves, CuttoffLevel, out Trunk, out boundingBox, out Leaves);
            Trunk.Projection = X.DefaultCamera.Projection;


            base.Load(Content);
        }

    }//end class
}//end namespace

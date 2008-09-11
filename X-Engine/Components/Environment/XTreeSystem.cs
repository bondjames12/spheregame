using System;
using System.Collections.Generic;
using System.Text;
using Feldthaus.Xna;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XEngine
{
    public struct TreeFile
    {
        public string Name;
        public string Profile;
        public string BarkTexture;
        public string LeafTexture;

        public TreeFile(string name, string profile, string bark, string leaf)
        {
            Name = name;
            Profile = profile;
            BarkTexture = bark;
            LeafTexture = leaf;
        }
    }

    /// <summary>
    /// Encapsulates a tree rendering system for the whole level or game
    /// Can make new XTrees
    /// Each XTree object is responsible for rendering itself!
    /// 
    /// </summary>
    public class XTreeSystem : XComponent
    {
        public static readonly TreeFile[] TreeTypes =
        {
            new TreeFile("Oak", "Oak.xml", "OakBark", "OakLeaf"),
            new TreeFile("Willow", "Willow.xml", "WillowBark", "WillowLeaf"),
            new TreeFile("Aspen", "Aspen.xml", "AspenBark", "AspenLeaf"),
            new TreeFile("Pine", "Pine.xml", "PineBark", "PineLeaf")
        };

        public const string treePath = "Content/Trees/";
        public const string texturePath = "Content/Textures/";
        XHeightMap heightMap;
        ParticleCloudSystem cloudSystem;
        string treeMapFile;
        List<Vector3> treeMap;

        List<TreeGenerator> generators;
        List<XTree> trees;

        #region Editor Properties
        public string TreeMapFile
        {
            get { return treeMapFile; }
            set { treeMapFile = value; }
        }

        public XHeightMap HeightMap
        { get { return heightMap; } set { heightMap = value; } }

        #endregion

        public XTreeSystem(ref XMain X, string TreeMapFile, XHeightMap heightMap)
            : base(ref X)
        {
            treeMapFile = TreeMapFile;
            this.heightMap = heightMap;
            treeMap = new List<Vector3>();
            trees = new List<XTree>();
        }

        public override void Load(ContentManager Content)
        {
            // Initialize the particle cloud system for tree leaves!
            cloudSystem = new ParticleCloudSystem(X.GraphicsDevice, X.Content, null);
            cloudSystem.Initialize();

            generators = new List<TreeGenerator>();

            // Create a tree generator for each kind of tree defined above in treeFiles
            for (int i = 0; i < TreeTypes.Length; i++)
            {
                TreeGenerator generator = new TreeGenerator(X.GraphicsDevice, cloudSystem,X.Content.Load<Effect>("Content/Effects/Lambert"));
                generator.LoadFromFile(treePath + TreeTypes[i].Profile);

                // Set the texture assigned to newly generated trees.
                //TODO: Add custom shader for tree trunks to allow Engine lighting, etc
                generator.TrunkEffect.Parameters["AmbientMap"].SetValue(X.Content.Load<Texture2D>(texturePath + TreeTypes[i].BarkTexture));
                //generator.TrunkEffect.TextureEnabled = true;
                //generator.TrunkEffect.EnableDefaultLighting();
                generator.LeafTexture = X.Content.Load<Texture>(texturePath + TreeTypes[i].LeafTexture);
                generators.Add(generator);
            }

            //Generate a list of Vector3 of all tree positions
            GenerateFromTreemap(X.Content.Load<Texture2D>(treeMapFile));

            base.Load(X.Content);
        }

        private void GenerateFromTreemap(Texture2D Heighmap)
        {
            int Width = Heighmap.Width;
            int Height = Heighmap.Height;
            //These 2 are to bring the x,z coords into the center of the game world
            //since our bit map is 0 to 256 and the game is -128 to 128
            int WidthAdjust = Width / 2;
            int HeightAdjust = Width / 2;
            //This is used to adjust the Z value that we get from the games heightmap, used to make sure the trees don't float about the map
            //just subtracts this amount from the z coord
            float YAdjust = 0.5f;

            Color[] colors = new Color[Width * Height];

            Heighmap.GetData(colors);

            for (int x = 1; x < Width; x++)
            {
                for (int y = 1; y < Height; y++)
                {
                    if (colors[x + y * Width].R == 0)
                        treeMap.Add(new Vector3(x - WidthAdjust, heightMap.Heights.GetHeight(new Vector3(x - WidthAdjust, 0, y - HeightAdjust)) - YAdjust, y - HeightAdjust));
                }
            }
        }

        
        public void GenerateTrees(XCamera Camera)
        {
            //Init some general vars left till now
            //Give the projection matrix to the particle cloud system as well not that we have it
            cloudSystem.Projection = Camera.Projection;

            //remove this later, incorp a way to get the same trees each time in my treemap using the color values
            Random rand = new Random();

            foreach (Vector3 pos in treeMap)
            {
                // Generate a tree.
                TreeModel tree = generators[rand.Next(0,3)].GenerateTreeMesh(rand.Next(), rand.Next(8,12), true, rand.Next(0,2));
                XTree Xtree = new XTree(ref X, new BoxObject(new Vector3(1,10,1), Matrix.Identity, pos), null, Vector3.Zero, Vector3.Zero,1000);
                Xtree.Immovable = true;
                Xtree.tree = tree;
                // Set the trunk's projection matrix, used in drawing function, static
                Xtree.tree.Trunk.Projection = Camera.Projection;
                // Enable/disable leaf sorting
                Xtree.tree.Leaves.SortingEnabled = false;

                //compute a world matrix from position on treeMap
                //tested out a scale value that sorta matchs the car we have
                Xtree.World = Matrix.Identity * Matrix.CreateTranslation(pos);

                //Xtree.boundingBox = tree.boundingBox; //this is only used for frustrum culling , we don't need this line
                //we do however need to translate the frustrum box to the position of the tree
                Vector3[] bbcorners = tree.boundingBox.GetCorners();
                for (int i = 0; i < bbcorners.Length; i++)
                {
                    bbcorners[i] = Vector3.Transform(bbcorners[i], Xtree.World);
                }
                tree.boundingBox = BoundingBox.CreateFromPoints(bbcorners);
                

                //Add to TreeModel list
                trees.Add(Xtree);
            }
        }
    
    }
}

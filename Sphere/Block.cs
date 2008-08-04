using System;
using System.Collections.Generic;
using System.Text;
using XEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Sphere
{
    public class Block : XComponent, XDrawable
    {
        private XModel _model;
        private Vector3 _scale;
        private XMain _X;
        private XMaterial _material;

        //references to bordering blocks
        public Block x1,x2,y1,y2,z1,z2;
        //reference to parent grid
        public Grid grid;

        //constructor
        public Block(XMain X, XModel model, Vector3 modelScale)
            : base(X)
        {
            _X = X;
            _model = model;
            _scale = modelScale;
        }
        
        public override void Load(ContentManager Content)
        {

            base.Load(Content);
        }

        public override void Update(GameTime gameTime)
        {


            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, XCamera Camera)
        {

            base.Draw(gameTime, Camera);
        }

        public override void SetProjection(Matrix Projection)
        {

            base.SetProjection(Projection);
        }

        public override void Disable()
        {

            base.Disable();
        }

    }
}

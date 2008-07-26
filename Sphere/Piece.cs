using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Sphere
{
    /// <summary>
    /// 1 Game Piece. Contains a linked list of all blocks part of this piece and methods to manipulate.
    /// </summary>
    public class Piece
    {
        public List<Block> blocks;

        public Piece()
        {
      
        }

        ~Piece()
        {
            throw new System.NotImplementedException();
        }
    }
}

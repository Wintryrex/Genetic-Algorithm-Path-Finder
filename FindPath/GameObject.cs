using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FindPath
{
    abstract class GameObject
    {
        protected Vector2 pos;
        protected Texture2D tex;
        protected SpriteBatch sb;

        public Vector2 Pos
        {
            get
            {
                return pos;
            }
        }

        public GameObject(Vector2 pos, Texture2D tex, SpriteBatch sb)
        {
            this.pos = pos;
            this.tex = tex;
            this.sb = sb;
        }

        public abstract void Update(GameTime gameTime);

        public virtual void Draw(GameTime gameTime)
        {
            sb.Draw(tex, pos, Color.White);
        }
    }
}

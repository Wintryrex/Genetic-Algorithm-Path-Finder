using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FindPath
{
    class Tile : GameObject
    {
        int width;
        int height;
        Color color;
        bool specialTile;

        public Color SetColor
        {
            set
            {
                color = value;
            }
        }

        public bool SpecialTile
        {
            get
            {
                return specialTile;
            }
            set
            {
                specialTile = true;
            }
        }
        public Tile(Vector2 pos, Texture2D tex, SpriteBatch sb, int width, int height, Color color, bool specialTile) : base(pos, tex, sb)
        {
            this.width = width;
            this.height = height;
            this.color = color;
            this.specialTile = specialTile;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            sb.Draw(tex, new Rectangle((int)pos.X, (int)pos.Y, width, height), new Rectangle(0, 0, width, height), color);
        }
    }
}

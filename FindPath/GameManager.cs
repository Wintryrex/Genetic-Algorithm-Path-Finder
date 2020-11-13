﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FindPath
{
    class GameManager
    {
        readonly int tilesX = 20;
        readonly int tilesY = 15;
        readonly int tileWidth;
        readonly int tileHeight;
        readonly Texture2D tileTex;
        readonly SpriteBatch sb;
        Vector2 startPos;
        Vector2 endPos;
        List<GameObject> gameObjects;
        Tile[,] tiles;
        string movements; 

        public GameManager(Texture2D tileTex, int tileWidth, int tileHeight, SpriteBatch sb)
        {
            this.tileTex = tileTex;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.sb = sb;

            gameObjects = new List<GameObject>();
            movements = "12345"; // 1: Up, 2: Down, 3: Left, 4: Right, 5: Nothing
            InitializeTiles();
            InitializePoints();
        }

        public void InitializeTiles()
        {
            tiles = new Tile[tilesY, tilesX];

            for (int y = 0; y < tilesY; ++y)
            {
                for (int x = 0; x < tilesX; ++x)
                {
                    Vector2 pos = new Vector2(tileTex.Width * x, tileTex.Height * y);
                    Tile g = new Tile(pos, tileTex, sb, tileWidth, tileHeight, Color.White, false);
                    gameObjects.Add(g);
                    tiles[y, x] = g;
                }
            }
        }

        public void InitializePoints()
        {
            startPos = new Vector2(tileTex.Width * 1, tileTex.Height * 1);
            Tile start = (Tile)GetTile(startPos);
            start.SetColor = Color.DarkTurquoise;
            start.SpecialTile = true;


            endPos = new Vector2(tileTex.Width * (tilesX - 4), tileTex.Height * (tilesY - 4));
            Tile end = (Tile)GetTile(endPos);
            end.SetColor = Color.BlueViolet;
            end.SpecialTile = true;
        }

        public void Update(GameTime gameTime)
        {
            foreach (GameObject g in gameObjects)
            {
                g.Update(gameTime);
            }

        }

        public void Draw(GameTime gameTime)
        {
            foreach (GameObject g in gameObjects)
            {
                g.Draw(gameTime);
            }
        }

        private void Iterate(ref int iteratorX, ref int iteratorY, char letter)
        {
            if (letter == '1')
                --iteratorY;
            else if (letter == '2')
                ++iteratorY;
            else if (letter == '3')
                --iteratorX;
            else if (letter == '4')
                ++iteratorX;
        }

        private Tile GetTile(Vector2 pos)
        {
            foreach (GameObject g in gameObjects)
            {
                if (g.Pos == pos && g is Tile)
                {
                    return (Tile)g;
                }
            }

            return null;
        }

        private Tile Decode(string movements, bool highlight)
        {
            int iteratorX = (int)startPos.X / tileTex.Width;
            int iteratorY = (int)startPos.Y / tileTex.Height;
            Vector2 start = startPos;

            for (int i = 0; i < movements.Length; ++i)
            {
                Iterate(ref iteratorX, ref iteratorY, movements[i]);
                start.X = iteratorX * tileTex.Width;
                start.Y = iteratorY * tileTex.Height;
                Tile tile = GetTile(start);
                if (tile != null && highlight && !tile.SpecialTile)
                    tile.SetColor = Color.Red;
            }

            bool inBounds = (iteratorX > -1 && iteratorX < tilesX) && (iteratorY > -1 && iteratorY < tilesY);
            int sum = iteratorX * iteratorY;

            if (inBounds && sum <= tiles.Length)
                return tiles[iteratorY, iteratorX];

            return null;
        }

        private float Distance(Vector2 startPos, Vector2 endPos) //Manhattan Distance
        {
            float x = (endPos.X - startPos.X) / tileWidth;
            float y = (endPos.Y - startPos.Y) / tileHeight;

            float sum = Math.Abs(x) + Math.Abs(y);
            return sum;
        }

    }
}

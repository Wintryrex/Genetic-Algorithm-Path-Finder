using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace FindPath
{
    class GameManager
    {
        bool stopTimer = true; // For stop displaying results once iteration is finished
        int genIterator; //Generation iterator
        float timer = 2f; // Timer that starts when the game opens
        readonly string movements; // Text that contains numbers. Each number represent a direction
        readonly float pathExtend = 1; //Adjusts how big path the random generator should generate
        readonly int tilesX = 20; // Amount of tiles in the x-axis
        readonly int tilesY = 15; // Amount of tiles in the y-axis
        readonly int tileWidth; //Tile width
        readonly int tileHeight; // Tile height
        readonly Texture2D tileTex; //Texture of the tile
        readonly SpriteBatch sb;
        Vector2 startPos; //Start position 
        Vector2 endPos; // End position
        List<GameObject> gameObjects; //List of all gameobjects
        Tile[,] tiles; // The grid
        Random rnd; // For random values
        GeneticAlgorithm<string> geneticAlgorithm; // Genetic Algorithm
        List<Generation<string>> records; // List of generations
        public GameManager(Texture2D tileTex, int tileWidth, int tileHeight, SpriteBatch sb)
        {
            this.tileTex = tileTex;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            this.sb = sb;

            gameObjects = new List<GameObject>();
            movements = "12345"; // 1: Up, 2: Down, 3: Left, 4: Right, 5: Nothing
            rnd = new Random();
            InitializeTiles();
            InitializePoints();
            InitializeGeneticAlgorithm();
            stopTimer = false;
        }

        public void Update(GameTime gameTime)
        {
            foreach (GameObject g in gameObjects)
            {
                g.Update(gameTime);
            }

            CreateResult(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            foreach (GameObject g in gameObjects)
            {
                g.Draw(gameTime);
            }
        }

        private void InitializeTiles()
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

        private void InitializePoints()
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

        private void InitializeGeneticAlgorithm()
        {
            geneticAlgorithm = new GeneticAlgorithm<string>(CreateRandomPath, CalculateFitness, RandomMovement, ContinueGeneticAlgorithm, rnd, 0.1f);
            geneticAlgorithm.Run(); // Runs the algorithm
            records = geneticAlgorithm.Records; // Get list of generations
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

        private void CreateResult(GameTime gameTime)
        {
            if (!stopTimer)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                timer -= elapsed;


                if (timer < 0)
                {
                    ResetGridColor();
                    string movements = string.Join("", records[genIterator].GetFittest().Genes);
                    Decode(movements, true);
                    ++genIterator;
                    if (genIterator >= records.Count)
                    {
                        stopTimer = true;
                    }

                    timer = 0.1f;
                }
            }
        }

        private void ResetGridColor()
        {
            foreach (Tile g in gameObjects)
            {
                if (!g.SpecialTile)
                    g.SetColor = Color.White;
            }
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

        private string[] CreateRandomPath()
        {
            int geneSize = (int)(Distance(startPos, endPos) * pathExtend); //33% more
            string[] genes = new string[geneSize];

            for (int i = 0; i < genes.Length; ++i)
            {
                string currentGenes = string.Join("", genes);
                Tile currentTile = Decode(currentGenes, false);
                string availableMovements = GetPossibleMoves(movements, currentTile.Pos);

                int index = rnd.Next(0, availableMovements.Length);
                genes[i] = availableMovements[index].ToString();
            }

            return genes;
        }

        private string RandomMovement()
        {
            int index = rnd.Next(0, movements.Length);

            return movements[index].ToString();
        }

        private string GetPossibleMoves(string movements, Vector2 currentPos)
        {
            string results = movements;
            int characterLength = 1;

            if (currentPos.X <= 0)
            {
                int pos = results.IndexOf('3');
                results = results.Remove(pos, characterLength);
            }
            else if (currentPos.X >= (tilesX - 1) * tileTex.Width)
            {
                int pos = results.IndexOf('4');
                results = results.Remove(pos, characterLength);
            }

            if (currentPos.Y <= 0)
            {
                int pos = results.IndexOf('1');
                results = results.Remove(pos, characterLength);

            }
            else if (currentPos.Y >= (tilesY - 1) * tileTex.Height)
            {
                int pos = results.IndexOf('2');
                results = results.Remove(pos, characterLength);
            }

            return results;
        }

        private bool ContinueGeneticAlgorithm(float[] fitnesses)
        {
            for (int i = 0; i < fitnesses.Length; ++i)
            {
                if (fitnesses[i] >= 1)
                    return false;
            }

            return true;
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

        private float CalculateFitness(string[] genes)
        {
            int iteratorX = (int)startPos.X / tileTex.Width;
            int iteratorY = (int)startPos.Y / tileTex.Height;

            for (int i = 0; i < genes.Length; ++i)
            {
                Iterate(ref iteratorX, ref iteratorY, char.Parse(genes[i]));
            }

            Vector2 destination = new Vector2(iteratorX * tileTex.Width, iteratorY * tileTex.Height);
            float dist = Distance(destination, endPos);

            int tilesAmount = (int)(Distance(startPos, endPos) * pathExtend); 

            return Math.Abs((tilesAmount - dist) / tilesAmount);
        }


    }
}

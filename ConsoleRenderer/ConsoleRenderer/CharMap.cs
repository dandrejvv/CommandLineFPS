using ConsoleRenderer.ConsoleScreens;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConsoleRenderer
{
    public class CharMap
    {
        private char[] _map;
        private int _width;
        private int _height;

        public CharMap(string mapFilePath)
        {
            MapFilePath = mapFilePath;

            LoadMap(mapFilePath);
        }

        public string MapFilePath { get; }
        public int Width { get { return _width; } private set { _width = value; } }
        public int Height { get { return _height; } private set { _height = value; } }
        public PositionInt2D PlayerStartingPosition { get; private set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public char GetAtPos(int posX, int posY)
        {
            return _map[posY * _width + posX];
        }

        public void DrawMap(IConsoleScreen screen, int posX, int posY, PositionInt2D currentPlayerPosition)
        {
            for (var row = 0; row < _height; row++)
            {
                screen.Draw(posX, posY + row, _map, row * _width, _width);
            }
            screen.Draw(currentPlayerPosition.PosX, currentPlayerPosition.PosY + 1, 'C');
        }

        private void LoadMap(string mapFilePath)
        {
            var lines = File.ReadAllLines(mapFilePath);
            if (lines == null || lines.Length == 0)
            {
                throw new Exception("Map file is empty");
            }

            var prevLineWith = lines.First().Length;
            var sb = new StringBuilder(lines.Length * prevLineWith);

            Height = lines.Length;
            Width = prevLineWith;

            for (var i = 0; i < lines.Length; i++)
            {
                var curLineWidth = lines[i].Length;
                if (curLineWidth != prevLineWith)
                {
                    throw new Exception($"Line {i+1}'s width in map file differs from previous line");
                }
                int playerLinePos;
                if ((playerLinePos = lines[i].IndexOf('C')) > 0)
                {
                    if (PlayerStartingPosition.PosX != 0 || PlayerStartingPosition.PosY != 0)
                    {
                        throw new Exception("There can only be one camera starting position on the map");
                    }
                    lines[i] = lines[i].Replace('C', '.');
                    PlayerStartingPosition = new PositionInt2D(playerLinePos, i);
                }
                sb.Append(lines[i]);
            }

            _map = sb.ToString().ToCharArray();
        }
    }
}

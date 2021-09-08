using System;
using System.Collections.Generic;

namespace SimonSebright.Sudoku
{
    public class Square : CellExclusionGroup
    {
        private readonly List<List<Cell>> mRows;

        internal Square(List<List<Cell>> rows)
        {
            mRows = rows;
        }

        public Cell this[int i, int j]
        {
            get
            {
                if (j < 0 || j >= Settings.SquareSize)
                {
                    throw new ArgumentOutOfRangeException("Row square access out of range: " + j);
                }

                if (i < 0 || i >= Settings.SquareSize)
                {
                    throw new ArgumentOutOfRangeException("Column square access out of range: " + i);
                }

                return mRows[j][i];
            }
        }

        public override List<Cell> GetCells()
        {
            var cells = new List<Cell>();
            foreach (var row in mRows)
            {
                cells.AddRange(row);
            }
            return cells;
        }
    }
}
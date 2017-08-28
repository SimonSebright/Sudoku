using System;
using System.Collections.Generic;

namespace SimonSebright.Sudoku
{
    public class Row : CellExclusionGroup
    {
        private readonly List<Cell> mCells;

        public Row(List<Cell> cells)
        {
            mCells = cells;
        }

        public Cell this[int i]
        {
            get
            {
                if (i < 0 || i >= Settings.GridSize)
                {
                    throw new ArgumentOutOfRangeException("Column access out of range: " + i);
                }

                return mCells[i];
            }
        }

        public override List<Cell> GetCells()
        {
            return mCells;
        }

        public static List<Cell> GetBlankRow()
        {
            var list = new List<Cell>();
            for (var i = 0; i < Settings.GridSize; ++i)
            {
                list.Add(Cell.BlankCell());
            }

            return list;
        }
    }
}
using System;
using System.Collections.Generic;

namespace SimonSebright.Sudoku
{
    public class Column : CellExclusionGroup
    {
        private readonly List<Cell> mCells;

        internal Column(List<Cell> cells)
        {
            mCells = cells;
        }

        public Cell this[int j]
        {
            get
            {
                if (j < 0 || j >= Settings.GridSize)
                {
                    throw new ArgumentOutOfRangeException("Row access out of range: " + j);
                }

                return mCells[j];
            }
        }

        public override List<Cell> GetCells()
        {
            return mCells;
        }
    }
}
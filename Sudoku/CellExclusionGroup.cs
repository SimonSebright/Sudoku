using System.Collections.Generic;

namespace SimonSebright.Sudoku
{
    public abstract class CellExclusionGroup
    {
        public abstract List<Cell> GetCells();

        public bool Contains(Cell cell)
        {
            foreach (var member in GetCells())
            {
                if (member == cell)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsValue(CellValue cellValue)
        {
            foreach (var member in GetCells())
            {
                if (member.CellValue == cellValue)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
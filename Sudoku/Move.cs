namespace SimonSebright.Sudoku
{
    public class Move
    {
        private readonly CellValue mCellValue;
        private readonly int mI;
        private readonly int mJ;

        public Move(int i, int j, CellValue cellValue)
        {
            mI = i;
            mJ = j;
            mCellValue = cellValue;
        }

        public CellValue CellValue => mCellValue;
        public int I => mI;
        public int J => mJ;

        public override string ToString()
        {
            return Cell.CellValueToString(mCellValue);
        }

        public string ToString(string format)
        {
            switch (format)
            {
                case "Full": return $"i:{mI} j:{mJ} value: {mCellValue.ToString()}";
                default: return ToString();
            }
        }
    }
}
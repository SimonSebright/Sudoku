using System;
using System.Collections.Generic;

namespace SimonSebright.Sudoku
{
    public class Cell
    {
        private CellType mCellType;
        private CellValue mCellValue;

        private Matrix mMatrix;

        public Cell(CellValue cellValue, CellType cellType)
        {
            mCellValue = cellValue;
            mCellType = cellType;
        }

        internal Cell()
        {
            mCellType = CellType.Original;
            mCellValue = CellValue.Blank;
        }

        public bool Original => mCellType == CellType.Original && mCellValue != CellValue.Blank;

        public CellValue CellValue => mCellValue;

        public CellType CellType => mCellType;

        public int I => mMatrix.I(this);

        public int J => mMatrix.J(this);

        public Row Row => mMatrix.Row(this);

        public Column Column => mMatrix.Column(this);

        public Square Square => mMatrix.Square(this);

        public List<CellExclusionGroup> ExclusionGroups => new List<CellExclusionGroup> {Row, Column, Square};

        internal Matrix Matrix
        {
            set => mMatrix = value;
        }

        internal Cell Clone()
        {
            return new Cell(mCellValue, mCellType);
        }

        public static List<CellValue> AllCellValues()
        {
            var cellValues = new List<CellValue>();

            for (var i = 1; i <= Settings.GridSize; ++i)
            {
                cellValues.Add((CellValue) i);
            }

            cellValues.Add(CellValue.Blank);

            return cellValues;
        }

        public static List<CellValue> AllCellValuesRandom()
        {
            var straight = AllCellValues();
            var random = new List<CellValue>();

            var r = new Random(DateTime.Now.Millisecond);

            while (straight.Count > 0)
            {
                var i = r.Next(straight.Count);
                random.Add(straight[i]);
                straight.RemoveAt(i);
            }

            return random;
        }

        private void MakeThisBlankCell()
        {
            mCellValue = CellValue.Blank;
            mCellType = CellType.Subsequent;
        }

        public static string CellValueToString(CellValue cellValue)
        {
            return cellValue == CellValue.Blank ? string.Empty : ((int) cellValue).ToString();
        }

        public override string ToString()
        {
            return CellValueToString(mCellValue);
        }

        public static Cell BlankCell()
        {
            return new Cell(CellValue.Blank, CellType.Original);
        }

        public static Cell Cell1()
        {
            return new Cell(CellValue.One, CellType.Original);
        }

        public static Cell Cell2()
        {
            return new Cell(CellValue.Two, CellType.Original);
        }

        public static Cell Cell3()
        {
            return new Cell(CellValue.Three, CellType.Original);
        }

        public static Cell Cell4()
        {
            return new Cell(CellValue.Four, CellType.Original);
        }

        public static Cell Cell5()
        {
            return new Cell(CellValue.Five, CellType.Original);
        }

        public static Cell Cell6()
        {
            return new Cell(CellValue.Six, CellType.Original);
        }

        public static Cell Cell7()
        {
            return new Cell(CellValue.Seven, CellType.Original);
        }

        public static Cell Cell8()
        {
            return new Cell(CellValue.Eight, CellType.Original);
        }

        public static Cell Cell9()
        {
            return new Cell(CellValue.Nine, CellType.Original);
        }
    }
}
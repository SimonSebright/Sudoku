/*
    Simon Sebright's Sudoku Player
    Copyright (C) 2007  Simon Sebright, www.simonsebright.com

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along
    with this program; if not, write to the Free Software Foundation, Inc.,
    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace SimonSebright.Sudoku
{
    public class MatrixException : ApplicationException
    {
        public MatrixException(string message) : base(message) { }
    }

    static public class Settings
    {
        public static int GridSize { get { return 9; } }
        public static int SquareSize { get { return 3; } }
        public static int NumSquares { get { return GridSize / SquareSize; } }
    }

    public enum CellValue
    {
        Blank,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine
    }

    public enum CellType
    {
        Original,
        Subsequent
    }

    public class Cell
    {
        public Cell(CellValue cellValue, CellType cellType)
        {
            m_cellValue = cellValue;
            m_cellType = cellType;
        }

        internal Cell Clone()
        {
            return new Cell( m_cellValue, m_cellType );
        }

        internal Cell()
        {
            m_cellType = CellType.Original;
            m_cellValue = CellValue.Blank;
        }

        static public List<CellValue> AllCellValues()
        {
            List<CellValue> cellValues = new List<CellValue>();

            for (int i = 1; i <= Settings.GridSize; ++i)
            {
                cellValues.Add( (CellValue)i);
            }

            cellValues.Add(CellValue.Blank);

            return cellValues;
        }

        static public List<CellValue> AllCellValuesRandom()
        {
            List<CellValue> straight = AllCellValues();
            List<CellValue> random = new List<CellValue>();

            Random r = new Random(DateTime.Now.Millisecond);

            while (straight.Count > 0)
            {
                int i = r.Next( straight.Count );
                random.Add( straight[i]);
                straight.RemoveAt(i);
            }

            return random;
        }

        private void MakeThisBlankCell()
        {
            m_cellValue = CellValue.Blank;
            m_cellType = CellType.Subsequent;
        }

        public static string CellValueToString(CellValue cellValue)
        {
            return cellValue == CellValue.Blank ? string.Empty : ((int)cellValue).ToString();
        }

        public override string ToString()
        {
            return CellValueToString(m_cellValue);
        }

        public bool Original { get { return (m_cellType == CellType.Original) &&
                                            (m_cellValue != CellValue.Blank); } }

        public CellValue CellValue
        {
            get { return m_cellValue; }
        }
        public CellType CellType
        {
            get { return m_cellType; }
        }

        public int I
        {
            get
            {
                return m_matrix.I( this );
            }
        }

        public int J
        {
            get
            {
                return m_matrix.J(this);
            }
        }

        public Row Row
        {
            get
            {
                return m_matrix.Row(this);
            }
        }

        public Column Column
        {
            get
            {
                return m_matrix.Column(this);
            }
        }

        public Square Square
        {
            get
            {
                return m_matrix.Square(this);
            }
        }

        public List<CellExclusionGroup> ExclusionGroups()
        {
            List<CellExclusionGroup> groups = new List<CellExclusionGroup>();
            groups.Add(Row);
            groups.Add(Column);
            groups.Add(Square);
            return groups;
        }

        internal Matrix Matrix
        {
            set { m_matrix = value; }
        }

        private Matrix m_matrix = null;
        private CellValue m_cellValue;
        private CellType m_cellType;

        public static Cell BlankCell() { return new Cell(CellValue.Blank, CellType.Original); }
        public static Cell Cell1() { return new Cell(CellValue.One, CellType.Original); }
        public static Cell Cell2() { return new Cell(CellValue.Two, CellType.Original); }
        public static Cell Cell3() { return new Cell(CellValue.Three, CellType.Original); }
        public static Cell Cell4() { return new Cell(CellValue.Four, CellType.Original); }
        public static Cell Cell5() { return new Cell(CellValue.Five, CellType.Original); }
        public static Cell Cell6() { return new Cell(CellValue.Six, CellType.Original); }
        public static Cell Cell7() { return new Cell(CellValue.Seven, CellType.Original); }
        public static Cell Cell8() { return new Cell(CellValue.Eight, CellType.Original); }
        public static Cell Cell9() { return new Cell(CellValue.Nine, CellType.Original); }
    }



    public abstract class CellExclusionGroup
    {
        public abstract List<Cell> GetCells();

        public bool Contains(Cell cell)
        {
            foreach (Cell member in GetCells())
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
            foreach (Cell member in GetCells())
            {
                if (member.CellValue == cellValue)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class Row : CellExclusionGroup
    {
        public Row(List<Cell> cells)
        {
            m_cells = cells;
        }

        public Cell this[int i]
        {
            get
            {
                if (i < 0 || i >= Settings.GridSize)
                {
                    throw new ArgumentOutOfRangeException("Column access out of range: " + i.ToString());
                }

                return m_cells[i];
            }
        }

        public override List<Cell> GetCells() { return m_cells; }

        private List<Cell> m_cells;

        public static List<Cell> GetBlankRow()
        {
            List<Cell> list = new List<Cell>();
            for (int i = 0; i < Settings.GridSize; ++i)
            {
                list.Add(Cell.BlankCell());
            }

            return list;
        }
    }

    public class Column : CellExclusionGroup
    {
        internal Column(List<Cell> cells)
        {
            m_cells = cells;
        }

        public Cell this[int j]
        {
            get
            {
                if (j < 0 || j >= Settings.GridSize)
                {
                    throw new ArgumentOutOfRangeException("Row access out of range: " + j.ToString());
                }

                return m_cells[j];
            }
        }

        public override List<Cell> GetCells() { return m_cells; }

        private List<Cell> m_cells;
    }

    public class Square : CellExclusionGroup
    {
        internal Square(List<List<Cell>> rows)
        {
            m_rows = rows;
        }

        public Cell this[int i, int j]
        {
            get
            {
                if (j < 0 || j >= Settings.SquareSize)
                {
                    throw new ArgumentOutOfRangeException("Row square access out of range: " + j.ToString());
                }

                if (i < 0 || i >= Settings.SquareSize)
                {
                    throw new ArgumentOutOfRangeException("Column square access out of range: " + i.ToString());
                }

                return m_rows[j][i];
            }
        }

        public override List<Cell> GetCells()
        {
            List<Cell> cells = new List<Cell>();
            foreach (List<Cell> row in m_rows)
            {
                cells.AddRange(row);
            }
            return cells;
        }

        private List<List<Cell>> m_rows;
    }

    public class Matrix
    {
        public Matrix(List<List<Cell>> rows)
        {
            if (rows.Count > Settings.GridSize)
            {
                throw new ArgumentOutOfRangeException("Too many rows in Matrix");
            }

            m_rows = new List<Row>();
            foreach (List<Cell> row in rows)
            {
                if (row.Count > Settings.GridSize)
                {
                    throw new ArgumentOutOfRangeException("Too many columns in Matrix");
                }
                m_rows.Add(new Row(row));
            }

            GiveCellsMatrixPointer();
        }

        public Matrix GetOriginalMatrix()
        {
            List<List<Cell>> rows = new List<List<Cell>>();

            foreach (Row row in m_rows)
            {
                List<Cell> newRow = new List<Cell>();
                foreach( Cell cell in row.GetCells() )
                {
                    newRow.Add( cell.Original ? cell.Clone() : Cell.BlankCell() );
                }
                rows.Add( newRow );
            }

            return new Matrix( rows );
        }

        private void GiveCellsMatrixPointer()
        {
            foreach (Row row in m_rows)
            {
                foreach (Cell cell in row.GetCells())
                {
                    cell.Matrix = this;
                }
            }
        }

        public Cell At(int i, int j)
        {
            return Row(j)[i];
        }

        internal int I(Cell cell)
        {
            for (int j = 0; j < m_rows.Count; ++j)
            {
                List<Cell> rowCells = m_rows[j].GetCells();
                for (int i = 0; i < rowCells.Count; ++i)
                {
                    if (rowCells[i] == cell)
                    {
                        return i;
                    }
                }
            }

            throw new ArgumentOutOfRangeException("Cannot find I property for cell: " + cell.ToString());
        }

        internal int J(Cell cell)
        {
            for (int j = 0; j < m_rows.Count; ++j)
            {
                List<Cell> rowCells = m_rows[j].GetCells();
                for (int i = 0; i < rowCells.Count; ++i)
                {
                    if (rowCells[i] == cell)
                    {
                        return j;
                    }
                }
            }

            throw new ArgumentOutOfRangeException("Cannot find I property for cell: " + cell.ToString());
        }

        public Row Row(int j)
        {
            if (j < 0 || j >= Settings.GridSize)
            {
                throw new ArgumentOutOfRangeException("Column access out of range: " + j.ToString());
            }

            return m_rows[j];
        }

        internal Row Row(Cell cell)
        {
            foreach (Row row in m_rows)
            {
                if (row.Contains(cell))
                {
                    return row;
                }
            }

            throw new ArgumentOutOfRangeException("Cell not found in Matrix for row");
        }

        internal List<Row> GetRows()
        {
            return m_rows;
        }

        internal List<Column> GetColumns()
        {
            List<Column> columns = new List<Column>();
            for (int i = 0; i < Settings.GridSize; ++i)
            {
                Column member = Column(i);
                columns.Add(member);
            }

            return columns;
        }

        internal Column Column(Cell cell)
        {
            foreach (Column member in GetColumns())
            {
                if (member.Contains(cell))
                {
                    return member;
                }
            }

            throw new ArgumentOutOfRangeException("Cell not found in Matrix for column");
        }

        internal List<Square> GetSquares()
        {
            List<Square> squares = new List<Square>();

            for (int i = 0; i < Settings.GridSize; ++i)
            {
                Square member = Square(i);
                squares.Add(member);
            }

            return squares;
        }

        internal Square Square(Cell cell)
        {
            foreach (Square member in GetSquares())
            {
                if (member.Contains(cell))
                {
                    return member;
                }
            }

            throw new ArgumentOutOfRangeException("Cell not found in Matrix for Square");
        }


        public Column Column(int i)
        {
            List<Cell> cells = new List<Cell>();

            for (int j = 0; j < Settings.GridSize; ++j)
            {
                cells.Add(Row(j)[i]);
            }

            return new Column(cells);
        }

        internal Square Square(int i)
        {
            return Square(i % 3, i / 3);
        }

        public Square Square(int i, int j)
        {
            if (i < 0 || i >= Settings.GridSize / Settings.SquareSize)
            {
                throw new ArgumentOutOfRangeException("Square access column out of range: " + i.ToString());
            }
            if (j < 0 || j >= Settings.GridSize / Settings.SquareSize)
            {
                throw new ArgumentOutOfRangeException("Square access row out of range: " + j.ToString());
            }

            List<List<Cell>> square = new List<List<Cell>>();

            for (int sj = j * Settings.SquareSize; sj < (j + 1) * Settings.SquareSize; ++sj)
            {
                List<Cell> row = new List<Cell>();
                for (int si = i * Settings.SquareSize; si < (i + 1) * Settings.SquareSize; ++si)
                {
                    row.Add(At(si, sj));
                }
                square.Add(row);
            }

            return new Square(square);
        }

        private List<Row> m_rows;

        /// <summary>
        /// Gets a new blank matrix
        /// </summary>
        public static Matrix Blank
        {
            get
            {
                List<List<Cell>> rows = new List<List<Cell>>();

                for (int i = 0; i < Settings.GridSize; ++i)
                {
                    rows.Add(Sudoku.Row.GetBlankRow());
                }

                return new Matrix(rows);
            }
        }

        /// <summary>
        /// Gets all the exclusions groups for this matrix
        /// </summary>
        /// <returns></returns>
        public List<CellExclusionGroup> GetExclusionGroups()
        {
            List<CellExclusionGroup> groups = new List<CellExclusionGroup>();
            foreach (Row row in GetRows()) groups.Add(row);
            foreach (Column column in GetColumns()) groups.Add( column );
            foreach (Square square in GetSquares()) groups.Add( square );

            return groups;
        }

        public Matrix MakeMove(Move move, CellType cellType)
        {
            List<Move> moves = new List<Move>();
            moves.Add(move);
            return MakeMoves(moves, cellType);
        }

        /// <summary>
        /// Makes a new matrix with the moves from the supplied list
        /// </summary>
        /// <param name="moves"></param>
        /// <returns></returns>
        public Matrix MakeMoves(List<Move> moves, CellType cellType)
        {
            List<List<Cell>> rows = new List<List<Cell>>();

            for (int j = 0; j < Settings.GridSize; ++j)
            {
                List<Cell> row = new List<Cell>();

                for (int i = 0; i < Settings.GridSize; ++i)
                {
                    row.Add(GetCellForNewMatrix(i, j, moves, cellType));
                }

                rows.Add(row);
            }

            return new Matrix(rows);
        }

        private Cell GetCellForNewMatrix(int i, int j, List<Move> moves, CellType cellType )
        {
            foreach (Move move in moves)
            {
                if (move.I == i && move.J == j)
                {
                    return new Cell(move.CellValue, cellType);
                }
            }

            return At(i, j).Clone();
        }

        public bool HasSubsequentCells
        {
            get
            {
                foreach (Cell cell in Cells)
                {
                    if (cell.CellType == CellType.Subsequent &&
                        cell.CellValue != CellValue.Blank ) 
                        return true;
                }
                return false;
            }
        }

        public List<Cell> Cells
        {
            get
            {
                List<Cell> cells = new List<Cell>();
                foreach (Row row in GetRows())
                {
                    foreach (Cell cell in row.GetCells())
                    {
                        cells.Add(cell);
                    }
                }

                return cells;
            }
        }
    }

    public class Move
    {
        public Move(int i, int j, CellValue cellValue)
        {
            m_i = i;
            m_j = j;
            m_cellValue = cellValue;
        }

        public override string ToString()
        {
            return Cell.CellValueToString(m_cellValue);
        }

        public string ToString(string format)
        {
            switch (format)
            {
                case "Full": return string.Format("i:{0} j:{1} value: {2}", m_i.ToString(), m_j.ToString(), m_cellValue.ToString());
                default: return ToString();
            }
        }

        public CellValue CellValue { get { return m_cellValue; } }
        public int I { get { return m_i; } }
        public int J { get { return m_j; } }

        private int m_i;
        private int m_j;
        private CellValue m_cellValue;
    }

}

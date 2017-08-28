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

namespace SimonSebright.Sudoku
{
    public class MatrixException : ApplicationException
    {
        public MatrixException(string message) : base(message)
        {
        }
    }

    public static class Settings
    {
        public static int GridSize => 9;
        public static int SquareSize => 3;
        public static int NumSquares => GridSize / SquareSize;
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
        private CellType m_cellType;
        private CellValue m_cellValue;

        private Matrix m_matrix;

        public Cell(CellValue cellValue, CellType cellType)
        {
            m_cellValue = cellValue;
            m_cellType = cellType;
        }

        internal Cell()
        {
            m_cellType = CellType.Original;
            m_cellValue = CellValue.Blank;
        }

        public bool Original => m_cellType == CellType.Original && m_cellValue != CellValue.Blank;

        public CellValue CellValue => m_cellValue;

        public CellType CellType => m_cellType;

        public int I => m_matrix.I(this);

        public int J => m_matrix.J(this);

        public Row Row => m_matrix.Row(this);

        public Column Column => m_matrix.Column(this);

        public Square Square => m_matrix.Square(this);

        public List<CellExclusionGroup> ExclusionGroups => new List<CellExclusionGroup> {Row, Column, Square};

        internal Matrix Matrix
        {
            set => m_matrix = value;
        }

        internal Cell Clone()
        {
            return new Cell(m_cellValue, m_cellType);
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
            m_cellValue = CellValue.Blank;
            m_cellType = CellType.Subsequent;
        }

        public static string CellValueToString(CellValue cellValue)
        {
            return cellValue == CellValue.Blank ? string.Empty : ((int) cellValue).ToString();
        }

        public override string ToString()
        {
            return CellValueToString(m_cellValue);
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

    public class Row : CellExclusionGroup
    {
        private readonly List<Cell> m_cells;

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
                    throw new ArgumentOutOfRangeException("Column access out of range: " + i);
                }

                return m_cells[i];
            }
        }

        public override List<Cell> GetCells()
        {
            return m_cells;
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

    public class Column : CellExclusionGroup
    {
        private readonly List<Cell> m_cells;

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
                    throw new ArgumentOutOfRangeException("Row access out of range: " + j);
                }

                return m_cells[j];
            }
        }

        public override List<Cell> GetCells()
        {
            return m_cells;
        }
    }

    public class Square : CellExclusionGroup
    {
        private readonly List<List<Cell>> m_rows;

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
                    throw new ArgumentOutOfRangeException("Row square access out of range: " + j);
                }

                if (i < 0 || i >= Settings.SquareSize)
                {
                    throw new ArgumentOutOfRangeException("Column square access out of range: " + i);
                }

                return m_rows[j][i];
            }
        }

        public override List<Cell> GetCells()
        {
            var cells = new List<Cell>();
            foreach (var row in m_rows)
            {
                cells.AddRange(row);
            }
            return cells;
        }
    }

    public class Matrix
    {
        private readonly List<Row> m_rows;

        public Matrix(List<List<Cell>> rows)
        {
            if (rows.Count > Settings.GridSize)
            {
                throw new ArgumentOutOfRangeException("Too many rows in Matrix");
            }

            m_rows = new List<Row>();
            foreach (var row in rows)
            {
                if (row.Count > Settings.GridSize)
                {
                    throw new ArgumentOutOfRangeException("Too many columns in Matrix");
                }
                m_rows.Add(new Row(row));
            }

            GiveCellsMatrixPointer();
        }

        /// <summary>
        ///     Gets a new blank matrix
        /// </summary>
        public static Matrix Blank
        {
            get
            {
                var rows = new List<List<Cell>>();

                for (var i = 0; i < Settings.GridSize; ++i)
                {
                    rows.Add(Sudoku.Row.GetBlankRow());
                }

                return new Matrix(rows);
            }
        }

        public bool HasSubsequentCells
        {
            get
            {
                foreach (var cell in Cells)
                {
                    if (cell.CellType == CellType.Subsequent &&
                        cell.CellValue != CellValue.Blank)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public List<Cell> Cells
        {
            get
            {
                var cells = new List<Cell>();
                foreach (var row in GetRows())
                {
                    foreach (var cell in row.GetCells())
                    {
                        cells.Add(cell);
                    }
                }

                return cells;
            }
        }

        public Matrix GetOriginalMatrix()
        {
            var rows = new List<List<Cell>>();

            foreach (var row in m_rows)
            {
                var newRow = new List<Cell>();
                foreach (var cell in row.GetCells())
                {
                    newRow.Add(cell.Original ? cell.Clone() : Cell.BlankCell());
                }
                rows.Add(newRow);
            }

            return new Matrix(rows);
        }

        private void GiveCellsMatrixPointer()
        {
            foreach (var row in m_rows)
            {
                foreach (var cell in row.GetCells())
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
            foreach (var t in m_rows)
            {
                var rowCells = t.GetCells();
                for (var i = 0; i < rowCells.Count; ++i)
                {
                    if (rowCells[i] == cell)
                    {
                        return i;
                    }
                }
            }

            throw new ArgumentOutOfRangeException("Cannot find I property for cell: " + cell);
        }

        internal int J(Cell cell)
        {
            for (var j = 0; j < m_rows.Count; ++j)
            {
                var rowCells = m_rows[j].GetCells();
                foreach (var t in rowCells)
                {
                    if (t == cell)
                    {
                        return j;
                    }
                }
            }

            throw new ArgumentOutOfRangeException("Cannot find I property for cell: " + cell);
        }

        public Row Row(int j)
        {
            if (j < 0 || j >= Settings.GridSize)
            {
                throw new ArgumentOutOfRangeException("Column access out of range: " + j);
            }

            return m_rows[j];
        }

        internal Row Row(Cell cell)
        {
            foreach (var row in m_rows)
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
            var columns = new List<Column>();
            for (var i = 0; i < Settings.GridSize; ++i)
            {
                var member = Column(i);
                columns.Add(member);
            }

            return columns;
        }

        internal Column Column(Cell cell)
        {
            foreach (var member in GetColumns())
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
            var squares = new List<Square>();

            for (var i = 0; i < Settings.GridSize; ++i)
            {
                var member = Square(i);
                squares.Add(member);
            }

            return squares;
        }

        internal Square Square(Cell cell)
        {
            foreach (var member in GetSquares())
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
            var cells = new List<Cell>();

            for (var j = 0; j < Settings.GridSize; ++j)
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
                throw new ArgumentOutOfRangeException("Square access column out of range: " + i);
            }
            if (j < 0 || j >= Settings.GridSize / Settings.SquareSize)
            {
                throw new ArgumentOutOfRangeException("Square access row out of range: " + j);
            }

            var square = new List<List<Cell>>();

            for (var sj = j * Settings.SquareSize; sj < (j + 1) * Settings.SquareSize; ++sj)
            {
                var row = new List<Cell>();
                for (var si = i * Settings.SquareSize; si < (i + 1) * Settings.SquareSize; ++si)
                {
                    row.Add(At(si, sj));
                }
                square.Add(row);
            }

            return new Square(square);
        }

        /// <summary>
        ///     Gets all the exclusions groups for this matrix
        /// </summary>
        /// <returns></returns>
        public List<CellExclusionGroup> GetExclusionGroups()
        {
            var groups = new List<CellExclusionGroup>();
            foreach (var row in GetRows())
            {
                groups.Add(row);
            }
            foreach (var column in GetColumns())
            {
                groups.Add(column);
            }
            foreach (var square in GetSquares())
            {
                groups.Add(square);
            }

            return groups;
        }

        public Matrix MakeMove(Move move, CellType cellType)
        {
            var moves = new List<Move> {move};
            return MakeMoves(moves, cellType);
        }

        /// <summary>
        ///     Makes a new matrix with the moves from the supplied list
        /// </summary>
        /// <param name="moves"></param>
        /// <returns></returns>
        public Matrix MakeMoves(List<Move> moves, CellType cellType)
        {
            var rows = new List<List<Cell>>();

            for (var j = 0; j < Settings.GridSize; ++j)
            {
                var row = new List<Cell>();

                for (var i = 0; i < Settings.GridSize; ++i)
                {
                    row.Add(GetCellForNewMatrix(i, j, moves, cellType));
                }

                rows.Add(row);
            }

            return new Matrix(rows);
        }

        private Cell GetCellForNewMatrix(int i, int j, List<Move> moves, CellType cellType)
        {
            foreach (var move in moves)
            {
                if (move.I == i && move.J == j)
                {
                    return new Cell(move.CellValue, cellType);
                }
            }

            return At(i, j).Clone();
        }
    }

    public class Move
    {
        private readonly CellValue m_cellValue;

        private readonly int m_i;
        private readonly int m_j;

        public Move(int i, int j, CellValue cellValue)
        {
            m_i = i;
            m_j = j;
            m_cellValue = cellValue;
        }

        public CellValue CellValue => m_cellValue;
        public int I => m_i;
        public int J => m_j;

        public override string ToString()
        {
            return Cell.CellValueToString(m_cellValue);
        }

        public string ToString(string format)
        {
            switch (format)
            {
                case "Full": return $"i:{m_i} j:{m_j} value: {m_cellValue.ToString()}";
                default: return ToString();
            }
        }
    }
}
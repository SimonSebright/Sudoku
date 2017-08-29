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
    public class Matrix
    {
        private readonly List<Row> mRows;

        public Matrix(List<List<Cell>> rows)
        {
            if (rows.Count > Settings.GridSize)
            {
                throw new ArgumentOutOfRangeException("Too many rows in Matrix");
            }

            mRows = new List<Row>();
            foreach (var row in rows)
            {
                if (row.Count > Settings.GridSize)
                {
                    throw new ArgumentOutOfRangeException("Too many columns in Matrix");
                }
                mRows.Add(new Row(row));
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

            foreach (var row in mRows)
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
            foreach (var row in mRows)
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
            foreach (var t in mRows)
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
            for (var j = 0; j < mRows.Count; ++j)
            {
                var rowCells = mRows[j].GetCells();
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

            return mRows[j];
        }

        internal Row Row(Cell cell)
        {
            foreach (var row in mRows)
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
            return mRows;
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
        /// <param name="cellType"></param>
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
}
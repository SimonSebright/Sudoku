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
using NUnit.Framework;
using SimonSebright.Sudoku;
using SimonSebright.Sudoku.Analyser;

namespace SimonSebright.Sudoku.Tester
{
    [TestFixture]
    public class Tester
    {
        private List<Cell> GetTooWideRow()
        {
            List<Cell> list = new List<Cell>();
            for (int i = 0; i < Settings.GridSize + 1; ++i)
            {
                list.Add(new Cell(CellValue.Blank, CellType.Original));
            }

            return list;
        }
        private List<Cell> GetAscRow()
        {
            List<Cell> list = new List<Cell>();
            for (int i = 0; i < Settings.GridSize; ++i)
            {
                list.Add(new Cell((CellValue)(i+1), CellType.Original));
            }

            return list;
        }

        private Matrix GetTooWideMatrix()
        {
            List<List<Cell>> rows = new List<List<Cell>>();

            for (int i = 0; i < Settings.GridSize; ++i)
            {
                rows.Add(GetTooWideRow());
            }

            return new Matrix(rows);
        }

        private Matrix GetTooDeepMatrix()
        {
            List<List<Cell>> rows = new List<List<Cell>>();

            for (int i = 0; i < Settings.GridSize + 1; ++i)
            {
                rows.Add(Row.GetBlankRow());
            }

            return new Matrix(rows);
        }

        private Matrix GetAscMatrix()
        {
            List<List<Cell>> rows = new List<List<Cell>>();

            for (int i = 0; i < Settings.GridSize; ++i)
            {
                rows.Add(GetAscRow());
            }

            return new Matrix(rows);
        }

        private Matrix GetSimpleRowMoveMatrix()
        {
            List<List<Cell>> rows = new List<List<Cell>>();

            Cell[] cells1 = { Cell.Cell1(), Cell.Cell2(), Cell.Cell3(), Cell.Cell4(), Cell.Cell5(), Cell.Cell6(), Cell.Cell7(), Cell.Cell8(), Cell.BlankCell() };
            List<Cell> row = new List<Cell>();
            row.AddRange(cells1);
            rows.Add(row);

            rows.Add(Row.GetBlankRow());
            rows.Add(Row.GetBlankRow());

            for (int j = 0; j < Settings.GridSize - Settings.SquareSize; ++j)
            {
                rows.Add(Row.GetBlankRow());
            }

            return new Matrix(rows);
        }

        private Matrix GetSimpleColumnMoveMatrix()
        {
            List<List<Cell>> rows = new List<List<Cell>>();

            Cell[] cells1 = { Cell.Cell1(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            Cell[] cells2 = { Cell.Cell2(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            Cell[] cells3 = { Cell.Cell3(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            Cell[] cells4 = { Cell.Cell4(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            Cell[] cells5 = { Cell.Cell5(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            Cell[] cells6 = { Cell.Cell6(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            Cell[] cells7 = { Cell.Cell7(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            Cell[] cells8 = { Cell.Cell8(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            Cell[] cells9 = { Cell.BlankCell(),  Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            List<Cell> row = new List<Cell>();
            row.Clear();
            row.AddRange(cells1);
            rows.Add(row);

            row = new List<Cell>();
            row.AddRange(cells2);
            rows.Add(row);

            row = new List<Cell>();
            row.AddRange(cells3);
            rows.Add(row);

            row = new List<Cell>();
            row.AddRange(cells4);
            rows.Add(row);

            row = new List<Cell>();
            row.AddRange(cells5);
            rows.Add(row);

            row = new List<Cell>();
            row.AddRange(cells6);
            rows.Add(row);

            row = new List<Cell>();
            row.AddRange(cells7);
            rows.Add(row);

            row = new List<Cell>();
            row.AddRange(cells8);
            rows.Add(row);

            row = new List<Cell>();
            row.AddRange(cells9);
            rows.Add(row);

            return new Matrix(rows);
        }

        private Matrix GetSimpleSquareMoveMatrix()
        {
            List<List<Cell>> rows = new List<List<Cell>>();

            Cell[] cells1 = { Cell.Cell1(), Cell.Cell2(), Cell.Cell3(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            List<Cell> row = new List<Cell>();
            row.AddRange(cells1);
            rows.Add(row);

            Cell[] cells2 = { Cell.Cell4(), Cell.Cell5(), Cell.Cell6(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            List<Cell> row2 = new List<Cell>();
            row2.AddRange(cells2);
            rows.Add(row2);

            Cell[] cells3 = { Cell.Cell7(), Cell.Cell8(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            List<Cell> row3 = new List<Cell>();
            row3.AddRange(cells3);
            rows.Add(row3);

            for (int j = 0; j < Settings.GridSize - Settings.SquareSize; ++j)
            {
                rows.Add(Row.GetBlankRow());
            }

            return new Matrix(rows);
        }

        private Matrix GetMediumMoveMatrix()
        {
            List<List<Cell>> rows = new List<List<Cell>>();

            Cell[] cells1 = { Cell.Cell1(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            List<Cell> row = new List<Cell>();
            row.AddRange(cells1);
            rows.Add(row);

            Cell[] cells2 = { Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.Cell1(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            row = new List<Cell>();
            row.AddRange(cells2);
            rows.Add(row);

            Cell[] cells3 = { Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.Cell2(), Cell.Cell3() };
            row = new List<Cell>();
            row.AddRange(cells3);
            rows.Add(row);

            for (int j = 0; j < Settings.GridSize - Settings.SquareSize; ++j)
            {
                rows.Add(Row.GetBlankRow());
            }

            return new Matrix(rows);
        }

        private Matrix GetHardMoveMatrix()
        {
            List<List<Cell>> rows = new List<List<Cell>>();

            Cell[] cells1 = { Cell.Cell1(), Cell.Cell2(), Cell.Cell3(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            List<Cell> row = new List<Cell>();
            row.AddRange(cells1);
            rows.Add(row);

            Cell[] cells2 = { Cell.Cell4(), Cell.Cell5(), Cell.Cell6(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell(), Cell.BlankCell() };
            row = new List<Cell>();
            row.AddRange(cells2);
            rows.Add(row);

            Cell[] cells3 = { Cell.Cell7(), Cell.BlankCell(), Cell.BlankCell(), Cell.Cell1(), Cell.Cell2(), Cell.Cell3(), Cell.Cell4(), Cell.Cell5(), Cell.BlankCell()};
            row = new List<Cell>();
            row.AddRange(cells3);
            rows.Add(row);

            for (int j = 0; j < Settings.GridSize - Settings.SquareSize; ++j)
            {
                rows.Add(Row.GetBlankRow());
            }

            return new Matrix(rows);
        }

        [Test]
        public void TestCells()
        {
            Cell gv = new Cell(CellValue.Blank, CellType.Original);
            Assert.AreEqual(CellValue.Blank, gv.CellValue);
            Assert.AreEqual(CellType.Original, gv.CellType);

            gv = new Cell(CellValue.Five, CellType.Subsequent);
            Assert.AreEqual(CellValue.Five, gv.CellValue);
            Assert.AreEqual(CellType.Subsequent, gv.CellType);
        }

        [Test]
        public void TestBlankMatrix()
        {
            Matrix m = Matrix.Blank;

            Assert.AreEqual(CellValue.Blank, m.At(0, 0).CellValue);
            Assert.AreEqual(CellType.Original, m.At(0, 0).CellType);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestTooBigRows()
        {
            Matrix m = GetTooWideMatrix();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestTooBigColumns()
        {
            Matrix m = GetTooDeepMatrix();
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestColumnOutOfRangeLow()
        {
            Matrix m = Matrix.Blank;
            m.At(-1, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestColumnOutOfRangeHigh()
        {
            Matrix m = Matrix.Blank;
            m.At(9, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRowOutOfRangeLow()
        {
            Matrix m = Matrix.Blank;
            m.At(0, -1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRowOutOfRangeHigh()
        {
            Matrix m = Matrix.Blank;
            m.At(0, 9);
        }

        [Test]
        public void TestNonBlankCells()
        {
            Matrix m = GetAscMatrix();

            Assert.AreEqual(CellValue.One,   m.At(0, 0).CellValue);
            Assert.AreEqual(CellValue.Two,   m.At(1, 0).CellValue);
            Assert.AreEqual(CellValue.Three, m.At(2, 0).CellValue);
            Assert.AreEqual(CellValue.Four,  m.At(Settings.SquareSize, 0).CellValue);
            Assert.AreEqual(CellValue.Five,  m.At(4, 0).CellValue);
            Assert.AreEqual(CellValue.Six,   m.At(5, 0).CellValue);
            Assert.AreEqual(CellValue.Seven, m.At(6, 0).CellValue);
            Assert.AreEqual(CellValue.Eight, m.At(7, 0).CellValue);
            Assert.AreEqual(CellValue.Nine,  m.At(8, 0).CellValue);

            Assert.AreEqual(CellValue.One,   m.At(0, 5).CellValue);
            Assert.AreEqual(CellValue.Two,   m.At(1, 2).CellValue);
            Assert.AreEqual(CellValue.Three, m.At(2, 8).CellValue);
            Assert.AreEqual(CellValue.Four,  m.At(Settings.SquareSize, 4).CellValue);
            Assert.AreEqual(CellValue.Five,  m.At(4, 2).CellValue);
            Assert.AreEqual(CellValue.Six,   m.At(5, 0).CellValue);
            Assert.AreEqual(CellValue.Seven, m.At(6, 7).CellValue);
            Assert.AreEqual(CellValue.Eight, m.At(7, 6).CellValue);
            Assert.AreEqual(CellValue.Nine,  m.At(8, 4).CellValue);
        }

        [Test]
        public void TestRowAccess()
        {
            Matrix m = GetAscMatrix();
            Row r = m.Row(0);

            Assert.AreEqual(CellValue.One, r[0].CellValue);
            Assert.AreEqual(CellValue.Two, r[1].CellValue);
            Assert.AreEqual(CellValue.Three, r[2].CellValue);
            Assert.AreEqual(CellValue.Four, r[Settings.SquareSize].CellValue);
            Assert.AreEqual(CellValue.Five, r[4].CellValue);
            Assert.AreEqual(CellValue.Six, r[5].CellValue);
            Assert.AreEqual(CellValue.Seven, r[6].CellValue);
            Assert.AreEqual(CellValue.Eight, r[7].CellValue);
            Assert.AreEqual(CellValue.Nine, r[8].CellValue);
        }

        [Test]
        public void TestColumnAccess()
        {
            Matrix m = GetAscMatrix();
            Column c = m.Column(0);

            Assert.AreEqual(CellValue.One, c[0].CellValue);
            Assert.AreEqual(CellValue.One, c[4].CellValue);
            Assert.AreEqual(CellValue.One, c[5].CellValue);

            c = m.Column(4);
            Assert.AreEqual(CellValue.Five, c[0].CellValue);
            Assert.AreEqual(CellValue.Five, c[4].CellValue);
            Assert.AreEqual(CellValue.Five, c[5].CellValue);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSquareAccessOutOfRangeXLow()
        {
            Matrix m = Matrix.Blank;
            Square s = m.Square(-1, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSquareAccessOutOfRangeXHigh()
        {
            Matrix m = Matrix.Blank;
            Square s = m.Square(Settings.SquareSize, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSquareAccessOutOfRangeYLow()
        {
            Matrix m = Matrix.Blank;
            Square s = m.Square(0, -1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSquareAccessOutOfRangeYHigh()
        {
            Matrix m = Matrix.Blank;
            Square s = m.Square(0, Settings.SquareSize);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInternalSquareAccessOutOfRangeXLow()
        {
            Matrix m = Matrix.Blank;
            Square s = m.Square(0, 0);
            Cell c = s[-1, 0];
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInternalSquareAccessOutOfRangeXHigh()
        {
            Matrix m = Matrix.Blank;
            Square s = m.Square(0, 0);
            Cell c = s[Settings.SquareSize, 0];
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInternalSquareAccessOutOfRangeYLow()
        {
            Matrix m = Matrix.Blank;
            Square s = m.Square(0, 0);
            Cell c = s[0, -1];
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInternalSquareAccessOutOfRangeYHigh()
        {
            Matrix m = Matrix.Blank;
            Square s = m.Square(0, 0);
            Cell c = s[0, Settings.SquareSize];
        }

        [Test]
        public void TestSquareCellValues()
        {
            Matrix m = GetAscMatrix();
            Square s = m.Square(0, 0);
            Assert.AreEqual(CellValue.One, s[0, 0].CellValue );
            Assert.AreEqual(CellValue.One, s[0, 2].CellValue );
            Assert.AreEqual(CellValue.Three, s[2, 0].CellValue );

            s = m.Square(1, 2);
            Assert.AreEqual(CellValue.Four, s[0, 0].CellValue);
            Assert.AreEqual(CellValue.Four, s[0, 2].CellValue);
            Assert.AreEqual(CellValue.Six, s[2, 0].CellValue);

            s = m.Square(2, 1);
            Assert.AreEqual(CellValue.Seven, s[0, 0].CellValue);
            Assert.AreEqual(CellValue.Seven, s[0, 2].CellValue);
            Assert.AreEqual(CellValue.Nine, s[2, 0].CellValue);
        }

        [Test]
        public void TestCellToCollectionAccess()
        {
            Matrix m = GetSimpleRowMoveMatrix();
            Cell cell = m.At(8, 0);

            Row row = cell.Row;
            Assert.AreEqual(cell, row[8]);

            Column column = cell.Column;
            Assert.AreEqual(cell, column[0]);

            Square square = cell.Square;
            Assert.AreEqual(cell, square[2, 0]);

            cell = m.At(3, 4);
            square = cell.Square;
            Assert.AreEqual(cell, square[0, 1]);
        }

        [Test]
        public void TestSimpleMove()
        {
            Matrix m = GetSimpleRowMoveMatrix();
            Analyser.Analyser ma = new Analyser.Analyser();

            CellValue cellValue;

            Assert.AreEqual(true, Analyser.Analyser.CanMove(m, 8, 0, out cellValue));
            Assert.AreEqual(CellValue.Nine, cellValue);
            Assert.AreEqual(false, Analyser.Analyser.CanMove(m, 7, 0, out cellValue));

            Assert.AreEqual(true, Analyser.Analyser.CanMove(m));

            m = GetSimpleColumnMoveMatrix();
            Column c = m.Column(0);
            Assert.AreEqual(CellValue.Eight, c[7].CellValue);
            Assert.AreEqual(CellValue.Blank, c[8].CellValue);
            Assert.AreEqual(true, Analyser.Analyser.CanMove(m, 0, 8, out cellValue));
            Assert.AreEqual(CellValue.Nine, cellValue);
            Assert.AreEqual(false, Analyser.Analyser.CanMove(m, 0, 7, out cellValue));

            Assert.AreEqual(true, Analyser.Analyser.CanMove(m));

            m = GetSimpleSquareMoveMatrix();
            Square s = m.Square(0, 0);
            Assert.AreEqual(true, Analyser.Analyser.CanMove(m, 2, 2, out cellValue));
            Assert.AreEqual(CellValue.Nine, cellValue);

            Assert.AreEqual(true, Analyser.Analyser.CanMove(m));
        }

        [Test]
        public void TestMoves()
        {
            Analyser.Analyser ma = new Analyser.Analyser();
            Matrix m = Matrix.Blank;
            List<Move> moves = Analyser.Analyser.GetAvailableMoves(m);


            Assert.AreEqual(0, moves.Count);

            m = GetSimpleRowMoveMatrix();
            moves = Analyser.Analyser.GetAvailableMoves(m);

            StringBuilder moveString = new StringBuilder();
            foreach( Move move in moves )
            {
                moveString.Append(move.ToString());
            }

            Assert.AreEqual(1, moves.Count);
            Assert.AreEqual(CellValue.Nine, moves[0].CellValue);
            Assert.AreEqual(8, moves[0].I);
            Assert.AreEqual(0, moves[0].J);
        }

        [Test]
        public void TestMediumMatrixMoves()
        {
            Analyser.Analyser ma = new Analyser.Analyser();
            Matrix m = GetMediumMoveMatrix();
            List<Move> moves = Analyser.Analyser.GetAvailableMoves(m);
            Assert.AreEqual(1, moves.Count);
        }

        [Test]
        public void TestHardMatrixMoves()
        {
            Analyser.Analyser ma = new Analyser.Analyser();
            Matrix m = GetMediumMoveMatrix();
            List<Move> moves = Analyser.Analyser.GetAvailableMoves(m);
            Assert.AreEqual(1, moves.Count);
        }
    }
}

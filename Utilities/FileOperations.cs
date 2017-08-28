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
using System.IO;
using SimonSebright.Sudoku;

namespace SimonSebright.Sudoku.Utilities
{
    public static class FileOperations
    {
        private static string Signature { get { return "www.simonsebright.com Sudoku file format"; }}
        private static int CurrentVersion = 1;

        public class MatrixFileException : ApplicationException
        {
            public MatrixFileException( string message ) : base( message ) {}
        }

        public static Matrix MatrixFromFile( string file )
        {
            if ( !File.Exists( file ) ) throw new MatrixFileException( "File does not exist: " + file );

            using ( StreamReader sr = new StreamReader( file ))
            {
                return MatrixFromTextReader( sr );
            }
        }

        public static Matrix MatrixFromTextReader(TextReader tr)
        {
            string signature = tr.ReadLine();
            if ( signature != Signature )  throw new MatrixFileException( "This file is not a sudoku puzzle file" );

            string version = tr.ReadLine();

            string prefix = "Version ";
            if (version.IndexOf(prefix) == -1) throw new MatrixFileException("Version entry not found");
            switch ( version.Substring( prefix.Length ))
            {
                case "1": return MatrixFromTextReader1( tr );
                default: throw new MatrixFileException( "Unrecognised version number" );
            }
        }

        private static Matrix MatrixFromTextReader1( TextReader tr )
        {
            List<List<Cell>> rows = new List<List<Cell>>();
            for (int j = 0; j < Settings.GridSize; ++j)
            {
                string rowText = tr.ReadLine();

                if (rowText == null)
                {
                    rows.Add(Row.GetBlankRow());
                }
                else
                {
                    string[] rowCellText = rowText.Split(',');

                    List<Cell> row = new List<Cell>();

                    for (int i = 0; i < Settings.GridSize; ++i)
                    {
                        string cellText = i < rowCellText.Length ? rowCellText[i] : string.Empty;
                        row.Add( GetCell( cellText ));
                    }

                    rows.Add(row);
                }
            }

            return new Matrix(rows);
        }

        private static Cell GetCell(string cellText)
        {
            CellType cellType = CellType.Subsequent;
            CellValue cellValue = CellValue.Blank;

            try
            {
                cellText = cellText.Trim();
                if (cellText != string.Empty)
                {
                    string number = cellText.Substring(0, 1);


                    int i = 0;
                    
                    // We'll forgive them - it's a blank
                    try { i = int.Parse(number); }
                    catch { }

                    if (i != 0)
                    {
                        cellValue = (CellValue)i;
                        cellType = cellText.Length > 1 ? CellType.Original : CellType.Subsequent;
                    }
                }

                return new Cell( cellValue, cellType );
            }
            catch (Exception)
            {
                throw new MatrixException("Could not convert text to cell value: " + cellText);
            }

        }

        public static void MatrixToFile(Matrix m, string fileName)
        {
            using( StreamWriter sr = new StreamWriter( fileName ) )
            {
                sr.WriteLine(Signature);
                sr.WriteLine("Version " + CurrentVersion.ToString());

                for (int j = 0; j < Settings.GridSize; ++j)
                {
                    Row row = m.Row(j);
                    string rowText = string.Empty;
                    for (int i = 0; i < Settings.GridSize; ++i)
                    {
                        if (i > 0) rowText += ',';
                        Cell cell = m.At( i, j );
                        rowText += Cell.CellValueToString( cell.CellValue );

                        if (cell.Original) rowText += 'O';
                    }

                    sr.WriteLine( rowText );
                }
            }
        }
    }
}
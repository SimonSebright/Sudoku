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
using System.ComponentModel;

namespace SimonSebright.Sudoku.Analyser
{
    public class Analyser
    {
        public delegate bool AnalyseDelegate(Matrix m, int i, int j, out CellValue cellValue);

        public static readonly AnalyseDelegate Analyse;

        private readonly Matrix mM;

        static Analyser()
        {
            Analyse += CanMoveInExclusionGroups;
            Analyse += CanMoveInWiderCircles;
        }

        public Analyser()
        {
        }

        public Analyser(Matrix m)
        {
            mM = m;
        }

        public static bool CanMove(Matrix m)
        {
            return GetAvailableMoves(m).Count > 0;
        }

        public void GetAvailableMoves(object sender, DoWorkEventArgs e)
        {
            e.Result = GetAvailableMoves(mM);
        }

        public static List<Move> GetAvailableMoves(Matrix m)
        {
            var moves = new List<Move>();

            for (var i = 0; i < Settings.GridSize; ++i)
            {
                for (var j = 0; j < Settings.GridSize; ++j)
                {
                    CellValue cellValue;
                    if (CanMove(m, i, j, out cellValue))
                    {
                        moves.Add(new Move(i, j, cellValue));
                    }
                }
            }

            return moves;
        }

        public static bool CanMove(Matrix m, int i, int j, out CellValue cellValue)
        {
            cellValue = CellValue.Blank;

            foreach (AnalyseDelegate analyseFunc in Analyse.GetInvocationList())
            {
                if (analyseFunc(m, i, j, out cellValue))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CanMoveInExclusionGroups(Matrix m, int i, int j, out CellValue cellValue)
        {
            cellValue = CellValue.Blank;

            var cell = m.At(i, j);
            var groups = cell.ExclusionGroups;

            foreach (var group in groups)
            {
                if (CanMoveCellInExclusionGroup(group, cell, out cellValue))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CanMoveCellInExclusionGroup(CellExclusionGroup group, Cell cell, out CellValue cellValue)
        {
            var canMove = false;
            cellValue = CellValue.Blank;

            if (!group.Contains(cell))
            {
                throw new IndexOutOfRangeException("Cell is not on this exclusion group");
            }

            if (cell.CellValue != CellValue.Blank)
            {
                return false;
            }

            var uniques = new Dictionary<CellValue, int>();

            foreach (var member in group.GetCells())
            {
                if (member.CellValue != CellValue.Blank)
                {
                    if (!uniques.ContainsKey(member.CellValue))
                    {
                        uniques.Add(member.CellValue, 1);
                    }
                    else
                    {
                        uniques[member.CellValue] += 1;
                    }
                }
            }

            if (uniques.Count == Settings.GridSize - 1)
            {
                // Nearly there, there's one thing missing from our list, is it us?
                if (cell.CellValue == CellValue.Blank)
                {
                    canMove = true;

                    // But which is it?
                    for (var enumValue = (int) CellValue.One; enumValue <= (int) CellValue.Nine; ++enumValue)
                    {
                        if (!uniques.ContainsKey((CellValue) enumValue))
                        {
                            cellValue = (CellValue) enumValue;
                        }
                    }
                }
            }

            return canMove;
        }

        public static bool CanMoveInWiderCircles(Matrix m, int i, int j, out CellValue cellValue)
        {
            cellValue = CellValue.Blank;
            var cell = m.At(i, j);

            if (cell.CellValue != CellValue.Blank)
            {
                return false;
            }


            var groups = cell.ExclusionGroups;

            var secondLevelExclusions = new Dictionary<Cell, Dictionary<CellValue, int>>();

            // For every exclusion group, the non-blank cells will have values they cannot be.  These come from their exclusion groups.
            // Thus, we hope to find at least one cell whose value cannot be (everything but one value) and that is the value we must be
            foreach (var group in groups)
            {
                foreach (var member in group.GetCells())
                {
                    if (!secondLevelExclusions.ContainsKey(member))
                    {
                        secondLevelExclusions.Add(member, new Dictionary<CellValue, int>());
                    }

                    // If we have a value already, then everything else is automatically an exclusion
                    if (member.CellValue != CellValue.Blank)
                    {
                        AddOtherExclusions(member.CellValue, secondLevelExclusions[member]);
                    }
                    else
                    {
                        var secondLevelGroups = member.ExclusionGroups;
                        foreach (var secondLevelGroup in secondLevelGroups)
                        {
                            foreach (var secondLevelMember in secondLevelGroup.GetCells())
                            {
                                var secondLevelValue = secondLevelMember.CellValue;

                                if (secondLevelValue != CellValue.Blank)
                                {
                                    secondLevelExclusions[member][secondLevelValue] = 0;
                                }
                            }
                        }
                    }
                }
            }

            // Now, we have all the values which the other cells in each group cannot be.  We hope to find that in one group, all cells
            // have the same non-permitted value
            foreach (var group in groups)
            {
                for (var enumValue = (int) CellValue.One; enumValue <= (int) CellValue.Nine; ++enumValue)
                {
                    var allOthersHaveExclusion = true;

                    foreach (var member in group.GetCells())
                    {
                        if (member != cell) // we are the one we are looking for!
                        {
                            var exclusionsThisMember = secondLevelExclusions[member];

                            if (!exclusionsThisMember.ContainsKey((CellValue) enumValue))
                            {
                                allOthersHaveExclusion = false;
                            }
                        }
                    }

                    if (allOthersHaveExclusion)
                    {
                        cellValue = (CellValue) enumValue;
                        return true;
                    }
                }
            }

            return false;
        }

        private static void AddOtherExclusions(CellValue cellValue, Dictionary<CellValue, int> dict)
        {
            for (var enumValue = (int) CellValue.One; enumValue <= (int) CellValue.Nine; ++enumValue)
            {
                if ((CellValue) enumValue != cellValue)
                {
                    dict[(CellValue) enumValue] = 0;
                }
            }
        }

        public void CalculateConsistency(object sender, DoWorkEventArgs e)
        {
            e.Result = IsSolved(mM) ? Consistency.Solved : TryToSolve(mM);
        }

        private static bool IsInconsistent(Matrix m)
        {
            foreach (var group in m.GetExclusionGroups())
            {
                var dict = new Dictionary<CellValue, int>();
                foreach (var cell in group.GetCells())
                {
                    if (cell.CellValue != CellValue.Blank)
                    {
                        if (dict.ContainsKey(cell.CellValue))
                        {
                            return true;
                        }

                        dict.Add(cell.CellValue, 0);
                    }
                }
            }

            return false;
        }

        private static bool IsSolved(Matrix m)
        {
            foreach (var group in m.GetExclusionGroups())
            {
                var dict = new Dictionary<CellValue, int>();
                foreach (var cell in group.GetCells())
                {
                    if (cell.CellValue == CellValue.Blank)
                    {
                        return false;
                    }

                    if (dict.ContainsKey(cell.CellValue))
                    {
                        return false;
                    }

                    dict.Add(cell.CellValue, 0);
                }
            }

            return true;
        }

        private static Consistency TryToSolve(Matrix m)
        {
            if (IsSolved(m))
            {
                return Consistency.Solvable;
            }
            if (IsInconsistent(m))
            {
                return Consistency.Inconsistent;
            }
            var moves = GetAvailableMoves(m);

            if (moves.Count != 0)
            {
                var m2 = m.MakeMoves(moves, CellType.Subsequent);
                return TryToSolve(m2);
            }
            return Consistency.Indeterminate;
        }

        public static void GenerateNewPuzzle(object sender, DoWorkEventArgs e)
        {
            e.Result = GenerateNewPuzzle(sender as BackgroundWorker);
        }

        private static Matrix MakeRandomValidMove(Matrix m)
        {
            var rand = new Random(DateTime.Now.Millisecond);

            var foundEmptyCell = false;
            var i = 0;
            var j = 0;

            while (!foundEmptyCell)
            {
                i = rand.Next(Settings.GridSize);
                j = rand.Next(Settings.GridSize);

                if (m.At(i, j).CellValue == CellValue.Blank)
                {
                    foundEmptyCell = true;
                }
            }

            var exclusions = m.At(i, j).ExclusionGroups;
            var cellValue = CellValue.Blank;

            foreach (var test in Cell.AllCellValuesRandom())
            {
                var containsValue = false;

                foreach (var group in exclusions)
                {
                    if (group.ContainsValue(test))
                    {
                        containsValue = true;
                        break;
                    }
                }

                if (!containsValue)
                {
                    cellValue = test;
                    break;
                }
            }

            if (cellValue == CellValue.Blank)
            {
                throw new ApplicationException("Failed to get a valid move");
            }

            var move = new Move(i, j, cellValue);

            return m.MakeMove(move, CellType.Original);
        }

        private static Matrix GenerateNewPuzzle(BackgroundWorker bw)
        {
            var attempts = new Stack<Matrix>();
            attempts.Push(Matrix.Blank);

            // Now, we have to do random moves until we find a situation which is solvable
            var finished = false;
            while (!finished)
            {
                var m1 = MakeRandomValidMove(attempts.Peek());

                var consistency = TryToSolve(m1);

                switch (consistency)
                {
                    case Consistency.Indeterminate:
                        attempts.Push(m1);
                        break;
                    case Consistency.Solvable:
                    case Consistency.Solved:
                        attempts.Push(m1);
                        finished = true;
                        break;
                    case Consistency.Inconsistent:
                        // A valid move leading to inconsistency means we get really stuck
                        // scrap this one and the attempt
                        attempts.Pop();
                        break;
                }

                if (bw.CancellationPending)
                {
                    finished = true;
                }
            }

            return attempts.Pop();
        }
    }
}
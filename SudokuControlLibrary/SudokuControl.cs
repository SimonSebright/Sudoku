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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using SimonSebright.Interfaces;
using SimonSebright.Sudoku;
using SimonSebright.Sudoku.Analyser;

namespace SimonSebright.SudokuControlLibrary
{
    [Designer(typeof(SudokuControlDesigner))]
    public partial class SudokuControl : Control, IUndoable
    {
        public delegate void ErrorEvent(Exception e);

        public enum Mode
        {
            Design,
            Play
        };

        private static readonly Pen m_bigPen = new Pen(Color.Black, 2);
        private static readonly Pen m_smallPen = new Pen(Color.Black, 1);

        private readonly string BlankString = ControlRes.BlankText;
        private readonly Stack<Matrix> m_redoStack = new Stack<Matrix>();

        private readonly Stack<Matrix> m_undoStack = new Stack<Matrix>();

        private List<Move> m_availableMoves = new List<Move>();

        private Consistency m_consistency = Consistency.Indeterminate;
        private int m_focusI = 0;
        private int m_focusJ = 0;

        private Point m_lastContextMenuClickPoint;

        private Matrix m_m;
        private Mode m_mode = Mode.Design;
        private Brush m_moveBrush = Brushes.Black;
        private Color m_moveColor = Color.Black;
        private Font m_moveFont;

        private Font m_originalFont;
        private bool m_showMoves;

        public SudokuControl()
        {
            m_m = Matrix.Blank;
            m_originalFont = Font.Clone() as Font;
            m_moveFont = Font.Clone() as Font;

            InitializeComponent();

            ContextMenuStrip = contextMenuStrip;
            var valueMenu = GetValueMenu();

            foreach (var cellValue in Cell.AllCellValues())
            {
                var text = GetCellMenuText(cellValue);
                var item = new ToolStripMenuItem(text);
                item.Tag = text;
                item.ToolTipText = "Make this cell " + text;
                item.Click += contextMenuStrip_ValueItemClicked;
                valueMenu.DropDownItems.Add(item);
            }

            foreach (ToolStripMenuItem item in GetModeMenu().DropDownItems)
            {
                item.Click += contextMenuStrip_GameItemClicked;
            }

            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        /// <summary>
        ///     Use this to update the matrix being shown when normal user operations occur
        ///     Undo and Redo as well as Starting new games have special considerations
        /// </summary>
        public Matrix Matrix
        {
            get => m_m;
            private set
            {
                // Redo goes out the window here
                m_redoStack.Clear();
                m_undoStack.Push(m_m);
                SwitchMatrix(value);
            }
        }

        public Consistency GameConsistency
        {
            get => m_consistency;
            private set
            {
                m_consistency = value;
                IssueConsistencyChangedEvent();
            }
        }

        public bool ShowingMoves => m_showMoves;

        public bool HasMoves => GameConsistency != Consistency.Inconsistent && NumberMoves > 0;

        public int NumberMoves => GameConsistency == Consistency.Inconsistent ? 0 : m_availableMoves.Count;

        public List<Move> AvailableMoves
        {
            get => m_availableMoves;
            private set
            {
                m_availableMoves = value;
                IssueAvailableMovesChangedEvent();
            }
        }

        [Description("Mode the control will start in.  Design is usual with a blank grid!")]
        [Category("Behaviour")]
        public Mode GameMode
        {
            get => m_mode;
            set
            {
                if (m_mode != value)
                {
                    if (value == Mode.Design && m_m.HasSubsequentCells)
                    {
                        throw new SudokoControlException("Cannot switch to design mode when moves have been played");
                    }

                    m_mode = value;
                    ModeChanged?.Invoke(this, null);
                }
            }
        }

        [Description("Font used for cells of the original starting game")]
        [Category("Appearance")]
        public Font OriginalCellFont
        {
            get => m_originalFont;
            set => m_originalFont = value;
        }

        [Description("Font used for available moves in the game")]
        [Category("Appearance")]
        public Font MoveFont
        {
            get => m_moveFont;
            set => m_moveFont = value;
        }

        [Description("Colour used for available moves in the game")]
        [Category("Appearance")]
        public Color MoveColor
        {
            get => m_moveColor;
            set
            {
                m_moveColor = value;
                if (m_moveBrush != Brushes.Black)
                {
                    m_moveBrush.Dispose();
                }

                m_moveBrush = new SolidBrush(m_moveColor);
            }
        }

        public Cell FocusCell => m_m.At(m_focusI, m_focusJ);

        private StringFormat CellFormat
        {
            get
            {
                var sf = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                return sf;
            }
        }

        private float BigCellSizeX => (float) ClientRectangle.Width / Settings.NumSquares;

        private float BigCellSizeY => (float) ClientRectangle.Height / Settings.NumSquares;

        private float SmallCellSizeX => (float) ClientRectangle.Width / Settings.GridSize;

        private float SmallCellSizeY => (float) ClientRectangle.Height / Settings.GridSize;

        private Pen BigPen => m_bigPen;

        private Pen SmallPen => m_smallPen;

        private Matrix SampleMatrix
        {
            get
            {
                var rows = new List<List<Cell>>();

                Cell[] cells1 = {Cell.Cell1(), Cell.Cell2(), Cell.Cell3(), Cell.Cell4(), Cell.Cell5(), Cell.Cell6(), Cell.Cell7(), Cell.Cell8(), Cell.BlankCell()};
                var row = new List<Cell>();
                row.AddRange(cells1);
                rows.Add(row);

                rows.Add(Row.GetBlankRow());
                rows.Add(Row.GetBlankRow());

                for (var j = 0; j < Settings.GridSize - Settings.SquareSize; ++j)
                {
                    rows.Add(Row.GetBlankRow());
                }

                return new Matrix(rows);
            }
        }

        private Move SampleMove => new Move(8, 0, CellValue.Nine);

        public bool CanUndo()
        {
            return m_undoStack.Count > 0;
        }

        public bool CanRedo()
        {
            return m_redoStack.Count > 0;
        }

        public void Undo()
        {
            if (CanUndo())
            {
                var current = m_m;
                var next = m_undoStack.Pop();

                m_redoStack.Push(m_m);
                SwitchMatrix(next);
            }
        }

        public void Redo()
        {
            if (CanRedo())
            {
                var current = m_m;
                var next = m_redoStack.Pop();

                m_undoStack.Push(m_m);
                SwitchMatrix(next);
            }
        }

        private void SwitchMatrix(Matrix m)
        {
            m_m = m;
            m_availableMoves.Clear();
            IssueMatrixChangedEvent();
            Invalidate();
        }

        public event EventHandler UndoRedoChangedEvent;

        private void IssueUndoRedoChangedEvent()
        {
            UndoRedoChangedEvent?.Invoke(this, null);
        }

        public event EventHandler IsDirtyEvent;

        private void IssueIsDirtyEvent()
        {
            IsDirtyEvent?.Invoke(this, null);

            Invalidate();
        }

        public event EventHandler MatrixChanged;

        private void IssueMatrixChangedEvent()
        {
            MatrixChanged?.Invoke(this, null);

            m_availableMoves.Clear();
            m_showMoves = false;

            CalculateAvailableMoves();
            CalculateConsistency();
        }

        public event EventHandler AvailableMovesChanged;

        private void IssueAvailableMovesChangedEvent()
        {
            AvailableMovesChanged?.Invoke(this, null);
        }

        public event EventHandler ConsistencyChanged;

        private void IssueConsistencyChangedEvent()
        {
            ConsistencyChanged?.Invoke(this, null);
        }

        private void CalculateConsistency()
        {
            GameConsistency = Consistency.Calculating;
            // Based on the matrix as it is.  It's immutable, so set up a worker thread to report on it.
            var a = new Analyser(m_m);
            var bw = new BackgroundWorker();
            bw.DoWork += a.CalculateConsistency;
            bw.RunWorkerCompleted += OnConsistencyCheckComplete;
            bw.RunWorkerAsync();
        }

        private void OnConsistencyCheckComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            GameConsistency = (Consistency) e.Result;
        }

        private void OnCalclateAvailableMovesComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            AvailableMoves = (List<Move>) e.Result;
        }

        private void CalculateAvailableMoves()
        {
            var a = new Analyser(m_m);
            var bw = new BackgroundWorker();
            bw.DoWork += a.GetAvailableMoves;
            bw.RunWorkerCompleted += OnCalclateAvailableMovesComplete;
            bw.RunWorkerAsync();
        }

        public void ToggleMoves()
        {
            if (ShowingMoves)
            {
                HideMoves();
            }
            else
            {
                ShowMoves();
            }
        }

        public void ShowMoves()
        {
            m_showMoves = true;
            Invalidate();
        }

        public void HideMoves()
        {
            m_showMoves = false;
            Invalidate();
        }

        public void MakeAvailableMoves()
        {
            MakeMoves(m_availableMoves);
        }

        /// <summary>
        ///     Uses random value in list so that we don't always do something near the top
        /// </summary>
        public void MakeOneAvailableMove()
        {
            if (HasMoves)
            {
                var r = new Random(DateTime.Now.Millisecond);
                var move = m_availableMoves[r.Next(m_availableMoves.Count)];
                MakeMove(move);
            }
        }

        private void MakeMove(Move move)
        {
            var moves = new List<Move> {move};
            MakeMoves(moves);
        }

        private CellType GetCurrentMoveCellType()
        {
            return GameMode == Mode.Design ? CellType.Original : CellType.Subsequent;
        }

        private void CheckMovesForOverwritingGameOrigin(List<Move> moves)
        {
            if (GameMode == Mode.Play)
            {
                foreach (var move in moves)
                {
                    if (m_m.At(move.I, move.J).Original)
                    {
                        throw new SudokoControlException(ControlRes.NoOverwriteDesign);
                    }
                }
            }
        }

        public void MakeMoves(List<Move> moves)
        {
            CheckMovesForOverwritingGameOrigin(moves);
            Matrix = m_m.MakeMoves(moves, GetCurrentMoveCellType());
            IssueIsDirtyEvent();
            IssueMatrixChangedEvent();
        }

        public event EventHandler ModeChanged;

        public void ToggleMode()
        {
            GameMode = GameMode == Mode.Design ? Mode.Play : Mode.Design;
        }

        public void StartNewGame()
        {
            Matrix = Matrix.Blank;
            m_mode = Mode.Design;
            ResetUndoRedo();
        }

        public void StartGame(Matrix m)
        {
            Matrix = m;
            GameMode = Mode.Play;
            ResetUndoRedo();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (DesignMode)
            {
                DrawDesigner(pe);
            }
            else
            {
                DrawRuntime(pe);
            }
        }

        private void DrawDesigner(PaintEventArgs pe)
        {
            RenderGrid(pe, SampleMatrix);
            RenderMove(pe, m_m, SampleMove);

            var transform = new System.Drawing.Drawing2D.Matrix();
            transform.RotateAt(-45, new Point(ClientRectangle.Width / 2, ClientRectangle.Height / 2));
            pe.Graphics.Transform = transform;

            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            pe.Graphics.DrawString("Form Design Mode!", Font, Brushes.Red, ClientRectangle, sf);
        }

        private void DrawRuntime(PaintEventArgs pe)
        {
            RenderGrid(pe, m_m);

            if (m_showMoves)
            {
                RenderMoves(pe, m_m, m_availableMoves);
            }

            if (Focused)
            {
                DrawFocusRect(pe.Graphics, m_focusI, m_focusJ, m_m);
            }
        }

        /// <summary>
        ///     This one a pain, as we have to take into account rounding and the larger bars for the
        ///     bigger square drawing.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        private Rectangle GetFocusRect(int i, int j, Matrix m)
        {
            var cellRectF = GetCellRect(i, j, m);
            cellRectF.Offset(new PointF(0.5F, 0.5F));
            // Combat int rounding
            var tl = new Point((int) cellRectF.X, (int) cellRectF.Y);
            var br = new Point((int) cellRectF.Right, (int) cellRectF.Bottom);
            var cellRect = new Rectangle(tl, new Size(br.X - tl.X, br.Y - tl.Y));
            cellRect.Offset(1, 1);
            cellRect.Width -= (i + 1) % Settings.SquareSize == 0 ? 2 : 1;
            cellRect.Height -= (j + 1) % Settings.SquareSize == 0 ? 2 : 1;

            return cellRect;
        }

        private void DrawFocusRect(Graphics g, int i, int j, Matrix m)
        {
            ControlPaint.DrawFocusRectangle(g, GetFocusRect(i, j, m));
        }

        private void ChangeFocusTo(int i, int j)
        {
            if (i != m_focusI || j != m_focusJ)
            {
                var g = CreateGraphics();
                Invalidate(GetFocusRect(m_focusI, m_focusJ, m_m));
                DrawFocusRect(g, i, j, m_m);
                m_focusI = i;
                m_focusJ = j;
            }
        }

        private void MoveFocus(Size offset)
        {
            var i = Math.Min(Math.Max(0, m_focusI + offset.Width), Settings.GridSize - 1);
            var j = Math.Min(Math.Max(0, m_focusJ + offset.Height), Settings.GridSize - 1);
            ChangeFocusTo(i, j);
        }

        public event ErrorEvent OnErrorOccurred;

        private void IssueErrorEvent(Exception e)
        {
            OnErrorOccurred?.Invoke(e);
        }

        private void ProtectedFunc(UIFunc func)
        {
            try { func(); }
            catch (Exception e)
            {
                IssueErrorEvent(e);
            }
        }

        private void ProtectedFunc(UIFuncCellValue func, CellValue cellValue)
        {
            try { func(cellValue); }
            catch (Exception e)
            {
                IssueErrorEvent(e);
            }
        }

        private void ProtectedFunc(UIFuncMode func, Mode modeValue)
        {
            try { func(modeValue); }
            catch (Exception e)
            {
                IssueErrorEvent(e);
            }
        }

        private void MoveAtFocus(CellValue cellValue)
        {
            ProtectedFunc(delegate()
                {
                    var move = new Move(m_focusI, m_focusJ, cellValue);
                    MakeMove(move);
                }
            );
        }

        private void RenderMoves(PaintEventArgs pe, Matrix m, List<Move> moves)
        {
            foreach (var move in moves)
            {
                RenderMove(pe, m, move);
            }
        }

        private void RenderMove(PaintEventArgs pe, Matrix m, Move move)
        {
            RenderCellValue(pe, m, move.I, move.J, move.ToString(), m_moveFont, m_moveBrush);
        }

        private void RenderGrid(PaintEventArgs pe, Matrix m)
        {
            RenderLines(pe, m);
            RenderNumbers(pe, m);
        }

        private void RenderLines(PaintEventArgs pe, Matrix m)
        {
            pe.Graphics.FillRectangle(Brushes.White, ClientRectangle);
            pe.Graphics.DrawRectangle(BigPen, ClientRectangle);

            for (var i = 1; i <= Settings.GridSize - 1; ++i)
            {
                var x = GridCellX(i);
                var pen = i % Settings.SquareSize == 0 ? BigPen : SmallPen;

                pe.Graphics.DrawLine(pen, new PointF(x, 0), new PointF(x, ClientRectangle.Height));
            }

            for (var j = 1; j <= Settings.GridSize - 1; ++j)
            {
                var y = GridCellY(j);
                var pen = j % Settings.SquareSize == 0 ? BigPen : SmallPen;

                pe.Graphics.DrawLine(pen, new PointF(0, y), new PointF(ClientRectangle.Width, y));
            }
        }

        private void RenderNumbers(PaintEventArgs pe, Matrix m)
        {
            var normal = Font;
            {
                for (var i = 0; i < Settings.GridSize; ++i)
                {
                    for (var j = 0; j < Settings.GridSize; ++j)
                    {
                        var font = m.At(i, j).Original ? m_originalFont : normal;
                        RenderCellValue(pe, m, i, j, m.At(i, j).ToString(), font);
                    }
                }
            }
        }

        private void RenderCellValue(PaintEventArgs pe, Matrix m, int i, int j, string text, Font font)
        {
            var cellRect = GetCellRect(i, j, m);
            pe.Graphics.DrawString(text, font, Brushes.Black, cellRect, CellFormat);
        }

        private void RenderCellValue(PaintEventArgs pe, Matrix m, int i, int j, string text, Font font, Brush brush)
        {
            var cellRect = GetCellRect(i, j, m);
            pe.Graphics.DrawString(text, font, brush, cellRect, CellFormat);
        }

        private RectangleF GetCellRect(int i, int j, Matrix m)
        {
            return new RectangleF(new PointF(GridCellX(i), GridCellY(j)),
                new SizeF(SmallCellSizeX, SmallCellSizeY));
        }

        private float GridCellX(int i)
        {
            return i * SmallCellSizeX;
        }

        private float GridCellY(int j)
        {
            return j * SmallCellSizeY;
        }

        private void contextMenuStrip_ValueItemClicked(object sender, EventArgs e)
        {
            var text = ((ToolStripItem) sender).Tag.ToString();
            var cellValue = GetCellValueFromMenuText(text);

            ProtectedFunc(delegate(CellValue moveValue)
                {
                    var clickedCell = HitTestCellOrFocus(m_lastContextMenuClickPoint);

                    var move = new Move(clickedCell.I, clickedCell.J, cellValue);
                    var moves = new List<Move> {move};
                    MakeMoves(moves);
                },
                cellValue
            );
        }

        private void contextMenuStrip_GameItemClicked(object sender, EventArgs e)
        {
            var text = ((ToolStripItem) sender).Tag.ToString();
            var mode = (Mode) Enum.Parse(typeof(Mode), text);

            ProtectedFunc(delegate(Mode modeValue) { GameMode = modeValue; },
                mode
            );
        }

        public ToolStripDropDown GetGameMenu()
        {
            return contextMenuStrip;
        }

        private ToolStripMenuItem GetValueMenu()
        {
            return (ToolStripMenuItem) contextMenuStrip.Items["MenuItemValues"];
        }

        private ToolStripMenuItem GetModeMenu()
        {
            return (ToolStripMenuItem) contextMenuStrip.Items["MenuItemGameMode"];
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            m_lastContextMenuClickPoint = PointToClient(MousePosition);

            var cell = HitTestCellOrFocus(m_lastContextMenuClickPoint);
            var menu = GetValueMenu();

            foreach (ToolStripMenuItem item in menu.DropDownItems)
            {
                item.Checked = GetCellMenuText(cell.CellValue) == item.Tag.ToString();
            }

            foreach (ToolStripMenuItem item in GetModeMenu().DropDownItems)
            {
                item.Checked = GameMode.ToString() == item.Tag.ToString();
            }
        }

        private Cell HitTestCellOrFocus(Point pt)
        {
            try
            {
                return HitTestCell(pt);
            }
            catch (SudokoControlException)
            {
                return FocusCell;
            }
        }

        private Cell HitTestCell(Point pt)
        {
            for (var i = 0; i < Settings.GridSize; ++i)
            {
                for (var j = 0; j < Settings.GridSize; ++j)
                {
                    if (GetCellRect(i, j, m_m).Contains(pt))
                    {
                        return m_m.At(i, j);
                    }
                }
            }

            throw new SudokoControlException("Requested point not in control: " + pt);
        }

        private void SudokuControl_MouseDown(object sender, MouseEventArgs e)
        {
            var cell = HitTestCell(e.Location);
            ChangeFocusTo(cell.I, cell.J);
        }

        private void SudokuControl_MouseClick(object sender, MouseEventArgs e)
        {
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right: return true;
                case Keys.Left: return true;
                case Keys.Down: return true;
                case Keys.Up: return true;

                default: return base.IsInputKey(keyData);
            }
        }

        private void SudokuControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right:
                    MoveFocus(new Size(1, 0));
                    break;
                case Keys.Left:
                    MoveFocus(new Size(-1, 0));
                    break;
                case Keys.Down:
                    MoveFocus(new Size(0, 1));
                    break;
                case Keys.Up:
                    MoveFocus(new Size(0, -1));
                    break;
                default: break;
            }
        }

        private void SudokuControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '1' && e.KeyChar <= '9')
            {
                var cellValue = (CellValue) (e.KeyChar - '1' + 1);
                MoveAtFocus(cellValue);
            }
            else if (e.KeyChar == ' ')
            {
                MoveAtFocus(CellValue.Blank);
            }
        }

        private string GetCellMenuText(CellValue cellValue)
        {
            var text = Cell.CellValueToString(cellValue);
            text = text == string.Empty ? BlankString : text;
            return text;
        }

        private CellValue GetCellValueFromMenuText(string text)
        {
            return text == BlankString ? CellValue.Blank : (CellValue) int.Parse(text);
        }

        private void ResetUndoRedo()
        {
            m_undoStack.Clear();
            m_redoStack.Clear();
            IssueUndoRedoChangedEvent();
        }

        private delegate void UIFunc();

        private delegate void UIFuncCellValue(CellValue cellValue);

        private delegate void UIFuncMode(Mode mode);
    }

    internal class SudokuControlDesigner : ControlDesigner
    {
    }
}
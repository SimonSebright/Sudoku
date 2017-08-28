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
using System.Drawing;
using System.Windows.Forms;
using SimonSebright.Sudoku.Analyser;
using SimonSebright.Sudoku.Utilities;

namespace SimonSebright.SudokuUI
{
    public partial class MainForm : Form
    {
        private bool m_dirty;

        private string m_fileName;

        public MainForm()
        {
            InitializeComponent();
        }

        private bool Dirty
        {
            get => m_dirty;
            set
            {
                m_dirty = value;
                UpdateControls();
            }
        }

        private void PerformUIConfirmAction(UIDelegate func)
        {
            if (ConfirmScrap())
            {
                PerformUIAction(func);
            }
        }

        private void PerformUIAction(UIDelegate func)
        {
            try { func(); }
            catch (Exception e)
            {
                ShowInfoTip(e.Message);
            }
        }

        private bool ConfirmScrap()
        {
            var scrapable = !Dirty;

            if (!scrapable)
            {
                var dr =
                    MessageBox.Show("The puzzle has changed. Save changes?",
                        "Save changes?",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button3);

                switch (dr)
                {
                    case DialogResult.No:
                        scrapable = true;
                        break;
                    case DialogResult.Cancel:
                        scrapable = false;
                        break;
                    default:
                        SaveFile();
                        scrapable = !Dirty;
                        break;
                }
            }

            return scrapable;
        }

        private void OnDirty(object senter, EventArgs e)
        {
            Dirty = true;
        }

        private void OnModeChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void OnConsistencyChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void OnAvailableMovesChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void OnUndoRedoChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            UINewFile();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UINewFile();
        }

        private void UINewFile()
        {
            PerformUIConfirmAction(NewFile);
        }

        private void NewFile()
        {
            sudokuControl1.StartNewGame();
            m_fileName = null;
            Dirty = false;
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            UIOpenFile();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UIOpenFile();
        }

        private void UIOpenFile()
        {
            PerformUIConfirmAction(OpenFile);
        }

        private void SetFileDialogFilter(FileDialog dlg)
        {
            dlg.Filter = "Sudoku puzzles (*.sdk) | *.sdk" +
                         "|All files (*.*) | *.*";
        }

        private void OpenFile()
        {
            var ofd = new OpenFileDialog();
            ofd.Title = "Open existing puzzle";
            SetFileDialogFilter(ofd);
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                OpenFile(ofd.FileName);
            }
        }

        private void OpenFile(string file)
        {
            var m = FileOperations.MatrixFromFile(file);
            sudokuControl1.StartGame(m);
            m_fileName = file;
            Dirty = false;
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            UISaveFile();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UISaveFile();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UISaveFileAs();
        }

        private void UISaveFile()
        {
            PerformUIAction(SaveFile);
        }

        private void UISaveFileAs()
        {
            PerformUIAction(SaveFileAs);
        }

        private void SaveFile()
        {
            if (m_fileName == null)
            {
                if (SaveFileAsWithSuccess())
                {
                    Dirty = false;
                }
            }
            else if (SaveFile(m_fileName))
            {
                Dirty = false;
            }
        }

        private void SaveFileAs()
        {
            SaveFileAsWithSuccess();
        }

        private bool SaveFileAsWithSuccess()
        {
            var sfd = new SaveFileDialog {Title = "Save sudoku puzzle"};
            SetFileDialogFilter(sfd);

            var result = sfd.ShowDialog() == DialogResult.OK;

            if (result)
            {
                SaveFile(sfd.FileName);
                // This after in case of exceptions
                m_fileName = sfd.FileName;
                Dirty = false;
            }

            return result;
        }

        private bool SaveFile(string fileName)
        {
            FileOperations.MatrixToFile(sudokuControl1.Matrix, fileName);
            return true;
        }

        private void ShowMovesButton_Click(object sender, EventArgs e)
        {
            UpdateUI(ToggleMoves);
        }

        private void makeMovesButton_Click(object sender, EventArgs e)
        {
            UpdateUIDirty(MakeMoves);
        }

        private void makeOneMoveButton_Click(object sender, EventArgs e)
        {
            UpdateUIDirty(MakeOneMove);
        }

        private void UpdateUI(UIDelegate func)
        {
            PerformUIAction(func);
            UpdateControls();
        }

        private void UpdateUIDirty(UIDelegate func)
        {
            PerformUIAction(func);
            Dirty = true;
            UpdateControls();
        }

        private void NotImplemented()
        {
            throw new NotImplementedException(AppRes.NotImplemented);
        }

        private void UpdateControls()
        {
            StripButtonShowMoves.Checked = sudokuControl1.ShowingMoves;
            StripButtonMakeMoves.Enabled = sudokuControl1.HasMoves;
            StripButtonMakeOneMove.Enabled = sudokuControl1.HasMoves;

            undoToolStripMenuItem.Enabled = StripButtonUndo.Enabled = sudokuControl1.CanUndo();
            redoToolStripMenuItem.Enabled = StripButtonRedo.Enabled = sudokuControl1.CanRedo();

            StatusLabelMode.Text = string.Format(AppRes.ModeFormat, sudokuControl1.GameMode.ToString());
            StatusLabelConsistent.Text = string.Format(AppRes.ConsistencyFormat, sudokuControl1.GameConsistency.ToString());
            StatusLabelConsistent.ForeColor = sudokuControl1.GameConsistency == Consistency.Inconsistent ? Color.Red : Color.Black;
            StatusLabelMovesAvailable.Text = string.Format(AppRes.AvailableMovesFormat, sudokuControl1.NumberMoves);

            var text = string.Format(AppRes.TitleFormat,
                AppRes.AppTitle,
                m_fileName ?? AppRes.UntitledName,
                Dirty ? AppRes.ChangedIndicator : string.Empty);
            Text = text;
        }

        private void OnErrorInControl(Exception e)
        {
            ShowInfoTip(e.Message);
        }

        private void ShowInfoTip(string message)
        {
            var tip = new ToolTip
            {
                IsBalloon = true,
                InitialDelay = 0,
                ToolTipIcon = ToolTipIcon.Info
            };
            var client = PointToClient(Cursor.Position);
            var control = GetChildAtPoint(client);
            control = control ?? this;

            tip.Show(message, control, control.PointToClient(Cursor.Position), 3000);

            var timer = new Timer
            {
                Interval = 3000,
                Tag = tip
            };
            timer.Tick += (sender, args) =>
            {
                timer.Stop();
                tip.Dispose();
                timer.Dispose();
            };
            timer.Start();
        }

        private void ToggleMoves()
        {
            if (!sudokuControl1.HasMoves)
            {
                ShowInfoTip(AppRes.NoMoves);
            }

            sudokuControl1.ToggleMoves();
        }

        private void MakeMoves()
        {
            sudokuControl1.MakeAvailableMoves();
        }

        private void MakeOneMove()
        {
            sudokuControl1.MakeOneAvailableMove();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            sudokuControl1.IsDirtyEvent += OnDirty;
            sudokuControl1.ModeChanged += OnModeChanged;
            sudokuControl1.ConsistencyChanged += OnConsistencyChanged;
            sudokuControl1.AvailableMovesChanged += OnAvailableMovesChanged;
            sudokuControl1.UndoRedoChangedEvent += OnUndoRedoChanged;
            sudokuControl1.OnErrorOccurred += OnErrorInControl;
            gameToolStripMenuItem.DropDown = sudokuControl1.GetGameMenu();
            UpdateControls();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Dirty && !ConfirmScrap())
            {
                e.Cancel = true;
            }
        }

        private void StatusLabelMode_DoubleClick(object sender, EventArgs e)
        {
            ToggleMode();
        }

        private void ToggleMode()
        {
            PerformUIAction(sudokuControl1.ToggleMode);
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UIPrint();
        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            UIPrint();
        }

        private void UIPrint()
        {
            PerformUIAction(NotImplemented);
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            UICut();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UICut();
        }

        private void UICut()
        {
            PerformUIAction(NotImplemented);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UICopy();
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            UICopy();
        }

        private void UICopy()
        {
            PerformUIAction(NotImplemented);
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            UIPaste();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UIPaste();
        }

        private void UIPaste()
        {
            PerformUIAction(NotImplemented);
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UISelectAll();
        }

        private void UISelectAll()
        {
            PerformUIAction(NotImplemented);
        }

        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UICustomize();
        }

        private void UICustomize()
        {
            PerformUIAction(NotImplemented);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UIOptions();
        }

        private void UIOptions()
        {
            PerformUIAction(NotImplemented);
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UIHelpContents();
        }

        private void UIHelpContents()
        {
            PerformUIAction(NotImplemented);
        }

        private void indexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UIHelpIndex();
        }

        private void UIHelpIndex()
        {
            PerformUIAction(NotImplemented);
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UIHelpSearch();
        }

        private void UIHelpSearch()
        {
            PerformUIAction(NotImplemented);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UIHelpAbout();
        }

        private void UIHelpAbout()
        {
            PerformUIAction(() =>
            {
                var ab = new AboutBox();
                ab.ShowDialog();
            });
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UIUndo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UIRedo();
        }

        private void UIUndo()
        {
            PerformUIAction(delegate() { sudokuControl1.Undo(); });
        }

        private void UIRedo()
        {
            PerformUIAction(delegate() { sudokuControl1.Redo(); });
        }

        private void StripButtonUndo_Click(object sender, EventArgs e)
        {
            UIUndo();
        }

        private void StripButtonRedo_Click(object sender, EventArgs e)
        {
            UIRedo();
        }

        private void StripButtonNewPuzzle_Click(object sender, EventArgs e)
        {
            UINewPuzzle();
        }

        private void UINewPuzzle()
        {
            PerformUIConfirmAction(delegate()
                {
                    var m = WaitingDialog.GenerateNewPuzzle();
                    if (m == null)
                    {
                        throw new SudokuException(AppRes.NoNewPuzzle);
                    }
                    sudokuControl1.StartGame(m);
                    // Treat as a new file
                    m_fileName = null;
                    Dirty = true;
                }
            );
        }

        private void StripButtonReset_Click(object sender, EventArgs e)
        {
            UIReset();
        }

        private void UIReset()
        {
            PerformUIConfirmAction(delegate()
                {
                    if (!sudokuControl1.Matrix.HasSubsequentCells)
                    {
                        throw new SudokuException(AppRes.GameNotPlayed);
                    }

                    var m = sudokuControl1.Matrix.GetOriginalMatrix();

                    sudokuControl1.StartGame(m);
                    Dirty = true;
                }
            );
        }

        private delegate void UIDelegate();
    }

    internal class SudokuException : ApplicationException
    {
        public SudokuException(string message) : base(message)
        {
        }
    }
}
#region Coypright and License

/*
 * AutoSolution - Copyright 2011, Svante Seleborg, All Rights Reserved
 *
 * This file is part of AutoSolution.
 *
 * AutoSolution is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * AutoSolution is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with AutoSolution.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * The source is maintained at http://autosolution.codeplex.com/ please visit for
 * updates, contributions and contact with the author. You may also visit
 * http://www.axantum.com for more information about the author.
*/

#endregion Coypright and License

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Axantum.AutoSolution.Presenter;

namespace Axantum.AutoSolution
{
    public partial class MainForm : Form, ICleanView
    {
        private Clean _cleanPresenter;

        public MainForm()
        {
            InitializeComponent();
            InitializeBackgroundWorker();

            checkBoxIncludeCs.Checked = true;
            checkBoxIncludeVb.Checked = true;
            checkBoxNoForceAuto.Checked = false;

            _cleanPresenter = new Clean(this, new Implementation.FileSystem());
            _cleanPresenter.CleanProgress += new EventHandler<CleanProgressEventArgs>(_cleanPresenter_CleanProgress);

            textBoxRootDirectory.Text = Properties.Settings.Default.DefaultRootDirectory;
            textBoxIgnoreFolders.Text = Properties.Settings.Default.DefaultIgnoreDirectories;
            textBoxSuperSolutionNameBase.Text = Properties.Settings.Default.DefaultRootSolutionNameBase;
            UpdateCleanPresenter();
        }

        private void _cleanPresenter_CleanProgress(object sender, CleanProgressEventArgs e)
        {
            AddMessageToProgress(e.ProgressMessage);
        }

        private void InitializeBackgroundWorker()
        {
            backgroundWorker1.DoWork += new DoWorkEventHandler(backgroundWorker1_DoWork);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker1_ProgressChanged);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker1_RunWorkerCompleted);
            backgroundWorker1.WorkerReportsProgress = true;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private void AddMessageToProgress(string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                lock (textBoxProgress)
                {
                    textBoxProgress.Text += message + Environment.NewLine;
                    textBoxProgress.Select(textBoxProgress.Text.Length, 0);
                    textBoxProgress.ScrollToCaret();
                    textBoxProgress.Update();
                }
            }
        }

        private void ClearProgress()
        {
            textBoxProgress.Text = String.Empty;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string msg = e.UserState as string;
            AddMessageToProgress(msg);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            DoRunGenerateSolutions();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBoxRootDirectory.Text = folderBrowserDialog1.SelectedPath;
        }

        private string RootDirectory
        {
            get { return textBoxRootDirectory.Text; }
        }

        private bool IsRootDirectoryOk
        {
            get
            {
                string rootDirectory = RootDirectory;

                if (String.IsNullOrEmpty(rootDirectory))
                {
                    return false;
                }
                if (!Directory.Exists(rootDirectory))
                {
                    return false;
                }
                return true;
            }
        }

        private void DoRunGenerateSolutions()
        {
            if (!IsRootDirectoryOk)
            {
                return;
            }

            string rootDirectory = RootDirectory;

            // Get a SolutionManager for all likely projects from the root directory and down
            SolutionGenerator.ProjectInfo.ForceAuto = !checkBoxNoForceAuto.Checked;

            SolutionGenerator sm = new SolutionGenerator(rootDirectory, IsProjectFileToBeIncluded);
            sm.ErrorReporter = FormsReporter;
            sm.ProgressReporter = FormsReporter;

            // Do the initial scan
            sm.ScanAllProjects();

            if (radioButtonRecursive.Checked)
            {             // Generate all auto projects
                sm.MakeAutoProjects(delegate(SolutionGenerator.ProjectInfo pi) { return true; });

                // Generate a solution per project for all projects
                sm.MakeAutoSolutions(delegate(SolutionGenerator.ProjectInfo pi) { return true; });

                // Generate aggregate solutions in sub-directories
                Program.MakeAggregateSolutions(sm, rootDirectory, IsProjectFileToBeIncluded);
            }
            if (radioButtonSuperSolution.Checked)
            {
                Program.MakeSuperSolution(sm, rootDirectory, IsProjectFileToBeIncluded);
            }
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void FormsReporter(string msg)
        {
            backgroundWorker1.ReportProgress(0, msg);
        }

        private IList<string> FoldersToIgnore
        {
            get
            {
                return textBoxIgnoreFolders.Text.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// Default command-line built-in rules to determine which projects are to be included.
        /// </summary>
        /// <param name="candidatePath">A path that may, or may not, be determined to be included.</param>
        /// <returns>true if the candidate path is a project that should be included.</returns>
        private bool IsProjectFileToBeIncluded(string candidatePath)
        {
            if (!Program.IsProjectFileToBeIncludedSpecialCases(candidatePath))
            {
                return false;
            }
            foreach (string folderToIgnore in FoldersToIgnore)
            {
                if (candidatePath.IndexOf(Path.DirectorySeparatorChar + folderToIgnore + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return false;
                }
            }
            if (checkBoxIncludeCs.Checked && Path.GetExtension(candidatePath).EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (checkBoxIncludeVb.Checked && Path.GetExtension(candidatePath).EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        private void buttonClean_Click(object sender, EventArgs e)
        {
            _cleanPresenter.DoClean();
        }

        private void CleanPresenterParameterChanged(object sender, EventArgs e)
        {
            UpdateCleanPresenter();
        }

        private void UpdateCleanPresenter()
        {
            _cleanPresenter.CleanBin = checkBoxBin.Checked;
            _cleanPresenter.CleanObj = checkBoxCleanObj.Checked;
            _cleanPresenter.CleanSolutions = checkBoxCleanAutoSolutions.Checked;
            _cleanPresenter.RootDirectory = textBoxRootDirectory.Text;
            _cleanPresenter.IgnoreDirectories = FoldersToIgnore;
        }

        private void textBoxRootDirectory_TextChanged(object sender, EventArgs e)
        {
            UpdateCleanPresenter();
            if (Properties.Settings.Default.DefaultRootDirectory != textBoxRootDirectory.Text)
            {
                Properties.Settings.Default.DefaultRootDirectory = textBoxRootDirectory.Text;
            }
        }

        private void textBoxIgnoreFolders_TextChanged(object sender, EventArgs e)
        {
            UpdateCleanPresenter();
            if (Properties.Settings.Default.DefaultIgnoreDirectories != textBoxIgnoreFolders.Text)
            {
                Properties.Settings.Default.DefaultIgnoreDirectories = textBoxIgnoreFolders.Text;
            }
        }

        private void textBoxSuperSolutionNameBase_TextChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.DefaultRootSolutionNameBase != textBoxSuperSolutionNameBase.Text)
            {
                Properties.Settings.Default.DefaultRootSolutionNameBase = textBoxSuperSolutionNameBase.Text;
            }
        }
    }
}
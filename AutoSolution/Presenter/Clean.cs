﻿#region Coypright and License

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
using System.IO;
using System.Linq;
using System.Text;

namespace Axantum.AutoSolution.Presenter
{
    public class Clean
    {
        private delegate void SourceControlPredicate(DirectoryInfo di, ref bool isControlled);

        private ICleanView _cleanView;
        private IFileSystem _cleanFileSystem;
        private SourceControlPredicate _isSourceControlled;

        public Clean(ICleanView cleanView, IFileSystem cleanFileSystem)
        {
            _cleanView = cleanView;
            _cleanFileSystem = cleanFileSystem;

            _backgroundWorker.DoWork += new DoWorkEventHandler(_backgroundWorker_DoWork);
            _backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(_backgroundWorker_ProgressChanged);
            _backgroundWorker.WorkerReportsProgress = true;

            _isSourceControlled += IsSubversionControlled;
            _isSourceControlled += IsCvsControlled;
            _isSourceControlled += IsTfsControlled;
            _isSourceControlled += IsToBeIgnored;
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string message = e.UserState as string;
            OnCleanProgress(message);
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            CleanDirectories(DirectoriesToClean);
            CleanFiles(FilesToClean);
        }

        public bool CleanBin { get; set; }

        public bool CleanObj { get; set; }

        public bool CleanSolutions { get; set; }

        public string RootDirectory { get; set; }

        public IList<String> IgnoreDirectories { get; set; }

        public event EventHandler<CleanProgressEventArgs> CleanProgress;

        protected virtual void OnCleanProgress(string message)
        {
            EventHandler<CleanProgressEventArgs> handler = CleanProgress;
            if (handler != null)
            {
                handler(this, new CleanProgressEventArgs(message));
            }
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

        public IList<string> DirectoriesToClean
        {
            get
            {
                List<string> patterns = new List<string>();

                if (CleanBin)
                {
                    patterns.Add(@"bin");
                }
                if (CleanObj)
                {
                    patterns.Add(@"obj");
                }

                return patterns;
            }
        }

        public IList<string> FilesToClean
        {
            get
            {
                List<string> patterns = new List<string>();

                if (CleanSolutions)
                {
                    patterns.Add("*.Auto.sln");
                    patterns.Add("*.Auto.vssscc");
                    patterns.Add("*.Auto.csproj");
                    patterns.Add("*.Auto.csproj.vspscc");
                    patterns.Add("*.Auto.vbproj");
                    patterns.Add("*.Auto.vbproj.vspscc");
                }

                return patterns;
            }
        }

        private BackgroundWorker _backgroundWorker = new BackgroundWorker();

        internal bool DoClean()
        {
            if (_backgroundWorker.IsBusy)
            {
                return false;
            }
            if (!IsRootDirectoryOk)
            {
                return false;
            }
            _backgroundWorker.RunWorkerAsync();
            return true;
        }

        private void IsSubversionControlled(DirectoryInfo di, ref bool isControlled)
        {
            if (isControlled)
            {
                return;
            }
            DirectoryInfo[] subversionMarker = _cleanFileSystem.SearchForDirectories(di, ".svn", SearchOption.TopDirectoryOnly);
            isControlled = subversionMarker.Length > 0;
            return;
        }

        private void IsCvsControlled(DirectoryInfo di, ref bool isControlled)
        {
            return;
        }

        private void IsTfsControlled(DirectoryInfo di, ref bool isControlled)
        {
            return;
        }

        private void IsToBeIgnored(DirectoryInfo di, ref bool isIgnored)
        {
            if (isIgnored)
            {
                return;
            }
            foreach (string ignoreDirectory in IgnoreDirectories)
            {
                if (di.FullName.IndexOf(ignoreDirectory, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    isIgnored = true;
                    return;
                }
            }
        }

        private void CleanDirectories(IList<string> directorypatterns)
        {
            foreach (string directorypattern in directorypatterns)
            {
                DirectoryInfo rootDirectory = new DirectoryInfo(RootDirectory);
                DirectoryInfo[] directories = _cleanFileSystem.SearchForDirectories(rootDirectory, directorypattern, SearchOption.AllDirectories);
                foreach (DirectoryInfo directory in directories)
                {
                    if (DeleteDirectoryRecursivelySelectively(directory))
                    {
                        _backgroundWorker.ReportProgress(0, directory.FullName);
                    }
                    else
                    {
                    }
                }
            }
        }

        private bool DeleteDirectoryRecursivelySelectively(DirectoryInfo directory)
        {
            bool isControlled = false;
            _isSourceControlled(directory, ref isControlled);
            if (isControlled)
            {
                return false;
            }
            if (_cleanFileSystem.IsReadOnly(directory))
            {
                return false;
            }

            DirectoryInfo[] subdirectories = _cleanFileSystem.SearchForDirectories(directory, "*", SearchOption.TopDirectoryOnly);
            foreach (DirectoryInfo subdirectory in subdirectories)
            {
                DeleteDirectoryRecursivelySelectively(subdirectory);
            }

            FileInfo[] files = _cleanFileSystem.SearchForFiles(directory, "*", SearchOption.TopDirectoryOnly);
            foreach (FileInfo file in files)
            {
                if (!_cleanFileSystem.IsReadOnly(file))
                {
                    _cleanFileSystem.DeleteFile(file);
                }
            }

            subdirectories = _cleanFileSystem.SearchForDirectories(directory, "*", SearchOption.TopDirectoryOnly);
            if (subdirectories.Length > 0)
            {
                return true;
            }

            files = _cleanFileSystem.SearchForFiles(directory, "*", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                return true;
            }
            _cleanFileSystem.DeleteDirectory(directory, false);
            return true;
        }

        private void CleanFiles(IList<string> filepatterns)
        {
            foreach (string filepattern in filepatterns)
            {
                DirectoryInfo rootDirectory = new DirectoryInfo(RootDirectory);
                FileInfo[] files = _cleanFileSystem.SearchForFiles(rootDirectory, filepattern, SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                {
                    if (_cleanFileSystem.IsReadOnly(file))
                    {
                        return;
                    }
                    if (_cleanFileSystem.DeleteFile(file))
                    {
                        _backgroundWorker.ReportProgress(0, file.FullName);
                    }
                    else
                    {
                    }
                }
            }
        }
    }
}
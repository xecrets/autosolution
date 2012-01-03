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
using System.IO;
using System.Linq;
using System.Text;
using Axantum.AutoSolution.Presenter;

namespace Axantum.AutoSolution.Implementation
{
    public class FileSystem : IFileSystem
    {
        #region ICleanFileSystem Members

        public DirectoryInfo[] SearchForDirectories(DirectoryInfo rootDirectory, string directoryPattern, SearchOption searchOption)
        {
            return rootDirectory.GetDirectories(directoryPattern, searchOption);
        }

        public FileInfo[] SearchForFiles(DirectoryInfo rootDirectory, string filePattern, SearchOption searchOption)
        {
            return rootDirectory.GetFiles(filePattern, searchOption);
        }

        public bool IsReadOnly(FileSystemInfo fileOrDirectory)
        {
            return (fileOrDirectory.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
        }

        public bool DeleteDirectory(DirectoryInfo directoryToDelete, bool recursive)
        {
            try
            {
                directoryToDelete.Delete(true);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool DeleteFile(FileInfo fileToDelete)
        {
            try
            {
                fileToDelete.Delete();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public Stream ReadFile(FileInfo fileToRead)
        {
            return fileToRead.OpenRead();
        }

        public Stream WriteFile(FileInfo fileToWrite)
        {
            return fileToWrite.Create();
        }

        #endregion ICleanFileSystem Members
    }
}
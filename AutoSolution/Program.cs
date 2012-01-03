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
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Axantum.AutoSolution
{
    internal static class Program
    {
        /// <summary>
        /// Get the AssemblyTitle attribute from the assembly meta data.
        /// </summary>
        private static string AssemblyTitle
        {
            get
            {
                AssemblyTitleAttribute ata = Attribute.GetCustomAttribute(typeof(Program).Assembly, typeof(AssemblyTitleAttribute), true) as AssemblyTitleAttribute;
                return ata.Title;
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    WindowsFormsMode();
                }
                else
                {
                    CommandLineMode(args);
                }
            }
            catch (Exception exception)
            {
                string msg = String.Format("Exception: {0} ({1})\n", exception.Message, exception.GetType().Name) + exception.StackTrace;
                MessageBox.Show(msg, String.Format("{0} Unexpected Exception", AssemblyTitle), MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// Enter interactive Windows Forms mode.
        /// </summary>
        private static void WindowsFormsMode()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Basic special cases filter for project file inclusion.
        /// </summary>
        /// <param name="candidatePath">A path that may, or may not, be determined to be included.</param>
        /// <returns>true if the candidate path is a project that should be included.</returns>
        public static bool IsProjectFileToBeIncludedSpecialCases(string candidatePath)
        {
            // Some WasaKredit specials...
            if (candidatePath.IndexOf("[obsolete]", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return false;
            }
            // ...and another
            if (candidatePath.IndexOf("[do not use]", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return false;
            }
            // A special case after conversion
            if (candidatePath.IndexOf(Path.DirectorySeparatorChar + "Backup" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Default command-line built-in rules to determine which projects are to be included.
        /// </summary>
        /// <param name="candidatePath">A path that may, or may not, be determined to be included.</param>
        /// <returns>true if the candidate path is a project that should be included.</returns>
        private static bool IsProjectFileToBeIncluded(string candidatePath)
        {
            if (!IsProjectFileToBeIncludedSpecialCases(candidatePath))
            {
                return false;
            }
            if (Path.GetExtension(candidatePath).EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (Path.GetExtension(candidatePath).EndsWith(".vbproj", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Write reports callback to the console for command line mode.
        /// </summary>
        /// <param name="msg">The message to write, not ended with new line.</param>
        private static void ConsoleReporter(string msg)
        {
            if (String.IsNullOrEmpty(msg))
            {
                return;
            }
            Console.WriteLine(msg);
        }

        /// <summary>
        /// Run in non-interactive console/command line mode.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        private static void CommandLineMode(string[] args)
        {
            if (args.Length > 1 || (args.Length == 1 && args[0].StartsWith("-")))
            {
                Usage(ConsoleReporter);
                System.Environment.Exit(1);
            }

            string rootDirectory = Path.GetFullPath(args[0]);

            // Get a SolutionManager for all likely projects from the root directory and down
            SolutionGenerator sm = new SolutionGenerator(rootDirectory, IsProjectFileToBeIncluded);
            sm.ErrorReporter = ConsoleReporter;
            sm.ProgressReporter = ConsoleReporter;

            // Do the initial scan
            sm.ScanAllProjects();

            // Generate all auto projects
            sm.MakeAutoProjects(delegate(SolutionGenerator.ProjectInfo pi) { return true; });

            // Generate a solution per project for all projects
            sm.MakeAutoSolutions(delegate(SolutionGenerator.ProjectInfo pi) { return true; });

            // Generate aggregate solutions in sub-directories
            MakeAggregateSolutions(sm, rootDirectory, IsProjectFileToBeIncluded);
        }

        /// <summary>
        /// Recursively generate aggregate solution files in directories that do not have projects in them.
        /// </summary>
        /// <param name="sm">The SolutionManager instance to use.</param>
        /// <param name="rootDirectory">The root directory to start descending into.</param>
        public static void MakeAggregateSolutions(SolutionGenerator sm, string rootDirectory, Predicate<string> isProjectFileToBeIncluded)
        {
            bool hasProject = false;
            foreach (string file in Directory.GetFiles(rootDirectory, "*.*proj"))
            {
                sm.ProgressReporter(null);

                if (isProjectFileToBeIncluded(file))
                {
                    hasProject = true;
                    break;
                }
            }
            if (!hasProject)
            {
                string prefix = rootDirectory;
                if (!prefix.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    prefix += Path.DirectorySeparatorChar;
                }
                sm.MakeAutoSolution(rootDirectory, Path.GetFileName(rootDirectory), delegate(SolutionGenerator.ProjectInfo pi) { return pi.FileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase); });
            }

            //Recurse and descend...
            foreach (string directory in Directory.GetDirectories(rootDirectory))
            {
                sm.ProgressReporter(null);

                MakeAggregateSolutions(sm, directory, isProjectFileToBeIncluded);
            }
        }

        /// <summary>
        /// Make a single super solution at the top directory
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="rootDirectory"></param>
        /// <param name="isProjetFileToBeIncluded"></param>
        public static void MakeSuperSolution(SolutionGenerator sm, string rootDirectory, Predicate<string> isProjetFileToBeIncluded)
        {
            string prefix = rootDirectory;
            if (!prefix.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                prefix += Path.DirectorySeparatorChar;
            }
            string baseName = Properties.Settings.Default.DefaultRootSolutionNameBase;
            if (String.IsNullOrEmpty(baseName))
            {
                baseName = Path.GetFileName(rootDirectory);
            }
            sm.MakeAutoSolution(rootDirectory, baseName, delegate(SolutionGenerator.ProjectInfo pi) { return pi.FileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase); });
        }

        private delegate void Reporter(string msg);

        /// <summary>
        /// Show how to run and use the program.
        /// </summary>
        /// <param name="reporter">Where to write the descriptive text.</param>
        private static void Usage(Reporter reporter)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Usage: {0} [Directory]\n", Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location));
            sb.AppendLine();
            sb.AppendLine("Without arguments starts interactive Windows mode.");
            sb.AppendLine("Directory: Find all projects in the directory and below.");
            sb.AppendLine();
            sb.AppendLine("Will generate a solution for each project found, and each directory with projects below.");

            reporter(sb.ToString());
        }
    }
}
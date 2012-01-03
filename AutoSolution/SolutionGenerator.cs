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
using System.Reflection;
using System.Text;
using System.Xml;

namespace Axantum.AutoSolution
{
    /// <summary>
    /// Search a directory hierarchy for Visual Studio projects according to given criteria,
    /// parse the project files, fixing up so all possible references within the hierarchy are
    /// project references, and generate solutions files at relevant levels that include the
    /// appropriate projects that are directly and indirectly references and the appropriate
    /// build dependencies derived from references.
    /// </summary>
    public class SolutionGenerator
    {
        /// <summary>
        /// Keep track of the state of a known project
        /// </summary>
        public class ProjectInfo
        {
            /// <summary>
            /// Gets or Sets the AssemblyName of the project
            /// </summary>
            public AssemblyName AssemblyName { get; set; }

            /// <summary>
            /// Gets or Sets the original file name of the project.
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            /// Gets or Sets the GUID of the project.
            /// </summary>
            public Guid ProjectGuid { get; set; }

            /// <summary>
            /// Gets or Sets the name of the project.
            /// </summary>
            public string ProjectName { get; set; }

            /// <summary>
            /// Gets or Sets a reference to a list of references. References may be of various types,
            /// typically AssemblyName, Guid or ProjectInfo.
            /// </summary>
            public List<object> References { get; set; }

            /// <summary>
            /// Gets or Sets a flag indicating whether this project has a (modified) auto generated project
            /// generated.
            /// </summary>
            public bool IsAuto { get; set; }

            /// <summary>
            /// Gets or Sets the suffix used for auto-generated files, it is placed just before the
            /// extension dot. The default is ".Auto". You should set this before doing anything else,
            /// otherwise inconsistent results may happen.
            /// </summary>
            public static string AutoSuffix { get; set; }

            /// <summary>
            /// Get the name used for the auto generated, fixed up version, of the project,
            /// or the original if no modification was necessary, unless ForceAuto is in effect.
            /// </summary>
            public string AutoFileName
            {
                get
                {
                    if (ForceAuto || IsAuto)
                    {
                        return FixupFileName(FileName);
                    }
                    else
                    {
                        return FileName;
                    }
                }
            }

            static ProjectInfo()
            {
                // Defaults...
                AutoSuffix = ".Auto";
                ForceAuto = true;
            }

            public ProjectInfo()
            {
                ProjectGuid = Guid.Empty;
                References = new List<object>();
            }

            /// <summary>
            /// Gets and sets a flag indicating if we'll always generate new project files, even if
            /// no changes are necessary. The default is true.
            /// </summary>
            public static bool ForceAuto { get; set; }

            /// <summary>
            /// Make a version of the file name that indicates that we have auto-generated it.
            /// </summary>
            /// <param name="fileName">The original file name.</param>
            /// <param name="extension">The extension to use.</param>
            /// <returns>A modified file name to use a the target for auto-generation.</returns>
            public static string FixupFileName(string fileName, string extension)
            {
                return Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + AutoSuffix + extension);
            }

            /// <summary>
            /// Make a version of the file name that indicates that we have auto-generated it.
            /// </summary>
            /// <param name="fileName">The original file name.</param>
            /// <returns>A modified filename to use as the target for auto-generation, with the same extension as the original.</returns>
            public static string FixupFileName(string fileName)
            {
                return FixupFileName(fileName, Path.GetExtension(fileName));
            }
        }

        private Dictionary<Guid, ProjectInfo> _projectsByGuid = new Dictionary<Guid, ProjectInfo>();
        private Dictionary<string, ProjectInfo> _projectsByAssemblySimpleName = new Dictionary<string, ProjectInfo>(StringComparer.OrdinalIgnoreCase);
        private string _rootDirectory;
        private Predicate<string> _isProjectIncluded;

        public SolutionGenerator(string rootDirectory, Predicate<string> isProjectIncluded)
        {
            _rootDirectory = rootDirectory;
            _isProjectIncluded = isProjectIncluded;

            // Default report to empty air
            ProgressReporter = delegate(string msg) { };
            ErrorReporter = delegate(string msg) { };
        }

        /// <summary>
        /// Do the reading and scanning of all projects. Must be done before generating anything.
        /// </summary>
        public void ScanAllProjects()
        {
            FindAllProjects(_rootDirectory, _isProjectIncluded);
            ResolveProjectGuids();
        }

        private List<ProjectInfo> _projects = new List<ProjectInfo>();

        public IEnumerable<ProjectInfo> Projects
        {
            get { return _projects; }
        }

        /// <summary>
        /// Accept a report of progress or errors etc
        /// </summary>
        /// <param name="msg">A message, possibly multi-line, but not ended with new line.</param>
        public delegate void Reporter(string msg);

        /// <summary>
        /// Accept progress reports here
        /// </summary>
        public Reporter ProgressReporter { get; set; }

        /// <summary>
        /// Accept error reports here
        /// </summary>
        public Reporter ErrorReporter { get; set; }

        /// <summary>
        /// Recursively get all directly and indirectly referenced projects
        /// </summary>
        /// <param name="project">The project to get references for</param>
        /// <param name="relatedProjects">The list to add references to</param>
        /// <returns>The resulting full list of references</returns>
        private List<ProjectInfo> GetAllReferencedProjects(ProjectInfo project, List<ProjectInfo> relatedProjects)
        {
            if (project.ProjectGuid == Guid.Empty)
            {
                return relatedProjects;
            }
            foreach (object o in project.References)
            {
                ProgressReporter(null);

                ProjectInfo related = o as ProjectInfo;
                if (related == null)
                {
                    continue;
                }
                if (!relatedProjects.Contains(related))
                {
                    relatedProjects.Add(related);
                    GetAllReferencedProjects(related, relatedProjects);
                }
            }
            return relatedProjects;
        }

        /// <summary>
        /// Create an Automatic project file for the given project
        /// </summary>
        /// <param name="project">The project to check and fix dependencies in.</param>
        private void MakeAutoProject(ProjectInfo project)
        {
            XmlDocument xmlProject = new XmlDocument();
            XmlNamespaceManager xnm = new XmlNamespaceManager(xmlProject.NameTable);
            xnm.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003");

            xmlProject.Load(project.FileName);

            List<ProjectInfo> referenceList = new List<ProjectInfo>();

            XmlNodeList assemblyReferences = xmlProject.SelectNodes("//msbuild:ItemGroup/msbuild:Reference", xnm);
            foreach (XmlNode assemblyReference in assemblyReferences)
            {
                ProgressReporter(null);

                AssemblyName referencedAssemblyName = new AssemblyName(assemblyReference.Attributes["Include"].Value);
                ProjectInfo reference;
                if (_projectsByAssemblySimpleName.TryGetValue(referencedAssemblyName.Name, out reference))
                {
                    XmlNode parent = assemblyReference.ParentNode;
                    parent.RemoveChild(assemblyReference);
                    referenceList.Add(reference);
                }
            }

            // If we have found any assembly references that should be converted to project references, find or create
            // an appropriate ItemGroup and place them there instead.
            if (referenceList.Count == 0 && !ProjectInfo.ForceAuto)
            {
                return;
            }

            XmlNode itemGroup = xmlProject.SelectSingleNode("//msbuild:ItemGroup/msbuild:ProjectReference", xnm);
            if (itemGroup != null)
            {
                itemGroup = itemGroup.ParentNode;
            }
            else
            {
                itemGroup = xmlProject.SelectSingleNode("//msbuild:ItemGroup/msbuild:Reference", xnm);
                if (itemGroup != null)
                {
                    itemGroup = itemGroup.ParentNode;
                }
                if (itemGroup == null)
                {
                    XmlNodeList itemGroups = xmlProject.SelectNodes("//msbuild:ItemGroup", xnm);
                    itemGroup = itemGroups[itemGroups.Count - 1];
                }
                itemGroup = itemGroup.ParentNode.InsertAfter(xmlProject.CreateElement("ItemGroup", xmlProject.DocumentElement.NamespaceURI), itemGroup);
            }
            foreach (ProjectInfo pi in referenceList)
            {
                ProgressReporter(null);

                itemGroup.AppendChild(CreateProjectReferenceElement(xmlProject, project, pi));
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            settings.CloseOutput = true;

            project.IsAuto = true;
            using (XmlWriter writer = XmlWriter.Create(project.AutoFileName, settings))
            {
                xmlProject.Save(writer);
            }

            ProgressReporter(String.Format("Generated project file {0}.", EvaluateRelativePath(_rootDirectory, project.AutoFileName)));
        }

        /// <summary>
        /// Make projects with assembly references converted to project references where possible
        /// </summary>
        /// <param name="isProjectIncluded">A predicate callback which must return true to have the project processed.</param>
        public void MakeAutoProjects(Predicate<ProjectInfo> isProjectIncluded)
        {
            foreach (ProjectInfo project in Projects)
            {
                ProgressReporter(null);

                if (isProjectIncluded(project))
                {
                    MakeAutoProject(project);
                }
            }
        }

        private void AddProjectDependencies(List<ProjectInfo> dependencies, ProjectInfo project)
        {
            foreach (object r in project.References)
            {
                ProjectInfo pi = r as ProjectInfo;
                if (pi == null)
                {
                    continue;
                }
                if (dependencies.Contains(pi))
                {
                    continue;
                }
                dependencies.Add(pi);
                AddProjectDependencies(dependencies, pi);
            }
        }

        /// <summary>
        /// Make an automatic solution
        /// </summary>
        /// <param name="directory">The directory to place the .sln file in</param>
        /// <param name="originalNameBase">The name upon which to base the solution name, typically a directory or project name. It should *not* include an extension.</param>
        /// <param name="isProjectIncluded">A predicate to indicate which projects should get a solution generated. Dependencies will be included automatically.</param>
        public void MakeAutoSolution(string directory, string originalNameBase, Predicate<ProjectInfo> isProjectIncluded)
        {
            List<ProjectInfo> projectsAlreadyInSolution = new List<ProjectInfo>();

            foreach (ProjectInfo project in Projects)
            {
                ProgressReporter(null);

                if (!isProjectIncluded(project))
                {
                    continue;
                }

                if (projectsAlreadyInSolution.Contains(project))
                {
                    continue;
                }
                AddProjectDependencies(projectsAlreadyInSolution, project);
                if (projectsAlreadyInSolution.Contains(project))
                {
                    continue;
                }
                projectsAlreadyInSolution.Add(project);
            }

            if (projectsAlreadyInSolution.Count == 0)
            {
                return;
            }

            string fileName = ProjectInfo.FixupFileName(originalNameBase + ".sln");
            string solutionFileName = Path.Combine(directory, fileName);
            using (StreamWriter writer = new StreamWriter(solutionFileName, false, Encoding.UTF8))
            {
                writer.WriteLine();
                writer.WriteLine("Microsoft Visual Studio Solution File, Format Version 11.00");
                writer.WriteLine("# Visual Studio 2010");
                foreach (ProjectInfo project in projectsAlreadyInSolution)
                {
                    AddProjectToSolution(writer, directory, project.References, project);
                }
                writer.WriteLine("Global");
                writer.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
                writer.WriteLine("\t\tDebug|Any CPU = Debug|Any CPU");
                writer.WriteLine("\t\tRelease|Any CPU = Release|Any CPU");
                writer.WriteLine("\tEndGlobalSection");
                writer.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
                foreach (ProjectInfo pi in projectsAlreadyInSolution)
                {
                    writer.WriteLine(String.Format("\t\t{0}.Debug|Any CPU.ActiveCfg = Debug|Any CPU", pi.ProjectGuid.ToString("B").ToUpper()));
                    writer.WriteLine(String.Format("\t\t{0}.Debug|Any CPU.Build.0 = Debug|Any CPU", pi.ProjectGuid.ToString("B").ToUpper()));
                    writer.WriteLine(String.Format("\t\t{0}.Release|Any CPU.ActiveCfg = Release|Any CPU", pi.ProjectGuid.ToString("B").ToUpper()));
                    writer.WriteLine(String.Format("\t\t{0}.Release|Any CPU.Build.0 = Release|Any CPU", pi.ProjectGuid.ToString("B").ToUpper()));
                }
                writer.WriteLine("\tEndGlobalSection");
                writer.WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
                writer.WriteLine("\t\tHideSolutionNode = FALSE");
                writer.WriteLine("\tEndGlobalSection");
                writer.WriteLine("EndGlobal");
            }
            ProgressReporter(String.Format("Generated solution file {0}", EvaluateRelativePath(_rootDirectory, solutionFileName)));
        }

        /// <summary>
        /// Make automatic solutions for those projects that are included
        /// </summary>
        /// <param name="isProjectIncluded">A predicate to indicate which projects should get a solution generated.</param>
        public void MakeAutoSolutions(Predicate<ProjectInfo> isProjectIncluded)
        {
            foreach (ProjectInfo project in _projects)
            {
                ProgressReporter(null);

                if (isProjectIncluded(project))
                {
                    MakeAutoSolution(Path.GetDirectoryName(project.FileName), Path.GetFileNameWithoutExtension(project.FileName), delegate(ProjectInfo pi) { return pi == project; });
                }
            }
        }

        /// <summary>
        /// Helper to write a project section to a solution. It defines the project, and includes all known project references.
        /// </summary>
        /// <param name="writer">The writer where the result is written.</param>
        /// <param name="basePath">The base path of the project file, reference paths are relative to this</param>
        /// <param name="solutionProjects">All projects in the solution, i.e. the list of possible project references.</param>
        /// <param name="project">The project to add to the solution.</param>
        private void AddProjectToSolution(StreamWriter writer, string basePath, List<object> solutionProjects, ProjectInfo project)
        {
            string fileName = project.IsAuto ? project.AutoFileName : project.FileName;
            writer.WriteLine(@"Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = """ + project.ProjectName + @""", """ + EvaluateRelativePath(basePath, fileName) + @""", """ + project.ProjectGuid.ToString("B").ToUpper() + @"""");
            bool hasDependencies = false;
            foreach (object o in project.References)
            {
                ProgressReporter(null);

                ProjectInfo reference = o as ProjectInfo;
                if (reference == null)
                {
                    continue;
                }
                if (solutionProjects.Contains(reference))
                {
                    if (!hasDependencies)
                    {
                        writer.WriteLine("\tProjectSection(ProjectDependencies) = postProject");
                    }
                    writer.WriteLine(String.Format("\t\t{0} = {0}", reference.ProjectGuid.ToString("B").ToUpper()));
                    hasDependencies = true;
                }
            }
            if (hasDependencies)
            {
                writer.WriteLine("\tEndProjectSection");
            }
            writer.WriteLine("EndProject");
        }

        /// <summary>
        /// Helper to create a ProjectReference element in the project XML DOM.
        /// </summary>
        /// <param name="xmlProject">The project XML DOM.</param>
        /// <param name="project">The project this XML DOM refers to.</param>
        /// <param name="reference">The project reference to add.</param>
        /// <returns>A ProjectReference node that can be added to the XML DOM.</returns>
        private static XmlNode CreateProjectReferenceElement(XmlDocument xmlProject, ProjectInfo project, ProjectInfo reference)
        {
            XmlElement projectReferenceElement = xmlProject.CreateElement("ProjectReference", xmlProject.DocumentElement.NamespaceURI);

            XmlAttribute includeAttribute = xmlProject.CreateAttribute("Include");
            includeAttribute.Value = EvaluateRelativePath(project.FileName, reference.AutoFileName);
            projectReferenceElement.Attributes.Append(includeAttribute);

            XmlElement projectGuid = xmlProject.CreateElement("Project", xmlProject.DocumentElement.NamespaceURI);
            projectGuid.InnerText = reference.ProjectGuid.ToString("B").ToUpper();
            projectReferenceElement.AppendChild(projectGuid);

            XmlElement projectName = xmlProject.CreateElement("Name", xmlProject.DocumentElement.NamespaceURI);
            projectName.InnerText = reference.ProjectName;
            projectReferenceElement.AppendChild(projectName);

            return projectReferenceElement;
        }

        /// <summary>
        /// Recursively descend and find all projects
        /// </summary>
        /// <param name="folder">The folder to start descending into to find project files.</param>
        /// <param name="isProjectIncluded">A predicate delegate that should return true to have the project included.</param>
        private void FindAllProjects(string folder, Predicate<string> isProjectIncluded)
        {
            foreach (string file in Directory.GetFiles(folder, "*.*proj"))
            {
                ProgressReporter(null);

                if (Path.GetFileNameWithoutExtension(file).EndsWith(ProjectInfo.AutoSuffix))
                {
                    continue;
                }

                if (isProjectIncluded(file))
                {
                    ProjectInfo info = ExtractProjectInfo(folder, file);
                    if (info == null)
                    {
                        continue;
                    }

                    ProgressReporter(String.Format("Found project file {0}", EvaluateRelativePath(_rootDirectory, info.FileName)));
                    _projects.Add(info);

                    if (_projectsByGuid.ContainsKey(info.ProjectGuid))
                    {
                        string msg = String.Format("The same project GUID is defined by {0} and {1}.", _projectsByGuid[info.ProjectGuid].FileName, info.FileName);
                        ErrorReporter(msg);
                    }
                    else
                    {
                        _projectsByGuid.Add(info.ProjectGuid, info);
                    }

                    if (_projectsByAssemblySimpleName.ContainsKey(info.AssemblyName.Name))
                    {
                        string msg = String.Format("The same simple assembly name is defined by {0} and {1}.", _projectsByAssemblySimpleName[info.AssemblyName.Name].FileName, info.FileName);
                        ErrorReporter(msg);
                    }
                    else
                    {
                        _projectsByAssemblySimpleName.Add(info.AssemblyName.Name, info);
                    }
                }
            }
            foreach (string str2 in Directory.GetDirectories(folder, "*.*"))
            {
                FindAllProjects(Path.Combine(folder, str2), isProjectIncluded);
            }
        }

        /// <summary>
        /// Helper to interpret a project build file, and extract the relevant info from it.
        /// </summary>
        /// <param name="folder">The folder where the project is.</param>
        /// <param name="file">The name of the project file.</param>
        /// <remarks>If the file is one generated by us, it is ignored.</remarks>
        private static ProjectInfo ExtractProjectInfo(string folder, string file)
        {
            if (file.IndexOf(ProjectInfo.AutoSuffix + ".", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return null;
            }

            ProjectInfo info = new ProjectInfo();
            info.ProjectName = Path.GetFileNameWithoutExtension(file);
            info.FileName = Path.Combine(folder, file);

            XmlDocument project = new XmlDocument();
            XmlNamespaceManager xnm = new XmlNamespaceManager(project.NameTable);
            xnm.AddNamespace("msbuild", "http://schemas.microsoft.com/developer/msbuild/2003");

            project.Load(info.FileName);

            XmlNode projectGuid = project.SelectSingleNode("msbuild:Project/msbuild:PropertyGroup/msbuild:ProjectGuid", xnm);
            if (projectGuid == null)
            {
                return null;
            }
            info.ProjectGuid = new Guid(projectGuid.InnerText);

            XmlNode assemblyName = project.SelectSingleNode("//msbuild:Project/msbuild:PropertyGroup/msbuild:AssemblyName", xnm);
            if (assemblyName == null)
            {
                return null;
            }
            info.AssemblyName = new AssemblyName(assemblyName.InnerText);

            XmlNodeList projectReferences = project.SelectNodes("//msbuild:ItemGroup/msbuild:ProjectReference", xnm);
            foreach (XmlNode projectReference in projectReferences)
            {
                XmlNode referenceGuid = projectReference.SelectSingleNode("msbuild:Project", xnm);
                if (referenceGuid != null)
                {
                    info.References.Add(new Guid(referenceGuid.InnerText));
                }
            }

            XmlNodeList assemblyReferences = project.SelectNodes("//msbuild:ItemGroup/msbuild:Reference", xnm);
            foreach (XmlNode assemblyReference in assemblyReferences)
            {
                string referencedAssemblyName = assemblyReference.Attributes["Include"].Value;
                if (!String.IsNullOrEmpty(referencedAssemblyName))
                {
                    info.References.Add(new AssemblyName(referencedAssemblyName));
                }
            }
            return info;
        }

        /// <summary>
        /// Check all project references, and where possible, resolve them to refer to projects
        /// that we have found, and can thus use for project references.
        /// </summary>
        private void ResolveProjectGuids()
        {
            foreach (ProjectInfo info in _projects)
            {
                ProgressReporter(null);

                for (int i = 0; i < info.References.Count; ++i)
                {
                    object o = info.References[i];
                    if (o is ProjectInfo)
                    {
                        continue;
                    }
                    if (o is Guid)
                    {
                        ProjectInfo reference;
                        if (_projectsByGuid.TryGetValue((Guid)o, out reference))
                        {
                            info.References[i] = reference;
                        }
                        continue;
                    }
                    if (o is AssemblyName)
                    {
                        ProjectInfo reference;
                        if (_projectsByAssemblySimpleName.TryGetValue(((AssemblyName)o).Name, out reference))
                        {
                            info.References[i] = reference;
                        }
                        continue;
                    }
                    throw new InvalidProgramException("Unexpected type in reference list");
                }
            }
        }

        /// <summary>
        /// Convert an absolute file path to a relative path, relative to the given base directory.
        /// </summary>
        /// <param name="basePath">The base directory to use.</param>
        /// <param name="absolutePath">The absolute path to a file to make relative, if possible.</param>
        /// <returns>The relative path if possible, otherwise the original absolute path.</returns>
        private static string EvaluateRelativePath(string baseDirectoryPath, string absoluteFilePath)
        {
            string[] firstPathParts = baseDirectoryPath.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            string[] secondPathParts = absoluteFilePath.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);

            int maxCompareCount = Math.Min(firstPathParts.Length, secondPathParts.Length);
            int sameCounter = 0;
            while (sameCounter < maxCompareCount)
            {
                if (String.Compare(firstPathParts[sameCounter], secondPathParts[sameCounter], StringComparison.OrdinalIgnoreCase) != 0)
                {
                    break;
                }
                ++sameCounter;
            }
            if (sameCounter == 0)
            {
                return absoluteFilePath;
            }
            StringBuilder newPath = new StringBuilder(absoluteFilePath.Length);
            for (int i = sameCounter; i < firstPathParts.Length; i++)
            {
                if (i > sameCounter)
                {
                    newPath.Append(Path.DirectorySeparatorChar);
                }
                newPath.Append("..");
            }
            for (int i = sameCounter; i < secondPathParts.Length; i++)
            {
                newPath.Append(Path.DirectorySeparatorChar).Append(secondPathParts[i]);
            }

            // If the relative path starts at the base path, remove the leading DirectorySeparatorChar
            if (newPath[0] == Path.DirectorySeparatorChar)
            {
                return newPath.ToString(1, newPath.Length - 1);
            }
            return newPath.ToString();
        }
    }
}
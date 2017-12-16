# autosolution

Project Description

Generate Visual Studio solution files and fixup project files to use project references. This tool makes it easy to maintain correct dependencies in projects and Visual Studio solutions. It's also great to generate large refactoring solutions with all projects in many directories

How to Use

AutoSolution is contained in a single executable that can be run in a simple command line mode, or interactively as a windows desktop application.

It can generate project files with project references and solution files with all the required projects by recursively descending into a directory structure.

All generated files have a '.Auto' pre-suffix, i.e. project files are named *.Auto.??proj and solutions are named *.Auto.sln .

Interactive mode will remember the user choices made between sessions, and there are also a utility feature to fully clean 'bin' and 'obj' folders as well as cleaning generated project and solution files.

You can choose to generate one single root solution of everything, or recursively generate a solution in each folder where a project file is found, that solution will then build that project along with all it's dependencies.

The solutions generated include direct and indirect references.

The singe root solution is extremely useful for refactoring larger code bases.

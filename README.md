Solution Dependency Builder
===========================

To develop my DVB.NET (et al) projects I like to be able to compile each assembly on it's own - especially
during debugging session. So each assembly will compile to a dedicated drop directory where other projects
can reference it. If a single debug session includes multiple assemblies using this approach you will have
to explicitly add project dependencies to get a proper compilation - especially if multi-core build is
enabled.

Although this is not a big problem during development there is one task where project dependencies are really
a pain: for DVB.NET there is a build solution which includes all (currently 28) projects with quite a bit
of book keeping to get the solution build to succeed. 

This tiny tool analyses the projects (C#, only experimental support for C++) in a Visual Studio 2012 solution
and tries to (re-)build the project dependencies based on the project assembly output name and the assembly
references found in the project files. Concerning my personal requirements it works quite well.

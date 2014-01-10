using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Beschreibt die Verwaltung einer Art von Projekt.
    /// </summary>
    public abstract class ProjectTypeHandler
    {
        /// <summary>
        /// Die Rohdaten.
        /// </summary>
        protected readonly XmlDocument Document = new XmlDocument();

        /// <summary>
        /// Der zu verwendende Namensraum.
        /// </summary>
        protected readonly XmlNamespaceManager Namespaces;

        /// <summary>
        /// Der volle Pfad zur Projektdatei.
        /// </summary>
        public FileInfo FilePath { get; private set; }

        /// <summary>
        /// Der XPath zum Namen der Assembly.
        /// </summary>
        protected abstract string TargetNamePath { get; }

        /// <summary>
        /// Der Name der Zieldatei.
        /// </summary>
        public string AssemblyName { get; private set; }

        /// <summary>
        /// Die Liste aller Abhängigkeiten.
        /// </summary>
        public HashSet<Guid> Dependencies { get; private set; }

        /// <summary>
        /// Die Liste aller Abhängigkeiten, iterativ aufgelöst.
        /// </summary>
        public HashSet<Guid> DeepDependencies { get; private set; }

        /// <summary>
        /// Initialisiert eine Verwaltung.
        /// </summary>
        /// <param name="path">Der volle Pfad zur Projektdatei.</param>
        protected ProjectTypeHandler( FileInfo path = null )
        {
            // Remember
            FilePath = path;

            // Attach to the table
            Namespaces = new XmlNamespaceManager( Document.NameTable );

            // Create the default name space
            Namespaces.AddNamespace( "msbuild", "http://schemas.microsoft.com/developer/msbuild/2003" );

            // None
            if (path == null)
            {
                // Fake name
                AssemblyName = Guid.NewGuid().ToString( "N" ).ToUpper();

                // Done
                return;
            }

            // Load
            Document.Load( FilePath.FullName );

            // Read the name of the target
            var assemblyName = Document.SelectSingleNode( TargetNamePath, Namespaces );
            if (assemblyName != null)
                AssemblyName = assemblyName.InnerText;

            // Validate
            if (string.IsNullOrEmpty( AssemblyName ))
                AssemblyName = Path.GetFileNameWithoutExtension( path.FullName );
        }

        /// <summary>
        /// Ermittelt alle Referenzen.
        /// </summary>
        protected virtual IEnumerable<string> References
        {
            get
            {
                // Process
                return
                    Document
                        .SelectNodes( "msbuild:Project/msbuild:ItemGroup/msbuild:Reference[@Include]", Namespaces )
                        .Cast<XmlElement>()
                        .Select( element => new AssemblyName( element.GetAttribute( "Include" ) ) )
                        .Select( name => name.Name );
            }
        }

        /// <summary>
        /// Löst alle Referenzen auf.
        /// </summary>
        /// <param name="projects">Alle bekannten Projekte.</param>
        public void Resolve( Dictionary<string, ProjectSection> projects )
        {
            // Create new
            Dependencies = new HashSet<Guid>();

            // Load references
            foreach (var reference in References)
            {
                // Look it up
                ProjectSection project;
                if (projects.TryGetValue( reference, out project ))
                    Dependencies.Add( project.UniqueIdentifier );
            }
        }

        /// <summary>
        /// Löst alle Referenzen in die Tiefe auf.
        /// </summary>
        /// <param name="projectLookup">Die Liste aller Projekte.</param>
        /// <returns>Die iterativ aufgelöste Liste der Abhängigkeiten.</returns>
        public IEnumerable<Guid> DeepResolve( Dictionary<Guid, ProjectSection> projectLookup )
        {
            // Create once
            if (DeepDependencies == null)
            {
                // Initial direct
                DeepDependencies = new HashSet<Guid>( Dependencies );

                // Resolve all
                foreach (var projectIdentifier in Dependencies)
                {
                    // Do the resolve
                    var dependencies = projectLookup[projectIdentifier].GetDeepDependencies( projectLookup );

                    // Merge with us
                    DeepDependencies.UnionWith( dependencies );
                }
            }

            // Report
            return DeepDependencies;
        }
    }
}

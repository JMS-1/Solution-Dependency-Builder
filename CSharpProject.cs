using System;
using System.IO;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Beschreibt eine C# Projektdatei.
    /// </summary>
    public class CSharpProject : ProjectTypeHandler
    {
        /// <summary>
        /// Initialisiert eine Verwaltung.
        /// </summary>
        /// <param name="path">Der volle Pfad zur Projektdatei.</param>
        public CSharpProject( FileInfo path )
            : base( path )
        {
        }
    }
}

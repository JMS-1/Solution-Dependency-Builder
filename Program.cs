using System;
using System.Collections.Generic;
using System.Linq;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Repräsentiert den Windows Prozess als Ganzes.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Gesetzt, wenn die Protokollierung aktiviert ist.
        /// </summary>
        public static bool Logging { get; private set; }

        /// <summary>
        /// Startet die Anwendung.
        /// </summary>
        /// <param name="args">Die Liste der Befehlszeilenparameter.</param>
        public static void Main( string[] args )
        {
            // Create command map
            var map = new HashSet<string>( args, StringComparer.InvariantCultureIgnoreCase );

            // Check flags
            Logging = map.Contains( "/log" );

            // Process files
            foreach (var path in map.Where( arg => arg.StartsWith( "/solution=", StringComparison.InvariantCultureIgnoreCase ) ).Select( arg => arg.Substring( 10 ) ))
                Process( path );
        }

        /// <summary>
        /// Bearbeitet eine einzelne Solution.
        /// </summary>
        /// <param name="path">Der volle Pfad zur Solution.</param>
        private static void Process( string path )
        {
            // Load the solution file
            var solution = new SolutionFile( path );
        }
    }
}

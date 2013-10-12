using System.Collections.Generic;


namespace JMS.Tools.SolutionUpdater
{
    /// <summary>
    /// Ein einzelner Bereich aus einer Solutiondatei.
    /// </summary>
    public abstract class SolutionFileSection
    {
        /// <summary>
        /// Ergänzt eine weitere Zeile.
        /// </summary>
        /// <param name="line">Die gewünschte Zeile.</param>
        /// <returns>Gesetzt, wenn weitere Zeilen aufgenommen werden können.</returns>
        public abstract bool Extend( string line );

        /// <summary>
        /// Rekonstruiert den Inhalt.
        /// </summary>
        /// <returns>Der hier verwaltete Inhalt.</returns>
        public abstract IEnumerable<string> Reconstruct();
    }
}

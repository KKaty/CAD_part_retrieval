namespace SolidWorksAddinUtility
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;

    public partial class Utility
    {
        /// <summary>
        /// The path.
        /// </summary>
       // private const string LogPath = "C:\\Users\\SoullessPG\\Desktop\\Debug\\";
        private const string LogPath = " C:\\Users\\Katia\\Desktop\\Debug\\";

        /// <summary>
        /// The print to file.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="fileName">
        /// The file Name.
        /// </param>
        public static void PrintToFile(string text, string fileName = "log.txt")
        {
            using (StreamWriter w = File.AppendText(LogPath + fileName))
            {
                //StackTrace stackTrace = new StackTrace();
                //string stackString = stackTrace.GetFrame(1).GetFileLineNumber() + " "
                  //                   + stackTrace.GetFrame(1).GetMethod();
                w.WriteLine(text);
            }
        }

        /// <summary>
        /// The delete debug files.
        /// </summary>
        public static void DeleteDebugFiles()
        {
            var filepaths = Directory.GetFiles(LogPath);
            foreach (var filePath in filepaths)
            {
                File.Delete(filePath);
            }
        }
    }
}

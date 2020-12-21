﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.InputFiles.Classes
{
    /// <summary>
    /// Manages <see cref="InputFile"/>s.
    /// </summary>
    public static class InputFileManager
    {
        public static List<InputFile> InputFiles { get; private set; } = new List<InputFile>();

        public static void AddInputFile(InputFile inputFile) => InputFiles.Add(inputFile);

        public static void RemoveInputFile(InputFile inputFile) => InputFiles.Remove(inputFile);

        public static DriverlessInputFile FirstDriverlessInputFile => (DriverlessInputFile)InputFiles.FirstOrDefault(x => x is DriverlessInputFile);
       // public static void RemoveInputFile(string fileName) => InputFiles.Remove(InputFiles.Find(x => x.FileName.Equals(fileName)));

     /*   public static InputFile GetInputFile(string fileName, string driverName)
            => InputFiles.Find(x => x.Name.Equals(fileName) && x.DriverName.Equals(driverName));*/
    }
}

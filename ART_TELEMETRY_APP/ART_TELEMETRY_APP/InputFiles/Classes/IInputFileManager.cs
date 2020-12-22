using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ART_TELEMETRY_APP.InputFiles.Classes
{
    public interface IInputFileManager <T>
    {
        List<T> InputFiles { get; }

        void AddInputFile(T inputFile);

        void RemoveInputFile(string inputFileName);

        T GetInputFile(string inputFileName);

    }
}

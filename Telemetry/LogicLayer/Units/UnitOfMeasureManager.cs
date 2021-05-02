using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using DataLayer.Units;
using PresentationLayer.Texts;
using LogicLayer.Extensions;

namespace PresentationLayer.Units
{
    public static class UnitOfMeasureManager
    {
        public static List<Unit> UnitOfMeasures { get; private set; } = new List<Unit>();

        /// <summary>
        /// Initializes unit of measures from file.
        /// </summary>
        /// <param name="fileName">File name that contains the unit of measures.</param>
        public static void InitializeUnitOfMeasures(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new Exception($"Couldn't load unit of measures, because file '{fileName}' doesn't found!");
            }

            if (new FileInfo(fileName).Length == 0)
            {
                throw new Exception($"'{fileName}' is empty");
            }

            ReadUnitOfMeasures(fileName);
        }

        private static void ReadUnitOfMeasures(string fileName)
        {
            using var reader = new StreamReader(fileName);

            try
            {
                dynamic unitOfMeasuresJSON = JsonConvert.DeserializeObject(reader.ReadToEnd());

                for (int index = 0; index < unitOfMeasuresJSON.Count; index++)
                {
                    if (unitOfMeasuresJSON[index].Name == null)
                    {
                        throw new Exception("Can't add unit of measure, because 'name' is null!");
                    }

                    if (unitOfMeasuresJSON[index].Name.ToString().Equals(string.Empty))
                    {
                        throw new Exception("Can't add unit of measure, because 'name' is empty!");
                    }

                    if (unitOfMeasuresJSON[index].Description == null)
                    {
                        throw new Exception("Can't add unit of measure, because 'description' is null!");
                    }

                    if (unitOfMeasuresJSON[index].UnitOfMeasure == null)
                    {
                        throw new Exception("Can't add unit of measure, because 'unit of measure' is null!");
                    }

                    if (unitOfMeasuresJSON[index].UnitOfMeasure.ToString().Equals(string.Empty))
                    {
                        throw new Exception("Can't add unit of measure, because 'unit of measure' is empty!");
                    }

                    AddUnitOfMeasure(new Unit(index,
                                              unitOfMeasuresJSON[index].Name.ToString(),
                                              unitOfMeasuresJSON[index].Description.ToString(),
                                              unitOfMeasuresJSON[index].UnitOfMeasure.ToString()));
                }
            }
            catch (JsonReaderException)
            {
                throw new Exception($"There was a problem reading '{TextManager.UnitOfMeasuresFileName}'");
            }
        }

        public static void Save()
        {
            string fileName = TextManager.UnitOfMeasuresFileName.MakePath(TextManager.DefaultFilesFolderName);
            if (!File.Exists(fileName))
            {
                throw new Exception($"Can't save unit of measures because '{fileName}' does not found!");
            }

            using var writer = new StreamWriter(fileName);

            var serializer = new JsonSerializer();

            try
            {
                serializer.Serialize(writer, UnitOfMeasures);
            }
            catch (Exception)
            {
                throw new Exception("Can't save file!");
            }
        }

        public static void AddUnitOfMeasure(Unit unit) => UnitOfMeasures.Add(unit);

        public static Unit GetUnitOfMeasure(int id) => UnitOfMeasures.Find(x => x.ID == id);
        public static Unit GetUnitOfMeasure(string name) => UnitOfMeasures.Find(x => x.Name.Equals(name));

        public static void RemoveUnitOfMeasure(int id) => UnitOfMeasures.Remove(GetUnitOfMeasure(id));

        public static void ChangeUnitOfMeasureName(int id, string newName) => UnitOfMeasures.Find(x => x.ID == id).Name = newName;
        public static void ChangeUnitOfMeasureDescription(int id, string newDescription) => UnitOfMeasures.Find(x => x.ID == id).Description = newDescription;
        public static void ChangeUnitOfMeasureFormula(int id, string newFormula) => UnitOfMeasures.Find(x => x.ID == id).UnitOfMeasure = newFormula;
    }
}

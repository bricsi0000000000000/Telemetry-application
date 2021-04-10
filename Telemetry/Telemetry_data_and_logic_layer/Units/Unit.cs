using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer.Units
{
    public class Unit
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string UnitOfMeasure { get; set; }

        public Unit(int id, string name, string description, string unitOfMeasure)
        {
            ID = id;
            Name = name;
            Description = description;
            UnitOfMeasure = unitOfMeasure;
        }
    }
}

namespace DataLayer.Models
{
    abstract public class Sensor<ValueType>
    {
        public int ID { get; set; }

        virtual public ValueType Value { get; set; }

        public int PackageID { get; set; }
    }
}

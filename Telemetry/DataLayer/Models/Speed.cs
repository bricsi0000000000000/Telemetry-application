namespace DataLayer.Models
{
    public class Speed : Sensor<int>
    {
        public override int Value { get => base.Value; set => base.Value = value; }
    }
}

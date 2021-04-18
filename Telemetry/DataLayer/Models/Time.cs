namespace DataLayer.Models
{
    public class Time : Sensor<float>
    {
        public override float Value { get => base.Value; set => base.Value = value; }
    }
}

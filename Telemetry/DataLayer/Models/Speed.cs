namespace DataLayer.Models
{
    public class Speed : Sensor<float>
    {
        public override float Value { get => base.Value; set => base.Value = value; }
    }
}

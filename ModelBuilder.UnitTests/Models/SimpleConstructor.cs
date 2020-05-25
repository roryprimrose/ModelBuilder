namespace ModelBuilder.UnitTests.Models
{
    public class SimpleConstructor
    {
        public SimpleConstructor(SlimModel? model)
        {
            Model = model;
        }

        public int Age { get; set; }

        public SlimModel? Model { get; set; }
    }
}
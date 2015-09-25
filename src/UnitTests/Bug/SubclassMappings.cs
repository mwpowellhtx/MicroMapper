using Xunit;

namespace MicroMapper.UnitTests.Bug
{
    public class SubclassMappings : AutoMapperSpecBase
    {
        public class Source
        {
            public string Name { get; set; }
        }

        public class Destination
        {
            public string Name { get; set; }
        }

        public class SubDestination : Destination
        {
            public string SubName { get; set; }
        }

        protected override void Establish_context()
        {
            MicroMapper.Mapper.CreateMap<Source, Destination>();
        }

        [Fact]
        public void TestCase()
        {

            var source = new Source() { Name = "Test" };
            var destination = new Destination();

            MicroMapper.Mapper.Map<Source, Destination>(source, destination); // Works

            var subDestination = new SubDestination();

            MicroMapper.Mapper.Map<Source, Destination>(source, subDestination); // Fails
        }
    }
}
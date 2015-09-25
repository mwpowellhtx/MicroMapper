namespace MicroMapper.UnitTests.Bug
{
    using Should;
    using Xunit;

    public class ObjectEnumToObjectEnum : AutoMapperSpecBase
    {
        private IMapperContext _context;
        Target _target;

        public enum SourceEnumValue
        {
            Donkey,
            Mule
        }

        public enum TargetEnumValue
        {
            Donkey,
            Mule
        }

        public class Source
        {
            public object Value { get; set; }
        }

        public class Target
        {
            public object Value { get; set; }
        }

        protected override void Establish_context()
        {

            _context = new MapperContext();
            var parentMapping = _context.CreateMap<Source, Target>();
            parentMapping.ForMember(dest => dest.Value, opt => opt.MapFrom(s => (TargetEnumValue) s.Value));
        }

        protected override void Because_of()
        {
            _target = _context.Map<Target>(new Source { Value = SourceEnumValue.Mule });
        }

        [Fact]
        public void Should_be_enum()
        {
            _target.Value.ShouldBeType<TargetEnumValue>();
        }
    }
}
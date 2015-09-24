namespace AutoMapper.UnitTests.Bug
{
    using System;
    using Should;
    using Xunit;

    public class RemovePrefixes : AutoMapperSpecBase
    {
        private IMapperContext _context;

        private class Source
        {
            public int GetNumber { get; set; }
        }

        private class Destination
        {
            public int Number { get; set; }
        }

        protected override void Establish_context()
        {
            _context = new MapperContext();
            _context.Configuration.ClearPrefixes();
            _context.CreateMap<Source, Destination>();
        }

        [Fact]
        public void Should_not_map_with_default_postfix()
        {
            new Action(_context.AssertConfigurationIsValid)
                .ShouldThrow<AutoMapperConfigurationException>();
        }
    }
}
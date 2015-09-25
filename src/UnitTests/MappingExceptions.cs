using System;
using Xunit;
using Should;

namespace MicroMapper.UnitTests
{
	namespace MappingExceptions
	{
        public class When_encountering_a_member_mapping_problem_during_mapping : NonValidatingSpecBase
        {
            public class Source
            {
                public string Value { get; set; }
            }

            public class Dest
            {
                public int Value { get; set; }
            }

            protected override void Establish_context()
            {
                Mapper.CreateMap<Source, Dest>();
            }

            [Fact]
            public void Should_provide_a_contextual_exception()
            {
                var source = new Source { Value = "adsf" };
                typeof(MicroMapperMappingException).ShouldBeThrownBy(() => Mapper.Map<Source, Dest>(source));
            }

            [Fact]
            public void Should_have_contextual_mapping_information()
            {
                var source = new Source { Value = "adsf" };
                MicroMapperMappingException thrown = null;
                try
                {
                    Mapper.Map<Source, Dest>(source);
                }
                catch (MicroMapperMappingException ex)
                {
                    thrown = ex;
                }
                thrown.ShouldNotBeNull();
            }
        }
    }
}
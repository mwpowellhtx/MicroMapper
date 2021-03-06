﻿using Xunit;
using Should;
using System;

namespace MicroMapper.UnitTests.Bug
{
    public class InternalProperties : AutoMapperSpecBase
    {
        public int SomeValue = 2354;
        private Destination _destination;

        class Source
        {
            internal int Number { get; set; }
        }
        class Destination
        {
            internal int Number { get; set; }
        }

        protected override void Establish_context()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.ShouldMapProperty = p => true;
                cfg.CreateMap<Source, Destination>();
            });
        }

        protected override void Because_of()
        {
            _destination = Mapper.Map<Source, Destination>(new Source { Number = SomeValue });
        }

        [Fact]
        public void Should_map_internal_property()
        {
            _destination.Number.ShouldEqual(SomeValue);
        }
    }
}
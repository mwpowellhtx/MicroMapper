﻿#if NET4 || MONODROID || MONOTOUCH || __IOS__ || DNXCORE50

namespace MicroMapper.UnitTests.Mappers
{
    using System.Collections.Specialized;
    using MicroMapper.Mappers;
    using Should;
    using Xunit;

    public class NameValueCollectionMapperTests
    {
        public class IsMatch
        {
            [Fact]
            public void ReturnsTrueWhenBothSourceAndDestinationTypesAreNameValueCollection()
            {
                var rc = new ResolutionContext(null, null, null, typeof (NameValueCollection),
                    typeof (NameValueCollection), null, Mapper.Context);
                var nvcm = new NameValueCollectionMapper();

                var result = nvcm.IsMatch(rc);

                result.ShouldBeTrue();
            }

            [Fact]
            public void ReturnsIsFalseWhenDestinationTypeIsNotNameValueCollection()
            {
                var rc = new ResolutionContext(null, null, null, typeof (NameValueCollection), typeof (object), null,
                    Mapper.Context);
                var nvcm = new NameValueCollectionMapper();

                var result = nvcm.IsMatch(rc);

                result.ShouldBeFalse();
            }

            [Fact]
            public void ReturnsIsFalseWhenSourceTypeIsNotNameValueCollection()
            {
                var rc = new ResolutionContext(null, null, null, typeof (object), typeof (NameValueCollection), null,
                    Mapper.Context);
                var nvcm = new NameValueCollectionMapper();

                var result = nvcm.IsMatch(rc);

                result.ShouldBeFalse();
            }
        }

        public class Map
        {
            [Fact]
            public void ReturnsNullIfSourceTypeIsNotNameValueCollection()
            {
                var rc = new ResolutionContext(null, new object(), new NameValueCollection(), typeof (object),
                    typeof (NameValueCollection), null, Mapper.Context);
                var nvcm = new NameValueCollectionMapper();

                var result = nvcm.Map(rc);

                result.ShouldBeNull();
            }

            [Fact]
            public void ReturnsNullIfSourceValueIsNull()
            {
                var rc = new ResolutionContext(null, null, new NameValueCollection(), typeof (NameValueCollection),
                    typeof (NameValueCollection), null, Mapper.Context);
                var nvcm = new NameValueCollectionMapper();

                var result = nvcm.Map(rc);

                result.ShouldBeNull();
            }

            [Fact]
            public void ReturnsEmptyCollectionWhenSourceCollectionIsEmpty()
            {
                var sourceValue = new NameValueCollection();
                var rc = new ResolutionContext(null, sourceValue, new NameValueCollection(),
                    typeof (NameValueCollection), typeof (NameValueCollection), null, Mapper.Context);
                var nvcm = new NameValueCollectionMapper();

                var result = nvcm.Map(rc) as NameValueCollection;

                result.ShouldBeEmpty();
            }

            [Fact]
            public void ReturnsMappedObjectWithExpectedValuesWhenSourceCollectionHasOneItem()
            {
                var sourceValue = new NameValueCollection() {{"foo", "bar"}};
                var rc = new ResolutionContext(null, sourceValue, new NameValueCollection(),
                    typeof (NameValueCollection), typeof (NameValueCollection), null, Mapper.Context);

                var nvcm = new NameValueCollectionMapper();

                var result = nvcm.Map(rc) as NameValueCollection;

                1.ShouldEqual(result.Count);
                "foo".ShouldEqual(result.AllKeys[0]);
                "bar".ShouldEqual(result["foo"]);
            }
        }
    }
}

#endif
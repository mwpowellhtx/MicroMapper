namespace Benchmark.Flattening
{
    using System;
    using MicroMapper;

    public class CtorMapper : IObjectToObjectMapper
    {
        private Model11 _model;

        public string Name => nameof(CtorMapper);

        public void Initialize()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<Model11, Dto11>());
            _model = new Model11 {Value = 5};
        }

        public void Map()
        {
            Mapper.Map<Model11, Dto11>(_model);
        }
    }

    public class ManualCtorMapper : IObjectToObjectMapper
    {
        private Model11 _model;

        public string Name => nameof(ManualCtorMapper);

        public void Initialize()
        {
            _model = new Model11 {Value = 5};
        }

        public void Map()
        {
            var dto = new Dto11(_model.Value);
        }
    }

    public class FlatteningMapper : IObjectToObjectMapper
    {
        private ModelObject _source;

        public string Name => nameof(FlatteningMapper);

        public void Initialize()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Model1, Dto1>();
                cfg.CreateMap<Model2, Dto2>();
                cfg.CreateMap<Model3, Dto3>();
                cfg.CreateMap<Model4, Dto4>();
                cfg.CreateMap<Model5, Dto5>();
                cfg.CreateMap<Model6, Dto6>();
                cfg.CreateMap<Model7, Dto7>();
                cfg.CreateMap<Model8, Dto8>();
                cfg.CreateMap<Model9, Dto9>();
                cfg.CreateMap<Model10, Dto10>();
                cfg.CreateMap<ModelObject, ModelDto>();
            });

            Mapper.AssertConfigurationIsValid();

            _source = new ModelObject
            {
                BaseDate = new DateTime(2007, 4, 5),
                Sub = new ModelSubObject
                {
                    ProperName = "Some name",
                    SubSub = new ModelSubSubObject {IAmACoolProperty = "Cool daddy-o"}
                },
                Sub2 = new ModelSubObject {ProperName = "Sub 2 name"},
                SubWithExtraName = new ModelSubObject {ProperName = "Some other name"}
            };
        }

        public void Map()
        {
            Mapper.Map<ModelObject, ModelDto>(_source);
        }
    }

    public class ManualMapper : IObjectToObjectMapper
    {
        private ModelObject _source;

        public string Name => nameof(ManualMapper);

        public void Initialize()
        {
            _source = new ModelObject
            {
                BaseDate = new DateTime(2007, 4, 5),
                Sub = new ModelSubObject
                {
                    ProperName = "Some name",
                    SubSub = new ModelSubSubObject {IAmACoolProperty = "Cool daddy-o"}
                },
                Sub2 = new ModelSubObject {ProperName = "Sub 2 name"},
                SubWithExtraName = new ModelSubObject {ProperName = "Some other name"}
            };
        }

        public void Map()
        {
            var destination = new ModelDto
            {
                BaseDate = _source.BaseDate,
                Sub2ProperName = _source.Sub2.ProperName,
                SubProperName = _source.Sub.ProperName,
                SubSubSubIAmACoolProperty = _source.Sub.SubSub.IAmACoolProperty,
                SubWithExtraNameProperName = _source.SubWithExtraName.ProperName
            };
        }
    }

    public class Model1
    {
        public int Value { get; set; }
    }

    public class Model2
    {
        public int Value { get; set; }
    }

    public class Model3
    {
        public int Value { get; set; }
    }

    public class Model4
    {
        public int Value { get; set; }
    }

    public class Model5
    {
        public int Value { get; set; }
    }

    public class Model6
    {
        public int Value { get; set; }
    }

    public class Model7
    {
        public int Value { get; set; }
    }

    public class Model8
    {
        public int Value { get; set; }
    }

    public class Model9
    {
        public int Value { get; set; }
    }

    public class Model10
    {
        public int Value { get; set; }
    }

    public class Model11
    {
        public int Value { get; set; }
    }

    public class Dto1
    {
        public int Value { get; set; }
    }

    public class Dto2
    {
        public int Value { get; set; }
    }

    public class Dto3
    {
        public int Value { get; set; }
    }

    public class Dto4
    {
        public int Value { get; set; }
    }

    public class Dto5
    {
        public int Value { get; set; }
    }

    public class Dto6
    {
        public int Value { get; set; }
    }

    public class Dto7
    {
        public int Value { get; set; }
    }

    public class Dto8
    {
        public int Value { get; set; }
    }

    public class Dto9
    {
        public int Value { get; set; }
    }

    public class Dto10
    {
        public int Value { get; set; }
    }

    public class Dto11
    {
        public Dto11(int value)
        {
            Value = value;
        }

        public int Value { get; }
    }

    public class ModelObject
    {
        public DateTime BaseDate { get; set; }

        public ModelSubObject Sub { get; set; }

        public ModelSubObject Sub2 { get; set; }

        public ModelSubObject SubWithExtraName { get; set; }
    }

    public class ModelSubObject
    {
        public string ProperName { get; set; }

        public ModelSubSubObject SubSub { get; set; }
    }

    public class ModelSubSubObject
    {
        public string IAmACoolProperty { get; set; }
    }

    public class ModelDto
    {
        public DateTime BaseDate { get; set; }

        public string SubProperName { get; set; }

        public string Sub2ProperName { get; set; }

        public string SubWithExtraNameProperName { get; set; }

        public string SubSubSubIAmACoolProperty { get; set; }
    }
}
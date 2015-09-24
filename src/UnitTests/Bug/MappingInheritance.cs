﻿
namespace AutoMapper.UnitTests.Bug
{
    using Should;
    using Xunit;

    public class MappingInheritance : AutoMapperSpecBase
	{
		private Entity testEntity;
		private EditModel testModel;

        protected override void Establish_context()
        {
            Mapper.CreateMap<Entity, ViewModel>();
            Mapper.CreateMap<Entity, BaseModel>()
                    .ForMember(model => model.Value1, mce => mce.MapFrom(entity => entity.Value2))
                    .ForMember(model => model.Value2, mce => mce.MapFrom(entity => entity.Value1))
                    .Include<Entity, EditModel>()
                    .Include<Entity, ViewModel>();
            Mapper.CreateMap<Entity, EditModel>()
                .ForMember(model => model.Value3, mce => mce.MapFrom(entity => entity.Value1 + entity.Value2));
        }

        protected override void Because_of()
        {
            testEntity = new Entity
            {
                Value1 = 1,
                Value2 = 2,
            };
            testModel = Mapper.Map<Entity, EditModel>(testEntity);
        }

        [Fact]
		public void AutoMapper_should_map_derived_types_properly()
		{
            testEntity.Value1.ShouldEqual(testModel.Value2);
            testEntity.Value2.ShouldEqual(testModel.Value1);
            (testEntity.Value1 + testEntity.Value2).ShouldEqual(testModel.Value3);
        }

        public class Entity
        {
            public int Value1 { get; set; }
            public int Value2 { get; set; }
        }

        public class BaseModel
        {
            public int Value1 { get; set; }
            public int Value2 { get; set; }
        }

        public class EditModel : BaseModel
        {
            public int Value3 { get; set; }
        }

        public class ViewModel : BaseModel { }
    }

    public class MappingInheritanceBug
    {
        [Fact]
        public void TestMethod1()
        {
            Mapper.Reset();

            Mapper.CreateMap<Order, OrderDto>()
                .Include<OnlineOrder, OnlineOrderDto>()
                .Include<MailOrder, MailOrderDto>();
            Mapper.CreateMap<OnlineOrder, OnlineOrderDto>();
            Mapper.CreateMap<MailOrder, MailOrderDto>();
            Mapper.Context.Configuration.Seal();

            //Mapper.AssertConfigurationIsValid();

            var mailOrder = new MailOrder() { NewId = 1 };
            var mapped = Mapper.Map<OrderDto>(mailOrder);

            mapped.ShouldBeType<MailOrderDto>();
        }

        public abstract class Base<T>
        {
        }

        public class Order : Base<Order> { }
        public class OnlineOrder : Order { }
        public class MailOrder : Order
        {
            public int NewId { get; set; }
        }

        public class OrderDto { }
        public class OnlineOrderDto : OrderDto { }
        public class MailOrderDto : OrderDto
        {
            public int NewId { get; set; }
        }
    }
}

namespace MicroMapper.Mappers
{
    using System;
    using System.Collections.Generic;

    /*
    TODO: couple of concerns: does this need to be static, per se?
    would it be more appropriate for this to be a read only collection?
    why not just 'declare' it in the now-objectmappercollection (formerly 'MapperRegistry') ?
    */
    public static class TypeMapObjectMapperRegistry
    {
        /// <summary>
        /// Provides default set of Mappers for <see cref="TypeMapMapper"/>.
        /// </summary>
        public static IEnumerable<ITypeMapObjectMapper> Mappers { get; }
            = new ITypeMapObjectMapper[]
            {
                new SubstutitionMapperStrategy(),
                new CustomMapperStrategy(),
                new NullMappingStrategy(),
                new CacheMappingStrategy(),
                new NewObjectPropertyMapMappingStrategy(),
                new ExistingObjectMappingStrategy()
            };

        private class CustomMapperStrategy : ITypeMapObjectMapper
        {
            public object Map(ResolutionContext context)
            {
                return context.TypeMap.CustomMapper(context);
            }

            public bool IsMatch(ResolutionContext context)
            {
                return context.TypeMap.CustomMapper != null;
            }
        }

        private class SubstutitionMapperStrategy : ITypeMapObjectMapper
        {
            public object Map(ResolutionContext context)
            {
                var runner = context.MapperContext.Runner;
                var newSource = context.TypeMap.Substitution(context.SourceValue);
                var typeMap = runner.ConfigurationProvider.ResolveTypeMap(newSource.GetType(), context.DestinationType);

                var substitutionContext = context.CreateTypeContext(typeMap, newSource, context.DestinationValue,
                    newSource.GetType(), context.DestinationType);

                return runner.Map(substitutionContext);
            }

            public bool IsMatch(ResolutionContext context)
            {
                return context.TypeMap.Substitution != null;
            }
        }

        private class NullMappingStrategy : ITypeMapObjectMapper
        {
            public object Map(ResolutionContext context)
            {
                return null;
            }

            public bool IsMatch(ResolutionContext context)
            {
                var runner = context.MapperContext.Runner;
                var profileConfiguration = runner.ConfigurationProvider.GetProfileConfiguration(context.TypeMap.Profile);
                return (context.SourceValue == null && profileConfiguration.MapNullSourceValuesAsNull);
            }
        }

        private class CacheMappingStrategy : ITypeMapObjectMapper
        {
            public object Map(ResolutionContext context)
            {
                return context.InstanceCache[context];
            }

            public bool IsMatch(ResolutionContext context)
            {
                return !context.Options.DisableCache && context.DestinationValue == null
                       && context.InstanceCache.ContainsKey(context);
            }
        }

        private abstract class PropertyMapMappingStrategy : ITypeMapObjectMapper
        {
            public object Map(ResolutionContext context)
            {
                var mappedObject = GetMappedObject(context);
                if (context.SourceValue != null && !context.Options.DisableCache)
                    context.InstanceCache[context] = mappedObject;

                context.TypeMap.BeforeMap(context.SourceValue, mappedObject);
                context.BeforeMap(mappedObject);

                foreach (var propertyMap in context.TypeMap.GetPropertyMaps())
                {
                    MapPropertyValue(context.CreatePropertyMapContext(propertyMap), mappedObject, propertyMap);
                }
                mappedObject = ReassignValue(context, mappedObject);

                context.AfterMap(mappedObject);
                context.TypeMap.AfterMap(context.SourceValue, mappedObject);

                return mappedObject;
            }

            protected virtual object ReassignValue(ResolutionContext context, object o)
            {
                return o;
            }

            public abstract bool IsMatch(ResolutionContext context);

            protected abstract object GetMappedObject(ResolutionContext context);

            private void MapPropertyValue(ResolutionContext context, object mappedObject, PropertyMap propertyMap)
            {
                var runner = context.MapperContext.Runner;
                if (!propertyMap.CanResolveValue() || !propertyMap.ShouldAssignValuePreResolving(context))
                    return;

                ResolutionResult result;

                Exception resolvingExc = null;
                try
                {
                    result = propertyMap.ResolveValue(context);
                }
                catch (MicroMapperMappingException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    var errorContext = CreateErrorContext(context, propertyMap, null);
                    resolvingExc = new MicroMapperMappingException(errorContext, ex);
                    result = new ResolutionResult(context);
                }

                if (result.ShouldIgnore)
                    return;

                var destinationValue = propertyMap.GetDestinationValue(mappedObject);

                var sourceType = result.Type;
                var destinationType = propertyMap.DestinationProperty.MemberType;

                var typeMap = runner.ConfigurationProvider.ResolveTypeMap(result, destinationType);

                var targetSourceType = typeMap != null ? typeMap.SourceType : sourceType;

                var newContext = context.CreateMemberContext(typeMap, result.Value, destinationValue,
                    targetSourceType, propertyMap);

                if (!propertyMap.ShouldAssignValue(newContext))
                    return;

                // If condition succeeded and resolving failed, throw
                if (resolvingExc != null)
                    throw resolvingExc;

                try
                {
                    var propertyValueToAssign = runner.Map(newContext);
                    AssignValue(propertyMap, mappedObject, propertyValueToAssign);
                }
                catch (MicroMapperMappingException mmmex)
                {
                    throw mmmex;
                }
                catch (Exception ex)
                {
                    throw new MicroMapperMappingException(newContext, ex);
                }
            }

            protected virtual void AssignValue(PropertyMap propertyMap, object mappedObject,
                object propertyValueToAssign)
            {
                if (propertyMap.CanBeSet)
                    propertyMap.DestinationProperty.SetValue(mappedObject, propertyValueToAssign);
            }

            private ResolutionContext CreateErrorContext(ResolutionContext context, PropertyMap propertyMap,
                object destinationValue)
            {
                return context.CreateMemberContext(
                    null,
                    context.SourceValue,
                    destinationValue,
                    context.SourceValue?.GetType() ?? typeof (object),
                    propertyMap);
            }
        }

        private class NewObjectPropertyMapMappingStrategy : PropertyMapMappingStrategy
        {
            public override bool IsMatch(ResolutionContext context)
            {
                return context.DestinationValue == null;
            }

            protected override object GetMappedObject(ResolutionContext context)
            {
                var runner = context.MapperContext.Runner;
                var result = runner.CreateObject(context);
                if (result == null)
                {
                    throw new InvalidOperationException($"Cannot create destination object. {context}");
                }
                return result;
            }
        }

        private class ExistingObjectMappingStrategy : PropertyMapMappingStrategy
        {
            public override bool IsMatch(ResolutionContext context)
            {
                return true;
            }

            protected override object GetMappedObject(ResolutionContext context)
            {
                return context.DestinationValue;
            }
        }
    }
}
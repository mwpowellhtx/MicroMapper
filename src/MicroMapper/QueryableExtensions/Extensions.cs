namespace MicroMapper.QueryableExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Impl;
    using ObjectDictionary = System.Collections.Generic.IDictionary<string, object>;

    /// <summary>
    /// Queryable extensions for MicroMapper
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Create an expression tree representing a mapping from the <typeparamref name="TSource"/> type to <typeparamref name="TDestination"/> type
        /// Includes flattening and expressions inside MapFrom member configuration
        /// </summary>
        /// <typeparam name="TSource">Source Type</typeparam>
        /// <typeparam name="TDestination">Destination Type</typeparam>
        /// <param name="mappingEngine">Mapping engine instance</param>
        /// <param name="parameters">Optional parameter object for parameterized mapping expressions</param>
        /// <param name="membersToExpand">Expand members explicitly previously marked as members to explicitly expand</param>
        /// <returns>Expression tree mapping source to destination type</returns>
        public static Expression<Func<TSource, TDestination>> CreateMapExpression<TSource, TDestination>(
            this IMappingEngine mappingEngine, IDictionary<string, object> parameters = null,
            params MemberInfo[] membersToExpand)
        {
            return
                (Expression<Func<TSource, TDestination>>)
                    mappingEngine.CreateMapExpression(typeof(TSource), typeof(TDestination), parameters, membersToExpand);
        }

        /// <summary>
        /// Create an expression tree representing a mapping from the source type to destination type
        /// Includes flattening and expressions inside MapFrom member configuration
        /// </summary>
        /// <param name="mappingEngine">Mapping engine instance</param>
        /// <param name="sourceType">Source Type</param>
        /// <param name="destinationType">Destination Type</param>
        /// <param name="parameters">Optional parameter object for parameterized mapping expressions</param>
        /// <param name="membersToExpand">Expand members explicitly previously marked as members to explicitly expand</param>
        /// <returns>Expression tree mapping source to destination type</returns>
        public static Expression CreateMapExpression(this IMappingEngine mappingEngine,
            Type sourceType, Type destinationType,
            IDictionary<string, object> parameters = null, params MemberInfo[] membersToExpand)
        {
            return mappingEngine.CreateMapExpression(sourceType, destinationType, parameters, membersToExpand);
        }

        /// <summary>
        /// Maps a queryable expression of a source type to a queryable expression of a destination type.
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="sourceQuery">Source queryable</param>
        /// <param name="destinationQuery">Destination queryable</param>
        /// <returns>Mapped destination queryable</returns>
        public static IQueryable<TDestination> Map<TSource, TDestination>(this IQueryable<TSource> sourceQuery,
            IQueryable<TDestination> destinationQuery)
        {
            return sourceQuery.Map(destinationQuery, Mapper.Context.Engine);
        }

        /// <summary>
        /// Maps a queryable expression of a source type to a queryable expression of a destination type.
        /// </summary>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="sourceQuery">Source queryable</param>
        /// <param name="destinationQuery">Destination queryable</param>
        /// <param name="mappingEngine">Mapping engine instance</param>
        /// <returns>Mapped destination queryable</returns>
        public static IQueryable<TDestination> Map<TSource, TDestination>(this IQueryable<TSource> sourceQuery,
            IQueryable<TDestination> destinationQuery, IMappingEngine mappingEngine)
        {
            return mappingEngine.MapQuery(sourceQuery, destinationQuery);
        }

        public static IQueryDataSourceInjection<TSource> UseAsDataSource<TSource>(
            this IQueryable<TSource> dataSource)
        {
            return dataSource.UseAsDataSource(Mapper.Context.Engine);
        }

        public static IQueryDataSourceInjection<TSource> UseAsDataSource<TSource>(
            this IQueryable<TSource> dataSource, IMappingEngine mappingEngine)
        {
            return mappingEngine.UseAsDataSource(dataSource);
        }

        /// <summary>
        /// Extension method to project from a queryable using the static <see cref="Mapper.Engine"/> property.
        /// Due to generic parameter inference, you need to call Project().To to execute the map
        /// </summary>
        /// <remarks>Projections are only calculated once and cached</remarks>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <param name="source">Queryable source</param>
        /// <returns>Expression to project into</returns>
        [Obsolete("Use ProjectTo instead")]
        public static IProjectionExpression Project<TSource>(this IQueryable<TSource> source)
        {
            return source.Project(Mapper.Context.Engine);
        }

        /// <summary>
        /// Extension method to project from a queryable using the provided mapping engine
        /// Due to generic parameter inference, you need to call Project().To to execute the map
        /// </summary>
        /// <remarks>Projections are only calculated once and cached</remarks>
        /// <typeparam name="TSource">Source type</typeparam>
        /// <param name="source">Queryable source</param>
        /// <param name="mappingEngine">Mapping engine instance</param>
        /// <returns>Expression to project into</returns>
        [Obsolete("Use ProjectTo instead")]
        public static IProjectionExpression Project<TSource>(this IQueryable<TSource> source,
            IMappingEngine mappingEngine)
        {
            return mappingEngine.ProjectQuery(source);
        }

        /// <summary>
        /// Extension method to project from a queryable using the provided mapping engine
        /// </summary>
        /// <remarks>Projections are only calculated once and cached</remarks>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Queryable source</param>
        /// <param name="membersToExpand">Explicit members to expand</param>
        /// <returns>Expression to project into</returns>
        public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source,
            params Expression<Func<TDestination, object>>[] membersToExpand
            )
        {
            return source.ProjectTo(null, membersToExpand);
        }

        /// <summary>
        /// Extension method to project from a queryable using the provided mapping engine
        /// </summary>
        /// <remarks>Projections are only calculated once and cached</remarks>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Queryable source</param>
        /// <param name="parameters">Optional parameter object for parameterized mapping expressions</param>
        /// <param name="membersToExpand">Explicit members to expand</param>
        /// <returns>Expression to project into</returns>
        public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source,
            object parameters, params Expression<Func<TDestination, object>>[] membersToExpand
            )
        {
            return source.ProjectTo(parameters, Mapper.Context.Engine, membersToExpand);
        }

        /// <summary>
        /// Extension method to project from a queryable using the provided mapping engine
        /// </summary>
        /// <remarks>Projections are only calculated once and cached</remarks>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Queryable source</param>
        /// <param name="mappingEngine">Mapping engine instance</param>
        /// <param name="parameters">Optional parameter object for parameterized mapping expressions</param>
        /// <param name="membersToExpand">Explicit members to expand</param>
        /// <returns>Expression to project into</returns>
        public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source,
            object parameters, IMappingEngine mappingEngine,
            params Expression<Func<TDestination, object>>[] membersToExpand)
        {
            return mappingEngine.ProjectTo(source, parameters, membersToExpand);
        }

        /// <summary>
        /// Projects the source type to the destination type given the mapping configuration
        /// </summary>
        /// <typeparam name="TDestination">Destination type to map to</typeparam>
        /// <param name="source">Queryable source</param>
        /// <param name="mappingEngine">Mapping engine instance</param>
        /// <param name="parameters">Optional parameter object for parameterized mapping expressions</param>
        /// <param name="membersToExpand">Explicit members to expand</param>
        /// <returns>Queryable result, use queryable extension methods to project and execute result</returns>
        public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source,
            ObjectDictionary parameters, IMappingEngine mappingEngine,
            params string[] membersToExpand)
        {
            return mappingEngine.ProjectTo<TDestination>(source, parameters, membersToExpand);
        }

        /// <summary>
        /// Projects the source type to the destination type given the mapping configuration
        /// </summary>
        /// <typeparam name="TDestination">Destination type to map to</typeparam>
        /// <param name="source">Queryable source</param>
        /// <param name="parameters">Optional parameter object for parameterized mapping expressions</param>
        /// <param name="membersToExpand">Explicit members to expand</param>
        /// <returns>Queryable result, use queryable extension methods to project and execute result</returns>
        public static IQueryable<TDestination> ProjectTo<TDestination>(this IQueryable source,
            ObjectDictionary parameters, params string[] membersToExpand)
        {
            return source.ProjectTo<TDestination>(parameters, Mapper.Context.Engine, membersToExpand);
        }
    }
}

using LambdaExpression = System.Linq.Expressions.LambdaExpression;
using ExpressionRequest = MicroMapper.QueryableExtensions.ExpressionRequest;

namespace MicroMapper
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using QueryableExtensions;
    using QueryableExtensions.Impl;
    using ObjectDictionary = System.Collections.Generic.IDictionary<string, object>;
    using ExpressionDictionary = Internal.IDictionary<ExpressionRequest, LambdaExpression>;

    /// <summary>
    /// Performs mapping based on configuration
    /// </summary>
    public interface IMappingEngine : IDisposable
    {
        /// <summary>
        /// Configuration provider for performaing maps
        /// </summary>
        IConfigurationProvider ConfigurationProvider { get; }

        /// <summary>
        /// Gets the mapping Runner.
        /// </summary>
        IMappingEngineRunner Runner { get; }

        /// <summary>
        /// Gets the ExpressionCache.
        /// </summary>
        ExpressionDictionary ExpressionCache { get; }

        /// <summary>
        /// Execute a mapping from the source object to a new destination object.
        /// The source type is inferred from the source object.
        /// </summary>
        /// <typeparam name="TDestination">Destination type to create</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <returns>Mapped destination object</returns>
        TDestination Map<TDestination>(object source);

        /// <summary>
        /// Execute a mapping from the source object to a new destination object with supplied
        /// mapping options.
        /// </summary>
        /// <typeparam name="TDestination">Destination type to create</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <param name="opts">Delegated mapping options action</param>
        /// <returns>Mapped destination object</returns>
        TDestination Map<TDestination>(object source, Action<IMappingOperationOptions> opts);

        /// <summary>
        /// Execute a mapping from the source object to a new destination object.
        /// </summary>
        /// <typeparam name="TSource">Source type to use, regardless of the runtime type</typeparam>
        /// <typeparam name="TDestination">Destination type to create</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <returns>Mapped destination object</returns>
        TDestination Map<TSource, TDestination>(TSource source);

        /// <summary>
        /// Execute a mapping from the source object to a new destination object with supplied
        /// mapping options.
        /// </summary>
        /// <typeparam name="TSource">Source type to use</typeparam>
        /// <typeparam name="TDestination">Destination type to create</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <param name="opts">Delegated mapping options action</param>
        /// <returns>Mapped destination object</returns>
        TDestination Map<TSource, TDestination>(TSource source,
            Action<IMappingOperationOptions<TSource, TDestination>> opts);

        /// <summary>
        /// Execute a mapping from the source object to the existing destination object.
        /// </summary>
        /// <typeparam name="TSource">Source type to use</typeparam>
        /// <typeparam name="TDestination">Dsetination type</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <param name="destination">Destination object to map into</param>
        /// <returns>The mapped destination object, same instance as the
        /// <paramref name="destination"/> object</returns>
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination);

        /// <summary>
        /// Execute a mapping from the source object to the existing destination object with
        /// supplied mapping options.
        /// </summary>
        /// <typeparam name="TSource">Source type to use</typeparam>
        /// <typeparam name="TDestination">Destination type</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <param name="destination">Destination object to map into</param>
        /// <param name="opts">Delegated mapping option action</param>
        /// <returns>The mapped destination object, same instance as the
        /// <paramref name="destination"/> object</returns>
        TDestination Map<TSource, TDestination>(TSource source, TDestination destination,
            Action<IMappingOperationOptions<TSource, TDestination>> opts);

        /// <summary>
        /// Execute a mapping from the source object to a new destination object with explicit
        /// <see cref="Type"/> objects
        /// </summary>
        /// <param name="source">Source object to map from</param>
        /// <param name="sourceType">Source type to use</param>
        /// <param name="destinationType">Destination type to create</param>
        /// <returns>Mapped destination object</returns>
        object Map(object source, Type sourceType, Type destinationType);

        /// <summary>
        /// Execute a mapping from the source object to a new destination object with explicit
        /// <see cref="Type"/> objects and supplied mapping options.
        /// </summary>
        /// <param name="source">Source object to map from</param>
        /// <param name="sourceType">Source type to use</param>
        /// <param name="destinationType">Destination type to create</param>
        /// <param name="opts">Delegated mapping options type</param>
        /// <returns>Mapped destination object</returns>
        object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts);

        /// <summary>
        /// Execute a mapping from the source object to existing destination object with explicit
        /// <see cref="System.Type"/> objects.
        /// </summary>
        /// <param name="source">Source object to map from</param>
        /// <param name="destination">Destination object to map into</param>
        /// <param name="sourceType">Source type to use</param>
        /// <param name="destinationType">Destination type to use</param>
        /// <returns>Mapped destination object, same instance as the
        /// <paramref name="destination"/> object</returns>
        object Map(object source, object destination, Type sourceType, Type destinationType);

        /// <summary>
        /// Execute a mapping from the source object to existing destination object with supplied
        /// mapping options and explicit <see cref="Type"/> objects.
        /// </summary>
        /// <param name="source">Source object to map from</param>
        /// <param name="destination">Destination object to map into</param>
        /// <param name="sourceType">Source type to use</param>
        /// <param name="destinationType">Destination type to use</param>
        /// <param name="opts">Delegated mapping options action</param>
        /// <returns>Mapped destination object,
        /// same instance as the <paramref name="destination"/> object</returns>
        object Map(object source, object destination, Type sourceType, Type destinationType,
            Action<IMappingOperationOptions> opts);

        /// <summary>
        /// Create a map between the <typeparamref name="TSource"/> and
        /// <typeparamref name="TDestination"/> types and execute the map.
        /// </summary>
        /// <typeparam name="TSource">Source type to use</typeparam>
        /// <typeparam name="TDestination">Destination type to use</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <returns>Mapped destination object</returns>
        TDestination DynamicMap<TSource, TDestination>(TSource source);

        /// <summary>
        /// Create a map between the <paramref name="source"/> object and
        /// <typeparamref name="TDestination"/> types and execute the map. Source type is inferred
        /// from the source object.
        /// </summary>
        /// <typeparam name="TDestination">Destination type to use</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <returns>Mapped destination object</returns>
        TDestination DynamicMap<TDestination>(object source);

        /// <summary>
        /// Create a map between the <paramref name="sourceType"/> and
        /// <paramref name="destinationType"/> types and execute the map. Use this method when the
        /// source and destination types are not known until runtime.
        /// </summary>
        /// <param name="source">Source object to map from</param>
        /// <param name="sourceType">Source type to use</param>
        /// <param name="destinationType">Destination type to use</param>
        /// <returns>Mapped destination object</returns>
        object DynamicMap(object source, Type sourceType, Type destinationType);

        /// <summary>
        /// Create a map between the <typeparamref name="TSource"/> and
        /// <typeparamref name="TDestination"/> types and execute the map to the existing
        /// destination object
        /// </summary>
        /// <typeparam name="TSource">Source type to use</typeparam>
        /// <typeparam name="TDestination">Destination type to use</typeparam>
        /// <param name="source">Source object to map from</param>
        /// <param name="destination">Destination object to map into</param>
        void DynamicMap<TSource, TDestination>(TSource source, TDestination destination);

        /// <summary>
        /// Create a map between the <paramref name="sourceType"/> and
        /// <paramref name="destinationType"/> types and execute the map to the existing
        /// destination object. Use this method when the source and destination types are
        /// not known until runtime.
        /// </summary>
        /// <param name="source">Source object to map from</param>
        /// <param name="destination"></param>
        /// <param name="sourceType">Source type to use</param>
        /// <param name="destinationType">Destination type to use</param>
        void DynamicMap(object source, object destination, Type sourceType, Type destinationType);

        /// <summary>
        /// Returns acreated map <see cref="Expression"/> given <paramref name="sourceType"/>,
        /// <paramref name="destinationType"/>, <paramref name="parameters"/> and
        /// <paramref name="membersToExpand"/>.
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        /// <param name="parameters"></param>
        /// <param name="membersToExpand"></param>
        /// <returns></returns>
        Expression CreateMapExpression(Type sourceType, Type destinationType,
            ObjectDictionary parameters = null, params MemberInfo[] membersToExpand);

        /// <summary>
        /// Maps the queryable <paramref name="source"/> to queryable
        /// <paramref name="destination"/>.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        IQueryable<TDestination> MapQuery<TSource, TDestination>(IQueryable<TSource> source,
            IQueryable<TDestination> destination);

        /// <summary>
        /// Configures <paramref name="dataSource"/> to use as data source.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        IQueryDataSourceInjection<TSource> UseAsDataSource<TSource>(IQueryable<TSource> dataSource);

        /// <summary>
        /// Projects the queryable <paramref name="source"/> to a result
        /// <see cref="IProjectionExpression"/>.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        [Obsolete("Use ProjectTo instead")]
        IProjectionExpression ProjectQuery<TSource>(IQueryable<TSource> source);

        /// <summary>
        /// Projects the queryable <paramref name="source"/> to a queryable
        /// <typeparamref name="TDestination"/> given <paramref name="parameters"/> and
        /// <paramref name="membersToExpand"/>.
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        /// <param name="membersToExpand"></param>
        /// <returns></returns>
        IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source, object parameters,
            params Expression<Func<TDestination, object>>[] membersToExpand);

        /// <summary>
        /// Projects the queryable <paramref name="source"/> to a queryable
        /// <typeparamref name="TDestination"/> given <paramref name="parameters"/> and
        /// <paramref name="membersToExpand"/>.
        /// </summary>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <param name="parameters"></param>
        /// <param name="membersToExpand"></param>
        /// <returns></returns>
        IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source,
            ObjectDictionary parameters, params string[] membersToExpand);
    }
}
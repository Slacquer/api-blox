using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Attributes;
using Microsoft.Extensions.Logging;

namespace Examples
{

    /// <summary>
    ///     Interface ITestme
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    /// <typeparam name="TId">The type of the t identifier.</typeparam>
    public interface ITestme<TEntity, TId>
    {
        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        TId Id { get; set; }

        /// <summary>
        ///     Gets or sets the entity.
        /// </summary>
        TEntity Entity { get; set; }
    }

    /// <summary>
    ///     Interface ITestme
    /// </summary>
    /// <typeparam name="TEntity">The type of the t entity.</typeparam>
    public interface ITestme<TEntity>
    {
        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the entity.
        /// </summary>
        TEntity Entity { get; set; }
    }

    /// <summary>
    ///     Class Testme.
    /// Implements the <see cref="Examples.TestMeBase{TEntity}" />
    /// Implements the <see cref="Examples.ITestme{TEntity, TId}" />
    /// </summary>
    [InjectableService]
    public class Testme<TEntity, TId> : TestMeBase<TEntity>, ITestme<TEntity, TId>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Testme{TEntity, TId}"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public Testme(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {

        }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        public TId Id { get; set; }
    }

    /// <summary>
    ///     Class Testme.
    /// Implements the <see cref="Examples.TestMeBase{TEntity}" />
    /// Implements the <see cref="Examples.ITestme{TEntity}" />
    /// </summary>
    [InjectableService]
    public class Testme<TEntity> : TestMeBase<TEntity>, ITestme<TEntity>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Testme{TEntity}"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public Testme(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {

        }

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        public Guid Id { get; set; }
    }


    /// <summary>
    ///     Class TestMeBase.
    /// </summary>
    public abstract class TestMeBase<TEntity>
    {
        private readonly ILoggerFactory _loggerFactory;
        /// <summary>
        ///     Initializes a new instance of the <see cref="TestMeBase{TEntity}"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        protected TestMeBase(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        ///     Gets or sets the entity.
        /// </summary>
        public TEntity Entity { get; set; }
    }
}

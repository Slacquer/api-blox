using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIBlox.NetCore.Attributes;
using Microsoft.Extensions.Logging;

namespace Examples
{
    public interface ITestme<TEntity, TId>
    {
        TId Id { get; set; }

        TEntity Entity { get; set; }
    }

    public interface ITestme<TEntity>
    {
        Guid Id { get; set; }

        TEntity Entity { get; set; }
    }

    [InjectableService]
    public class Testme<TEntity, TId> : TestMeBase<TEntity>, ITestme<TEntity, TId>
    {
        public Testme(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {

        }

        public TId Id { get; set; }
        public TEntity Entity { get; set; }
    }

    [InjectableService]
    public class Testme<TEntity> : TestMeBase<TEntity>, ITestme<TEntity>
    {
        public Testme(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {

        }

        public Guid Id { get; set; }
        public TEntity Entity { get; set; }
    }


    public abstract class TestMeBase<TEntity>
    {
        private readonly ILoggerFactory _loggerFactory; 
        protected TestMeBase(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public TEntity Entity { get; set; }
    }
}

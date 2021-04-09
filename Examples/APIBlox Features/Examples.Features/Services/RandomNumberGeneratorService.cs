using System;
using APIBlox.NetCore.Attributes;
using Examples.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Examples.Services
{
    /// <summary>
    ///     Class RandomNumberGeneratorService.
    /// </summary>
    /// <seealso cref="Examples.Contracts.IRandomNumberGeneratorService" />
    [InjectableService(ServiceLifetime = ServiceLifetime.Singleton)]
    public class RandomNumberGeneratorService : IRandomNumberGeneratorService
    {
        private static readonly Random Rnd = new((int) (DateTime.Now.Ticks % 100));

        /// <summary>
        ///     Generates the number.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int GenerateNumber(int max)
        {
            return Rnd.Next(1, max);
        }
    }
}

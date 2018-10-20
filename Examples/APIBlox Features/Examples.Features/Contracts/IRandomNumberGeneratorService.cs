namespace Examples.Contracts
{
    /// <summary>
    ///     Interface IRandomNumberGeneratorService
    /// </summary>
    public interface IRandomNumberGeneratorService
    {
        /// <summary>
        ///     Generates the number.
        /// </summary>
        /// <returns>System.Int32.</returns>
        int GenerateNumber(int max);
    }
}

using Examples.DomainModels;

namespace Examples.Contracts
{
    /// <summary>
    ///     Interface ILameRepository
    /// </summary>
    public interface ILameRepository
    {
        /// <summary>
        ///     Adds the domain object.
        /// </summary>
        /// <param name="domainObject">The domain object.</param>
        void AddDomainObject(DomainObject domainObject);

        /// <summary>
        ///     Saves the changes.
        /// </summary>
        void SaveChanges();
    }
}

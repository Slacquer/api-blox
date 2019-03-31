using System.ComponentModel.DataAnnotations;

namespace Examples.Resources
{
    /// <summary>
    /// Class PersonModel.
    /// </summary>
    public class PersonModel
    {
        /// <summary>
        /// Gets or sets the age.
        /// </summary>
        /// <value>The age.</value>
        [Required]
        public int Age { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        [Required]
        public string LastName { get; set; }
    }
}
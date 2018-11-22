using System.ComponentModel.DataAnnotations;

namespace Examples.Resources
{
    /// <summary>
    ///     Class ExampleRequestObject.
    /// </summary>
    public class ExampleRequestObject
    {
        /// <summary>
        ///     Gets the parent value id.
        /// <para>
        ///     The action filter will populate private properties from query and route params.
        /// </para>
        /// </summary>
        /// <value>The parent value id.</value>
        public int ValueId { get; private set; }

        /// <summary>
        ///     Gets or sets the cool new value.  I am required, so if
        ///     you omit me, then the APIBlox ValidateResourceActionFilter will kick in resulting in an 400.
        /// </summary>
        /// <value>The cool new value.</value>
        [Required]
        public string CoolNewValue { get; set; }
    }
}

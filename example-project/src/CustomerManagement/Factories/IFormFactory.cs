using System.Windows.Forms;

namespace CustomerManagement.Factories
{
    /// <summary>
    /// Factory interface for creating forms with dependency injection.
    /// This pattern replaces the Service Locator anti-pattern and provides
    /// better testability and explicit dependencies.
    /// </summary>
    public interface IFormFactory
    {
        /// <summary>
        /// Creates an instance of the specified form type with all dependencies resolved.
        /// </summary>
        /// <typeparam name="TForm">The form type to create.</typeparam>
        /// <returns>A fully initialized form instance.</returns>
        TForm Create<TForm>() where TForm : Form;
    }
}

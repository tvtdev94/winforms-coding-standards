using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

namespace CustomerManagement.Factories
{
    /// <summary>
    /// Implementation of the form factory pattern.
    /// Creates forms with all dependencies resolved from the DI container.
    /// </summary>
    public class FormFactory : IFormFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormFactory"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider for dependency resolution.</param>
        public FormFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <inheritdoc/>
        public TForm Create<TForm>() where TForm : Form
        {
            return _serviceProvider.GetRequiredService<TForm>();
        }
    }
}

// Template: Form Factory Pattern
// Replace: YourNamespace
// Note: This is the ONLY place where IServiceProvider should be injected!

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

namespace YourNamespace.Factories
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

    /// <summary>
    /// Implementation of the form factory pattern.
    /// Creates forms with all dependencies resolved from the DI container.
    ///
    /// IMPORTANT: This is the ONLY class that should inject IServiceProvider.
    /// All forms should inject IFormFactory instead of IServiceProvider.
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

/*
 * USAGE IN PROGRAM.CS:
 *
 * private static void ConfigureServices(IServiceCollection services)
 * {
 *     // Factory Pattern (Singleton - one instance for entire app)
 *     services.AddSingleton<IFormFactory, FormFactory>();
 *
 *     // Forms (Transient - new instance each time)
 *     services.AddTransient<MainForm>();
 *     services.AddTransient<CustomerForm>();
 *     services.AddTransient<OrderForm>();
 * }
 */

/*
 * USAGE IN FORMS:
 *
 * public class MainForm : Form
 * {
 *     private readonly IFormFactory _formFactory;
 *
 *     public MainForm(IFormFactory formFactory)
 *     {
 *         _formFactory = formFactory ?? throw new ArgumentNullException(nameof(formFactory));
 *         InitializeComponent();
 *     }
 *
 *     private void OnOpenCustomerForm_Click(object sender, EventArgs e)
 *     {
 *         // Create form using factory
 *         var form = _formFactory.Create<CustomerForm>();
 *         form.ShowDialog(this);
 *     }
 *
 *     private void OnEditCustomer_Click(object sender, EventArgs e)
 *     {
 *         // Modal dialog with using statement
 *         using (var editForm = _formFactory.Create<CustomerEditForm>())
 *         {
 *             editForm.Initialize(customerId);
 *
 *             if (editForm.ShowDialog(this) == DialogResult.OK)
 *             {
 *                 // Handle result
 *                 LoadCustomers();
 *             }
 *         }
 *     }
 * }
 */

/*
 * TESTING:
 *
 * [Fact]
 * public void OpenCustomerForm_UsesFactory_CreatesForm()
 * {
 *     // Arrange
 *     var mockFactory = new Mock<IFormFactory>();
 *     var mockForm = new Mock<CustomerForm>();
 *
 *     mockFactory
 *         .Setup(f => f.Create<CustomerForm>())
 *         .Returns(mockForm.Object);
 *
 *     var mainForm = new MainForm(mockFactory.Object);
 *
 *     // Act
 *     mainForm.OnCustomersClick(null, EventArgs.Empty);
 *
 *     // Assert
 *     mockFactory.Verify(f => f.Create<CustomerForm>(), Times.Once);
 * }
 */

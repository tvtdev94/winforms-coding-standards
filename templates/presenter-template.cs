// Template: MVP Presenter for WinForms
// Replace: YourEntity, YourPresenter, IYourView, IYourService
// IMPORTANT: Presenter contains ALL business logic, Form is just a thin UI shell

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace YourNamespace.Presenters
{
    #region View Interface

    /// <summary>
    /// View interface for YourEntity form.
    /// Defines the contract between the view (Form) and presenter.
    /// </summary>
    /// <remarks>
    /// The view interface should:
    /// - Define properties for all form data (two-way binding)
    /// - Define properties for UI state (IsLoading, IsEditMode, etc.)
    /// - Define events that the presenter subscribes to
    /// - Define methods for showing messages and closing
    /// </remarks>
    public interface IYourEntityView
    {
        #region Events

        /// <summary>
        /// Raised when the form is loaded.
        /// </summary>
        event EventHandler? LoadRequested;

        /// <summary>
        /// Raised when the user requests to save.
        /// </summary>
        event EventHandler? SaveRequested;

        /// <summary>
        /// Raised when the user requests to cancel.
        /// </summary>
        event EventHandler? CancelRequested;

        /// <summary>
        /// Raised when the user requests to delete.
        /// </summary>
        event EventHandler? DeleteRequested;

        /// <summary>
        /// Raised when the user requests to refresh data.
        /// </summary>
        event EventHandler? RefreshRequested;

        #endregion

        #region Data Properties

        /// <summary>
        /// Gets or sets the entity ID (0 for new entity).
        /// </summary>
        int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the entity name.
        /// </summary>
        string EntityName { get; set; }

        // TODO: Add more properties for your entity fields
        // Example:
        // string Description { get; set; }
        // decimal Price { get; set; }
        // bool IsActive { get; set; }
        // DateTime CreatedDate { get; set; }

        #endregion

        #region UI State Properties

        /// <summary>
        /// Gets or sets a value indicating whether the view is in loading state.
        /// Used to show/hide loading indicator and disable controls.
        /// </summary>
        bool IsLoading { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the form is in edit mode (vs create mode).
        /// </summary>
        bool IsEditMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the save button is enabled.
        /// </summary>
        bool IsSaveEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the delete button is visible.
        /// </summary>
        bool IsDeleteVisible { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Sets a validation error for a specific field.
        /// </summary>
        /// <param name="fieldName">The field name (matches property name).</param>
        /// <param name="errorMessage">The error message to display.</param>
        void SetFieldError(string fieldName, string errorMessage);

        /// <summary>
        /// Clears all validation errors.
        /// </summary>
        void ClearAllErrors();

        /// <summary>
        /// Shows a success message to the user.
        /// </summary>
        /// <param name="message">The message to display.</param>
        void ShowSuccess(string message);

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        void ShowError(string message);

        /// <summary>
        /// Shows a confirmation dialog and returns the user's choice.
        /// </summary>
        /// <param name="message">The confirmation message.</param>
        /// <param name="title">The dialog title.</param>
        /// <returns>True if user confirmed; otherwise, false.</returns>
        bool ShowConfirmation(string message, string title);

        /// <summary>
        /// Closes the view with a dialog result.
        /// </summary>
        /// <param name="success">True if operation was successful; otherwise, false.</param>
        void CloseWithResult(bool success);

        #endregion
    }

    #endregion

    #region Presenter Implementation

    /// <summary>
    /// Presenter for YourEntity edit/create view.
    /// Contains ALL business logic - the Form (View) should be a thin UI shell.
    /// </summary>
    /// <remarks>
    /// MVP Pattern responsibilities:
    /// - Model: Entity classes and services (IYourService)
    /// - View: Form that implements IYourEntityView (thin UI shell)
    /// - Presenter: This class - handles all logic, validation, service calls
    ///
    /// Key principles:
    /// 1. Presenter NEVER references Windows.Forms or UI controls
    /// 2. Presenter ONLY communicates via IYourEntityView interface
    /// 3. ALL business logic lives in Presenter
    /// 4. Form only handles UI wiring and nothing else
    /// </remarks>
    public class YourEntityPresenter : IDisposable
    {
        private readonly IYourEntityView _view;
        private readonly IYourService _service;
        private readonly ILogger<YourEntityPresenter> _logger;
        private YourEntity? _currentEntity;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="YourEntityPresenter"/> class.
        /// </summary>
        /// <param name="view">The view interface.</param>
        /// <param name="service">The entity service.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
        public YourEntityPresenter(
            IYourEntityView view,
            IYourService service,
            ILogger<YourEntityPresenter> logger)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Subscribe to view events
            SubscribeToViewEvents();
        }

        #region Event Subscription

        /// <summary>
        /// Subscribes to all view events.
        /// Call this in constructor.
        /// </summary>
        private void SubscribeToViewEvents()
        {
            _view.LoadRequested += OnLoadRequested;
            _view.SaveRequested += OnSaveRequested;
            _view.CancelRequested += OnCancelRequested;
            _view.DeleteRequested += OnDeleteRequested;
            _view.RefreshRequested += OnRefreshRequested;
        }

        /// <summary>
        /// Unsubscribes from all view events.
        /// Call this in Dispose.
        /// </summary>
        private void UnsubscribeFromViewEvents()
        {
            _view.LoadRequested -= OnLoadRequested;
            _view.SaveRequested -= OnSaveRequested;
            _view.CancelRequested -= OnCancelRequested;
            _view.DeleteRequested -= OnDeleteRequested;
            _view.RefreshRequested -= OnRefreshRequested;
        }

        #endregion

        #region Load Handler

        /// <summary>
        /// Handles the load requested event.
        /// </summary>
        private async void OnLoadRequested(object? sender, EventArgs e)
        {
            await LoadEntityAsync();
        }

        /// <summary>
        /// Loads the entity data.
        /// </summary>
        private async Task LoadEntityAsync()
        {
            try
            {
                // Create mode (new entity)
                if (_view.EntityId == 0)
                {
                    _view.IsEditMode = false;
                    _view.IsDeleteVisible = false;
                    _view.IsSaveEnabled = true;
                    _logger.LogInformation("Creating new entity");
                    return;
                }

                // Edit mode (existing entity)
                _view.IsEditMode = true;
                _view.IsDeleteVisible = true;
                _view.IsLoading = true;

                _logger.LogInformation("Loading entity with ID: {EntityId}", _view.EntityId);

                _currentEntity = await _service.GetByIdAsync(_view.EntityId);

                if (_currentEntity == null)
                {
                    _view.ShowError($"Entity with ID {_view.EntityId} not found.");
                    _view.CloseWithResult(false);
                    return;
                }

                // Populate view with entity data
                PopulateViewFromEntity(_currentEntity);

                _view.IsSaveEnabled = true;
                _logger.LogInformation("Entity loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading entity with ID: {EntityId}", _view.EntityId);
                _view.ShowError($"Failed to load entity: {ex.Message}");
                _view.CloseWithResult(false);
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        /// <summary>
        /// Populates the view properties from the entity.
        /// </summary>
        /// <param name="entity">The entity to display.</param>
        private void PopulateViewFromEntity(YourEntity entity)
        {
            _view.EntityName = entity.Name ?? string.Empty;
            // TODO: Map other properties
            // _view.Description = entity.Description ?? string.Empty;
            // _view.Price = entity.Price;
            // _view.IsActive = entity.IsActive;
        }

        #endregion

        #region Save Handler

        /// <summary>
        /// Handles the save requested event.
        /// </summary>
        private async void OnSaveRequested(object? sender, EventArgs e)
        {
            await SaveEntityAsync();
        }

        /// <summary>
        /// Saves the entity data (create or update).
        /// </summary>
        private async Task SaveEntityAsync()
        {
            try
            {
                _view.ClearAllErrors();
                _view.IsLoading = true;
                _view.IsSaveEnabled = false;

                // Build entity from view
                var entity = BuildEntityFromView();

                // Validate entity
                var validationErrors = ValidateEntity(entity);
                if (validationErrors.Any())
                {
                    _logger.LogWarning("Validation failed for entity");

                    foreach (var error in validationErrors)
                    {
                        _view.SetFieldError(error.Key, error.Value);
                    }

                    _view.ShowError("Please fix the validation errors.");
                    return;
                }

                // Save entity
                if (_view.IsEditMode)
                {
                    // Update existing entity
                    _logger.LogInformation("Updating entity with ID: {EntityId}", entity.Id);
                    await _service.UpdateAsync(entity);
                    _view.ShowSuccess("Entity updated successfully.");
                }
                else
                {
                    // Create new entity
                    _logger.LogInformation("Creating new entity");
                    await _service.CreateAsync(entity);
                    _view.ShowSuccess("Entity created successfully.");
                }

                _view.CloseWithResult(true);
            }
            catch (InvalidOperationException ex)
            {
                // Business logic validation errors
                _logger.LogWarning(ex, "Business validation failed");
                _view.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving entity");
                _view.ShowError($"Failed to save entity: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
                _view.IsSaveEnabled = true;
            }
        }

        /// <summary>
        /// Builds an entity object from the view properties.
        /// </summary>
        /// <returns>The entity populated from view data.</returns>
        private YourEntity BuildEntityFromView()
        {
            var entity = _currentEntity ?? new YourEntity();

            entity.Name = _view.EntityName?.Trim() ?? string.Empty;
            // TODO: Map other properties from view to entity
            // entity.Description = string.IsNullOrWhiteSpace(_view.Description) ? null : _view.Description.Trim();
            // entity.Price = _view.Price;
            // entity.IsActive = _view.IsActive;

            return entity;
        }

        /// <summary>
        /// Validates the entity and returns validation errors.
        /// </summary>
        /// <param name="entity">The entity to validate.</param>
        /// <returns>Dictionary of field names and error messages. Empty if valid.</returns>
        private Dictionary<string, string> ValidateEntity(YourEntity entity)
        {
            var errors = new Dictionary<string, string>();

            // Required field validation
            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                errors[nameof(_view.EntityName)] = "Name is required.";
            }
            else if (entity.Name.Length > 100)
            {
                errors[nameof(_view.EntityName)] = "Name cannot exceed 100 characters.";
            }

            // TODO: Add more validation rules
            // Example:
            // if (entity.Price < 0)
            // {
            //     errors[nameof(_view.Price)] = "Price cannot be negative.";
            // }

            return errors;
        }

        #endregion

        #region Delete Handler

        /// <summary>
        /// Handles the delete requested event.
        /// </summary>
        private async void OnDeleteRequested(object? sender, EventArgs e)
        {
            await DeleteEntityAsync();
        }

        /// <summary>
        /// Deletes the current entity.
        /// </summary>
        private async Task DeleteEntityAsync()
        {
            try
            {
                if (_currentEntity == null || _view.EntityId == 0)
                {
                    _view.ShowError("No entity selected to delete.");
                    return;
                }

                // Confirm deletion
                var confirmed = _view.ShowConfirmation(
                    $"Are you sure you want to delete '{_currentEntity.Name}'?",
                    "Confirm Delete");

                if (!confirmed)
                {
                    return;
                }

                _view.IsLoading = true;
                _logger.LogInformation("Deleting entity with ID: {EntityId}", _view.EntityId);

                await _service.DeleteAsync(_view.EntityId);

                _view.ShowSuccess("Entity deleted successfully.");
                _view.CloseWithResult(true);
            }
            catch (InvalidOperationException ex)
            {
                // Business logic errors (e.g., cannot delete because of dependencies)
                _logger.LogWarning(ex, "Cannot delete entity");
                _view.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity with ID: {EntityId}", _view.EntityId);
                _view.ShowError($"Failed to delete entity: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        #endregion

        #region Other Handlers

        /// <summary>
        /// Handles the cancel requested event.
        /// </summary>
        private void OnCancelRequested(object? sender, EventArgs e)
        {
            _logger.LogInformation("Entity edit cancelled");
            _view.CloseWithResult(false);
        }

        /// <summary>
        /// Handles the refresh requested event.
        /// </summary>
        private async void OnRefreshRequested(object? sender, EventArgs e)
        {
            await LoadEntityAsync();
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Releases unmanaged and managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and managed resources.
        /// </summary>
        /// <param name="disposing">True if called from Dispose(); false if called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Unsubscribe from view events to prevent memory leaks
                UnsubscribeFromViewEvents();
            }

            _disposed = true;
        }

        #endregion
    }

    #endregion
}

#region List Presenter Template

namespace YourNamespace.Presenters
{
    /// <summary>
    /// View interface for YourEntity list form.
    /// </summary>
    public interface IYourEntityListView
    {
        #region Events

        /// <summary>
        /// Raised when the form is loaded.
        /// </summary>
        event EventHandler? LoadRequested;

        /// <summary>
        /// Raised when the user requests to refresh the list.
        /// </summary>
        event EventHandler? RefreshRequested;

        /// <summary>
        /// Raised when the user types in the search box.
        /// </summary>
        event EventHandler<string>? SearchRequested;

        /// <summary>
        /// Raised when the user requests to add a new entity.
        /// </summary>
        event EventHandler? AddRequested;

        /// <summary>
        /// Raised when the user requests to edit an entity.
        /// </summary>
        event EventHandler<int>? EditRequested;

        /// <summary>
        /// Raised when the user requests to delete an entity.
        /// </summary>
        event EventHandler<int>? DeleteRequested;

        /// <summary>
        /// Raised when the user double-clicks a row to view details.
        /// </summary>
        event EventHandler<int>? ViewDetailsRequested;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the list of entities to display.
        /// </summary>
        List<YourEntity> Entities { get; set; }

        /// <summary>
        /// Gets the currently selected entity (if any).
        /// </summary>
        YourEntity? SelectedEntity { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the view is in loading state.
        /// </summary>
        bool IsLoading { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the add button is enabled.
        /// </summary>
        bool IsAddEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the edit button is enabled.
        /// </summary>
        bool IsEditEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the delete button is enabled.
        /// </summary>
        bool IsDeleteEnabled { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Shows a success message to the user.
        /// </summary>
        void ShowSuccess(string message);

        /// <summary>
        /// Shows an error message to the user.
        /// </summary>
        void ShowError(string message);

        /// <summary>
        /// Shows a confirmation dialog.
        /// </summary>
        bool ShowConfirmation(string message, string title);

        /// <summary>
        /// Closes the view.
        /// </summary>
        void Close();

        #endregion
    }

    /// <summary>
    /// Presenter for YourEntity list view.
    /// Handles loading, searching, and CRUD operations for the list.
    /// </summary>
    public class YourEntityListPresenter : IDisposable
    {
        private readonly IYourEntityListView _view;
        private readonly IYourService _service;
        private readonly ILogger<YourEntityListPresenter> _logger;
        private CancellationTokenSource? _searchCts;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="YourEntityListPresenter"/> class.
        /// </summary>
        public YourEntityListPresenter(
            IYourEntityListView view,
            IYourService service,
            ILogger<YourEntityListPresenter> logger)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            _view.LoadRequested += OnLoadRequested;
            _view.RefreshRequested += OnRefreshRequested;
            _view.SearchRequested += OnSearchRequested;
            _view.AddRequested += OnAddRequested;
            _view.EditRequested += OnEditRequested;
            _view.DeleteRequested += OnDeleteRequested;
            _view.ViewDetailsRequested += OnViewDetailsRequested;
        }

        private void UnsubscribeFromViewEvents()
        {
            _view.LoadRequested -= OnLoadRequested;
            _view.RefreshRequested -= OnRefreshRequested;
            _view.SearchRequested -= OnSearchRequested;
            _view.AddRequested -= OnAddRequested;
            _view.EditRequested -= OnEditRequested;
            _view.DeleteRequested -= OnDeleteRequested;
            _view.ViewDetailsRequested -= OnViewDetailsRequested;
        }

        #region Load & Refresh

        private async void OnLoadRequested(object? sender, EventArgs e)
        {
            await LoadEntitiesAsync();
        }

        private async void OnRefreshRequested(object? sender, EventArgs e)
        {
            await LoadEntitiesAsync();
        }

        private async Task LoadEntitiesAsync()
        {
            try
            {
                _logger.LogInformation("Loading entities");
                _view.IsLoading = true;

                var entities = await _service.GetAllAsync();
                _view.Entities = entities;

                _logger.LogInformation("Loaded {Count} entities", entities.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading entities");
                _view.ShowError($"Failed to load entities: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        #endregion

        #region Search

        private async void OnSearchRequested(object? sender, string searchTerm)
        {
            // Cancel previous search if still running
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var cancellationToken = _searchCts.Token;

            try
            {
                // Debounce: wait 300ms before searching
                await Task.Delay(300, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                _logger.LogInformation("Searching entities with term: {SearchTerm}", searchTerm);
                _view.IsLoading = true;

                var entities = await _service.SearchAsync(searchTerm, cancellationToken);
                _view.Entities = entities;

                _logger.LogInformation("Found {Count} entities", entities.Count);
            }
            catch (OperationCanceledException)
            {
                // Search was cancelled, this is expected
                _logger.LogDebug("Search cancelled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching entities");
                _view.ShowError($"Failed to search: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        #endregion

        #region CRUD Actions

        private void OnAddRequested(object? sender, EventArgs e)
        {
            _logger.LogInformation("Add entity requested");
            // Note: The Form handles opening the edit form via FormFactory
            // Presenter just logs and can do pre-validation if needed
        }

        private void OnEditRequested(object? sender, int entityId)
        {
            _logger.LogInformation("Edit entity requested for ID: {EntityId}", entityId);
            // Note: The Form handles opening the edit form via FormFactory
        }

        private async void OnDeleteRequested(object? sender, int entityId)
        {
            try
            {
                var entity = _view.SelectedEntity;
                if (entity == null)
                {
                    _view.ShowError("Please select an entity to delete.");
                    return;
                }

                var confirmed = _view.ShowConfirmation(
                    $"Are you sure you want to delete '{entity.Name}'?",
                    "Confirm Delete");

                if (!confirmed)
                {
                    return;
                }

                _logger.LogInformation("Deleting entity with ID: {EntityId}", entityId);
                _view.IsLoading = true;

                await _service.DeleteAsync(entityId);

                _view.ShowSuccess($"Entity '{entity.Name}' deleted successfully.");
                await LoadEntitiesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity with ID: {EntityId}", entityId);
                _view.ShowError($"Failed to delete: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        private void OnViewDetailsRequested(object? sender, int entityId)
        {
            _logger.LogInformation("View details requested for ID: {EntityId}", entityId);
            // Note: The Form handles opening the details view
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                UnsubscribeFromViewEvents();
                _searchCts?.Cancel();
                _searchCts?.Dispose();
            }

            _disposed = true;
        }

        #endregion
    }
}

#endregion

#region Placeholder Types (Remove when using actual types)

// TODO: Remove these placeholders and use your actual types

namespace YourNamespace.Presenters
{
    /// <summary>
    /// Placeholder entity class. Replace with your actual entity.
    /// </summary>
    public class YourEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Add your entity properties here
    }

    /// <summary>
    /// Placeholder service interface. Replace with your actual service interface.
    /// </summary>
    public interface IYourService
    {
        Task<List<YourEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<YourEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<YourEntity> CreateAsync(YourEntity entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(YourEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<List<YourEntity>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    }
}

#endregion

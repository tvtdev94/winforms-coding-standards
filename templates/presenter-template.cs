// Template: MVP Presenter for WinForms
// Replace: YourEntity, YourPresenter, IYourView, IYourService
// Presenter contains ALL business logic - Form is just a thin UI shell

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace YourNamespace.Presenters
{
    #region Edit View Interface

    /// <summary>
    /// View interface for entity edit form.
    /// </summary>
    public interface IYourEntityView
    {
        // Events
        event EventHandler? LoadRequested;
        event EventHandler? SaveRequested;
        event EventHandler? CancelRequested;
        event EventHandler? DeleteRequested;

        // Data Properties
        int EntityId { get; set; }
        string EntityName { get; set; }
        // TODO: Add more properties for your entity fields

        // UI State
        bool IsLoading { get; set; }
        bool IsEditMode { get; set; }
        bool IsSaveEnabled { get; set; }
        bool IsDeleteVisible { get; set; }

        // Methods
        void SetFieldError(string fieldName, string errorMessage);
        void ClearAllErrors();
        void ShowSuccess(string message);
        void ShowError(string message);
        bool ShowConfirmation(string message, string title);
        void CloseWithResult(bool success);
    }

    #endregion

    #region Edit Presenter

    /// <summary>
    /// Presenter for entity edit/create view.
    /// </summary>
    public class YourEntityPresenter : IDisposable
    {
        private readonly IYourEntityView _view;
        private readonly IYourService _service;
        private readonly ILogger<YourEntityPresenter> _logger;
        private YourEntity? _currentEntity;
        private bool _disposed;

        public YourEntityPresenter(
            IYourEntityView view,
            IYourService service,
            ILogger<YourEntityPresenter> logger)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            SubscribeToViewEvents();
        }

        private void SubscribeToViewEvents()
        {
            _view.LoadRequested += OnLoadRequested;
            _view.SaveRequested += OnSaveRequested;
            _view.CancelRequested += OnCancelRequested;
            _view.DeleteRequested += OnDeleteRequested;
        }

        private void UnsubscribeFromViewEvents()
        {
            _view.LoadRequested -= OnLoadRequested;
            _view.SaveRequested -= OnSaveRequested;
            _view.CancelRequested -= OnCancelRequested;
            _view.DeleteRequested -= OnDeleteRequested;
        }

        #region Load

        private async void OnLoadRequested(object? sender, EventArgs e) => await LoadEntityAsync();

        private async Task LoadEntityAsync()
        {
            try
            {
                if (_view.EntityId == 0)
                {
                    // Create mode
                    _view.IsEditMode = false;
                    _view.IsDeleteVisible = false;
                    _view.IsSaveEnabled = true;
                    return;
                }

                // Edit mode
                _view.IsEditMode = true;
                _view.IsDeleteVisible = true;
                _view.IsLoading = true;

                _currentEntity = await _service.GetByIdAsync(_view.EntityId);

                if (_currentEntity == null)
                {
                    _view.ShowError($"Entity with ID {_view.EntityId} not found.");
                    _view.CloseWithResult(false);
                    return;
                }

                PopulateViewFromEntity(_currentEntity);
                _view.IsSaveEnabled = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading entity");
                _view.ShowError($"Failed to load: {ex.Message}");
                _view.CloseWithResult(false);
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        private void PopulateViewFromEntity(YourEntity entity)
        {
            _view.EntityName = entity.Name ?? string.Empty;
            // TODO: Map other properties
        }

        #endregion

        #region Save

        private async void OnSaveRequested(object? sender, EventArgs e) => await SaveEntityAsync();

        private async Task SaveEntityAsync()
        {
            try
            {
                _view.ClearAllErrors();
                _view.IsLoading = true;
                _view.IsSaveEnabled = false;

                var entity = BuildEntityFromView();
                var errors = ValidateEntity(entity);

                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                        _view.SetFieldError(error.Key, error.Value);
                    _view.ShowError("Please fix the validation errors.");
                    return;
                }

                if (_view.IsEditMode)
                {
                    await _service.UpdateAsync(entity);
                    _view.ShowSuccess("Updated successfully.");
                }
                else
                {
                    await _service.CreateAsync(entity);
                    _view.ShowSuccess("Created successfully.");
                }

                _view.CloseWithResult(true);
            }
            catch (InvalidOperationException ex)
            {
                _view.ShowError(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving entity");
                _view.ShowError($"Failed to save: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
                _view.IsSaveEnabled = true;
            }
        }

        private YourEntity BuildEntityFromView()
        {
            var entity = _currentEntity ?? new YourEntity();
            entity.Name = _view.EntityName?.Trim() ?? string.Empty;
            // TODO: Map other properties
            return entity;
        }

        private Dictionary<string, string> ValidateEntity(YourEntity entity)
        {
            var errors = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(entity.Name))
                errors[nameof(_view.EntityName)] = "Name is required.";
            else if (entity.Name.Length > 100)
                errors[nameof(_view.EntityName)] = "Name cannot exceed 100 characters.";

            // TODO: Add more validation rules
            return errors;
        }

        #endregion

        #region Delete

        private async void OnDeleteRequested(object? sender, EventArgs e) => await DeleteEntityAsync();

        private async Task DeleteEntityAsync()
        {
            try
            {
                if (_currentEntity == null) return;

                if (!_view.ShowConfirmation($"Delete '{_currentEntity.Name}'?", "Confirm Delete"))
                    return;

                _view.IsLoading = true;
                await _service.DeleteAsync(_view.EntityId);
                _view.ShowSuccess("Deleted successfully.");
                _view.CloseWithResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting entity");
                _view.ShowError($"Failed to delete: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        #endregion

        private void OnCancelRequested(object? sender, EventArgs e) => _view.CloseWithResult(false);

        public void Dispose()
        {
            if (_disposed) return;
            UnsubscribeFromViewEvents();
            _disposed = true;
        }
    }

    #endregion

    #region List View Interface

    /// <summary>
    /// View interface for entity list form.
    /// </summary>
    public interface IYourEntityListView
    {
        // Events
        event EventHandler? LoadRequested;
        event EventHandler? RefreshRequested;
        event EventHandler<string>? SearchRequested;
        event EventHandler? AddRequested;
        event EventHandler<int>? EditRequested;
        event EventHandler<int>? DeleteRequested;

        // Properties
        List<YourEntity> Entities { get; set; }
        YourEntity? SelectedEntity { get; }
        bool IsLoading { get; set; }
        bool IsAddEnabled { get; set; }
        bool IsEditEnabled { get; set; }
        bool IsDeleteEnabled { get; set; }

        // Methods
        void ShowSuccess(string message);
        void ShowError(string message);
        bool ShowConfirmation(string message, string title);
    }

    #endregion

    #region List Presenter

    /// <summary>
    /// Presenter for entity list view.
    /// </summary>
    public class YourEntityListPresenter : IDisposable
    {
        private readonly IYourEntityListView _view;
        private readonly IYourService _service;
        private readonly ILogger<YourEntityListPresenter> _logger;
        private CancellationTokenSource? _searchCts;
        private bool _disposed;

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
            _view.DeleteRequested += OnDeleteRequested;
        }

        private void UnsubscribeFromViewEvents()
        {
            _view.LoadRequested -= OnLoadRequested;
            _view.RefreshRequested -= OnRefreshRequested;
            _view.SearchRequested -= OnSearchRequested;
            _view.DeleteRequested -= OnDeleteRequested;
        }

        private async void OnLoadRequested(object? sender, EventArgs e) => await LoadEntitiesAsync();
        private async void OnRefreshRequested(object? sender, EventArgs e) => await LoadEntitiesAsync();

        private async Task LoadEntitiesAsync()
        {
            try
            {
                _view.IsLoading = true;
                _view.Entities = await _service.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading entities");
                _view.ShowError($"Failed to load: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        private async void OnSearchRequested(object? sender, string searchTerm)
        {
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();

            try
            {
                await Task.Delay(300, _searchCts.Token); // Debounce
                _view.IsLoading = true;
                _view.Entities = await _service.SearchAsync(searchTerm, _searchCts.Token);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching");
                _view.ShowError($"Search failed: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        private async void OnDeleteRequested(object? sender, int entityId)
        {
            try
            {
                var entity = _view.SelectedEntity;
                if (entity == null) return;

                if (!_view.ShowConfirmation($"Delete '{entity.Name}'?", "Confirm Delete"))
                    return;

                _view.IsLoading = true;
                await _service.DeleteAsync(entityId);
                _view.ShowSuccess("Deleted successfully.");
                await LoadEntitiesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting");
                _view.ShowError($"Failed to delete: {ex.Message}");
            }
            finally
            {
                _view.IsLoading = false;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            UnsubscribeFromViewEvents();
            _searchCts?.Dispose();
            _disposed = true;
        }
    }

    #endregion

    #region Placeholder Types (Replace with actual types)

    public class YourEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public interface IYourService
    {
        Task<List<YourEntity>> GetAllAsync(CancellationToken ct = default);
        Task<YourEntity?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<YourEntity> CreateAsync(YourEntity entity, CancellationToken ct = default);
        Task UpdateAsync(YourEntity entity, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task<List<YourEntity>> SearchAsync(string term, CancellationToken ct = default);
    }

    #endregion
}

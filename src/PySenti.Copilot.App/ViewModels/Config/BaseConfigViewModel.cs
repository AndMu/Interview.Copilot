using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;

namespace PySenti.Copilot.App.ViewModels.Config;

public abstract class BaseConfigViewModel<T> : ObservableObject
    where T : class
{
    private T? selectedItem;
    private T? editingItem;
    private bool isEditing;
    private bool isAddingNew;
    private ObservableCollection<T> items;

    protected BaseConfigViewModel()
    {
        AddCommand = new RelayCommand(Add, () => !IsEditing);
        EditCommand = new RelayCommand(Edit, () => SelectedItem != null && !IsEditing);
        RemoveCommand = new RelayCommand(RemoveItem, () => SelectedItem != null && !IsEditing);
        SaveCommand = new RelayCommand(Save, () => IsEditing);
        CancelCommand = new RelayCommand(CancelEdit, () => IsEditing);
        items = [];
    }

    public T? SelectedItem
    {
        get => selectedItem;
        set
        {
            SetProperty(ref selectedItem, value);
            UpdateCommandStates();
        }
    }

    public T? EditingItem
    {
        get => editingItem;
        set
        {
            SetProperty(ref editingItem, value);
            UpdateCommandStates();
        }
    }

    public bool IsEditing
    {
        get => isEditing;
        private set
        {
            SetProperty(ref isEditing, value);
            UpdateCommandStates();
        }
    }

    public ObservableCollection<T> Items
    {
        get => items;
        private set
        {
            SetProperty(ref items, value);
            UpdateCommandStates();
        }
    }

    public IRelayCommand AddCommand { get; }

    public IRelayCommand EditCommand { get; }

    public IRelayCommand RemoveCommand { get; }

    public IRelayCommand SaveCommand { get; }

    public IRelayCommand CancelCommand { get; }

    public abstract Task Load();

    protected abstract bool CanSave();

    protected virtual void OnSaved()
    {
    }

    protected virtual void UpdateCommandStates()
    {
        AddCommand.NotifyCanExecuteChanged();
        EditCommand.NotifyCanExecuteChanged();
        RemoveCommand.NotifyCanExecuteChanged();
        SaveCommand.NotifyCanExecuteChanged();
        CancelCommand.NotifyCanExecuteChanged();
    }

    protected abstract T Create();

    private void Edit()
    {
        if (SelectedItem == null)
        {
            return;
        }

        isAddingNew = false;
        EditingItem = SelectedItem;
        IsEditing = true;
    }

    private void Add()
    {
        isAddingNew = true;
        EditingItem = Create();
        IsEditing = true;
    }

    private void RemoveItem()
    {
        if (SelectedItem == null)
        {
            return;
        }

        if (Items.Count <= 1)
        {
            MessageBox.Show("At least one item must remain.", "Cannot Delete", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        
        Items.Remove(SelectedItem);
        SelectedItem = null;
        EditingItem = null;
        OnSaved();
    }

    private void Save()
    {
        if (EditingItem == null)
        {
            return;
        }

        if (!CanSave())
        {
            MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (isAddingNew)
        {
            Items.Add(EditingItem);            
        }
        
        SelectedItem = EditingItem;
        EditingItem = null;
        IsEditing = false;
        OnSaved();
    }

    private void CancelEdit()
    {
        EditingItem = null;
        IsEditing = false;
    }
}
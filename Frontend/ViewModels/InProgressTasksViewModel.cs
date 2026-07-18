using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using Backend.ServiceLayer;
using Frontend.Infrastructure;
using Frontend.Models;

namespace Frontend.ViewModels
{
    /// <summary>
    /// ViewModel for the "My In Progress Tasks" screen (Requirement 22): a read-only,
    /// consolidated list of every task assigned to the current user that is currently
    /// in the "in progress" column, across all of their boards.
    /// </summary>
    // A- this screen is read-only (no edit/advance/assign) because TaskService.InProgressTasks
    // A- returns plain Task data with no board reference attached - there is no boardName available
    // A- here to call UpdateTask*/AdvanceTask/AssignTask with, so editing isn't possible from this
    // A- view. That matches the requirement's wording too: it only asks to "list" these tasks.
    public class InProgressTasksViewModel : ViewModelBase
    {
        private readonly TaskService _taskService;
        private readonly string _currentUserEmail;

        private string _errorMessage = string.Empty;

        /// <summary>Raised when the user chooses to go back to the boards list.</summary>
        public event Action BackRequested;

        /// <summary>
        /// Creates the InProgressTasksViewModel and immediately loads the user's in-progress tasks.
        /// </summary>
        /// <param name="taskService">The shared, DB-connected TaskService built by the composition root.</param>
        /// <param name="currentUserEmail">The email of the currently logged-in user.</param>
        public InProgressTasksViewModel(TaskService taskService, string currentUserEmail)
        {
            _taskService = taskService;
            _currentUserEmail = currentUserEmail;

            Tasks = new ObservableCollection<TaskModel>();
            BackCommand = new RelayCommand(_ => BackRequested?.Invoke());

            LoadTasks();
        }

        /// <summary>The current user's in-progress tasks, across every board they belong to.</summary>
        public ObservableCollection<TaskModel> Tasks { get; }

        /// <summary>The last error message to display, or empty if there is none.</summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetField(ref _errorMessage, value);
        }

        /// <summary>Command bound to the "Back" button, returning to the boards list.</summary>
        public RelayCommand BackCommand { get; }

        /// <summary>
        /// Loads every in-progress task assigned to the current user, from all of their boards.
        /// </summary>
        private void LoadTasks()
        {
            Tasks.Clear();

            string json = _taskService.InProgressTasks(_currentUserEmail);
            var response = JsonSerializer.Deserialize<ServiceResponse<List<TaskModel>>>(json, JsonOptions);

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                ErrorMessage = response.ErrorMessage;
                return;
            }

            foreach (TaskModel task in response.ReturnValue ?? new List<TaskModel>())
                Tasks.Add(task);

            ErrorMessage = string.Empty;
        }
    }
}

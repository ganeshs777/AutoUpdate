using System;
using Microsoft.Win32.TaskScheduler;

namespace AutoUpdater
{
    public class TaskSchedulerHelper
    {
        public static void CreateDailyTask(string taskName, string executablePath)
        {
            // Create a new task definition
            using (TaskService taskService = new TaskService())
            {
                TaskDefinition taskDefinition = taskService.NewTask();
                taskDefinition.RegistrationInfo.Description = "Auto Updater Task";

                // Set the trigger to run daily
                DailyTrigger dailyTrigger = new DailyTrigger();
                dailyTrigger.StartBoundary = DateTime.Today.AddHours(1); // Start at 1 AM
                dailyTrigger.DaysInterval = 1; // Run every day
                taskDefinition.Triggers.Add(dailyTrigger);

                // Set the action to run the executable
                ExecAction execAction = new ExecAction(executablePath);
                taskDefinition.Actions.Add(execAction);

                // Register the task in the root folder
                taskService.RootFolder.RegisterTaskDefinition(taskName, taskDefinition);
            }
        }

        public static bool TaskExists(string taskName)
        {
            try
            {
                using (TaskService taskService = new TaskService())
                {
                    Microsoft.Win32.TaskScheduler.Task task = taskService.GetTask(taskName);
                    return task != null;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void DeleteTask(string taskName)
        {
            // Delete the task if it exists
            using (TaskService taskService = new TaskService())
            {
                taskService.RootFolder.DeleteTask(taskName, false);
            }
        }
    

    public static void Main(string[] args)
        {
            // Check for updates
            AutoUpdater.CheckForUpdates();

            // Check if the task exists before creating it
            string taskName = "AutoUpdaterSystem1Task";
            if (!TaskSchedulerHelper.TaskExists(taskName))
            {
                TaskSchedulerHelper.CreateDailyTask(taskName, "C:\\Program Files\\AutoUpdater\\AutoUpdater.exe");
            }
            else
                Console.WriteLine("AutoUpdaterSystem1Task is already exists in Task Scheduler!");

        }

    }
}

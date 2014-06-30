using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToDoList
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            EventManager.RegisterClassHandler(typeof(TextBox),
                                              TextBox.KeyUpEvent,
                                              new System.Windows.Input.KeyEventHandler(textbox_keyup));
        }

        // attach a global event handler to allows enter keypresses as accepts
        private void textbox_keyup(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key != System.Windows.Input.Key.Enter) return;

            // your event handler here
            e.Handled = true;
            yes_click(sender, e);
        }

        private void add_click(object sender, RoutedEventArgs e)
        {
            // show add/edit task window on click
            InputBox.Visibility = System.Windows.Visibility.Visible;
            g_edit = false;
        }

        private void edit_click(object sender, RoutedEventArgs e)
        {
            // show add/edit task window on click (if selection valid)
            if (ToDoListView.SelectedIndex != -1)
            {
                InputBox.Visibility = System.Windows.Visibility.Visible;
                g_edit = true;

                // Set text box values to selected task's values
                if(ToDoListView.SelectedItem is MyTask)
                {
                    // Check for type correctness, cast object to a task, and then change text box
                    MyTask tempTask = (MyTask)ToDoListView.SelectedItem;
                    InputTextBox.Text = tempTask.task;
                }

                DateTime date = new DateTime(g_dueDates[ToDoListView.SelectedIndex]);
                DatePickerBox.Text = date.ToShortDateString();
            }
            else
            {
                //Display error
                MessageBox.Show("Select a task to edit.", "Error");
            }
        }

        private void remove_click(object sender, RoutedEventArgs e)
        {
            if (ToDoListView.SelectedIndex != -1)
            {
                //Remove item and corresponding due date
                g_dueDates.RemoveAt(ToDoListView.SelectedIndex);
                ToDoListView.Items.RemoveAt(ToDoListView.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Select a task to remove.", "Error");
            }
        }

        private void yes_click(object sender, RoutedEventArgs e)
        {
            // close window
            InputBox.Visibility = System.Windows.Visibility.Collapsed;

            String currentTask = InputTextBox.Text;
            int index = 0;

            if (g_edit)
            {
                // If we are in edit task mode, we remove the old task out and add the editted one
                g_dueDates.RemoveAt(ToDoListView.SelectedIndex);
                ToDoListView.Items.Remove(ToDoListView.SelectedItem); 
            }

            foreach (long date in g_dueDates)
            {
                // Loop through each due date and find out where our current date fits
                // This determines where it'll be placed in the list box
                if (g_currentDateLong < g_dueDates[index])
                {
                    //g_dueDates is in ascending order, so once we find something larger we break
                    break;
                }
                index++;
            }

            //Now that we have the correct index we can insert the task
            g_dueDates.Insert(index, g_currentDateLong);
            ToDoListView.Items.Insert(index, new MyTask{ task = currentTask, dueDate = g_currentDate }); 
            
            // clear out text box and reset date string for next time
            InputTextBox.Text = String.Empty;
            DatePickerBox.Text = DateTime.Now.ToShortDateString();
        }

        private void cancel_click(object sender, RoutedEventArgs e)
        {
            // close window
            InputBox.Visibility = System.Windows.Visibility.Collapsed;

            // clear out text box and reset date string for next time
            InputTextBox.Text = String.Empty;
            DatePickerBox.Text = DateTime.Now.ToShortDateString();
        }

        private void selected_date_changed(object sender, SelectionChangedEventArgs e)
        {
            // ... Get DatePicker reference.
            var picker = sender as DatePicker;

            // ... Get nullable DateTime from SelectedDate.
            DateTime? date = picker.SelectedDate;
            if (date != null)
            {
                // Chop off time and set current date

                g_currentDate = date.Value.ToShortDateString();
                g_currentDateLong = date.Value.Ticks;
            }
        }

        private String g_currentDate;
        private long g_currentDateLong;
        private bool g_edit;

        //Contains the list of corresponding due dates
        //Used for sorting
        private List<long> g_dueDates = new List<long>();
    }

    public class MyTask
    {
        public String task { get; set; }
        public String dueDate { get; set; }
    }

}

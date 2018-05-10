using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace TestWinForm.Utils
{
    class ControlUtil
    {
        public static Label create_label(string text, Size size, Point location)
        {
            Label label = new Label();
            label.Text = text;
            label.Size = size;
            label.Location = location;
            return label;
        }
        public static TextBox create_textbox(string text, Size size, Point location, KeyPressEventHandler keypress_event, EventHandler textchanged_event)
        {
            TextBox textbox = new TextBox();
            textbox.Text = text;
            textbox.Size = size;
            textbox.Location = location;
            textbox.KeyPress += keypress_event;
            textbox.TextChanged += textchanged_event;
            return textbox;
        }
        public static ComboBox create_combobox(object[] values, object selecet_item, Size size, Point location, EventHandler selected_index_changed_event)
        {
            ComboBox combobox = new ComboBox();
            combobox.Items.AddRange(values);
            combobox.SelectedItem = selecet_item;
            combobox.Size = size;
            combobox.Location = location;
            combobox.SelectedIndexChanged += selected_index_changed_event;
            return combobox;
        }
    }
}

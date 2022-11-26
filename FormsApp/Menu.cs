using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormsApp
{
    internal class Menu
    {
        string? title;
        Database.DatabaseAccess db;
        List<string> options;
        List<Action<Menu, Database.DatabaseAccess>> functions;

        public int? UserInput { get; set; }
        public List<Control> Controls { get; set; }
        public Form Form { get; set; }

        public Menu(Form form, string? title, Database.DatabaseAccess db, List<string> options, List<Action<Menu, Database.DatabaseAccess>> functions) 
        {
            if (options == null || functions == null || options.Count != functions.Count)
            {
                throw new Exception("Wrong arguements!");
            }

            this.title = title;
            this.db = db;
            this.options = options;
            this.functions = functions;
            this.Form = form;
            
            UserInput = null;
            Controls = new List<Control>();
        }

        public override string ToString()
        {
            string res = title + "\n\n";

            for (int i = 0; i < options.Count; i++)
            {
                res += (i + 1).ToString() + ". " + options[i].ToString() + "\n";
            }

            res += "\nSelect an option: \n";

            return res;
        }

        public void Input(Label lblText)
        {
            TextBox txt = new TextBox
            {
                Width = 100,
                Height = 20,
                MaxLength = (int) Math.Ceiling(Math.Log10(options.Count)),
                Location = new Point(Form.Width / 2, lblText.Height),
            };

            Form.Controls.Add(txt);
            Controls.Add(txt);

            Button btnSubmit = new Button
            {
                Width = 100,
                Height = 50,
                Text = "Submit",
                Location = new Point(Form.Width / 2, txt.Top + txt.Height),
            };

            Form.Controls.Add(btnSubmit);
            Controls.Add(btnSubmit);

            btnSubmit.Click += (s, e) => { UserInput = Convert.ToInt32(txt.Text); HandleInput();  };
        }

        public void HandleInput()
        {
            if ((UserInput == null) || (UserInput > functions.Count) || (UserInput < 1))
            {
                throw new Exception("Invalid input!");
            }

            functions[(int) UserInput - 1](this, db);
        }

        public void Hide()
        {
            foreach (Control control in Controls)
            {
                control.Dispose();
            }
        }

        public void Run() 
        {
            Label lblText = new Label
            {
                Text = ToString(),
                Location = new Point(Form.Width / 2, 0),
                AutoSize = true,
            };

            Form.Controls.Add(lblText);
            Controls.Add(lblText);

            Input(lblText);
        }
    }
}

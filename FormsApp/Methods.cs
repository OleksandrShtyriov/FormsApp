using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormsApp
{
    internal class Methods
    {
        public static void Hide(List<Control> controls)
        {
            foreach (Control control in controls)
            {
                control.Dispose();
            }
        }
        private static List<Control> loginFields(Form form)
        {
            Label lblUsername = new Label
            {
                Text = "Username: ",
                AutoSize = true,
                Location = new Point(form.Width / 2, 0),
                Height = 30,
            };
            form.Controls.Add(lblUsername);

            Label lblPassword = new Label
            {
                Text = "Password: ",
                AutoSize = true,
                Location = new Point(form.Width / 2, lblUsername.Height),
                Height = 30,
            };
            form.Controls.Add(lblPassword);

            TextBox txtUsername = new TextBox
            {
                Location = new Point(lblUsername.Left + lblUsername.Width, 0),
                Height = 30,
            };
            form.Controls.Add(txtUsername);

            TextBox txtPassword = new TextBox
            {
                Location = new Point(lblPassword.Left + lblPassword.Width, txtUsername.Height),
            };
            form.Controls.Add(txtPassword);

            Button btnSubmit = new Button
            {
                Text = "Submit",
                Location = new Point((lblPassword.Left + txtPassword.Left + txtPassword.Width) / 2, (txtPassword.Top + txtPassword.Height) + 50),
                Width = 100,
                Height = 50,
            };
            form.Controls.Add(btnSubmit);

            return new List<Control> { txtUsername, txtPassword, btnSubmit, lblUsername, lblPassword };
        }

        private static (string, string) getInput(TextBox txtUsername, TextBox txtPassword)
        {
            string username = txtUsername.Text;
            string password = txtPassword.Text;

            return (username, password);
        }

        public static void HandleLogin(Menu menu, Database.DatabaseAccess db)
        {
            menu.Hide();
            List<Control> controls = loginFields(menu.Form);
            TextBox txtUsername = (TextBox)controls[0];
            TextBox txtPassword = (TextBox)controls[1];
            Button btnSubmit = (Button)controls[2];

            btnSubmit.Click += (s, e) =>
            {
                (string username, string password) = getInput(txtUsername, txtPassword);
                string user = db.FindEqual("dbo.Users", new List<(string, string)> { ("Username", username), ("Password", password) });
                string[] userFields = user.Split(' ');

                Hide(controls);

                if (user != "")
                {
                    if (userFields[userFields.Length - 2] == "manager")
                    {
                        ToMainMenu(menu, db);
                    }
                    else
                    {
                        (new Menu(menu.Form, "Main menu", db, new List<string> { "Logout" }, new List<Action<Menu, Database.DatabaseAccess>> { HandleLogout })).Run();
                    }
                }
                else
                {
                    throw (new Exception("Wrong username or password!"));
                }
            };
        }

        public static void HandleRegister(Menu menu, Database.DatabaseAccess db)
        {
            menu.Hide();
            List<Control> controls = loginFields(menu.Form);
            TextBox txtUsername = (TextBox)controls[0];
            TextBox txtPassword = (TextBox)controls[1];
            Button btnSubmit = (Button)controls[2];

            btnSubmit.Click += (s, e) =>
            {
                (string username, string password) = getInput(txtUsername, txtPassword);
                Hide(controls);
                db.Insert("dbo.Users", new List<string> { username, password, "guest" });

                ToMainMenu(menu, db);
            };
        }

        public static void ToMainMenu(Menu menu, Database.DatabaseAccess db)
        {
            Menu mainMenu = new Menu(menu.Form, "Main menu", db,
                                     new List<string> { "View actions", "Add an action", "Modify an action", "Logout" },
                                     new List<Action<Menu, Database.DatabaseAccess>> { HandleView, HandleAdd, HandleModify, HandleLogout });
            menu.Hide();
            mainMenu.Run();
        }

        public static void HandleLogout(Menu menu, Database.DatabaseAccess db)
        {
            Application.Exit();
        }

        public static void HandleView(Menu menu, Database.DatabaseAccess db)
        {
            Menu actionsMenu = new Menu(menu.Form, "Actions menu", db,
                                        new List<string> { "View all actions", "View active actions", "View past actions", "View future actions", "Back to main menu", "Logout" },
                                        new List<Action<Menu, Database.DatabaseAccess>> { HandleAll, HandleActive, HandlePast, HandleFuture, ToMainMenu, HandleLogout });
            menu.Hide();

            actionsMenu.Run();
        }

        private static List<Control> viewScreen(Menu menu)
        {
            List<Control> controls = new List<Control>();
            Form form = menu.Form;

            menu.Hide();

            Label lblActions = new Label
            {
                Text = "",
                Location = new Point(form.Width / 2, 0),
                AutoSize = true,
            };
            controls.Add(lblActions);
            form.Controls.Add(lblActions);

            Button btnOk = new Button
            {
                Text = "OK",
                Location = new Point(0, lblActions.Height),
                Height = 50,
                Width = 100,
            };

            btnOk.Click += (s, e) =>
            {
                Hide(controls);
                menu.Run();
            };

            controls.Add(btnOk);
            form.Controls.Add(btnOk);

            return controls;
        }

        public static void HandleAll(Menu menu, Database.DatabaseAccess db)
        {
            Label lblActions = (Label)viewScreen(menu)[0];
            string result = db.FindAll("Actions");

            if (result != "")
            {
                lblActions.Text = result;
            }
            else
            {
                lblActions.Text = "There are no actions yet.";
            }
        }

        public static void HandlePast(Menu menu, Database.DatabaseAccess db)
        {
            Label lblActions = (Label)viewScreen(menu)[0];
            string result = db.FindEqual("Actions", new List<(string, string)> { ("State", "past") });

            if (result != "")
            {
                lblActions.Text = result;
            }
            else
            {
                lblActions.Text = "There are no actions yet.";
            }
        }

        public static void HandleActive(Menu menu, Database.DatabaseAccess db)
        {
            Label lblActions = (Label)viewScreen(menu)[0];
            string result = db.FindEqual("Actions", new List<(string, string)> { ("State", "active") });

            if (result != "")
            {
                lblActions.Text = result;
            }
            else
            {
                lblActions.Text = "There are no actions yet.";
            }
        }

        public static void HandleFuture(Menu menu, Database.DatabaseAccess db)
        {
            Label lblActions = (Label)viewScreen(menu)[0];
            string result = db.FindEqual("Actions", new List<(string, string)> { ("State", "future") });

            if (result != "")
            {
                lblActions.Text = result;
            }
            else
            {
                lblActions.Text = "There are no actions yet.";
            }
        }

        private static void modifyScreen(Menu menu, Database.DatabaseAccess db)
        {
            List<Control> controls = new List<Control>();
            List<TextBox> textBoxes = new List<TextBox>();
            Form form = menu.Form;
            List<string> names = new List<string>
            {
                "Article: ",
                "Discount: ",
                "New state: ",
                "New article: ",
                "New discount: ",
            };

            int y = 0;

            foreach (string name in names) 
            {
                Label lbl = new Label
                {
                    Text = name,
                    Location = new Point(form.Width / 2, y),
                    Height = 30,
                    Width = 200,
                };
                controls.Add(lbl);
                form.Controls.Add(lbl);

                TextBox txt = new TextBox
                {
                    Location = new Point(lbl.Right, y),
                    Height = 30,
                    Width = 100,
                };
                controls.Add(txt); 
                form.Controls.Add(txt);
                textBoxes.Add(txt);

                y += 30;
            }

            Button btnSubmit = new Button
            {
                Text = "Submit",
                Location = new Point(form.Width / 2, y),
                Width = 100,
                Height = 50,
            };

            btnSubmit.Click += (s, e) =>
            {
                string article, discount, newState, newArticle, newDiscount;

                article = textBoxes[0].Text;
                discount = textBoxes[1].Text;
                newState = textBoxes[2].Text;
                newArticle = textBoxes[3].Text;
                newDiscount = textBoxes[4].Text;

                if (article == "" || discount == "" || newState == "" || newArticle == "" || newDiscount == "")
                {
                    throw new Exception("Wrong values!");
                }

                db.Update("Actions", new List<(string, string)> { ("State", newState), ("Article", newArticle), ("Discount", newDiscount) },
                          new List<(string, string)> { ("State", "future"), ("Article", article), ("Discount", discount) });

                Hide(controls);
                menu.Run();
            };

            form.Controls.Add(btnSubmit);
            controls.Add(btnSubmit);
        }

        private static void addScreen(Menu menu, Database.DatabaseAccess db)
        {
            List<Control> controls = new List<Control>();
            List<TextBox> textBoxes = new List<TextBox>();
            Form form = menu.Form;
            List<string> names = new List<string>
            {
                "State: ",
                "Article: ",
                "Discount: ",
            };

            int y = 0;

            foreach (string name in names)
            {
                Label lbl = new Label
                {
                    Text = name,
                    Location = new Point(form.Width / 2, y),
                    Height = 30,
                    Width = 100,
                };
                controls.Add(lbl);
                form.Controls.Add(lbl);

                TextBox txt = new TextBox
                {
                    Location = new Point(lbl.Right, y),
                    Height = 30,
                    Width = 100,
                };
                controls.Add(txt);
                form.Controls.Add(txt);
                textBoxes.Add(txt);

                y += 30;
            }

            Button btnSubmit = new Button
            {
                Text = "Submit",
                Location = new Point(form.Width / 2, y),
                Width = 100,
                Height = 50,
            };

            btnSubmit.Click += (s, e) =>
            {
                string state, article, discount;

                state = textBoxes[0].Text;
                article = textBoxes[1].Text;
                discount = textBoxes[2].Text;

                if (state == "" || article == "" || discount == "")
                {
                    throw new Exception("Wrong values!");
                }

                db.Insert("Actions", new List<string> { state, article, discount.ToString() });

                Hide(controls);
                menu.Run();
            };

            controls.Add(btnSubmit);
            form.Controls.Add(btnSubmit);
        }

        public static void HandleModify(Menu menu, Database.DatabaseAccess db)
        {
            menu.Hide();
            modifyScreen(menu, db);
        }

        public static void HandleAdd(Menu menu, Database.DatabaseAccess db)
        {
            menu.Hide();
            addScreen(menu, db);
        }
    }
}

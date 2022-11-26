namespace FormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string connectionString = "Data Source =.; Initial Catalog = StockManager; Integrated Security = True;";
            Database.DatabaseAccess db = new Database.DatabaseAccess(connectionString);
            Menu loginMenu = new Menu(this, "Stock manager menu", db, new List<string> { "Login", "Register" }, new List<Action<Menu, Database.DatabaseAccess>> { Methods.HandleLogin, Methods.HandleRegister });
            loginMenu.Run();
        }
    }
}

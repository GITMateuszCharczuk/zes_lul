namespace MyMauiApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        // Set the content of ShellContent directly
        ShellContent productsContent = new ShellContent
        {
            Title = "Products",
            Content = new ProductsView()  // Directly assigning ProductsView as content
        };
        Items.Add(productsContent);  // Add it to the shell's items
    }
}

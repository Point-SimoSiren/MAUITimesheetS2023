namespace MAUITimesheetS2023;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new EmployeePage();
	}
}

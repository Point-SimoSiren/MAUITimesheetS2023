namespace MAUITimesheetS2023;

public partial class MainPage : TabbedPage
{

	public MainPage()
	{
		InitializeComponent();
		Liuku1.Minimum = 0;
        Liuku1.Maximum = 100;

    }


    private void OmaNappi_Clicked(object sender, EventArgs e)
    {
		OmaLabel.Text = "Hello Careeria!";
		OmaLabel.TextColor = Colors.Orange;
		OmaNappi.RotateTo(360, 400);
	
    }

}


using MAUITimesheetS2023.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace MAUITimesheetS2023;

public partial class EmployeePage : ContentPage
{


    // Muuttujan alustaminen päätasolla jotta hakufunktio näkee muuttujan
    ObservableCollection<Employee> dataa = new ObservableCollection<Employee>();

#if DEBUG
    private static readonly string Base = "http://10.0.2.2";
    private static readonly string ApiBaseUrl = $"{Base}:5126/";

#else
    private static readonly string ApiBaseUrl = "https://jotain.azurewebsites.net";
#endif



    public EmployeePage()
	{
		InitializeComponent();


        emp_lataus.Text = "Ladataan työntekijöitä...";

        
       LoadDataFromRestApi();
    }

    async void LoadDataFromRestApi()
    {
    

        try
        {

#if DEBUG
            HttpsClientHandlerService handler = new HttpsClientHandlerService();
            HttpClient client = new HttpClient(handler.GetPlatformMessageHandler());
#else
                    client = new HttpClient();
#endif

            client.BaseAddress = new Uri(ApiBaseUrl);
            string json = await client.GetStringAsync("api/employees");

            IEnumerable<Employee> employees = JsonConvert.DeserializeObject<Employee[]>(json);
            // dataa -niminen observableCollection on alustettukin jo ylhäällä päätasolla että hakutoiminto,
            // pääsee siihen käsiksi.
            dataa = new ObservableCollection<Employee>(employees);

            // Asetetaan datat näkyviin xaml tiedostossa olevalle listalle
            employeeList.ItemsSource = dataa;

            // Tyhjennetään latausilmoitus label
            emp_lataus.Text = "";

        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", ex.InnerException.ToString(), "ok");
        }

    }

}


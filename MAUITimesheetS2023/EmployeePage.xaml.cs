using MAUITimesheetS2023.Models;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace MAUITimesheetS2023;

public partial class EmployeePage : ContentPage
{


    // Muuttujan alustaminen päätasolla jotta hakufunktio näkee muuttujan
    ObservableCollection<Employee> dataa = new ObservableCollection<Employee>();

/*
    private static readonly string Base = "http://10.0.2.2";
    private static readonly string ApiBaseUrl = $"{Base}:5001/";
*/

    private static readonly string ApiBaseUrl = "https://tuntiapi.azurewebsites.net";


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

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(ApiBaseUrl);
            string json = await client.GetStringAsync("api/employees");

            // json data deserialisoidaan json muodosta C# muotoon Employee tyyppiseksi taulukoksi
            IEnumerable<Employee> employees = JsonConvert.DeserializeObject<Employee[]>(json);

            // dataa -niminen observableCollection on alustettukin jo ylhäällä päätasolla että hakutoiminto
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

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
    // SearchBar searchBar = (SearchBar)sender;   alla sama asia eri syntksilla
        SearchBar searchBar = sender as SearchBar;

        string searchText = searchBar.Text;

        // Työntekijälistaukseen valitaan nyt vain ne joiden etu- tai sukunimeen sisältyy annettu hakutermi
        // "var dataa" on tiedoston päätasolla alustettu muuttuja, johon sijoitettiin alussa koko lista työntekijöistä.
        // Nyt siihen sijoitetaan vain hakuehdon täyttävät työntekijät
        employeeList.ItemsSource = dataa.Where(x => x.LastName.ToLower().Contains(searchText.ToLower())
        || x.FirstName.ToLower().Contains(searchText.ToLower()));

    }

    async void navibutton_Clicked(object sender, EventArgs e)
    {
        Employee selected = (Employee)employeeList.SelectedItem;

        if (selected != null)
        {
            int eid = selected.IdEmployee;
            await Navigation.PushAsync(new WorkassignmentPage(eid));
        }
        else
        {
            await DisplayAlert("Huomio", "Valitse ensin työntekijä", "ok");
        }
    }
}





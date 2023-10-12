using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Maui.Devices.Sensors;
using System.Collections.ObjectModel;
using System.ComponentModel;
using MAUITimesheetS2023.Models;
using System.Text;

namespace MAUITimesheetS2023;

public partial class WorkassignmentPage : ContentPage
{

    int empId;
    string lat;
    string lon;
    // MAUI Geolocation dokumentaation ohjeen mukaan laitettu:
    private CancellationTokenSource _cancelTokenSource;
    private bool _isCheckingLocation;


    // Muuttujan alustaminen p��tasolla jotta hakufunktio n�kee muuttujan
    ObservableCollection<WorkAssignment> dataa = new ObservableCollection<WorkAssignment>();

    /*
        private static readonly string Base = "http://10.0.2.2";
        private static readonly string ApiBaseUrl = $"{Base}:5001/";
    */

    private static readonly string ApiBaseUrl = "https://tuntiapi.azurewebsites.net";

    public WorkassignmentPage(int eid)
	{

		InitializeComponent();

        empId= eid;

        waLataus.Text = "Ladataan ty�teht�vi�...";

        latLabel.Text = "Haetaan sijaintitietoa...";

        GetCurrentLocation();
            
        LoadDataFromRestApi();

    }

    // Sijainnin haku
    public async Task GetCurrentLocation()
    {
        try
        {
            _isCheckingLocation = true;

            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));

            _cancelTokenSource = new CancellationTokenSource();

            Location location = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (location != null)
            {

                lat = location.Latitude.ToString();
                lon = location.Longitude.ToString();

                latLabel.Text = $"Latitude: {location.Latitude}";
                lonLabel.Text = $"Longitude: {location.Longitude}";
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Huomio", "Sijainnin haku ei onnistunut.", "ok");
        }
        finally
        {
            _isCheckingLocation = false;
        }
    }



    async void LoadDataFromRestApi()
    {

        try
        {

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri(ApiBaseUrl);
            string json = await client.GetStringAsync("api/workassignments");

            // json data deserialisoidaan json muodosta C# muotoon wa tyyppiseksi taulukoksi
            IEnumerable<WorkAssignment> wa = JsonConvert.DeserializeObject<WorkAssignment[]>(json);

            // dataa -niminen observableCollection on alustettukin jo ylh��ll� p��tasolla ett� hakutoiminto
            // p��see siihen k�siksi.
            dataa = new ObservableCollection<WorkAssignment>(wa);

            // Asetetaan datat n�kyviin xaml tiedostossa olevalle listalle
            waList.ItemsSource = dataa;

            // Tyhjennet��n latausilmoitus label
            waLataus.Text = "";

        }
        catch (Exception ex)
        {
            await DisplayAlert("Virhe", ex.InnerException.ToString(), "ok");
        }

    }

    async void startButton_Clicked(object sender, EventArgs e)
    {
        WorkAssignment wa = (WorkAssignment)waList.SelectedItem;

        if (wa == null)
        {
            await DisplayAlert("Valinta puuttuu", "Valitse ty�teht�v�.", "OK");
            return;
        }

        try
        {

        // Pyydet��n k�ytt�j�lt� kommentti
        string comment = await DisplayPromptAsync("Palaute", "Voit j�tt�� nyt kommentin halutessasi", "Valmis");
            if (comment == null)
            {
                comment = "-";
            }

            // Luodaan l�hetett�v� objekti
            Operation op = new Operation();
            op.EmployeeID = empId;
            op.WorkAssignmentID = wa.IdWorkAssignment;
            op.Comment = comment;
            op.OperationType = "start";
            op.Latitude = lat;
            op.Longitude = lon;

            HttpClient client= new HttpClient();
            client.BaseAddress = new Uri(ApiBaseUrl);

            // Muutetaan em. data objekti Jsoniksi
            var input = JsonConvert.SerializeObject(op);

            HttpContent content = new StringContent(input, Encoding.UTF8, "application/json");
          
            // L�hetet��n serialisoitu objekti back-endiin Post pyynt�n�
            HttpResponseMessage message = await client.PostAsync("/api/workassignments", content);

            // Otetaan vastaan palvelimen vastaus
            string reply = await message.Content.ReadAsStringAsync();

           
            //Asetetaan vastaus de-serialisoituna success muuttujaan
            bool success = JsonConvert.DeserializeObject<bool>(reply);

            if (success == false)
            {
                await DisplayAlert("Ei voida aloittaa", "Ty� on jo k�ynniss�", "OK");
            }
            else if (success == true)
            {
                await DisplayAlert("Ty� aloitettu", "Ty� on aloitettu", "OK");
            }
        }

        catch (Exception ex) {
            await DisplayAlert(ex.GetType().Name, ex.Message, "OK");
        }

    }


    async void stopButton_Clicked(object sender, EventArgs e)
    {
        WorkAssignment wa = (WorkAssignment)waList.SelectedItem;

        if (wa == null)
        {
            await DisplayAlert("Valinta puuttuu", "Valitse ty�teht�v�.", "OK");
            return;
        }

        try
        {

            // Pyydet��n k�ytt�j�lt� kommentti
            string comment = await DisplayPromptAsync("Palaute", "Voit j�tt�� nyt kommentin halutessasi", "Valmis");
            if (comment == null)
            {
                comment = "-";
            }

            // Luodaan l�hetett�v� objekti
            Operation op = new Operation();
            op.EmployeeID = empId;
            op.WorkAssignmentID = wa.IdWorkAssignment;
            op.Comment = comment;
            op.OperationType = "stop";
            op.Latitude = lat;
            op.Longitude = lon;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ApiBaseUrl);

            // Muutetaan em. data objekti Jsoniksi
            var input = JsonConvert.SerializeObject(op);

            HttpContent content = new StringContent(input, Encoding.UTF8, "application/json");

            // L�hetet��n serialisoitu objekti back-endiin Post pyynt�n�
            HttpResponseMessage message = await client.PostAsync("/api/workassignments", content);

            // Otetaan vastaan palvelimen vastaus
            string reply = await message.Content.ReadAsStringAsync();

            //Asetetaan vastaus de-serialisoituna success muuttujaan
            bool success = JsonConvert.DeserializeObject<bool>(reply);

            if (success == false)
            {
                await DisplayAlert("Ei voida lopettaa", "Ty�t� ei ole viel� aloitettu", "OK");
            }
            else if (success == true)
            {
                await DisplayAlert("Ty� lopetettu", "Ty� on lopetettu", "OK");
                LoadDataFromRestApi();
            }
        }

        catch (Exception ex)
        {
            await DisplayAlert(ex.GetType().Name, ex.Message, "OK");
        }

    }
}
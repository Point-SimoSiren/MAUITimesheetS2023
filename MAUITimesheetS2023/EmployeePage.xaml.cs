﻿using MAUITimesheetS2023.Models;
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


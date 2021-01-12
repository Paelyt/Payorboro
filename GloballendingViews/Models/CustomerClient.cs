using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Net;

namespace GloballendingViews.Models
{
    public class CustomerClient
    {
        private string Base_Url = "http://localhost:7304/api";
        //private string Base_Url = "http://localhost:3346/api";

           
        public IEnumerable<CustomerModel> findAll()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/Customers/GetCustomers").Result;
                if (response.IsSuccessStatusCode)
               
    return response.Content.ReadAsAsync<IEnumerable<CustomerModel>>().Result;
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public CustomerModel find(int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/Customers/"+ id).Result;
                if (response.IsSuccessStatusCode)

                    return response.Content.ReadAsAsync<CustomerModel>().Result;
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public bool Create(CustomerModel Customer)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/Customers",Customer).Result;

                    return response.IsSuccessStatusCode;
                
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
               
                return false;
            }
        }


        public bool Edit(CustomerModel Customer)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PutAsJsonAsync("api/Customers/"+Customer.ID,Customer).Result;

                return response.IsSuccessStatusCode;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }


        public bool Delete(int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.DeleteAsync("api/Customers/" + id).Result;

                return response.IsSuccessStatusCode;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }
    }
}
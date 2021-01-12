using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace GloballendingViews.Models
{
    public class GLobalClient
    {
        private string Base_Url = "http://localhost:7304/api";

        public IEnumerable<Merchant> findAllMerchant()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/Merchants/GetMerchants").Result;
                if (response.IsSuccessStatusCode)

                    return response.Content.ReadAsAsync<IEnumerable<Merchant>>().Result;
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public IEnumerable<CustomerServices> GetCustomerSevicesByUser(int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/CustomerServices/GetCustomerServiceByUser/" + id ).Result;
                if (response.IsSuccessStatusCode)

                    return response.Content.ReadAsAsync<IEnumerable<CustomerServices>>().Result;
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public IEnumerable<AccountsModel> findAllUser()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/Users").Result;
                if (response.IsSuccessStatusCode)

                    return response.Content.ReadAsAsync<IEnumerable<AccountsModel>>().Result;
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public IEnumerable<Page> findAllPages()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/Pags").Result;
                if (response.IsSuccessStatusCode)

                    return response.Content.ReadAsAsync<IEnumerable<Page>>().Result;
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public IEnumerable<RoleModel> findAllRole()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/Roles").Result;
                if (response.IsSuccessStatusCode)

                    return response.Content.ReadAsAsync<IEnumerable<RoleModel>>().Result;
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public IEnumerable<LoanModel> findAll()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/Loans").Result;
                if (response.IsSuccessStatusCode)

                    return response.Content.ReadAsAsync<IEnumerable<LoanModel>>().Result;
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public LoanModel find(int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/Loans/" + id).Result;
                if (response.IsSuccessStatusCode)

                    return response.Content.ReadAsAsync<LoanModel>().Result;
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public bool Create(LoanModel Loan)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/Loans", Loan).Result;

              return response.IsSuccessStatusCode;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }


        public bool CreateUser(AccountsModel AccountsModel)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/Users/PostUser", AccountsModel).Result;

                return response.IsSuccessStatusCode;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }

        public bool CreateRole(Page Page)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/Pages", Page).Result;

                return response.IsSuccessStatusCode;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }


        public bool CreateRole(RoleModel RoleModel)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/Roles", RoleModel).Result;

                return response.IsSuccessStatusCode;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }


        public bool CreatePage(Page PageModel)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/Pags", PageModel).Result;

                return response.IsSuccessStatusCode;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }
        public bool Edit(LoanModel Loan)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PutAsJsonAsync("api/Loans/" + Loan.ID, Loan).Result;

                return response.IsSuccessStatusCode;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }



        public bool DeleteCustomerService(CustomerServices CustomerServices , int id)
        {
            try
            {
                CustomerServices.ID = id;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PutAsJsonAsync("api/CustomerServices/DeleteCustomerServices/"+CustomerServices.ID , CustomerServices).Result;

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
                HttpResponseMessage response = client.DeleteAsync("api/Loans/" + id).Result;

                return response.IsSuccessStatusCode;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }

        public bool CreateBank(LoanBankModel LoanBank)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/LoanBanks", LoanBank).Result;

                return response.IsSuccessStatusCode ;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }

        public bool CreateSocail(LoanSocialModel LoanSocial)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/LoanSocials", LoanSocial).Result;

                return response.IsSuccessStatusCode;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }

        public bool CreateLoanEmployee(LoanEmployeeModel LoanEmployee)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/LoanEmployeeInfoes", LoanEmployee).Result;

                return response.IsSuccessStatusCode;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
               
                return false;
            }
        }

        public bool CreateCustomer(CustomerModel Customer)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/Customers", Customer).Result;

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
                HttpResponseMessage response = client.PutAsJsonAsync("Customers/" + Customer.ContactEmail, Customer).Result;

                return response.IsSuccessStatusCode;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }

        public CustomerModel FindCustomer(string Email)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/Customers/"+Email).Result;
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


        public bool CreateCustomerService(CustomerServices CustomerServices)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsJsonAsync("api/CustomerServices/PostCustomerService", CustomerServices).Result;

                return response.IsSuccessStatusCode;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }

        public CustomerServices FindCustomerService(int id)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(Base_Url);
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync("api/CustomerServices/GetCustomerService/" + id).Result;
                if (response.IsSuccessStatusCode)

                    return response.Content.ReadAsAsync<CustomerServices>().Result;
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
    }
}
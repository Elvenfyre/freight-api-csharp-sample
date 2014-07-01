using freight_api_csharp_sample.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace freight_api_csharp_sample
{
    public class Sample
    {
        HttpClient client;
        MediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();

        public Sample()
        {
            client = new HttpClient();
            client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiBasePath"]);
            client.DefaultRequestHeaders.Add("access_key", ConfigurationManager.AppSettings["ApiAccessKey"]);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        internal void RunAll()
        {
            
            NewOrder();
            NewOrders();

            Order();
            PendingOrders();

            AvailableRates();

            PrintCheapestCourier();

            SelectiveCreateAndPrint();

            CreateAndPrint();
        }

        private List<string> PendingOrders()
        {
            Console.WriteLine("===== {0} =====", "GET pendingorders");
            Console.WriteLine("");
            List<string> ret = new List<string>();

            HttpResponseMessage response = client.GetAsync("v2/pendingorders").Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var data = response.Content.ReadAsStringAsync().Result;
                var lines = JsonConvert.DeserializeObject<List<Record>>(data);

                foreach (var p in lines)
                {
                    Console.WriteLine("{0:15}\t{1:30};\t{2:30}", p.packingslipno, p.consignee, p.suburb);
                }

                ret.AddRange(lines.Select(p => p.packingslipno).ToList());
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                Console.ForegroundColor = ConsoleColor.White;
            }

            return ret;
        }
        void Order()
        {
            Console.WriteLine("===== {0} =====", "GET Order");
            Console.WriteLine("");

            var orders = PendingOrders();

            HttpResponseMessage response = client.GetAsync("v2/order/" + orders.First()).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var data = response.Content.ReadAsStringAsync().Result;
                var p = JsonConvert.DeserializeObject<RecordStatus>(data);

                Console.WriteLine("{0}\t{1};\t{2}", p.packingslipno, p.consignee, p.Status);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        void NewOrder()
        {
            Console.WriteLine("===== {0} =====", "POST neworder");
            Console.WriteLine("");

            Record save = new Record
            {
                packingslipno = "test-" + DateTime.Now.ToString("yy-MM-dd"),
                address1 = "1 Queens Street",
                address2 = "",
                suburb = "Auckland Centrol",
                city = "Auckland",
                postcode = "",
                consignee = "Test,"
            };

            HttpResponseMessage response = client.PostAsync("v2/neworder", save, jsonFormatter).Result;  // Blocking call!

            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var data = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine(data);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        void NewOrders()
        {
            Console.WriteLine("===== {0} =====", "POST neworders");
            Console.WriteLine("");

            Record[] save = new Record[2];

            save[0] = new Record
            {
                packingslipno = "test1-" + DateTime.Now.ToString("yy-MM-dd"),
                address1 = "1 Queens Street",
                address2 = "",
                suburb = "Auckland Central",
                city = "Auckland",
                postcode = "",
                consignee = "Test 1"
            };

            save[1] = new Record
            {
                packingslipno = "test2-" + DateTime.Now.ToString("yy-MM-dd"),
                address1 = "1 Queens Street",
                address2 = "",
                suburb = "Auckland Central",
                city = "Auckland",
                postcode = "",
                consignee = "Test 2"
            };
            
            HttpResponseMessage response = client.PostAsJsonAsync("v2/neworders", save).Result;  // Blocking call!

            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var data = response.Content.ReadAsStringAsync().Result;

                var lines = JsonConvert.DeserializeObject<List<BatchOrdersResponse>>(data);

                foreach (var item in lines)
                {
                    Console.WriteLine(item.packingslipno + " - " + item.result);
                }

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }


        private void AvailableRates()
        {
            Console.WriteLine("===== {0} =====", "POST availablerates");
            Console.WriteLine("");

           
            var post = new 
            {
                DeliveryReference = "ORDER123",
                Destination = new 
                {
                    Id = 0,
                    Name = "DestinationName",
                    Address = new 
                    {
                        BuildingName = "",
                        StreetAddress = "DestinationStreetAddress",
                        Suburb = "Avonside",
                        City = "Christchurch",
                        PostCode = "8061",
                        CountryCode = "NZ",
                    },
                    ContactPerson = "DestinationContact",
                    PhoneNumber = "123456789",
                    Email = "destinationemail@email.com",
                    DeliveryInstructions = "Desinationdeliveryinstructions"
                },
                IsSaturdayDelivery = false,
                IsSignatureRequired = true,
                Packages = new List<object>(new[] { new { 
                    Height = 1, 
                    Length = 1, 
                    Id = 0, 
                    Width = 10, 
                    Kg = 0.1M, 
                    Name = "GSS-DLE SATCHEL", 
                    PackageCode = "DLE",
                    Type = "Box"
                }
                })
            };

            HttpResponseMessage response = client.PostAsJsonAsync("RatesQueryV1/availablerates", post).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var data = response.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(data) && data != "null")
                {
                    var lines = JsonConvert.DeserializeObject<AvailabeRatesResponse>(data);

                    foreach (var item in lines.Available)
                    {
                        Console.WriteLine("{0} {1} {2} {3}", item.CarrierName, item.Cost, item.DeliveryType, item.CarrierId);
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                Console.ForegroundColor = ConsoleColor.White;
            }


        }

        void PrintCheapestCourier()
        {

            Console.WriteLine("===== {0} =====", "POST printcheapestcourier");
            Console.WriteLine("");


            var post = new
            {
                DeliveryReference = "ORDER123",
                Destination = new
                {
                    Id = 0,
                    Name = "DestinationName",
                    Address = new
                    {
                        BuildingName = "",
                        StreetAddress = "DestinationStreetAddress",
                        Suburb = "Avonside",
                        City = "Christchurch",
                        PostCode = "8061",
                        CountryCode = "NZ",
                    },
                    ContactPerson = "DestinationContact",
                    PhoneNumber = "123456789",
                    Email = "destinationemail@email.com",
                    DeliveryInstructions = "Desinationdeliveryinstructions"
                },
                IsSaturdayDelivery = false,
                IsSignatureRequired = true,
                Packages = new List<object>(new[] { new { 
                    Height = 1, 
                    Length = 1, 
                    Id = 0, 
                    Width = 10, 
                    Kg = 0.1M, 
                    Name = "GSS-DLE SATCHEL", 
                    PackageCode = "DLE",
                    Type = "Box"
                }
                }),
                PrintToPrinter = true
            };
            
            HttpResponseMessage response = client.PostAsJsonAsync("ratesqueryv1/printcheapestcourier", post).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var data = response.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(data) && data != "null")
                {
                    var lines = JsonConvert.DeserializeObject<CreateShipmentResponse>(data);

                    foreach (var item in lines.Consignments)
                    {
                        Console.WriteLine("{0}", item.Connote);
                    }

                    foreach (var item in lines.Errors)
                    {
                        Console.WriteLine("{0} --> {1}", item.Property, item.Message);
                    }


                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                Console.ForegroundColor = ConsoleColor.White;
            }

        }


        void CreateAndPrint()
        {
            Console.WriteLine("===== {0} =====", "POST createandprint");
            Console.WriteLine("");


            var post = new
            {
                DeliveryReference = "ORDER123",
                Destination = new
                {
                    Id = 0,
                    Name = "DestinationName",
                    Address = new
                    {
                        BuildingName = "",
                        StreetAddress = "DestinationStreetAddress",
                        Suburb = "Avonside",
                        City = "Christchurch",
                        PostCode = "8061",
                        CountryCode = "NZ",
                    },
                    ContactPerson = "DestinationContact",
                    PhoneNumber = "123456789",
                    Email = "destinationemail@email.com",
                    DeliveryInstructions = "Desinationdeliveryinstructions"
                },
                IsSaturdayDelivery = false,
                IsSignatureRequired = true,
                Packages = new List<object>(new[] { new { 
                    Height = 1, 
                    Length = 1, 
                    Id = 0, 
                    Width = 10, 
                    Kg = 0.1M, 
                    Name = "GSS-DLE SATCHEL", 
                    PackageCode = "DLE",
                    Type = "Box"
                }
                }),
                PrintToPrinter = "DAMON-HP >> ZDESIGNER GC420D (EPL) (123)",

                Commodities = new List<object>(new[] { 
                    new { Description = "Apparel", UnitKg = 0.33M, UnitValue = 1, Units = 1, Country = "NZ", Currency = "NZD" }
                }),
                Outputs = new List<object>(new string[] { "LABEL_PDF", "LABEL_PNG" })
            };


            HttpResponseMessage response = client.PostAsJsonAsync("ratesqueryv1/createandprint", post).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var data = response.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(data) && data != "null")
                {
                    var lines = JsonConvert.DeserializeObject<CreateShipmentResponse>(data);

                    foreach (var item in lines.Consignments)
                    {
                        Console.WriteLine("{0}", item.Connote);
                    }

                    foreach (var item in lines.Errors)
                    {
                        Console.WriteLine("{0} --> {1}", item.Property, item.Message);
                    }

                    foreach (var item in lines.Consignments)
                    {
                        foreach (var ot in item.OutputFiles)
                        {
                            foreach (var att in ot.Value)
                            {
                                string file = "";
                                if (ot.Key.Contains("PNG"))
                                    file = GetTempFolder() + "\\" + Guid.NewGuid() + ".png";
                                else
                                    file = GetTempFolder() + "\\" + Guid.NewGuid() + ".pdf";

                                System.IO.File.WriteAllBytes(file, att);

                                System.Diagnostics.Process.Start(file);
                            }
                        }
                    }

                    Console.WriteLine("{0}", lines.Message);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                Console.ForegroundColor = ConsoleColor.White;
            }

        }

        void SelectiveCreateAndPrint()
        {
            Console.WriteLine("===== {0} =====", "POST createandprint");
            Console.WriteLine("");


            var post = new
            {
                DeliveryReference = "ORDER123",
                Destination = new
                {
                    Id = 0,
                    Name = "DestinationName",
                    Address = new
                    {
                        BuildingName = "",
                        StreetAddress = "DestinationStreetAddress",
                        Suburb = "Avonside",
                        City = "Christchurch",
                        PostCode = "8061",
                        CountryCode = "NZ",
                    },
                    ContactPerson = "DestinationContact",
                    PhoneNumber = "123456789",
                    Email = "destinationemail@email.com",
                    DeliveryInstructions = "Desinationdeliveryinstructions"
                },
                IsSaturdayDelivery = false,
                IsSignatureRequired = true,
                Packages = new List<object>(new[] { new { 
                    Height = 1, 
                    Length = 1, 
                    Id = 0, 
                    Width = 10, 
                    Kg = 0.1M, 
                    Name = "GSS-DLE SATCHEL", 
                    PackageCode = "DLE",
                    Type = "Box"
                }
                })
            };

            AvailabeRatesResponse lines = null;

            HttpResponseMessage response = client.PostAsJsonAsync("RatesQueryV1/availablerates", post).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                var data = response.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(data) && data != "null")
                {
                    lines = JsonConvert.DeserializeObject<AvailabeRatesResponse>(data);

                    
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                Console.ForegroundColor = ConsoleColor.White;
            }


            if (lines.Available.Any())
            {
                for (int i = 0; i < lines.Available.Count; i++)
                {
                    var item = lines.Available[i];
                    Console.WriteLine("{0}:{1} {2} {3}", i, item.CarrierName, item.Cost, item.DeliveryType);
                }

                Console.Write("Enter the price line to print? :");
                var ky = Console.ReadLine();
                Console.WriteLine("");

                var quote = lines.Available[int.Parse(ky)];


                var post2 = new
                {
                    QuoteId = quote.QuoteId,
                    DeliveryReference = "ORDER123",
                    Destination = new
                    {
                        Id = 0,
                        Name = "DestinationName",
                        Address = new
                        {
                            BuildingName = "",
                            StreetAddress = "DestinationStreetAddress",
                            Suburb = "Avonside",
                            City = "Christchurch",
                            PostCode = "8061",
                            CountryCode = "NZ",
                        },
                        ContactPerson = "DestinationContact",
                        PhoneNumber = "123456789",
                        Email = "destinationemail@email.com",
                        DeliveryInstructions = "Desinationdeliveryinstructions"
                    },
                    IsSaturdayDelivery = false,
                    IsSignatureRequired = true,
                    Packages = new List<object>(new[] { new { 
                    Height = 1, 
                    Length = 1, 
                    Id = 0, 
                    Width = 10, 
                    Kg = 0.1M, 
                    Name = "GSS-DLE SATCHEL", 
                    PackageCode = "DLE",
                    Type = "Box"
                }
                }),
                    PrintToPrinter = true,
                    Commodities = new List<object>(new[] { 
                    new { Description = "Apparel", UnitKg = 0.33M, UnitValue = 1, Units = 1, Country = "NZ", Currency = "NZD" }
                }),
                    Outputs = new List<object>(new string[] { "LABEL_PDF", "LABEL_PNG" }),
                };


                response = client.PostAsJsonAsync("ratesqueryv1/createandprint", post2).Result;  // Blocking call!
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body. Blocking!
                    var data = response.Content.ReadAsStringAsync().Result;

                    if (!string.IsNullOrEmpty(data) && data != "null")
                    {
                        var lines2 = JsonConvert.DeserializeObject<CreateShipmentResponse>(data);

                        foreach (var item in lines2.Consignments)
                        {
                            Console.WriteLine("{0}", item.Connote);
                        }

                        foreach (var item in lines2.Errors)
                        {
                            Console.WriteLine("{0} --> {1}", item.Property, item.Message);
                        }

                        foreach (var item in lines2.Consignments)
                        {
                            foreach (var ot in item.OutputFiles)
                            {
                                foreach (var att in ot.Value)
                                {
                                    string file = "";
                                    if (ot.Key.Contains("PNG"))
                                        file = GetTempFolder() + "\\" + Guid.NewGuid() + ".png";
                                    else
                                        file = GetTempFolder() + "\\" + Guid.NewGuid() + ".pdf";

                                    System.IO.File.WriteAllBytes(file, att);

                                    System.Diagnostics.Process.Start(file);
                                }
                            }
                        }

                        Console.WriteLine("{0}", lines2.Message);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

        }

        private string GetTempFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.InternetCache);
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure;
using System.Configuration;

namespace tableStor
{
    class Program
    {
        static void Main(string[] args)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationSettings.AppSettings["StorageConnectionString"]);

            // Create an Azure table customer
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable tableCx = tableClient.GetTableReference("customers");
            tableCx.CreateIfNotExists();

            //Insert records
            OrderEntity newOrder = new OrderEntity("Archer", "20141216");
            newOrder.OrderNumber = "101";
            newOrder.ShippedDate = Convert.ToDateTime("2014/12/18");
            newOrder.RequiredDate = Convert.ToDateTime("2014/12/22");
            newOrder.Status = "shipped";
            TableOperation insertOperation = TableOperation.Insert(newOrder);
            tableCx.Execute(insertOperation);



            // Console.WriteLine(storageAccount);
            Console.ReadKey();
        }
    }

    public class OrderEntity : TableEntity
    {
        public OrderEntity(string customerName, String orderDate)
        {
            this.PartitionKey = customerName;
            this.RowKey = orderDate;
        }
        public OrderEntity() { }
        public string OrderNumber { get; set; }
        public DateTime RequiredDate { get; set; }
        public DateTime ShippedDate { get; set; }
        public string Status { get; set; }
    }



}


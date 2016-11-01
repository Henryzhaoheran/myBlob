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
            OrderEntity newOrder = new OrderEntity("Lana", "20141216");
            newOrder.OrderNumber = "101";
            newOrder.ShippedDate = Convert.ToDateTime("2014/12/18");
            newOrder.RequiredDate = Convert.ToDateTime("2014/12/22");
            newOrder.Status = "shipped";
            TableOperation insertOperation = TableOperation.Insert(newOrder);
            // tableCx.Execute(insertOperation);


            // Batch Insert
            TableBatchOperation batchOperation = new TableBatchOperation();

            OrderEntity newOrder1 = new OrderEntity("Lana", "20141217");
            newOrder1.OrderNumber = "102";
            newOrder1.ShippedDate = Convert.ToDateTime("1/1/1900");
            newOrder1.RequiredDate = Convert.ToDateTime("1/1/1900");
            newOrder1.Status = "pending";
            OrderEntity newOrder2 = new OrderEntity("Lana", "20141218");
            newOrder2.OrderNumber = "103";
            newOrder2.ShippedDate = Convert.ToDateTime("1/1/1900");
            newOrder2.RequiredDate = Convert.ToDateTime("12/25/2014");
            newOrder2.Status = "open";
            OrderEntity newOrder3 = new OrderEntity("Lana", "20141219");
            newOrder3.OrderNumber = "103";
            newOrder3.ShippedDate = Convert.ToDateTime("12/17/2014");
            newOrder3.RequiredDate = Convert.ToDateTime("12/17/2014");
            newOrder3.Status = "shipped";
            batchOperation.Insert(newOrder1);
            batchOperation.Insert(newOrder2);
            batchOperation.Insert(newOrder3);
            // tableCx.ExecuteBatch(batchOperation);


            // Query all keys
            TableQuery<OrderEntity> query = new TableQuery<OrderEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Lana"));

            foreach (OrderEntity entity in tableCx.ExecuteQuery(query))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                entity.Status, entity.RequiredDate);
            }

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


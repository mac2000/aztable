using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzTable
{
	public class Metrics
	{
		protected readonly CloudTable Table;

		public Metrics(string accountName, string accountKey)
		{
			Table = CloudStorageAccount
				.Parse($"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey}")
				.CreateCloudTableClient()
				.GetTableReference("metrics");

			Table.CreateIfNotExists();
		}

		public TableResult Save(string metric, double value, DateTime date) => Table.Execute(TableOperation.InsertOrReplace(new Metric
		{
			PartitionKey = metric,
			RowKey = new DateTimeOffset(date).ToUnixTimeSeconds().ToString()
		}));

		internal class Metric : TableEntity
		{
			public double Value { get; set; }
		}
	}
}

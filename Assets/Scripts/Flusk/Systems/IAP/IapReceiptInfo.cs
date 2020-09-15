namespace Flusk.Systems.IAP
{
	public struct IapReceiptInfo
	{
		public string Receipt
		{
			private set;
			get;
		}

		public string ReceiptSignature
		{
			private set;
			get;
		}

		public string TransactionId
		{
			private set;
			get;
		}

		public IapReceiptInfo(string receipt, string receiptSignature, string transactionId)
		{
			Receipt = receipt;
			ReceiptSignature = receiptSignature;
			TransactionId = transactionId;
		}

		public static readonly IapReceiptInfo Empty = new IapReceiptInfo(string.Empty, string.Empty, string.Empty);
	}
}
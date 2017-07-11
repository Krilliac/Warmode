using System;
using UnityEngine;

namespace Facebook.Unity.Example
{
	internal class Pay : MenuBase
	{
		private string payProduct = string.Empty;

		protected override void GetGui()
		{
			base.LabelAndTextField("Product: ", ref this.payProduct);
			if (base.Button("Call Pay"))
			{
				this.CallFBPay();
			}
			GUILayout.Space(10f);
		}

		private void CallFBPay()
		{
			FacebookDelegate<IPayResult> callback = new FacebookDelegate<IPayResult>(base.HandleResult);
			FB.Canvas.Pay(this.payProduct, "purchaseitem", 1, null, null, null, null, null, callback);
		}
	}
}

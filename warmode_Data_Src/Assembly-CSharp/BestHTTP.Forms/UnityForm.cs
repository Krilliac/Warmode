using System;
using UnityEngine;

namespace BestHTTP.Forms
{
	public sealed class UnityForm : HTTPFormBase
	{
		public WWWForm Form
		{
			get;
			set;
		}

		public UnityForm()
		{
		}

		public UnityForm(WWWForm form)
		{
			this.Form = form;
		}

		public override void CopyFrom(HTTPFormBase fields)
		{
			base.Fields = fields.Fields;
			base.IsChanged = true;
			if (this.Form == null)
			{
				this.Form = new WWWForm();
				if (base.Fields != null)
				{
					for (int i = 0; i < base.Fields.Count; i++)
					{
						HTTPFieldData hTTPFieldData = base.Fields[i];
						if (string.IsNullOrEmpty(hTTPFieldData.Text) && hTTPFieldData.Binary != null)
						{
							this.Form.AddBinaryData(hTTPFieldData.Name, hTTPFieldData.Binary, hTTPFieldData.FileName, hTTPFieldData.MimeType);
						}
						else
						{
							this.Form.AddField(hTTPFieldData.Name, hTTPFieldData.Text, hTTPFieldData.Encoding);
						}
					}
				}
			}
		}

		public override void PrepareRequest(HTTPRequest request)
		{
			if (this.Form.headers.ContainsKey("Content-Type"))
			{
				request.SetHeader("Content-Type", this.Form.headers["Content-Type"]);
			}
			else
			{
				request.SetHeader("Content-Type", "application/x-www-form-urlencoded");
			}
		}

		public override byte[] GetData()
		{
			return this.Form.data;
		}
	}
}

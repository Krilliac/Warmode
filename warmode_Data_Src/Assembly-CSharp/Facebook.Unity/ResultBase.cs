using System;
using System.Collections.Generic;

namespace Facebook.Unity
{
	internal abstract class ResultBase : IInternalResult, IResult
	{
		internal const long CancelDialogCode = 4201L;

		internal const string ErrorCodeKey = "error_code";

		internal const string ErrorMessageKey = "error_message";

		public virtual string Error
		{
			get;
			protected set;
		}

		public virtual IDictionary<string, object> ResultDictionary
		{
			get;
			protected set;
		}

		public virtual string RawResult
		{
			get;
			protected set;
		}

		public virtual bool Cancelled
		{
			get;
			protected set;
		}

		public virtual string CallbackId
		{
			get;
			protected set;
		}

		protected long? CanvasErrorCode
		{
			get;
			private set;
		}

		internal ResultBase(ResultContainer result)
		{
			string errorValue = ResultBase.GetErrorValue(result.ResultDictionary);
			bool cancelledValue = ResultBase.GetCancelledValue(result.ResultDictionary);
			string callbackId = ResultBase.GetCallbackId(result.ResultDictionary);
			this.Init(result, errorValue, cancelledValue, callbackId);
		}

		internal ResultBase(ResultContainer result, string error, bool cancelled)
		{
			this.Init(result, error, cancelled, null);
		}

		public override string ToString()
		{
			return Utilities.FormatToString(base.ToString(), base.GetType().Name, new Dictionary<string, string>
			{
				{
					"Error",
					this.Error
				},
				{
					"RawResult",
					this.RawResult
				},
				{
					"Cancelled",
					this.Cancelled.ToString()
				}
			});
		}

		protected void Init(ResultContainer result, string error, bool cancelled, string callbackId)
		{
			this.RawResult = result.RawResult;
			this.ResultDictionary = result.ResultDictionary;
			this.Cancelled = cancelled;
			this.Error = error;
			this.CallbackId = callbackId;
			if (this.ResultDictionary != null)
			{
				long num;
				if (this.ResultDictionary.TryGetValue("error_code", out num))
				{
					this.CanvasErrorCode = new long?(num);
					if (num == 4201L)
					{
						this.Cancelled = true;
					}
				}
				string error2;
				if (this.ResultDictionary.TryGetValue("error_message", out error2))
				{
					this.Error = error2;
				}
			}
		}

		private static string GetErrorValue(IDictionary<string, object> result)
		{
			if (result == null)
			{
				return null;
			}
			string result2;
			if (result.TryGetValue("error", out result2))
			{
				return result2;
			}
			return null;
		}

		private static bool GetCancelledValue(IDictionary<string, object> result)
		{
			if (result == null)
			{
				return false;
			}
			object obj;
			if (result.TryGetValue("cancelled", out obj))
			{
				bool? flag = obj as bool?;
				if (flag.HasValue)
				{
					return flag.HasValue && flag.Value;
				}
				string text = obj as string;
				if (text != null)
				{
					return Convert.ToBoolean(text);
				}
				int? num = obj as int?;
				if (num.HasValue)
				{
					return num.HasValue && num.Value != 0;
				}
			}
			return false;
		}

		private static string GetCallbackId(IDictionary<string, object> result)
		{
			if (result == null)
			{
				return null;
			}
			string result2;
			if (result.TryGetValue("callback_id", out result2))
			{
				return result2;
			}
			return null;
		}
	}
}

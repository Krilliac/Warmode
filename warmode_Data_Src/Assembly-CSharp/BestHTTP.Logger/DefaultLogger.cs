using System;
using UnityEngine;

namespace BestHTTP.Logger
{
	public class DefaultLogger : ILogger
	{
		public Loglevels Level
		{
			get;
			set;
		}

		public string FormatVerbose
		{
			get;
			set;
		}

		public string FormatInfo
		{
			get;
			set;
		}

		public string FormatWarn
		{
			get;
			set;
		}

		public string FormatErr
		{
			get;
			set;
		}

		public string FormatEx
		{
			get;
			set;
		}

		public DefaultLogger()
		{
			this.FormatVerbose = "I [{0}]: {1}";
			this.FormatInfo = "I [{0}]: {1}";
			this.FormatWarn = "W [{0}]: {1}";
			this.FormatErr = "Err [{0}]: {1}";
			this.FormatEx = "Ex [{0}]: {1} - Message: {2}  StackTrace: {3}";
			this.Level = ((!Debug.isDebugBuild) ? Loglevels.Error : Loglevels.Warning);
		}

		public void Verbose(string division, string verb)
		{
			if (this.Level <= Loglevels.All)
			{
				try
				{
					Debug.Log(string.Format(this.FormatVerbose, division, verb));
				}
				catch
				{
				}
			}
		}

		public void Information(string division, string info)
		{
			if (this.Level <= Loglevels.Information)
			{
				try
				{
					Debug.Log(string.Format(this.FormatInfo, division, info));
				}
				catch
				{
				}
			}
		}

		public void Warning(string division, string warn)
		{
			if (this.Level <= Loglevels.Warning)
			{
				try
				{
					Debug.LogWarning(string.Format(this.FormatWarn, division, warn));
				}
				catch
				{
				}
			}
		}

		public void Error(string division, string err)
		{
			if (this.Level <= Loglevels.Error)
			{
				try
				{
					Debug.LogError(string.Format(this.FormatErr, division, err));
				}
				catch
				{
				}
			}
		}

		public void Exception(string division, string msg, Exception ex)
		{
			if (this.Level <= Loglevels.Exception)
			{
				try
				{
					Debug.LogError(string.Format(this.FormatEx, new object[]
					{
						division,
						msg,
						(ex == null) ? "null" : ex.Message,
						(ex == null) ? "null" : ex.StackTrace
					}));
				}
				catch
				{
				}
			}
		}
	}
}

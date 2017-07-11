using Facebook.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FBManager : MonoBehaviour
{
	private static string Status = string.Empty;

	private static string LastResponse = string.Empty;

	public static void StartAuth()
	{
		if (FB.IsInitialized && AccessToken.CurrentAccessToken != null)
		{
			BaseData.Profile = false;
			WebHandler.get_profile("&version=" + Client.version);
			return;
		}
		FB.Init(new InitDelegate(FBManager.OnInitComplete), new HideUnityDelegate(FBManager.OnHideUnity), null);
		FBManager.Status = "FB.Init() called with " + FB.AppId;
	}

	private static void OnInitComplete()
	{
		FBManager.Status = "Success - Check log for details";
		FBManager.LastResponse = "Success Response: OnInitComplete Called\n";
		string text = string.Format("OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}'", FB.IsLoggedIn, FB.IsInitialized);
		if (AccessToken.CurrentAccessToken != null)
		{
		}
		if (FB.IsInitialized)
		{
			MonoBehaviour.print("FB.IsInitialized");
			FBManager.CallFBLogin();
			FBManager.Status = "Login called";
		}
	}

	private static void OnHideUnity(bool isGameShown)
	{
		FBManager.Status = "Success - Check log for details";
		string text = string.Format("Success Response: OnHideUnity Called {0}\n", isGameShown);
	}

	private static void CallFBLogin()
	{
		FB.LogInWithReadPermissions(new List<string>
		{
			"public_profile",
			"email",
			"user_friends"
		}, new FacebookDelegate<ILoginResult>(FBManager.HandleResult));
	}

	protected static void HandleResult(IResult result)
	{
		if (result == null)
		{
			FBManager.LastResponse = "Null Response\n";
			return;
		}
		if (!string.IsNullOrEmpty(result.Error))
		{
			FBManager.Status = "Error - Check log for details";
			FBManager.LastResponse = "Error Response:\n" + result.Error;
		}
		else if (result.Cancelled)
		{
			FBManager.Status = "Cancelled - Check log for details";
			FBManager.LastResponse = "Cancelled Response:\n" + result.RawResult;
		}
		else if (!string.IsNullOrEmpty(result.RawResult))
		{
			FBManager.Status = "Success - Check log for details";
			FBManager.LastResponse = "Success Response:\n" + result.RawResult;
			string key = "accessToken";
			string key2 = "userID";
			if (result.ResultDictionary.TryGetValue(key, out BaseData.key) && result.ResultDictionary.TryGetValue(key2, out BaseData.uid))
			{
				WebHandler.get_facebook_key();
			}
		}
		else
		{
			FBManager.LastResponse = "Empty Response\n";
		}
	}

	public static void BuyCoins(int coins)
	{
		FB.Canvas.Pay("https://warmodegame.com/core/fb/coin.html", "purchaseitem", coins, null, null, null, null, null, delegate(IPayResult response)
		{
			BaseData.cs.Refresh2SecDelay();
		});
	}

	public static void BuyUnbun(int ucount)
	{
		FB.Canvas.Pay("https://warmodegame.com/core/fb/unbun.html", "purchaseitem", ucount, null, null, null, null, null, delegate(IPayResult response)
		{
			BaseData.cs.Refresh2SecDelay();
		});
	}
}

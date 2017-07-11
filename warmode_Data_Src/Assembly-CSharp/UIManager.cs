using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public Text DebugText;

	private static Canvas canvas;

	private static GameObject loadingGo;

	private static Text loadingTextB;

	private static Text loadingTextW;

	private static Image progressImg;

	private static RectTransform coolerRT;

	private static Sprite loadingProgr1;

	private static Sprite loadingProgr2;

	private static Sprite loadingProgr3;

	private static Sprite loadingProgr4;

	private static Sprite loadingProgr5;

	private static Sprite loadingProgr6;

	private static Sprite loadingProgr7;

	public static bool canvasActive;

	public static bool loadingActive;

	public static void PostAwake()
	{
		UIManager.canvas = GameObject.Find("UI").GetComponent<Canvas>();
		UIManager.loadingGo = GameObject.Find("UI/Loading");
		UIManager.loadingTextB = GameObject.Find("UI/Loading/TextB").GetComponent<Text>();
		UIManager.loadingTextW = GameObject.Find("UI/Loading/TextB/TextW").GetComponent<Text>();
		UIManager.progressImg = GameObject.Find("UI/Loading/Progress").GetComponent<Image>();
		UIManager.coolerRT = GameObject.Find("UI/Loading/Cooler").GetComponent<RectTransform>();
		UIManager.loadingProgr1 = Resources.Load<Sprite>("UI/loading_progr1");
		UIManager.loadingProgr2 = Resources.Load<Sprite>("UI/loading_progr2");
		UIManager.loadingProgr3 = Resources.Load<Sprite>("UI/loading_progr3");
		UIManager.loadingProgr4 = Resources.Load<Sprite>("UI/loading_progr4");
		UIManager.loadingProgr5 = Resources.Load<Sprite>("UI/loading_progr5");
		UIManager.loadingProgr6 = Resources.Load<Sprite>("UI/loading_progr6");
		UIManager.loadingProgr7 = Resources.Load<Sprite>("UI/loading_progr7");
	}

	public static void SetCanvasActive(bool val)
	{
		UIManager.canvasActive = val;
		if (UIManager.canvas == null)
		{
			return;
		}
		UIManager.canvas.enabled = val;
	}

	public static void SetLoadingActive(bool val)
	{
		UIManager.loadingActive = val;
		if (UIManager.loadingGo == null)
		{
			return;
		}
		UIManager.loadingGo.SetActive(val);
	}

	public static void SetLoadingProgress(int progress)
	{
		if (!UIManager.loadingActive)
		{
			return;
		}
		string text = string.Concat(new object[]
		{
			Lang.Get("_LOADING"),
			" ",
			progress,
			"%"
		});
		UIManager.loadingTextB.text = text;
		UIManager.loadingTextW.text = text;
		if (progress < 15)
		{
			UIManager.progressImg.overrideSprite = UIManager.loadingProgr1;
		}
		else if (progress >= 15 && progress < 31)
		{
			UIManager.progressImg.overrideSprite = UIManager.loadingProgr2;
		}
		else if (progress >= 31 && progress < 47)
		{
			UIManager.progressImg.overrideSprite = UIManager.loadingProgr3;
		}
		else if (progress >= 47 && progress < 63)
		{
			UIManager.progressImg.overrideSprite = UIManager.loadingProgr4;
		}
		else if (progress >= 63 && progress < 79)
		{
			UIManager.progressImg.overrideSprite = UIManager.loadingProgr5;
		}
		else if (progress >= 79 && progress < 95)
		{
			UIManager.progressImg.overrideSprite = UIManager.loadingProgr6;
		}
		else if (progress >= 95)
		{
			UIManager.progressImg.overrideSprite = UIManager.loadingProgr7;
		}
	}

	public static void SetCoolerAngle(float angleZ)
	{
		if (!UIManager.loadingActive)
		{
			return;
		}
		UIManager.coolerRT.rotation = Quaternion.Euler(0f, 0f, -angleZ / 1f);
	}
}

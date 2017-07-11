using System;
using UnityEngine;
using UnityEngine.Events;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("Code Stage/Anti-Cheat Toolkit/Obscured Cheating Detector")]
	public class ObscuredCheatingDetector : ActDetectorBase
	{
		internal const string COMPONENT_NAME = "Obscured Cheating Detector";

		internal const string FINAL_LOG_PREFIX = "[ACTk] Obscured Cheating Detector: ";

		private static int instancesInScene;

		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredFloat. Increase in case of false positives.")]
		public float floatEpsilon = 0.0001f;

		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredVector2. Increase in case of false positives.")]
		public float vector2Epsilon = 0.1f;

		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredVector3. Increase in case of false positives.")]
		public float vector3Epsilon = 0.1f;

		[Tooltip("Max allowed difference between encrypted and fake values in ObscuredQuaternion. Increase in case of false positives.")]
		public float quaternionEpsilon = 0.1f;

		public static ObscuredCheatingDetector Instance
		{
			get;
			private set;
		}

		private static ObscuredCheatingDetector GetOrCreateInstance
		{
			get
			{
				if (ObscuredCheatingDetector.Instance != null)
				{
					return ObscuredCheatingDetector.Instance;
				}
				if (ActDetectorBase.detectorsContainer == null)
				{
					ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
				}
				ObscuredCheatingDetector.Instance = ActDetectorBase.detectorsContainer.AddComponent<ObscuredCheatingDetector>();
				return ObscuredCheatingDetector.Instance;
			}
		}

		internal static bool IsRunning
		{
			get
			{
				return ObscuredCheatingDetector.Instance != null && ObscuredCheatingDetector.Instance.isRunning;
			}
		}

		private ObscuredCheatingDetector()
		{
		}

		public static void StartDetection()
		{
			if (ObscuredCheatingDetector.Instance != null)
			{
				ObscuredCheatingDetector.Instance.StartDetectionInternal(null);
			}
			else
			{
				Debug.LogError("[ACTk] Obscured Cheating Detector: can't be started since it doesn't exists in scene or not yet initialized!");
			}
		}

		public static void StartDetection(UnityAction callback)
		{
			ObscuredCheatingDetector.GetOrCreateInstance.StartDetectionInternal(callback);
		}

		public static void StopDetection()
		{
			if (ObscuredCheatingDetector.Instance != null)
			{
				ObscuredCheatingDetector.Instance.StopDetectionInternal();
			}
		}

		public static void Dispose()
		{
			if (ObscuredCheatingDetector.Instance != null)
			{
				ObscuredCheatingDetector.Instance.DisposeInternal();
			}
		}

		private void Awake()
		{
			ObscuredCheatingDetector.instancesInScene++;
			if (this.Init(ObscuredCheatingDetector.Instance, "Obscured Cheating Detector"))
			{
				ObscuredCheatingDetector.Instance = this;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			ObscuredCheatingDetector.instancesInScene--;
		}

		private void OnLevelWasLoaded()
		{
			this.OnLevelLoadedCallback();
		}

		private void OnLevelLoadedCallback()
		{
			if (ObscuredCheatingDetector.instancesInScene < 2)
			{
				if (!this.keepAlive)
				{
					this.DisposeInternal();
				}
			}
			else if (!this.keepAlive && ObscuredCheatingDetector.Instance != this)
			{
				this.DisposeInternal();
			}
		}

		private void StartDetectionInternal(UnityAction callback)
		{
			if (this.isRunning)
			{
				Debug.LogWarning("[ACTk] Obscured Cheating Detector: already running!", this);
				return;
			}
			if (!base.enabled)
			{
				Debug.LogWarning("[ACTk] Obscured Cheating Detector: disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
				return;
			}
			if (callback != null && this.detectionEventHasListener)
			{
				Debug.LogWarning("[ACTk] Obscured Cheating Detector: has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?", this);
			}
			if (callback == null && !this.detectionEventHasListener)
			{
				Debug.LogWarning("[ACTk] Obscured Cheating Detector: was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
				base.enabled = false;
				return;
			}
			this.detectionAction = callback;
			this.started = true;
			this.isRunning = true;
		}

		protected override void StartDetectionAutomatically()
		{
			this.StartDetectionInternal(null);
		}

		protected override void PauseDetector()
		{
			this.isRunning = false;
		}

		protected override void ResumeDetector()
		{
			if (this.detectionAction == null && !this.detectionEventHasListener)
			{
				return;
			}
			this.isRunning = true;
		}

		protected override void StopDetectionInternal()
		{
			if (!this.started)
			{
				return;
			}
			this.detectionAction = null;
			this.started = false;
			this.isRunning = false;
		}

		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (ObscuredCheatingDetector.Instance == this)
			{
				ObscuredCheatingDetector.Instance = null;
			}
		}
	}
}

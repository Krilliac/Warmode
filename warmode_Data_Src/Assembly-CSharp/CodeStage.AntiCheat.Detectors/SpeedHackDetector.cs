using System;
using UnityEngine;
using UnityEngine.Events;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("Code Stage/Anti-Cheat Toolkit/Speed Hack Detector")]
	public class SpeedHackDetector : ActDetectorBase
	{
		internal const string COMPONENT_NAME = "Speed Hack Detector";

		internal const string FINAL_LOG_PREFIX = "[ACTk] Speed Hack Detector: ";

		private const long TICKS_PER_SECOND = 10000000L;

		private const int THRESHOLD = 5000000;

		private static int instancesInScene;

		[Tooltip("Time (in seconds) between detector checks.")]
		public float interval = 1f;

		[Tooltip("Maximum false positives count allowed before registering speed hack.")]
		public byte maxFalsePositives = 3;

		[Tooltip("Amount of sequential successful checks before clearing internal false positives counter.\nSet 0 to disable Cool Down feature.")]
		public int coolDown = 30;

		private byte currentFalsePositives;

		private int currentCooldownShots;

		private long ticksOnStart;

		private long vulnerableTicksOnStart;

		private long prevTicks;

		private long prevIntervalTicks;

		public static SpeedHackDetector Instance
		{
			get;
			private set;
		}

		private static SpeedHackDetector GetOrCreateInstance
		{
			get
			{
				if (SpeedHackDetector.Instance != null)
				{
					return SpeedHackDetector.Instance;
				}
				if (ActDetectorBase.detectorsContainer == null)
				{
					ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
				}
				SpeedHackDetector.Instance = ActDetectorBase.detectorsContainer.AddComponent<SpeedHackDetector>();
				return SpeedHackDetector.Instance;
			}
		}

		private SpeedHackDetector()
		{
		}

		public static void StartDetection()
		{
			if (SpeedHackDetector.Instance != null)
			{
				SpeedHackDetector.Instance.StartDetectionInternal(null, SpeedHackDetector.Instance.interval, SpeedHackDetector.Instance.maxFalsePositives, SpeedHackDetector.Instance.coolDown);
			}
			else
			{
				Debug.LogError("[ACTk] Speed Hack Detector: can't be started since it doesn't exists in scene or not yet initialized!");
			}
		}

		public static void StartDetection(UnityAction callback)
		{
			SpeedHackDetector.StartDetection(callback, SpeedHackDetector.GetOrCreateInstance.interval);
		}

		public static void StartDetection(UnityAction callback, float interval)
		{
			SpeedHackDetector.StartDetection(callback, interval, SpeedHackDetector.GetOrCreateInstance.maxFalsePositives);
		}

		public static void StartDetection(UnityAction callback, float interval, byte maxFalsePositives)
		{
			SpeedHackDetector.StartDetection(callback, interval, maxFalsePositives, SpeedHackDetector.GetOrCreateInstance.coolDown);
		}

		public static void StartDetection(UnityAction callback, float interval, byte maxFalsePositives, int coolDown)
		{
			SpeedHackDetector.GetOrCreateInstance.StartDetectionInternal(callback, interval, maxFalsePositives, coolDown);
		}

		public static void StopDetection()
		{
			if (SpeedHackDetector.Instance != null)
			{
				SpeedHackDetector.Instance.StopDetectionInternal();
			}
		}

		public static void Dispose()
		{
			if (SpeedHackDetector.Instance != null)
			{
				SpeedHackDetector.Instance.DisposeInternal();
			}
		}

		private void Awake()
		{
			SpeedHackDetector.instancesInScene++;
			if (this.Init(SpeedHackDetector.Instance, "Speed Hack Detector"))
			{
				SpeedHackDetector.Instance = this;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			SpeedHackDetector.instancesInScene--;
		}

		private void OnLevelWasLoaded()
		{
			this.OnLevelLoadedCallback();
		}

		private void OnLevelLoadedCallback()
		{
			if (SpeedHackDetector.instancesInScene < 2)
			{
				if (!this.keepAlive)
				{
					this.DisposeInternal();
				}
			}
			else if (!this.keepAlive && SpeedHackDetector.Instance != this)
			{
				this.DisposeInternal();
			}
		}

		private void OnApplicationPause(bool pause)
		{
			if (!pause)
			{
				this.ResetStartTicks();
			}
		}

		private void Update()
		{
			if (!this.isRunning)
			{
				return;
			}
			long ticks = DateTime.UtcNow.Ticks;
			long num = ticks - this.prevTicks;
			if (num < 0L || num > 10000000L)
			{
				this.ResetStartTicks();
				return;
			}
			this.prevTicks = ticks;
			long num2 = (long)(this.interval * 1E+07f);
			if (ticks - this.prevIntervalTicks >= num2)
			{
				long num3 = (long)Environment.TickCount * 10000L;
				if (Mathf.Abs((float)(num3 - this.vulnerableTicksOnStart - (ticks - this.ticksOnStart))) > 5000000f)
				{
					this.currentFalsePositives += 1;
					if (this.currentFalsePositives > this.maxFalsePositives)
					{
						this.OnCheatingDetected();
					}
					else
					{
						this.currentCooldownShots = 0;
						this.ResetStartTicks();
					}
				}
				else if (this.currentFalsePositives > 0 && this.coolDown > 0)
				{
					this.currentCooldownShots++;
					if (this.currentCooldownShots >= this.coolDown)
					{
						this.currentFalsePositives = 0;
					}
				}
				this.prevIntervalTicks = ticks;
			}
		}

		private void StartDetectionInternal(UnityAction callback, float checkInterval, byte falsePositives, int shotsTillCooldown)
		{
			if (this.isRunning)
			{
				Debug.LogWarning("[ACTk] Speed Hack Detector: already running!", this);
				return;
			}
			if (!base.enabled)
			{
				Debug.LogWarning("[ACTk] Speed Hack Detector: disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
				return;
			}
			if (callback != null && this.detectionEventHasListener)
			{
				Debug.LogWarning("[ACTk] Speed Hack Detector: has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?", this);
			}
			if (callback == null && !this.detectionEventHasListener)
			{
				Debug.LogWarning("[ACTk] Speed Hack Detector: was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
				base.enabled = false;
				return;
			}
			this.detectionAction = callback;
			this.interval = checkInterval;
			this.maxFalsePositives = falsePositives;
			this.coolDown = shotsTillCooldown;
			this.ResetStartTicks();
			this.currentFalsePositives = 0;
			this.currentCooldownShots = 0;
			this.started = true;
			this.isRunning = true;
		}

		protected override void StartDetectionAutomatically()
		{
			this.StartDetectionInternal(null, this.interval, this.maxFalsePositives, this.coolDown);
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
			if (SpeedHackDetector.Instance == this)
			{
				SpeedHackDetector.Instance = null;
			}
		}

		private void ResetStartTicks()
		{
			this.ticksOnStart = DateTime.UtcNow.Ticks;
			this.vulnerableTicksOnStart = (long)Environment.TickCount * 10000L;
			this.prevTicks = this.ticksOnStart;
			this.prevIntervalTicks = this.ticksOnStart;
		}
	}
}

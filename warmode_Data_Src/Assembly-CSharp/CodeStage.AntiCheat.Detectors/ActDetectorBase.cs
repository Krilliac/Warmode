using System;
using UnityEngine;
using UnityEngine.Events;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("")]
	public abstract class ActDetectorBase : MonoBehaviour
	{
		protected const string CONTAINER_NAME = "Anti-Cheat Toolkit Detectors";

		protected const string MENU_PATH = "Code Stage/Anti-Cheat Toolkit/";

		protected const string GAME_OBJECT_MENU_PATH = "GameObject/Create Other/Code Stage/Anti-Cheat Toolkit/";

		protected static GameObject detectorsContainer;

		[Tooltip("Automatically start detector. Detection Event will be called on detection.")]
		public bool autoStart = true;

		[Tooltip("Detector will survive new level (scene) load if checked.")]
		public bool keepAlive = true;

		[Tooltip("Automatically dispose Detector after firing callback.")]
		public bool autoDispose = true;

		[SerializeField]
		protected UnityEvent detectionEvent;

		protected UnityAction detectionAction;

		[SerializeField]
		protected bool detectionEventHasListener;

		protected bool isRunning;

		protected bool started;

		private void Start()
		{
			if (ActDetectorBase.detectorsContainer == null && base.gameObject.name == "Anti-Cheat Toolkit Detectors")
			{
				ActDetectorBase.detectorsContainer = base.gameObject;
			}
			if (this.autoStart && !this.started)
			{
				this.StartDetectionAutomatically();
			}
		}

		private void OnEnable()
		{
			if (!this.started || (!this.detectionEventHasListener && this.detectionAction == null))
			{
				return;
			}
			this.ResumeDetector();
		}

		private void OnDisable()
		{
			if (!this.started)
			{
				return;
			}
			this.PauseDetector();
		}

		private void OnApplicationQuit()
		{
			this.DisposeInternal();
		}

		protected virtual void OnDestroy()
		{
			this.StopDetectionInternal();
			if (base.transform.childCount == 0 && base.GetComponentsInChildren<Component>().Length <= 2)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else if (base.name == "Anti-Cheat Toolkit Detectors" && base.GetComponentsInChildren<ActDetectorBase>().Length <= 1)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		protected virtual bool Init(ActDetectorBase instance, string detectorName)
		{
			if (instance != null && instance != this && instance.keepAlive)
			{
				if (GameData.gSteam)
				{
					Debug.LogWarning("[ACTk] " + base.name + ": self-destroying, other instance already exists & only one instance allowed!", base.gameObject);
				}
				UnityEngine.Object.Destroy(this);
				return false;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			return true;
		}

		protected virtual void DisposeInternal()
		{
			UnityEngine.Object.Destroy(this);
		}

		internal virtual void OnCheatingDetected()
		{
			if (this.detectionAction != null)
			{
				this.detectionAction();
			}
			if (this.detectionEventHasListener)
			{
				this.detectionEvent.Invoke();
			}
			if (this.autoDispose)
			{
				this.DisposeInternal();
			}
			else
			{
				this.StopDetectionInternal();
			}
		}

		protected abstract void StartDetectionAutomatically();

		protected abstract void StopDetectionInternal();

		protected abstract void PauseDetector();

		protected abstract void ResumeDetector();
	}
}

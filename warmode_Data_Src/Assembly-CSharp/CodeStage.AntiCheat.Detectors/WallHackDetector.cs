using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("Code Stage/Anti-Cheat Toolkit/WallHack Detector")]
	public class WallHackDetector : ActDetectorBase
	{
		internal const string COMPONENT_NAME = "WallHack Detector";

		internal const string FINAL_LOG_PREFIX = "[ACTk] WallHack Detector: ";

		private const string SERVICE_CONTAINER_NAME = "[WH Detector Service]";

		private const string WIREFRAME_SHADER_NAME = "Hidden/ACTk/WallHackTexture";

		private const int SHADER_TEXTURE_SIZE = 4;

		private const int RENDER_TEXTURE_SIZE = 4;

		private readonly Vector3 rigidPlayerVelocity = new Vector3(0f, 0f, 1f);

		private static int instancesInScene;

		private readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

		[SerializeField, Tooltip("Check for the \"walk through the walls\" kind of cheats made via Rigidbody hacks?")]
		private bool checkRigidbody = true;

		[SerializeField, Tooltip("Check for the \"walk through the walls\" kind of cheats made via Character Controller hacks?")]
		private bool checkController = true;

		[SerializeField, Tooltip("Check for the \"see through the walls\" kind of cheats made via shader or driver hacks (wireframe, color alpha, etc.)?")]
		private bool checkWireframe = true;

		[SerializeField, Tooltip("Check for the \"shoot through the walls\" kind of cheats made via Raycast hacks?")]
		private bool checkRaycast = true;

		[Range(1f, 60f), Tooltip("Delay between Wireframe module checks, from 1 up to 60 secs.")]
		public int wireframeDelay = 10;

		[Range(1f, 60f), Tooltip("Delay between Raycast module checks, from 1 up to 60 secs.")]
		public int raycastDelay = 10;

		[Tooltip("World position of the container for service objects within 3x3x3 cube (drawn as red wire cube in scene).")]
		public Vector3 spawnPosition;

		[Tooltip("Maximum false positives in a row for each detection module before registering a wall hack.")]
		public byte maxFalsePositives = 3;

		private GameObject serviceContainer;

		private GameObject solidWall;

		private GameObject thinWall;

		private Camera wfCamera;

		private MeshRenderer foregroundRenderer;

		private MeshRenderer backgroundRenderer;

		private Color wfColor1 = Color.black;

		private Color wfColor2 = Color.black;

		private Shader wfShader;

		private Material wfMaterial;

		private Texture2D shaderTexture;

		private Texture2D targetTexture;

		private RenderTexture renderTexture;

		private int whLayer = -1;

		private int raycastMask = -1;

		private Rigidbody rigidPlayer;

		private CharacterController charControllerPlayer;

		private float charControllerVelocity;

		private byte rigidbodyDetections;

		private byte controllerDetections;

		private byte wireframeDetections;

		private byte raycastDetections;

		private bool wireframeDetected;

		public bool CheckRigidbody
		{
			get
			{
				return this.checkRigidbody;
			}
			set
			{
				if (this.checkRigidbody == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
				{
					return;
				}
				this.checkRigidbody = value;
				if (!this.started)
				{
					return;
				}
				this.UpdateServiceContainer();
				if (this.checkRigidbody)
				{
					this.StartRigidModule();
				}
				else
				{
					this.StopRigidModule();
				}
			}
		}

		public bool CheckController
		{
			get
			{
				return this.checkController;
			}
			set
			{
				if (this.checkController == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
				{
					return;
				}
				this.checkController = value;
				if (!this.started)
				{
					return;
				}
				this.UpdateServiceContainer();
				if (this.checkController)
				{
					this.StartControllerModule();
				}
				else
				{
					this.StopControllerModule();
				}
			}
		}

		public bool CheckWireframe
		{
			get
			{
				return this.checkWireframe;
			}
			set
			{
				if (this.checkWireframe == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
				{
					return;
				}
				this.checkWireframe = value;
				if (!this.started)
				{
					return;
				}
				this.UpdateServiceContainer();
				if (this.checkWireframe)
				{
					this.StartWireframeModule();
				}
				else
				{
					this.StopWireframeModule();
				}
			}
		}

		public bool CheckRaycast
		{
			get
			{
				return this.checkRaycast;
			}
			set
			{
				if (this.checkRaycast == value || !Application.isPlaying || !base.enabled || !base.gameObject.activeSelf)
				{
					return;
				}
				this.checkRaycast = value;
				if (!this.started)
				{
					return;
				}
				this.UpdateServiceContainer();
				if (this.checkRaycast)
				{
					this.StartRaycastModule();
				}
				else
				{
					this.StopRaycastModule();
				}
			}
		}

		public static WallHackDetector Instance
		{
			get;
			private set;
		}

		private static WallHackDetector GetOrCreateInstance
		{
			get
			{
				if (WallHackDetector.Instance != null)
				{
					return WallHackDetector.Instance;
				}
				if (ActDetectorBase.detectorsContainer == null)
				{
					ActDetectorBase.detectorsContainer = new GameObject("Anti-Cheat Toolkit Detectors");
				}
				WallHackDetector.Instance = ActDetectorBase.detectorsContainer.AddComponent<WallHackDetector>();
				return WallHackDetector.Instance;
			}
		}

		private WallHackDetector()
		{
		}

		public static void StartDetection()
		{
			if (WallHackDetector.Instance != null)
			{
				WallHackDetector.Instance.StartDetectionInternal(null, WallHackDetector.Instance.spawnPosition, WallHackDetector.Instance.maxFalsePositives);
			}
			else
			{
				UnityEngine.Debug.LogError("[ACTk] WallHack Detector: can't be started since it doesn't exists in scene or not yet initialized!");
			}
		}

		public static void StartDetection(UnityAction callback)
		{
			WallHackDetector.StartDetection(callback, WallHackDetector.GetOrCreateInstance.spawnPosition);
		}

		public static void StartDetection(UnityAction callback, Vector3 spawnPosition)
		{
			WallHackDetector.StartDetection(callback, spawnPosition, WallHackDetector.GetOrCreateInstance.maxFalsePositives);
		}

		public static void StartDetection(UnityAction callback, Vector3 spawnPosition, byte maxFalsePositives)
		{
			WallHackDetector.GetOrCreateInstance.StartDetectionInternal(callback, spawnPosition, maxFalsePositives);
		}

		public static void StopDetection()
		{
			if (WallHackDetector.Instance != null)
			{
				WallHackDetector.Instance.StopDetectionInternal();
			}
		}

		public static void Dispose()
		{
			if (WallHackDetector.Instance != null)
			{
				WallHackDetector.Instance.DisposeInternal();
			}
		}

		private void Awake()
		{
			WallHackDetector.instancesInScene++;
			if (this.Init(WallHackDetector.Instance, "WallHack Detector"))
			{
				WallHackDetector.Instance = this;
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			base.StopAllCoroutines();
			if (this.serviceContainer != null)
			{
				UnityEngine.Object.Destroy(this.serviceContainer);
			}
			if (this.wfMaterial != null)
			{
				this.wfMaterial.mainTexture = null;
				this.wfMaterial.shader = null;
				this.wfMaterial = null;
				this.wfShader = null;
				this.shaderTexture = null;
				this.targetTexture = null;
				this.renderTexture.DiscardContents();
				this.renderTexture.Release();
				this.renderTexture = null;
			}
			WallHackDetector.instancesInScene--;
		}

		private void OnLevelWasLoaded()
		{
			this.OnLevelLoadedCallback();
		}

		private void OnLevelLoadedCallback()
		{
			if (WallHackDetector.instancesInScene < 2)
			{
				if (!this.keepAlive)
				{
					this.DisposeInternal();
				}
			}
			else if (!this.keepAlive && WallHackDetector.Instance != this)
			{
				this.DisposeInternal();
			}
		}

		private void FixedUpdate()
		{
			if (!this.isRunning || !this.checkRigidbody || this.rigidPlayer == null)
			{
				return;
			}
			if (this.rigidPlayer.transform.localPosition.z > 1f)
			{
				this.rigidbodyDetections += 1;
				if (!this.Detect())
				{
					this.StopRigidModule();
					this.StartRigidModule();
				}
			}
		}

		private void Update()
		{
			if (!this.isRunning || !this.checkController || this.charControllerPlayer == null)
			{
				return;
			}
			if (this.charControllerVelocity > 0f)
			{
				this.charControllerPlayer.Move(new Vector3(UnityEngine.Random.Range(-0.002f, 0.002f), 0f, this.charControllerVelocity));
				if (this.charControllerPlayer.transform.localPosition.z > 1f)
				{
					this.controllerDetections += 1;
					if (!this.Detect())
					{
						this.StopControllerModule();
						this.StartControllerModule();
					}
				}
			}
		}

		private void StartDetectionInternal(UnityAction callback, Vector3 servicePosition, byte falsePositivesInRow)
		{
			if (this.isRunning)
			{
				UnityEngine.Debug.LogWarning("[ACTk] WallHack Detector: already running!", this);
				return;
			}
			if (!base.enabled)
			{
				UnityEngine.Debug.LogWarning("[ACTk] WallHack Detector: disabled but StartDetection still called from somewhere (see stack trace for this message)!", this);
				return;
			}
			if (callback != null && this.detectionEventHasListener)
			{
				UnityEngine.Debug.LogWarning("[ACTk] WallHack Detector: has properly configured Detection Event in the inspector, but still get started with Action callback. Both Action and Detection Event will be called on detection. Are you sure you wish to do this?", this);
			}
			if (callback == null && !this.detectionEventHasListener)
			{
				UnityEngine.Debug.LogWarning("[ACTk] WallHack Detector: was started without any callbacks. Please configure Detection Event in the inspector, or pass the callback Action to the StartDetection method.", this);
				base.enabled = false;
				return;
			}
			this.detectionAction = callback;
			this.spawnPosition = servicePosition;
			this.maxFalsePositives = falsePositivesInRow;
			this.rigidbodyDetections = 0;
			this.controllerDetections = 0;
			this.wireframeDetections = 0;
			this.raycastDetections = 0;
			base.StartCoroutine(this.InitDetector());
			this.started = true;
			this.isRunning = true;
		}

		protected override void StartDetectionAutomatically()
		{
			this.StartDetectionInternal(null, this.spawnPosition, this.maxFalsePositives);
		}

		protected override void PauseDetector()
		{
			if (!this.isRunning)
			{
				return;
			}
			this.isRunning = false;
			this.StopRigidModule();
			this.StopControllerModule();
			this.StopWireframeModule();
			this.StopRaycastModule();
		}

		protected override void ResumeDetector()
		{
			if (this.detectionAction == null && !this.detectionEventHasListener)
			{
				return;
			}
			this.isRunning = true;
			if (this.checkRigidbody)
			{
				this.StartRigidModule();
			}
			if (this.checkController)
			{
				this.StartControllerModule();
			}
			if (this.checkWireframe)
			{
				this.StartWireframeModule();
			}
			if (this.checkRaycast)
			{
				this.StartRaycastModule();
			}
		}

		protected override void StopDetectionInternal()
		{
			if (!this.started)
			{
				return;
			}
			this.PauseDetector();
			this.detectionAction = null;
			this.isRunning = false;
		}

		protected override void DisposeInternal()
		{
			base.DisposeInternal();
			if (WallHackDetector.Instance == this)
			{
				WallHackDetector.Instance = null;
			}
		}

		private void UpdateServiceContainer()
		{
			if (base.enabled && base.gameObject.activeSelf)
			{
				if (this.whLayer == -1)
				{
					this.whLayer = LayerMask.NameToLayer("Ignore Raycast");
				}
				if (this.raycastMask == -1)
				{
					this.raycastMask = LayerMask.GetMask(new string[]
					{
						"Ignore Raycast"
					});
				}
				if (this.serviceContainer == null)
				{
					this.serviceContainer = new GameObject("[WH Detector Service]");
					this.serviceContainer.layer = this.whLayer;
					this.serviceContainer.transform.position = this.spawnPosition;
					UnityEngine.Object.DontDestroyOnLoad(this.serviceContainer);
				}
				if ((this.checkRigidbody || this.checkController) && this.solidWall == null)
				{
					this.solidWall = new GameObject("SolidWall");
					this.solidWall.AddComponent<BoxCollider>();
					this.solidWall.layer = this.whLayer;
					this.solidWall.transform.parent = this.serviceContainer.transform;
					this.solidWall.transform.localScale = new Vector3(3f, 3f, 0.5f);
					this.solidWall.transform.localPosition = Vector3.zero;
				}
				else if (!this.checkRigidbody && !this.checkController && this.solidWall != null)
				{
					UnityEngine.Object.Destroy(this.solidWall);
				}
				if (this.checkWireframe && this.wfCamera == null)
				{
					if (this.wfShader == null)
					{
						this.wfShader = Shader.Find("Hidden/ACTk/WallHackTexture");
					}
					if (this.wfShader == null)
					{
						UnityEngine.Debug.LogError("[ACTk] WallHack Detector: can't find 'Hidden/ACTk/WallHackTexture' shader!\nPlease make sure you have it included at the Editor > Project Settings > Graphics.", this);
						this.checkWireframe = false;
					}
					else if (!this.wfShader.isSupported)
					{
						UnityEngine.Debug.LogError("[ACTk] WallHack Detector: can't detect wireframe cheats on this platform!", this);
						this.checkWireframe = false;
					}
					else
					{
						if (this.wfColor1 == Color.black)
						{
							this.wfColor1 = WallHackDetector.GenerateColor();
							do
							{
								this.wfColor2 = WallHackDetector.GenerateColor();
							}
							while (WallHackDetector.ColorsSimilar(this.wfColor1, this.wfColor2, 10));
						}
						if (this.shaderTexture == null)
						{
							this.shaderTexture = new Texture2D(4, 4, TextureFormat.RGB24, false);
							this.shaderTexture.filterMode = FilterMode.Point;
							Color[] array = new Color[16];
							for (int i = 0; i < 16; i++)
							{
								if (i < 8)
								{
									array[i] = this.wfColor1;
								}
								else
								{
									array[i] = this.wfColor2;
								}
							}
							this.shaderTexture.SetPixels(array, 0);
							this.shaderTexture.Apply();
						}
						if (this.renderTexture == null)
						{
							this.renderTexture = new RenderTexture(4, 4, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
							this.renderTexture.generateMips = false;
							this.renderTexture.filterMode = FilterMode.Point;
							this.renderTexture.Create();
						}
						if (this.targetTexture == null)
						{
							this.targetTexture = new Texture2D(4, 4, TextureFormat.RGB24, false);
							this.targetTexture.filterMode = FilterMode.Point;
						}
						if (this.wfMaterial == null)
						{
							this.wfMaterial = new Material(this.wfShader);
							this.wfMaterial.mainTexture = this.shaderTexture;
						}
						if (this.foregroundRenderer == null)
						{
							GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
							UnityEngine.Object.Destroy(gameObject.GetComponent<BoxCollider>());
							gameObject.name = "WireframeFore";
							gameObject.layer = this.whLayer;
							gameObject.transform.parent = this.serviceContainer.transform;
							gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
							this.foregroundRenderer = gameObject.GetComponent<MeshRenderer>();
							this.foregroundRenderer.sharedMaterial = this.wfMaterial;
							this.foregroundRenderer.shadowCastingMode = ShadowCastingMode.Off;
							this.foregroundRenderer.receiveShadows = false;
							this.foregroundRenderer.enabled = false;
						}
						if (this.backgroundRenderer == null)
						{
							GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Quad);
							UnityEngine.Object.Destroy(gameObject2.GetComponent<MeshCollider>());
							gameObject2.name = "WireframeBack";
							gameObject2.layer = this.whLayer;
							gameObject2.transform.parent = this.serviceContainer.transform;
							gameObject2.transform.localPosition = new Vector3(0f, 0f, 1f);
							gameObject2.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
							this.backgroundRenderer = gameObject2.GetComponent<MeshRenderer>();
							this.backgroundRenderer.sharedMaterial = this.wfMaterial;
							this.backgroundRenderer.shadowCastingMode = ShadowCastingMode.Off;
							this.backgroundRenderer.receiveShadows = false;
							this.backgroundRenderer.enabled = false;
						}
						if (this.wfCamera == null)
						{
							this.wfCamera = new GameObject("WireframeCamera").AddComponent<Camera>();
							this.wfCamera.gameObject.layer = this.whLayer;
							this.wfCamera.transform.parent = this.serviceContainer.transform;
							this.wfCamera.transform.localPosition = new Vector3(0f, 0f, -1f);
							this.wfCamera.clearFlags = CameraClearFlags.Color;
							this.wfCamera.backgroundColor = Color.black;
							this.wfCamera.orthographic = true;
							this.wfCamera.orthographicSize = 0.5f;
							this.wfCamera.nearClipPlane = 0.01f;
							this.wfCamera.farClipPlane = 2.1f;
							this.wfCamera.depth = 0f;
							this.wfCamera.renderingPath = RenderingPath.Forward;
							this.wfCamera.useOcclusionCulling = false;
							this.wfCamera.hdr = false;
							this.wfCamera.targetTexture = this.renderTexture;
							this.wfCamera.enabled = false;
						}
					}
				}
				else if (!this.checkWireframe && this.wfCamera != null)
				{
					UnityEngine.Object.Destroy(this.foregroundRenderer.gameObject);
					UnityEngine.Object.Destroy(this.backgroundRenderer.gameObject);
					this.wfCamera.targetTexture = null;
					UnityEngine.Object.Destroy(this.wfCamera.gameObject);
				}
				if (this.checkRaycast && this.thinWall == null)
				{
					this.thinWall = GameObject.CreatePrimitive(PrimitiveType.Plane);
					this.thinWall.name = "ThinWall";
					this.thinWall.layer = this.whLayer;
					this.thinWall.transform.parent = this.serviceContainer.transform;
					this.thinWall.transform.localScale = new Vector3(0.2f, 1f, 0.2f);
					this.thinWall.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
					this.thinWall.transform.localPosition = new Vector3(0f, 0f, 1.4f);
					UnityEngine.Object.Destroy(this.thinWall.GetComponent<Renderer>());
					UnityEngine.Object.Destroy(this.thinWall.GetComponent<MeshFilter>());
				}
				else if (!this.checkRaycast && this.thinWall != null)
				{
					UnityEngine.Object.Destroy(this.thinWall);
				}
			}
			else if (this.serviceContainer != null)
			{
				UnityEngine.Object.Destroy(this.serviceContainer);
			}
		}

		[DebuggerHidden]
		private IEnumerator InitDetector()
		{
			WallHackDetector.<InitDetector>c__Iterator5 <InitDetector>c__Iterator = new WallHackDetector.<InitDetector>c__Iterator5();
			<InitDetector>c__Iterator.<>f__this = this;
			return <InitDetector>c__Iterator;
		}

		private void StartRigidModule()
		{
			if (!this.checkRigidbody)
			{
				this.StopRigidModule();
				this.UninitRigidModule();
				this.UpdateServiceContainer();
				return;
			}
			if (!this.rigidPlayer)
			{
				this.InitRigidModule();
			}
			if (this.rigidPlayer.transform.localPosition.z <= 1f && this.rigidbodyDetections > 0)
			{
				this.rigidbodyDetections = 0;
			}
			this.rigidPlayer.rotation = Quaternion.identity;
			this.rigidPlayer.angularVelocity = Vector3.zero;
			this.rigidPlayer.transform.localPosition = new Vector3(0.75f, 0f, -1f);
			this.rigidPlayer.velocity = this.rigidPlayerVelocity;
			base.Invoke("StartRigidModule", 4f);
		}

		private void StartControllerModule()
		{
			if (!this.checkController)
			{
				this.StopControllerModule();
				this.UninitControllerModule();
				this.UpdateServiceContainer();
				return;
			}
			if (!this.charControllerPlayer)
			{
				this.InitControllerModule();
			}
			if (this.charControllerPlayer.transform.localPosition.z <= 1f && this.controllerDetections > 0)
			{
				this.controllerDetections = 0;
			}
			this.charControllerPlayer.transform.localPosition = new Vector3(-0.75f, 0f, -1f);
			this.charControllerVelocity = 0.01f;
			base.Invoke("StartControllerModule", 4f);
		}

		private void StartWireframeModule()
		{
			if (!this.checkWireframe)
			{
				this.StopWireframeModule();
				this.UpdateServiceContainer();
				return;
			}
			if (!this.wireframeDetected)
			{
				base.Invoke("ShootWireframeModule", (float)this.wireframeDelay);
			}
		}

		private void ShootWireframeModule()
		{
			base.StartCoroutine(this.CaptureFrame());
			base.Invoke("ShootWireframeModule", (float)this.wireframeDelay);
		}

		[DebuggerHidden]
		private IEnumerator CaptureFrame()
		{
			WallHackDetector.<CaptureFrame>c__Iterator6 <CaptureFrame>c__Iterator = new WallHackDetector.<CaptureFrame>c__Iterator6();
			<CaptureFrame>c__Iterator.<>f__this = this;
			return <CaptureFrame>c__Iterator;
		}

		private void StartRaycastModule()
		{
			if (!this.checkRaycast)
			{
				this.StopRaycastModule();
				this.UpdateServiceContainer();
				return;
			}
			base.Invoke("ShootRaycastModule", (float)this.raycastDelay);
		}

		private void ShootRaycastModule()
		{
			if (Physics.Raycast(this.serviceContainer.transform.position, this.serviceContainer.transform.TransformDirection(Vector3.forward), 1.5f, this.raycastMask))
			{
				if (this.raycastDetections > 0)
				{
					this.raycastDetections = 0;
				}
			}
			else
			{
				this.raycastDetections += 1;
				if (this.Detect())
				{
					return;
				}
			}
			base.Invoke("ShootRaycastModule", (float)this.raycastDelay);
		}

		private void StopRigidModule()
		{
			if (this.rigidPlayer)
			{
				this.rigidPlayer.velocity = Vector3.zero;
			}
			base.CancelInvoke("StartRigidModule");
		}

		private void StopControllerModule()
		{
			if (this.charControllerPlayer)
			{
				this.charControllerVelocity = 0f;
			}
			base.CancelInvoke("StartControllerModule");
		}

		private void StopWireframeModule()
		{
			base.CancelInvoke("ShootWireframeModule");
		}

		private void StopRaycastModule()
		{
			base.CancelInvoke("ShootRaycastModule");
		}

		private void InitRigidModule()
		{
			GameObject gameObject = new GameObject("RigidPlayer");
			gameObject.AddComponent<CapsuleCollider>().height = 2f;
			gameObject.layer = this.whLayer;
			gameObject.transform.parent = this.serviceContainer.transform;
			gameObject.transform.localPosition = new Vector3(0.75f, 0f, -1f);
			this.rigidPlayer = gameObject.AddComponent<Rigidbody>();
			this.rigidPlayer.useGravity = false;
		}

		private void InitControllerModule()
		{
			GameObject gameObject = new GameObject("ControlledPlayer");
			gameObject.AddComponent<CapsuleCollider>().height = 2f;
			gameObject.layer = this.whLayer;
			gameObject.transform.parent = this.serviceContainer.transform;
			gameObject.transform.localPosition = new Vector3(-0.75f, 0f, -1f);
			this.charControllerPlayer = gameObject.AddComponent<CharacterController>();
		}

		private void UninitRigidModule()
		{
			if (!this.rigidPlayer)
			{
				return;
			}
			UnityEngine.Object.Destroy(this.rigidPlayer.gameObject);
			this.rigidPlayer = null;
		}

		private void UninitControllerModule()
		{
			if (!this.charControllerPlayer)
			{
				return;
			}
			UnityEngine.Object.Destroy(this.charControllerPlayer.gameObject);
			this.charControllerPlayer = null;
		}

		private bool Detect()
		{
			bool result = false;
			if (this.controllerDetections > this.maxFalsePositives || this.rigidbodyDetections > this.maxFalsePositives || this.wireframeDetections > this.maxFalsePositives || this.raycastDetections > this.maxFalsePositives)
			{
				this.OnCheatingDetected();
				result = true;
			}
			return result;
		}

		private static Color32 GenerateColor()
		{
			return new Color32((byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), (byte)UnityEngine.Random.Range(0, 256), 255);
		}

		private static bool ColorsSimilar(Color32 c1, Color32 c2, int tolerance)
		{
			return Math.Abs((int)(c1.r - c2.r)) < tolerance && Math.Abs((int)(c1.g - c2.g)) < tolerance && Math.Abs((int)(c1.b - c2.b)) < tolerance;
		}
	}
}

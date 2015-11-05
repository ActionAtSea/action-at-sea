using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UltimateToonWaterC))]
[ExecuteInEditMode]
public class UltimateToonWaterCEditor : Editor { 
	private SerializedObject dimObj;
	private SerializedObject wConfigSer;

	private bool showDefaultInspectorOptions = false;
	
	public bool openSetup = true;
	public bool openBasic = true;
	public bool openAppearance = true;
	public bool openWaves = true;
	public bool openDefIns = false;
	
	private WaterConfig waterConfig;
	
	private string lastConfigName;

	private SerializedProperty

		bUseGPU,
		bStickToTarget,
		bMoveTextures,
		bMoveMesh,
		bPreviewMode,
		tTarget,

		fNow,
		config,

		aWaveForms,
		terminator;
	
	void OnEnable () {
		dimObj = new SerializedObject(target);
		bUseGPU = dimObj.FindProperty("useGPU");
		bStickToTarget = dimObj.FindProperty("stickToTarget");
		bMoveTextures = dimObj.FindProperty("moveTextures");
		bMoveMesh = dimObj.FindProperty("meshWaves");
		bPreviewMode = dimObj.FindProperty("previewMode");
		tTarget = dimObj.FindProperty("target");












		fNow = dimObj.FindProperty("now");
		config = dimObj.FindProperty("config");
		InitWaterConfig();
		
		UltimateToonWaterC UTW = (UltimateToonWaterC)target;
		UTW.Init();

		EditorApplication.update += Simulate;
	}

	private void InitWaterConfig() {
		waterConfig = (WaterConfig) config.objectReferenceValue;
		if (waterConfig != null) {
			wConfigSer = new SerializedObject(waterConfig);
			aWaveForms = wConfigSer.FindProperty("waveForms");
		}
	}
	
	private void Simulate()
	{
		if (Application.isPlaying == false) {
			UltimateToonWaterC UTW = (UltimateToonWaterC)target;
			if (UTW != null) {
				if (UTW.previewMode == true) {
					EditorUtility.SetSelectedWireframeHidden(UTW.GetComponent<Renderer>(), true);
					UTW.now = (float) EditorApplication.timeSinceStartup;
					UTW.HandleWaves();
					UTW.HandleTexture();
				} else {
					EditorUtility.SetSelectedWireframeHidden(UTW.GetComponent<Renderer>(), false);
				}
			}
		}
	}
	
	void OnDisable()
	{
		EditorApplication.update -= Simulate;
	}
	
	
	public override void OnInspectorGUI ()
	{
		UltimateToonWaterC UTW = (UltimateToonWaterC)target;

		bool needGenerateMesh = false;
		bool needUpdateTextures = false;
		bool needUpdateColors = false;
		bool needUpdateTextureSize = false;
		bool needUpdateTextureProps = false;

		bPreviewMode.boolValue = GUILayout.Toggle(bPreviewMode.boolValue,"Preview water in editor.");

		openSetup = EditorGUILayout.Foldout(openSetup,"Setup:");
		bool safe = true;
		if (openSetup == true) {
			bUseGPU.boolValue = EditorGUILayout.Toggle("Use GPU rendering:",bUseGPU.boolValue);
			if (bUseGPU.boolValue == false) {
				EditorGUILayout.HelpBox("Always use GPU rendering if possible.\nThis is much faster!",MessageType.Warning);
			}
			bMoveMesh.boolValue = EditorGUILayout.Toggle("Generate Waves:",bMoveMesh.boolValue);
			bMoveTextures.boolValue = EditorGUILayout.Toggle("Move Textures:",bMoveTextures.boolValue);
			bStickToTarget.boolValue = EditorGUILayout.Toggle("Snap to Target:",bStickToTarget.boolValue);
			if (bStickToTarget.boolValue == true) {
				tTarget.objectReferenceValue = EditorGUILayout.ObjectField("Target:", tTarget.objectReferenceValue, typeof(Transform), true);
			}

			string lastName = "";
			if (config.objectReferenceValue != null) {
				lastName = config.objectReferenceValue.name;
			}

			config.objectReferenceValue = EditorGUILayout.ObjectField("Water Config:", config.objectReferenceValue, typeof(WaterConfig), true);
			if (config.objectReferenceValue != null && config.objectReferenceValue.name != lastName) {
				InitWaterConfig();
				needGenerateMesh = true;
				needUpdateTextures = true;
				needUpdateColors = true;
				needUpdateTextureSize = true;
				needUpdateTextureProps = true;
				safe = false;
			}
		}

		if (config.objectReferenceValue != null && safe == true) {
			openBasic = EditorGUILayout.Foldout(openBasic,"Basic settings:");
			if (openBasic == true) {
				float fSizeOld = waterConfig.size;
				float fMeshPointDistanceOld = waterConfig.meshPointDistance;

				waterConfig.size = EditorGUILayout.FloatField("Square Water Size: ",waterConfig.size);
				waterConfig.meshPointDistance = EditorGUILayout.FloatField("Meshpoint Distance: ",waterConfig.meshPointDistance);
				if (fSizeOld != waterConfig.size || fMeshPointDistanceOld != waterConfig.meshPointDistance) {
					needGenerateMesh = true;
				}
			}

			openAppearance = EditorGUILayout.Foldout(openAppearance,"Appearance:");
			if (openAppearance == true) {
				Color cWaterColorNew = waterConfig.waterColor;
				Color cWaveColorNew = waterConfig.waveColor;
				cWaterColorNew = EditorGUILayout.ColorField("Water Color:",cWaterColorNew);
				cWaveColorNew = EditorGUILayout.ColorField("Wave Color:",cWaveColorNew);
				cWaterColorNew.a = EditorGUILayout.Slider("Water transparency:",cWaterColorNew.a,0f,1f);

				if (cWaterColorNew !=  waterConfig.waterColor || cWaveColorNew != waterConfig.waveColor) {
					waterConfig.waterColor = cWaterColorNew;
					waterConfig.waveColor = cWaveColorNew;
					needUpdateColors = true;
				}

				float fColorTextureRatioOld = waterConfig.colorTextureRatio;
				float fHeightColoringOld = waterConfig.heightColoring;
				float fShoreFoamLineOld = waterConfig.shoreFoamLine;
				float fShoreFoamCutOld = waterConfig.shoreFoamCut;
				float fTextureBiasOld = waterConfig.textureBias;

				waterConfig.colorTextureRatio = EditorGUILayout.Slider("Color/Texture ratio:",waterConfig.colorTextureRatio,0f,1f);
				waterConfig.heightColoring = EditorGUILayout.Slider("Steepness coloring:",waterConfig.heightColoring,0f,1f);
				waterConfig.textureBias = EditorGUILayout.Slider("Texture mix:",waterConfig.textureBias,0f,1f);
				waterConfig.shoreFoamLine = EditorGUILayout.FloatField("Shore foam amount:",waterConfig.shoreFoamLine);
				waterConfig.shoreFoamCut = EditorGUILayout.Slider("Shoe foam cut:",waterConfig.shoreFoamCut,0f,1f);

				if (waterConfig.colorTextureRatio != fColorTextureRatioOld ||
				    waterConfig.heightColoring != fHeightColoringOld ||
				    waterConfig.textureBias != fTextureBiasOld ||
				    waterConfig.shoreFoamLine != fShoreFoamLineOld ||
				    waterConfig.shoreFoamCut != fShoreFoamCutOld
				    )
				{
					needUpdateTextureProps = true;
				}








				Object tMainTextureOld = waterConfig.mainTexture;
				Object tDetailTextureOld = waterConfig.detailTexture;
				
				Texture2D texMain = (Texture2D) EditorGUILayout.ObjectField("Main Texture:", (waterConfig.mainTexture != null ? waterConfig.mainTexture : null),typeof(Texture2D),false);
				if (texMain != null) {
					waterConfig.mainTexture = texMain;
				}
				Texture2D texDetail = (Texture2D) EditorGUILayout.ObjectField("Detail Texture:",(waterConfig.detailTexture != null ? waterConfig.detailTexture : null),typeof(Texture2D),false);
				if (texDetail != null) {
					waterConfig.detailTexture = texDetail;
				}

				if (waterConfig.mainTexture != tMainTextureOld || waterConfig.detailTexture != tDetailTextureOld) {
					needUpdateTextures = true;
				}

				Vector2 v2MainTextureTitleOld = waterConfig.mainTextureTitle;
				Vector2 v2DetailTextureTitleOld = waterConfig.detailTextureTitle;
				Vector2 v2MainTexturScrollOld = waterConfig.mainTexturScroll;
				Vector2 v2DetailTexturScrollOld = waterConfig.detailTexturScroll;

				waterConfig.mainTextureTitle = EditorGUILayout.Vector2Field("Main Texture Size:", waterConfig.mainTextureTitle);
				waterConfig.detailTextureTitle = EditorGUILayout.Vector2Field("Detail Texture Size:",waterConfig.detailTextureTitle);
				waterConfig.mainTexturScroll = EditorGUILayout.Vector2Field("Main Texture Scroll:",waterConfig.mainTexturScroll);
				waterConfig.detailTexturScroll = EditorGUILayout.Vector2Field("Detail Texture Scroll:",waterConfig.detailTexturScroll);

				if (waterConfig.mainTextureTitle != v2MainTextureTitleOld ||
				    waterConfig.detailTextureTitle != v2DetailTextureTitleOld ||
				    waterConfig.mainTexturScroll != v2MainTexturScrollOld ||
				    waterConfig.detailTexturScroll != v2DetailTexturScrollOld)
				{
					needUpdateTextureSize = true;
				}
			}

			openWaves = EditorGUILayout.Foldout(openWaves,"Waves:");
			if (openWaves == true) {
				if (waterConfig.waveForms.Count > 2) {
					EditorGUILayout.HelpBox("If more than 2 waves are used,\nCPU rendering will be enabled.\nThis is much slower than GPU rendering.",MessageType.Warning);
				}
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField(aWaveForms, true);


				if(EditorGUI.EndChangeCheck()) {
					wConfigSer.ApplyModifiedProperties();
				}
				//EditorGUIUtility.LookLikeControls();
			}
		} else {
			EditorGUILayout.HelpBox("Please select a water config first.",MessageType.Warning);
		}

		showDefaultInspectorOptions = EditorGUILayout.Foldout(showDefaultInspectorOptions,"Default Inspector:");
		if (showDefaultInspectorOptions == true) {
			EditorGUILayout.HelpBox("Only edit if you understand the functioning of the script!",MessageType.Warning);
			DrawDefaultInspector();
		}
		
		dimObj.ApplyModifiedProperties();
		if (wConfigSer != null) {
			wConfigSer.ApplyModifiedProperties();
		}

		if (needGenerateMesh == true) {
			int numVerts = (int) (waterConfig.size/waterConfig.meshPointDistance);
			numVerts *= numVerts;
			if (numVerts > 65000) {
				waterConfig.meshPointDistance = Mathf.CeilToInt(waterConfig.size / Mathf.Sqrt(65000f));
				dimObj.ApplyModifiedProperties();
			}
			
			UTW.generateMesh();
		}

		if (needUpdateTextures == true) {
			UTW.UpdateTextures();
		}

		if (needUpdateColors == true) {
			UTW.UpdateColor();
		}

		if (needUpdateTextureSize == true) {
			UTW.UpdateTextureSize();
		}

		if (needUpdateTextureProps == true) {
			UTW.UpdateTextureProps();
		}


		
	}



}

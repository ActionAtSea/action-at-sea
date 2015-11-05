using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UltimateToonWater))]
[ExecuteInEditMode]
public class UltimateToonWaterEditor : Editor { 
	private SerializedObject dimObj;
	
	private bool showDefaultInspectorOptions = false;

	public bool openSetup = true;
	public bool openBasic = true;
	public bool openAppearance = true;
	public bool openWaves = true;
	public bool openDefIns = false;
	 
	private SerializedProperty
		bUseGPU,
		bStickToTarget,
		bMoveTextures,
		bMoveMesh,
		bPreviewMode,
		tTarget,
		fSize,
		fMeshPointDistance,
		cWaterColor,
		cWaveColor,
		tMainTexture,
		tDetailTexture,
		v2MainTextureTitle,
		v2DetailTextureTitle,
		v2MainTexturScroll,
		v2DetailTexturScroll,
		fColorTextureRatio,
		fHeightColoring,
		fShoreFoamLine,
		fShoreFoamCut,
		fTextureBias,
		aWaveForms,
		fNow,
		terminator;
	
	void OnEnable () {
		dimObj = new SerializedObject(target);
		bUseGPU = dimObj.FindProperty("useGPU");
		bStickToTarget = dimObj.FindProperty("stickToTarget");
		bMoveTextures = dimObj.FindProperty("moveTextures");
		bMoveMesh = dimObj.FindProperty("moveMesh");
		bPreviewMode = dimObj.FindProperty("previewMode");
		tTarget = dimObj.FindProperty("target");
		fSize = dimObj.FindProperty("size");
		fMeshPointDistance = dimObj.FindProperty("meshPointDistance");
		cWaterColor = dimObj.FindProperty("waterColor");
		cWaveColor = dimObj.FindProperty("waveColor");
		tMainTexture = dimObj.FindProperty("mainTexture");
		tDetailTexture = dimObj.FindProperty("detailTexture");
		v2MainTextureTitle = dimObj.FindProperty("mainTextureTitle");
		v2DetailTextureTitle = dimObj.FindProperty("detailTextureTitle");
		v2MainTexturScroll = dimObj.FindProperty("mainTexturScroll");
		v2DetailTexturScroll = dimObj.FindProperty("detailTexturScroll");

		fColorTextureRatio = dimObj.FindProperty("colorTextureRatio");
		fHeightColoring = dimObj.FindProperty("heightColoring");
		fShoreFoamLine = dimObj.FindProperty("shoreFoamLine");
		fShoreFoamCut = dimObj.FindProperty("shoreFoamCut");
		fTextureBias = dimObj.FindProperty("textureBias");

		aWaveForms = dimObj.FindProperty("waveForms");
		fNow = dimObj.FindProperty("now");


		UltimateToonWater UTW = (UltimateToonWater)target;
		UTW.Init();

		EditorApplication.update += Simulate; 
	}
	
	private void Simulate()
	{
		if (Application.isPlaying == false) {
			UltimateToonWater UTW = (UltimateToonWater)target;
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
		UltimateToonWater UTW = (UltimateToonWater)target;

		bool needGenerateMesh = false;
		bool needUpdateTextures = false;
		bool needUpdateColors = false;
		bool needUpdateTextureSize = false;
		bool needUpdateTextureProps = false;
		
		bPreviewMode.boolValue = GUILayout.Toggle(bPreviewMode.boolValue,"Preview water in editor.");

		openSetup = EditorGUILayout.Foldout(openSetup,"Setup:");
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
			GUILayout.Label("* = Optional setting, can be left empty.");
		}



		openBasic = EditorGUILayout.Foldout(openBasic,"Basic settings:");
		if (openBasic == true) {
			float fSizeOld = fSize.floatValue;
			float fMeshPointDistanceOld = fMeshPointDistance.floatValue;

			fSize.floatValue = EditorGUILayout.FloatField("Square Water Size: ",fSize.floatValue);
			fMeshPointDistance.floatValue = EditorGUILayout.FloatField("Meshpoint Distance: ",fMeshPointDistance.floatValue);
			if (fSizeOld != fSize.floatValue || fMeshPointDistanceOld != fMeshPointDistance.floatValue) {
				needGenerateMesh = true;
			}
		}
		openAppearance = EditorGUILayout.Foldout(openAppearance,"Appearance:");
		if (openAppearance == true) {
			Color cWaterColorNew = cWaterColor.colorValue;
			Color cWaveColorNew = cWaveColor.colorValue;
			cWaterColorNew = EditorGUILayout.ColorField("Water Color:",cWaterColorNew);
			cWaveColorNew = EditorGUILayout.ColorField("Wave Color:",cWaveColorNew);
			cWaterColorNew.a = EditorGUILayout.Slider("Water transparency:",cWaterColorNew.a,0f,1f);

			if (cWaterColorNew != cWaterColor.colorValue || cWaveColorNew != cWaveColor.colorValue) {
				cWaterColor.colorValue = cWaterColorNew;
				cWaveColor.colorValue = cWaveColorNew;
				needUpdateColors = true;
			}

			float fColorTextureRatioOld = fColorTextureRatio.floatValue;
			float fHeightColoringOld = fHeightColoring.floatValue;
			float fShoreFoamLineOld = fShoreFoamLine.floatValue;
			float fShoreFoamCutOld = fShoreFoamCut.floatValue;
			float fTextureBiasOld = fTextureBias.floatValue;

			fColorTextureRatio.floatValue = EditorGUILayout.Slider("Color/Texture ratio:",fColorTextureRatio.floatValue,0f,1f);
			fTextureBias.floatValue = EditorGUILayout.Slider("Texture mix:",fTextureBias.floatValue,0f,1f);
			fHeightColoring.floatValue = EditorGUILayout.Slider("Steepness coloring:",fHeightColoring.floatValue,0f,1f);
			fShoreFoamLine.floatValue = EditorGUILayout.FloatField("Shore foam amount:",fShoreFoamLine.floatValue);
			fShoreFoamCut.floatValue = EditorGUILayout.Slider("Shoe foam cut:",fShoreFoamCut.floatValue,0f,1f);

			if (fColorTextureRatio.floatValue != fColorTextureRatioOld ||
			    fTextureBias.floatValue != fTextureBiasOld ||
			    fHeightColoring.floatValue != fHeightColoringOld ||
			    fShoreFoamLine.floatValue != fShoreFoamLineOld ||
			    fShoreFoamCut.floatValue != fShoreFoamCutOld
			    )
			{
				needUpdateTextureProps = true;
			}


			Object tMainTextureOld = tMainTexture.objectReferenceValue;
			Object tDetailTextureOld = tDetailTexture.objectReferenceValue;

			tMainTexture.objectReferenceValue = EditorGUILayout.ObjectField("Main Texture:", tMainTexture.objectReferenceValue,typeof(Texture2D),false);
			tDetailTexture.objectReferenceValue = EditorGUILayout.ObjectField("Detail Texture:",tDetailTexture.objectReferenceValue,typeof(Texture2D),false);


			if (tMainTexture.objectReferenceValue != tMainTextureOld || tDetailTexture.objectReferenceValue != tDetailTextureOld) {
				needUpdateTextures = true;
			}

			Vector2 v2MainTextureTitleOld = v2MainTextureTitle.vector2Value;
			Vector2 v2DetailTextureTitleOld = v2DetailTextureTitle.vector2Value;
			Vector2 v2MainTexturScrollOld = v2MainTexturScroll.vector2Value;
			Vector2 v2DetailTexturScrollOld = v2DetailTexturScroll.vector2Value;

			v2MainTextureTitle.vector2Value = EditorGUILayout.Vector2Field("Main Texture Size:",v2MainTextureTitle.vector2Value);
			v2DetailTextureTitle.vector2Value = EditorGUILayout.Vector2Field("Detail Texture Size:",v2DetailTextureTitle.vector2Value);
			v2MainTexturScroll.vector2Value = EditorGUILayout.Vector2Field("Main Texture Scroll:",v2MainTexturScroll.vector2Value);
			v2DetailTexturScroll.vector2Value = EditorGUILayout.Vector2Field("Detail Texture Scroll:",v2DetailTexturScroll.vector2Value);

			if (v2MainTextureTitle.vector2Value != v2MainTextureTitleOld ||
			    v2DetailTextureTitle.vector2Value != v2DetailTextureTitleOld ||
			    v2MainTexturScroll.vector2Value != v2MainTexturScrollOld ||
			    v2DetailTexturScroll.vector2Value != v2DetailTexturScrollOld)
			{
				needUpdateTextureSize = true;
			}
		}

		openWaves = EditorGUILayout.Foldout(openWaves,"Waves:");
		if (openWaves == true) {
			//GUILayout.Label("Warning!: If more than 2 waves are used,\nCPU rendering will be enabled.\nThis is much slower than GPU rendering.",warningStyle);
			if (UTW.waveForms.Count > 2) {
				EditorGUILayout.HelpBox("If more than 2 waves are used,\nCPU rendering will be enabled.\nThis is much slower than GPU rendering.",MessageType.Warning);
			}
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(aWaveForms, true);
			if(EditorGUI.EndChangeCheck()) {
				serializedObject.ApplyModifiedProperties();
			}
			EditorGUIUtility.LookLikeControls();
		}

		showDefaultInspectorOptions = EditorGUILayout.Foldout(showDefaultInspectorOptions,"Default Inspector:");
		if (showDefaultInspectorOptions == true) {
			EditorGUILayout.HelpBox("Only edit if you understand the functioning of the script!",MessageType.Warning);
			DrawDefaultInspector();
		}
		
		dimObj.ApplyModifiedProperties();
	

		if (needGenerateMesh == true) {
			int numVerts = (int) (fSize.floatValue/fMeshPointDistance.floatValue);
			numVerts *= numVerts;
			if (numVerts > 65000) {
				fMeshPointDistance.floatValue = Mathf.CeilToInt(fSize.floatValue / Mathf.Sqrt(65000f));
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

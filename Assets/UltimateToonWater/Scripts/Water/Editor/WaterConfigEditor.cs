using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(WaterConfig))]
public class WaterConfigEditor : Editor {

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
		tTarget,
		pFloaterParticles,
		pWaveParticles,
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
		terminator;
	
	void OnEnable () {
		dimObj = new SerializedObject(target);
		bMoveTextures = dimObj.FindProperty("moveTextures");
		bMoveMesh = dimObj.FindProperty("moveMesh");
		tTarget = dimObj.FindProperty("target");
		pFloaterParticles = dimObj.FindProperty("floaterParticles");
		pWaveParticles = dimObj.FindProperty("waveParticles");
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
	}

	public override void OnInspectorGUI ()
	{
		EditorGUILayout.HelpBox("Not all changes made during editing in the asset inspector will be applied to the scene.",MessageType.Warning);
		WaterConfig config = (WaterConfig)target;

		openBasic = EditorGUILayout.Foldout(openBasic,"Basic settings:");
		if (openBasic == true) {
			float fSizeOld = fSize.floatValue;
			float fMeshPointDistanceOld = fMeshPointDistance.floatValue;
			
			fSize.floatValue = EditorGUILayout.FloatField("Square Water Size: ",fSize.floatValue);
			fMeshPointDistance.floatValue = EditorGUILayout.FloatField("Meshpoint Distance: ",fMeshPointDistance.floatValue);
			if (fSizeOld != fSize.floatValue || fMeshPointDistanceOld != fMeshPointDistance.floatValue) {
				//needGenerateMesh = true;
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
				//needUpdateColors = true;
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
				//needUpdateTextureProps = true;
			}
			
			
			Object tMainTextureOld = tMainTexture.objectReferenceValue;
			Object tDetailTextureOld = tDetailTexture.objectReferenceValue;
			
			tMainTexture.objectReferenceValue = EditorGUILayout.ObjectField("Main Texture:", tMainTexture.objectReferenceValue,typeof(Texture2D),false);
			tDetailTexture.objectReferenceValue = EditorGUILayout.ObjectField("Detail Texture:",tDetailTexture.objectReferenceValue,typeof(Texture2D),false);
			
			
			if (tMainTexture.objectReferenceValue != tMainTextureOld || tDetailTexture.objectReferenceValue != tDetailTextureOld) {
				//needUpdateTextures = true;
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
				//needUpdateTextureSize = true;
			}
		}
		
		openWaves = EditorGUILayout.Foldout(openWaves,"Waves:");
		if (openWaves == true) {
			if (config.waveForms.Count > 2) {
				EditorGUILayout.HelpBox("If more than 2 waves are used,\nCPU rendering will be enabled.\nThis is much slower than GPU rendering.",MessageType.Warning);
			}
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(aWaveForms, true);
			if(EditorGUI.EndChangeCheck()) {
				serializedObject.ApplyModifiedProperties();
			}
		}

		showDefaultInspectorOptions = EditorGUILayout.Foldout(showDefaultInspectorOptions,"Default Inspector:");
		if (showDefaultInspectorOptions == true) {
			DrawDefaultInspector();
		}
		
		dimObj.ApplyModifiedProperties();
	}
}

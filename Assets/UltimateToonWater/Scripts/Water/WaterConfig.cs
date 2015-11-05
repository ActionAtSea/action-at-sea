using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaterConfig : ScriptableObject {
	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Water Config")]
	public static void CreateAsset ()
	{
		ScriptableObjectUtility.CreateAsset<WaterConfig> ();
	}
	#endif

	public Material baseMaterial;

	// Basic Settings
	public float size = 32;
	public float meshPointDistance = 2f;

	public Color waterColor;
	public Color waveColor;
	
	public Texture detailTexture;
	public Texture mainTexture;
	
	public Vector2 mainTextureTitle;
	public Vector2 detailTextureTitle;
	
	public Vector2 mainTexturScroll = new Vector2(0.1f,0.2f);
	public Vector2 detailTexturScroll = new Vector2(0.1f,0.2f);
	
	private string mainTextureName = "_MainTex";
	private string detailTextureName = "_DetailTex";
	
	private Vector3[] vertices;
	
	private Vector2 anchorOffset = Vector2.zero;
	private Mesh m;
	private Vector2 uvOffset = Vector2.zero;
	private Vector2 uvOffset2 = Vector2.zero;
	
	public float colorTextureRatio = 0.5f;
	public float heightColoring = 0.15f;
	public float shoreFoamLine = 6f;
	public float shoreFoamCut = 0f;
	public float textureBias = 0.5f;

	public List<UltimateToonWaterC.WaveForm> waveForms = new List<UltimateToonWaterC.WaveForm>();
}

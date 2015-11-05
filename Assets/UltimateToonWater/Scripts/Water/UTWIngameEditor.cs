using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UTWIngameEditor : MonoBehaviour {
	public UltimateToonWater UTW;
	public UltimateToonWaterC UTWC;
	public WaveParticles waveParticles;
	public FloaterParticles floaterParticles;
	public Transform island;


	private Rect windowRect;
	private Rect waveRect;

	private bool advanced = true;
	
	void Awake() {
		//Auto load if not set
		if (UTWC == null) {
			GameObject WaterC = GameObject.Find("WaterC");
			if (WaterC != null) {
				UTWC = WaterC.GetComponent<UltimateToonWaterC>();
			}
		}
		if (UTW == null) {
			GameObject Water = GameObject.Find("Water");
			if (Water != null) {
				UTW = Water.GetComponent<UltimateToonWater>();
			}
		}
	}

	// Use this for initialization
	void Start () {

	}

	void OnGUI() {
		windowRect = new Rect(Screen.width-200,0,200,Screen.height);
		waveRect = new Rect(0,0,200,Screen.height);
		windowRect = GUI.Window(0,windowRect,ShowControls,"Controls");
		waveRect = GUI.Window(1,waveRect,ShowWaves,"Waves");
	}

	void ShowControls(int id) {
		bool needGenerateMesh = false;
		GUILayout.BeginVertical();

		if (GUILayout.Button("Toggle Particles")) {
			if (waveParticles != null) {
				waveParticles.gameObject.SetActive(!waveParticles.gameObject.activeSelf);
			}
			if (floaterParticles != null) {
				floaterParticles.gameObject.SetActive(!floaterParticles.gameObject.activeSelf);
			}
		}

		if (island != null) {
			GUILayout.BeginHorizontal();
			GUILayout.Label("Island Y:",GUILayout.Width(100));
			float islandHeight = GUILayout.HorizontalSlider(island.position.y,-50,-10);
			island.position = new Vector3(island.position.x,islandHeight,island.position.z);
			GUILayout.EndHorizontal();
		}

		GUILayout.BeginHorizontal();
		float startSize = UTW.size;
		GUILayout.Label("Water Size (" + startSize + "):" ,GUILayout.Width(100));
		UTW.size = Mathf.RoundToInt(GUILayout.HorizontalSlider(startSize,100,2000));
		if (UTW.size != startSize) {
				needGenerateMesh = true;
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		float startMPD = UTW.meshPointDistance;
		GUILayout.Label("MeshPoint Distance (" + startMPD + "):" ,GUILayout.Width(100));
		UTW.meshPointDistance = GUILayout.HorizontalSlider(startMPD,1,20);
		if (UTW.meshPointDistance != startMPD) {
			needGenerateMesh = true;
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		Color color = UTW.GetComponent<Renderer>().material.GetColor("_Color");
		string colorString = color.ToString();
		GUILayout.Label("Base Color:" ,GUILayout.Width(100));
		GUILayout.BeginVertical();
		color.r = GUILayout.HorizontalSlider(color.r,0f,1f);
		color.g = GUILayout.HorizontalSlider(color.g,0f,1f);
		color.b = GUILayout.HorizontalSlider(color.b,0f,1f);
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Water Alpha:" ,GUILayout.Width(100));
		color.a = GUILayout.HorizontalSlider(color.a,0f,1f);
		if (colorString != color.ToString()) {
			UTW.GetComponent<Renderer>().material.SetColor("_Color",color);
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		float startTC = UTW.GetComponent<Renderer>().material.GetFloat("_ColorBias");
		float cloneTC = startTC;
		GUILayout.Label("Texture/Color:" ,GUILayout.Width(100));
		startTC = GUILayout.HorizontalSlider(startTC,0f,1f);
		if (cloneTC != startTC) {
			UTW.GetComponent<Renderer>().material.SetFloat("_ColorBias",startTC);
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		float startTT = UTW.GetComponent<Renderer>().material.GetFloat("_TextureBias");
		float cloneTT = startTT;
		GUILayout.Label("Texture/Texture:" ,GUILayout.Width(100));
		startTT = GUILayout.HorizontalSlider(startTT,0f,1f);
		if (cloneTT != startTT) {
			UTW.GetComponent<Renderer>().material.SetFloat("_TextureBias",startTT);
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		float startT1S = UTW.mainTextureTitle.x;
		GUILayout.Label("Texture1 Size:" ,GUILayout.Width(100));
		startT1S = GUILayout.HorizontalSlider(startT1S,1f,1200);
		if (startT1S != UTW.mainTextureTitle.x) {
			UTW.mainTextureTitle = new Vector2(startT1S,startT1S);
			UTW.UpdateTextureSize();
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Texture1 Scroll:" ,GUILayout.Width(100));
		GUILayout.BeginVertical();
		//mainTexturScroll
		Vector2 scroll = UTW.mainTexturScroll;
		scroll.x = GUILayout.HorizontalSlider(scroll.x,-1f,1f);
		scroll.y = GUILayout.HorizontalSlider(scroll.y,-1f,1f);
		UTW.mainTexturScroll = scroll;
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		float startT2S = UTW.detailTextureTitle.x;
		GUILayout.Label("Texture2 Size:" ,GUILayout.Width(100));
		startT2S = GUILayout.HorizontalSlider(startT2S,1f,100);
		if (startT2S != UTW.detailTextureTitle.x) {
			UTW.detailTextureTitle = new Vector2(startT2S,startT2S);
			UTW.UpdateTextureSize();
		}
		GUILayout.EndHorizontal();

		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Texture2 Scroll:" ,GUILayout.Width(100));
		GUILayout.BeginVertical();
		//mainTexturScroll
		Vector2 scroll2 = UTW.detailTexturScroll;
		scroll2.x = GUILayout.HorizontalSlider(scroll2.x,-1f,1f);
		scroll2.y = GUILayout.HorizontalSlider(scroll2.y,-1f,1f);
		UTW.detailTexturScroll = scroll2;
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		float startHF = UTW.GetComponent<Renderer>().material.GetFloat("_HeightFactor");
		float cloneHF = startHF;
		GUILayout.Label("Height Coloring:" ,GUILayout.Width(100));
		startHF = GUILayout.HorizontalSlider(startHF,0f,0.5f);
		if (cloneHF != startHF) {
			UTW.GetComponent<Renderer>().material.SetFloat("_HeightFactor",startHF);
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		float startSD = UTW.GetComponent<Renderer>().material.GetFloat("_HighlightThresholdMax");
		float cloneSD = startSD;
		GUILayout.Label("Shore Foam:" ,GUILayout.Width(100));
		startSD = GUILayout.HorizontalSlider(startSD,0f,10f);
		if (cloneSD != startSD) {
			UTW.GetComponent<Renderer>().material.SetFloat("_HighlightThresholdMax",startSD);
		}
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		Color color2 = UTW.GetComponent<Renderer>().material.GetColor("_Color2");
		string color2String = color2.ToString();
		GUILayout.Label("Shore Foam Color:" ,GUILayout.Width(100));
		GUILayout.BeginVertical();
		color2.r = GUILayout.HorizontalSlider(color2.r,0f,1f);
		color2.g = GUILayout.HorizontalSlider(color2.g,0f,1f);
		color2.b = GUILayout.HorizontalSlider(color2.b,0f,1f);
		if (color2String != color2.ToString()) {
			UTW.GetComponent<Renderer>().material.SetColor("_Color2",color2);
		}
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		float startSDC = UTW.GetComponent<Renderer>().material.GetFloat("_HighlightThresholdRange");
		float cloneSDC = startSDC;
		GUILayout.Label("Shore Foam Cut:" ,GUILayout.Width(100));
		startSDC = GUILayout.HorizontalSlider(startSDC,0f,1f);
		if (cloneSDC != startSDC) {
			UTW.GetComponent<Renderer>().material.SetFloat("_HighlightThresholdRange",startSDC);
		}
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();

		if (needGenerateMesh == true) {

			int numVerts = (int) (UTW.size/UTW.meshPointDistance);
			numVerts *= numVerts;
			if (numVerts > 65000) {
				UTW.meshPointDistance = Mathf.CeilToInt(UTW.size / Mathf.Sqrt(65000f));
			}

			UTW.generateMesh();
		}
	}

	private List<UltimateToonWater.WaveForm> removeWaves = new List<UltimateToonWater.WaveForm>();

	void ShowWaves(int id) {
		GUILayout.BeginVertical();
		if (GUILayout.Button("Add Wave")) {
			UltimateToonWater.WaveForm form = new UltimateToonWater.WaveForm();
			form.speed = Vector3.zero;
			form.waveLength = Vector2.one;
			form.scale = 0f;
			form.offset = Random.insideUnitCircle;
			UTW.waveForms.Add(form);
		}

		//GUILayout.BeginHorizontal();
		//advanced = GUILayout.Toggle(advanced,"Advanced");
		//GUILayout.EndHorizontal();

		removeWaves.Clear();

		foreach(UltimateToonWater.WaveForm wave in UTW.waveForms) {
			if (advanced == true) {
				GUILayout.BeginHorizontal();
				GUILayout.Label("Speed (" + wave.speed.ToString() +"):" ,GUILayout.Width(100));
				GUILayout.BeginVertical();
				//mainTexturScroll
				Vector2 speed = wave.speed;
				speed.x = GUILayout.HorizontalSlider(speed.x,-0.25f,0.25f);
				speed.y = GUILayout.HorizontalSlider(speed.y,-0.25f,0.25f);
				wave.speed = speed;
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Wavelength (" + wave.waveLength.ToString() +"):" ,GUILayout.Width(100));
				GUILayout.BeginVertical();
				//mainTexturScroll
				Vector2 waveLength = wave.waveLength;
				waveLength.x = GUILayout.HorizontalSlider(waveLength.x,0.01f,1000f);
				waveLength.y = GUILayout.HorizontalSlider(waveLength.y,0.01f,1000f);
				wave.waveLength = waveLength;
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Height (" + wave.scale.ToString() +"):" ,GUILayout.Width(100));
				wave.scale = GUILayout.HorizontalSlider(wave.scale,0.01f,10f);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Gain (" + wave.waveGain.ToString() +"):" ,GUILayout.Width(100));
				GUILayout.BeginVertical();
				//mainTexturScroll
				Vector2 waveGain = wave.waveGain;
				waveGain.x = GUILayout.HorizontalSlider(waveGain.x,1f,5f);
				waveGain.y = GUILayout.HorizontalSlider(waveGain.y,1f,5f);
				wave.waveGain = waveGain;
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Offset (" + wave.offset.ToString() +"):" ,GUILayout.Width(100));
				GUILayout.BeginVertical();
				//mainTexturScroll
				Vector2 waveOffset = wave.offset;
				waveOffset.x = GUILayout.HorizontalSlider(waveOffset.x,-Mathf.PI,Mathf.PI);
				waveOffset.y = GUILayout.HorizontalSlider(waveOffset.y,-Mathf.PI,Mathf.PI);
				wave.offset = waveOffset;
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			} else {
				GUILayout.BeginHorizontal();
				GUILayout.Label("Size (Small - Big):" ,GUILayout.Width(100));
				GUILayout.BeginVertical();
				float oldSize = wave.size;
				wave.size = GUILayout.HorizontalSlider(wave.size,UTW.meshPointDistance*3f,200f);
				if (wave.size != oldSize) {
					wave.UpdateSimple();
				}

				GUILayout.EndVertical();
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Rougness (Clam - Rough):" ,GUILayout.Width(100));
				GUILayout.BeginVertical();
				float oldRoughness = wave.roughness;
				wave.roughness = GUILayout.HorizontalSlider(wave.roughness,0.05f,1f);
				if (wave.roughness != oldRoughness) {
					wave.UpdateSimple();
				}
				
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}

			if (GUILayout.Button("Remove Wave")) {
				removeWaves.Add(wave);
			}
			GUILayout.Space(10f);
		}

		foreach(UltimateToonWater.WaveForm wave in removeWaves) {
			if (UTW.waveForms.Contains(wave)) {
				UTW.waveForms.Remove(wave);
			}
		}

		GUILayout.EndVertical();
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class UltimateToonWater : MonoBehaviour {
	
	[System.Serializable]
	public class WaveForm {
		public Vector2 speed = Vector2.one;			// Wave travel speed (Meters/Second)
		public Vector2 waveLength = new Vector2(50,50);		// Wavelength (Peak-to-Peak) (Meters)
		public float scale = 1f;				// Waveheight (Meters)
		public Vector2 offset = Vector2.zero;			// Offset
		public Vector2 waveGain = new Vector2(1,1);		// Wave gain if above 0

		// BELOW Unused, but to be used in the future
		[HideInInspector]
		public float size;
		[HideInInspector]
		public float roughness;

		public void UpdateSimple() {
			waveLength.x = size*Random.Range(0.8f,1.2f);
			waveLength.y = size*Random.Range(0.8f,1.2f);
			if (Random.Range(0,1) == 0) {
				waveLength.x *= -1;
			}
			if (Random.Range(0,1) == 0) {
				waveLength.y *= -1;
			}

			scale = Random.Range(0.8f,1.2f)*roughness*Mathf.Sqrt(Mathf.Sqrt(size*2f));
			 
			speed.x = Random.Range(0.8f,1.2f)*roughness*roughness*roughness*0.5f;
			speed.y = Random.Range(0.8f,1.2f)*roughness*roughness*roughness*0.5f;
			if (Random.Range(0,1) == 0) {
				speed.x *= -1;
			}
			if (Random.Range(0,1) == 0) {
				speed.y *= -1;
			}
		}
	}
	
	public Material baseMaterial;

	public bool useGPU = true;
	public bool stickToTarget = true;
	public Transform target;
	public bool moveTextures = true;
	public bool moveMesh = true;
	public bool previewMode = false;
	public float now;
	
	// Some constant variables
	private const float twopi  = 6.28318530718f; // CPU/GPU identical float
	private const string mainTextureName = "_MainTex";
	private const string detailTextureName = "_DetailTex";

	// Mesh Config
	private Vector3[] vertices;
	private Vector2 anchorOffset = Vector2.zero;
	private Mesh m;
	
	// Texture Offset
	private Vector2 uvOffset = Vector2.zero;
	private Vector2 uvOffset2 = Vector2.zero;

	// Appearance Configuration:
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

	public float colorTextureRatio = 0.5f;
	public float heightColoring = 0.15f;
	public float shoreFoamLine = 6f;
	public float shoreFoamCut = 0f;
	public float textureBias = 0.5f;

	public List<WaveForm> waveForms = new List<WaveForm>();

	private Material usedMaterial;

	// Clone Material to UTW & Generate Mesh
	void Awake() {
		Init();
	}

	void Start() {
		Init();
	}

	public void Init() {
		SetMaterial();
		generateMesh();
		UpdateTextures();
		UpdateColor();
		UpdateTextureSize();
		UpdateTextureProps();
	}
	
	// Update is called once per frame
	public void Update () {
		if (Application.isPlaying == true) {
			now = Time.time;
			HandleWaves();
			HandleTexture();
		}
	}
	
	// Stick to target
	void LateUpdate() {
		if (stickToTarget == true && target != null) {
			Vector3 pos = transform.position;
			pos.x = target.transform.position.x;
			pos.z = target.transform.position.z;
			transform.position = pos;
		}
	}

	public void SetMaterial() {
		usedMaterial = new Material(baseMaterial);
		usedMaterial.name = "UTW: " + baseMaterial.name;
		if (Application.isPlaying) {
			GetComponent<Renderer>().materials[0] = usedMaterial;
		} else {
			GetComponent<Renderer>().sharedMaterials[0] = usedMaterial;
		}
	}

	// Generate the water Mesh
	public void generateMesh() {
		if (size > 0 && meshPointDistance > 0 && size > (meshPointDistance*2)) {
			int numVerticesQuart = Mathf.RoundToInt((size/meshPointDistance)/2f);
			int numVerticesHalf = 2* numVerticesQuart;
			m = new Mesh();
			m.MarkDynamic();
			
			int hCount2 = numVerticesHalf+1;
			int vCount2 = numVerticesHalf+1;
			int numTriangles = numVerticesHalf * numVerticesHalf * 6;
			int numVertices = hCount2 * vCount2;

			vertices = new Vector3[numVertices];
			Vector2[] uvs = new Vector2[numVertices];
			int[] triangles = new int[numTriangles];
			
			int index = 0;
			float uvFactorX = 1.0f/numVerticesHalf;
			float uvFactorY = 1.0f/numVerticesHalf;
			float scaleX = size/numVerticesHalf;
			float scaleY = size/numVerticesHalf;
			for (float y = 0.0f; y < vCount2; y++)
			{
				for (float x = 0.0f; x < hCount2; x++)
				{

					vertices[index] = new Vector3(x*scaleX - size/2f - anchorOffset.x, 0.0f, y*scaleY - size/2f - anchorOffset.y);
					uvs[index++] = new Vector2(x*uvFactorX, y*uvFactorY);
				}
			}
			
			index = 0;
			for (int y = 0; y < numVerticesHalf; y++)
			{
				for (int x = 0; x < numVerticesHalf; x++)
				{
					triangles[index]   = (y     * hCount2) + x;
					triangles[index+1] = ((y+1) * hCount2) + x;
					triangles[index+2] = (y     * hCount2) + x + 1;
					
					triangles[index+3] = ((y+1) * hCount2) + x;
					triangles[index+4] = ((y+1) * hCount2) + x + 1;
					triangles[index+5] = (y     * hCount2) + x + 1;
					index += 6;
				}
			}

			m.vertices = vertices;
			m.uv = uvs;
			m.triangles = triangles;
			m.RecalculateNormals();
			m.RecalculateBounds();

			gameObject.GetComponent<MeshFilter>().mesh = m;
			UpdateTextureSize();
		}
	}

	public void UpdateTextureSize() {
		if (size > 0 && mainTextureTitle.x > 0 && mainTextureTitle.y > 0 && detailTextureTitle.x > 0 && detailTextureTitle.y > 0) {
			if (Application.isPlaying) {
				GetComponent<Renderer>().materials[0].SetTextureScale(mainTextureName,new Vector2(size/mainTextureTitle.x,size/mainTextureTitle.y));
				GetComponent<Renderer>().materials[0].SetTextureScale(detailTextureName,new Vector2(size/detailTextureTitle.x,size/detailTextureTitle.y));
			} else {
				GetComponent<Renderer>().sharedMaterials[0].SetTextureScale(mainTextureName,new Vector2(size/mainTextureTitle.x,size/mainTextureTitle.y));
				GetComponent<Renderer>().sharedMaterials[0].SetTextureScale(detailTextureName,new Vector2(size/detailTextureTitle.x,size/detailTextureTitle.y));
			}
		}
	}

	public void UpdateColor() {
		if (Application.isPlaying) {
			GetComponent<Renderer>().materials[0].SetColor("_Color",waterColor);
			GetComponent<Renderer>().materials[0].SetColor("_Color2",waveColor);
		} else {
			GetComponent<Renderer>().sharedMaterials[0].SetColor("_Color",waterColor);
			GetComponent<Renderer>().sharedMaterials[0].SetColor("_Color2",waveColor);
		}
	}

	public void UpdateTextures() {
		if (Application.isPlaying) {
			GetComponent<Renderer>().materials[0].SetTexture(mainTextureName,mainTexture);
			GetComponent<Renderer>().materials[0].SetTexture(detailTextureName,detailTexture);
		} else {
			GetComponent<Renderer>().sharedMaterials[0].SetTexture(mainTextureName,mainTexture);
			GetComponent<Renderer>().sharedMaterials[0].SetTexture(detailTextureName,detailTexture);
		}
	}

	public void UpdateTextureProps() {
		if (Application.isPlaying) {
			GetComponent<Renderer>().materials[0].SetFloat("_ColorBias",colorTextureRatio);
			GetComponent<Renderer>().materials[0].SetFloat("_HeightFactor",heightColoring);
			GetComponent<Renderer>().materials[0].SetFloat("_HighlightThresholdMax",shoreFoamLine);
			GetComponent<Renderer>().materials[0].SetFloat("_HighlightThresholdRange",shoreFoamCut);
			GetComponent<Renderer>().materials[0].SetFloat("_TextureBias",textureBias);
		} else {
			GetComponent<Renderer>().sharedMaterials[0].SetFloat("_ColorBias",colorTextureRatio);
			GetComponent<Renderer>().sharedMaterials[0].SetFloat("_HeightFactor",heightColoring);
			GetComponent<Renderer>().sharedMaterials[0].SetFloat("_HighlightThresholdMax",shoreFoamLine);
			GetComponent<Renderer>().sharedMaterials[0].SetFloat("_HighlightThresholdRange",shoreFoamCut);
			GetComponent<Renderer>().sharedMaterials[0].SetFloat("_TextureBias",textureBias);
		}
	}

	public float getHeightByPos(Vector2 pos) {

		float offsetY = 0.0f;
		float offsetX = 0.0f;
		float offset = 0.0f;
		if (useGPU == true || moveMesh == true) {
			int i=0;
			foreach(WaveForm waveForm in waveForms) {
				float yComp = Mathf.Cos((now*(waveForm.speed.y)*twopi) + ((pos.y*twopi) /waveForm.waveLength.y) + waveForm.offset.y);
				float xComp = Mathf.Cos((now*(waveForm.speed.x)*twopi) + ((pos.x*twopi) /waveForm.waveLength.x) + waveForm.offset.x);
				float xyComp = (xComp + yComp)/2;
				if (xyComp > 0 && waveForm.waveGain.x > 1) {
					xyComp = Mathf.Pow(xyComp,waveForm.waveGain.x);
				} else if (xyComp < 0 && waveForm.waveGain.y > 1) {
					xyComp = -1*Mathf.Pow(xyComp*-1,waveForm.waveGain.y);
				}
				offset += waveForm.scale * xyComp;
				i++;
				if (i >= 2) {
					break;
				}
			}
		}

		return offset+transform.position.y+1;
	}

	public float getHeightByPos(Vector3 pos) {
		return getHeightByPos(new Vector2(pos.x,pos.z));
	}

	public void HandleWaves() {
		if (moveMesh == true) {
			if (Application.isPlaying) {
				if (useGPU == true) {
					GetComponent<Renderer>().sharedMaterials[0].SetFloat("_GameTime",now);
					if (waveForms.Count > 1) {
						WaveForm wForm1 = waveForms[0];
						WaveForm wForm2 = waveForms[1];
						GetComponent<Renderer>().materials[0].SetInt("_UseGPU",2);
						GetComponent<Renderer>().materials[0].SetVector("_WaveSpeed",new Vector4(wForm1.speed.x,wForm1.speed.y,wForm2.speed.x,wForm2.speed.y));
						GetComponent<Renderer>().materials[0].SetVector("_Wavelength",new Vector4(wForm1.waveLength.x, wForm1.waveLength.y,wForm2.waveLength.x, wForm2.waveLength.y));
						GetComponent<Renderer>().materials[0].SetVector("_WaveOffset",new Vector4(wForm1.offset.x, wForm1.offset.y,wForm2.offset.x, wForm2.offset.y));
						GetComponent<Renderer>().materials[0].SetVector("_WaveGain",new Vector4(wForm1.waveGain.x,wForm1.waveGain.y, wForm2.waveGain.x,wForm2.waveGain.y));
						GetComponent<Renderer>().materials[0].SetFloat("_WaveScale1",wForm1.scale);
						GetComponent<Renderer>().materials[0].SetFloat("_WaveScale2",wForm2.scale);

					} else if (waveForms.Count == 1) {
						WaveForm wForm = waveForms[0];
						GetComponent<Renderer>().materials[0].SetInt("_UseGPU",1);
						GetComponent<Renderer>().materials[0].SetVector("_WaveSpeed",new Vector4(wForm.speed.x,wForm.speed.y,0,0));
						GetComponent<Renderer>().materials[0].SetVector("_Wavelength",new Vector4(wForm.waveLength.x, wForm.waveLength.y,0,0));
						GetComponent<Renderer>().materials[0].SetVector("_WaveOffset",new Vector4(wForm.offset.x, wForm.offset.y,0,0));
						GetComponent<Renderer>().materials[0].SetVector("_WaveGain",new Vector4(wForm.waveGain.x,wForm.waveGain.y,0,0));
						GetComponent<Renderer>().materials[0].SetFloat("_WaveScale1",wForm.scale);
					}
				} else {
					GetComponent<Renderer>().materials[0].SetInt("_UseGPU",0);
				}
			} else {
				if (useGPU == true) {
					GetComponent<Renderer>().sharedMaterials[0].SetFloat("_GameTime",now);
					if (waveForms.Count > 1) {
						WaveForm wForm1 = waveForms[0];
						WaveForm wForm2 = waveForms[1];
						GetComponent<Renderer>().sharedMaterials[0].SetInt("_UseGPU",2);
						GetComponent<Renderer>().sharedMaterials[0].SetVector("_WaveSpeed",new Vector4(wForm1.speed.x,wForm1.speed.y,wForm2.speed.x,wForm2.speed.y));
						GetComponent<Renderer>().sharedMaterials[0].SetVector("_Wavelength",new Vector4(wForm1.waveLength.x, wForm1.waveLength.y,wForm2.waveLength.x, wForm2.waveLength.y));
						GetComponent<Renderer>().sharedMaterials[0].SetVector("_WaveOffset",new Vector4(wForm1.offset.x, wForm1.offset.y,wForm2.offset.x, wForm2.offset.y));
						GetComponent<Renderer>().sharedMaterials[0].SetVector("_WaveGain",new Vector4(wForm1.waveGain.x,wForm1.waveGain.y, wForm2.waveGain.x,wForm2.waveGain.y));
						GetComponent<Renderer>().sharedMaterials[0].SetFloat("_WaveScale1",wForm1.scale);
						GetComponent<Renderer>().sharedMaterials[0].SetFloat("_WaveScale2",wForm2.scale);
						
					} else if (waveForms.Count == 1) {
						WaveForm wForm = waveForms[0];
						GetComponent<Renderer>().sharedMaterials[0].SetInt("_UseGPU",1);
						GetComponent<Renderer>().sharedMaterials[0].SetVector("_WaveSpeed",new Vector4(wForm.speed.x,wForm.speed.y,0,0));
						GetComponent<Renderer>().sharedMaterials[0].SetVector("_Wavelength",new Vector4(wForm.waveLength.x, wForm.waveLength.y,0,0));
						GetComponent<Renderer>().sharedMaterials[0].SetVector("_WaveOffset",new Vector4(wForm.offset.x, wForm.offset.y,0,0));
						GetComponent<Renderer>().sharedMaterials[0].SetVector("_WaveGain",new Vector4(wForm.waveGain.x,wForm.waveGain.y,0,0));
						GetComponent<Renderer>().sharedMaterials[0].SetFloat("_WaveScale1",wForm.scale);
					}
				} else {
					GetComponent<Renderer>().sharedMaterials[0].SetInt("_UseGPU",0);
				}
			}

			if (useGPU == false || (waveForms.Count > 2)) {
				float offsetY = 0.0f;
				float offsetX = 0.0f;
				for(int j=0;j<vertices.Length;j++) {
					offsetX = 0f;
					offsetY = 0f;
					Vector3 vertex = vertices[j];
					Vector3 v = transform.TransformPoint(vertex);
					int run = 1;
					foreach(WaveForm waveForm in waveForms) {
						if (useGPU == true && run < 2) {
							run++;
							continue;
						}
						offsetY += waveForm.scale * Mathf.Cos((now*(waveForm.speed.y)*twopi) + ((v.z*twopi) /waveForm.waveLength.y) + waveForm.offset.y);
						offsetX += waveForm.scale * Mathf.Cos((now*(waveForm.speed.x)*twopi) + ((v.x*twopi) /waveForm.waveLength.x) + waveForm.offset.x);
					}
					vertices[j].y = offsetX + offsetY;
				}
			}

			if ((useGPU == true && waveForms.Count > 2) || useGPU == false) {
				m.vertices = vertices;
				m.RecalculateNormals();
			}
		}
	}

	public void HandleTexture() {
		if (stickToTarget == true && target != null) {
			uvOffset.x = target.position.x /(mainTextureTitle.x) ;
			uvOffset.y = target.position.z /(mainTextureTitle.y) ;
			uvOffset2.x = target.position.x /(detailTextureTitle.x) ;
			uvOffset2.y = target.position.z /(detailTextureTitle.y) ;
		} else {
			uvOffset = Vector2.zero;
			uvOffset2 = Vector2.zero;
		}
			
		if (moveTextures == true) {
			uvOffset.x += ( mainTexturScroll.x * now * 10f/mainTextureTitle.x);
			uvOffset.y += ( mainTexturScroll.y * now * 10f/mainTextureTitle.y);
			uvOffset2.x += ( detailTexturScroll.x * now * 10f/detailTextureTitle.x);
			uvOffset2.y += ( detailTexturScroll.y * now * 10f/detailTextureTitle.y);
		}
		
		uvOffset.x = ClampRepeat(uvOffset.x);
		uvOffset.y = ClampRepeat(uvOffset.y);
		uvOffset2.x = ClampRepeat(uvOffset2.x);
		uvOffset2.y = ClampRepeat(uvOffset2.y);

		if (Application.isPlaying) {
			GetComponent<Renderer>().materials[0].SetTextureOffset( mainTextureName, uvOffset );
			GetComponent<Renderer>().materials[0].SetTextureOffset( detailTextureName, uvOffset2 );
		} else {
			GetComponent<Renderer>().sharedMaterials[0].SetTextureOffset( mainTextureName, uvOffset );
			GetComponent<Renderer>().sharedMaterials[0].SetTextureOffset( detailTextureName, uvOffset2 );
		}
		
	}

	float ClampRepeat(float value) {
		value = value % 1f;

		while (value > 1) {
			value -= 1;
		}
		while (value < 0) {
			value += 1;
		}

		return value;
	}


}

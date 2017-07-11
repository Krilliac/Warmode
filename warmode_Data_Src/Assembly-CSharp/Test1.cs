using System;
using UnityEngine;

public class Test1 : MonoBehaviour
{
	public Vector2 uv = new Vector2(0f, 0f);

	private Vector2 uv2 = new Vector2(0f, 0f);

	private float mipCount = 5f;

	private float mipLevel;

	private float tileCountX = 4f;

	private float atlasSize = 64f;

	private float atlasSizeFull = 128f;

	private float[] mipOffset = new float[]
	{
		0f,
		0.5f,
		0.75f,
		0.875f,
		0.9375f
	};

	private float[] texSize = new float[]
	{
		16f,
		8f,
		4f,
		2f,
		1f
	};

	private float texId = 5f;

	private void Start()
	{
		this.uv2.x = this.uv.x - (float)((int)(this.uv.x / this.tileCountX)) * (1f / this.tileCountX) + (this.uv.x - (float)((int)(this.uv.x / this.tileCountX)));
		float num = 1f / this.atlasSizeFull;
		this.uv2.y = (float)((int)(this.uv.x / this.tileCountX)) * (1f / this.tileCountX) + (this.uv.x - (float)((int)(this.uv.x / this.tileCountX)));
	}
}

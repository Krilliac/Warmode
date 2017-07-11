using System;
using UnityEngine;

[Serializable]
public class FilmicCurve
{
	[SerializeField]
	public float m_BlackPoint;

	[SerializeField]
	public float m_WhitePoint = 1f;

	[SerializeField]
	public float m_CrossOverPoint = 0.5f;

	[SerializeField]
	public float m_ToeStrength;

	[SerializeField]
	public float m_ShoulderStrength;

	[SerializeField]
	public float m_LuminositySaturationPoint = 0.95f;

	public float m_k;

	public Vector4 m_ToeCoef;

	public Vector4 m_ShoulderCoef;

	public float ComputeK(float t, float c, float b, float s, float w)
	{
		float num = (1f - t) * (c - b);
		float num2 = (1f - s) * (w - c) + (1f - t) * (c - b);
		return num / num2;
	}

	public float Toe(float x, float t, float c, float b, float s, float w, float k)
	{
		float num = this.m_ToeCoef.x * x;
		float num2 = this.m_ToeCoef.y * x;
		return (num + this.m_ToeCoef.z) / (num2 + this.m_ToeCoef.w);
	}

	public float Shoulder(float x, float t, float c, float b, float s, float w, float k)
	{
		float num = this.m_ShoulderCoef.x * x;
		float num2 = this.m_ShoulderCoef.y * x;
		return (num + this.m_ShoulderCoef.z) / (num2 + this.m_ShoulderCoef.w) + k;
	}

	public float Graph(float x, float t, float c, float b, float s, float w, float k)
	{
		if (x <= this.m_CrossOverPoint)
		{
			return this.Toe(x, t, c, b, s, w, k);
		}
		return this.Shoulder(x, t, c, b, s, w, k);
	}

	public void StoreK()
	{
		this.m_k = this.ComputeK(this.m_ToeStrength, this.m_CrossOverPoint, this.m_BlackPoint, this.m_ShoulderStrength, this.m_WhitePoint);
	}

	public void ComputeShaderCoefficients(float t, float c, float b, float s, float w, float k)
	{
		float x = k * (1f - t);
		float z = k * (1f - t) * -b;
		float y = -t;
		float w2 = c - (1f - t) * b;
		this.m_ToeCoef = new Vector4(x, y, z, w2);
		float x2 = 1f - k;
		float z2 = (1f - k) * -c;
		float w3 = (1f - s) * w - c;
		this.m_ShoulderCoef = new Vector4(x2, s, z2, w3);
	}

	public void UpdateCoefficients()
	{
		this.StoreK();
		this.ComputeShaderCoefficients(this.m_ToeStrength, this.m_CrossOverPoint, this.m_BlackPoint, this.m_ShoulderStrength, this.m_WhitePoint, this.m_k);
	}
}

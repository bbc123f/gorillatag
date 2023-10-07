using System;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000115 RID: 277
[Serializable]
public class AdvancedItemState
{
	// Token: 0x06000715 RID: 1813 RVA: 0x0002CFAE File Offset: 0x0002B1AE
	public void Encode()
	{
		this._encodedValue = this.EncodeData();
	}

	// Token: 0x06000716 RID: 1814 RVA: 0x0002CFBC File Offset: 0x0002B1BC
	public void Decode()
	{
		AdvancedItemState advancedItemState = this.DecodeData(this._encodedValue);
		this.index = advancedItemState.index;
		this.preData = advancedItemState.preData;
		this.limitAxis = advancedItemState.limitAxis;
		this.reverseGrip = advancedItemState.reverseGrip;
		this.angle = advancedItemState.angle;
	}

	// Token: 0x06000717 RID: 1815 RVA: 0x0002D014 File Offset: 0x0002B214
	public Quaternion GetQuaternion()
	{
		Vector3 one = Vector3.one;
		if (this.reverseGrip)
		{
			switch (this.limitAxis)
			{
			case LimitAxis.NoMovement:
				return Quaternion.identity;
			case LimitAxis.YAxis:
				return Quaternion.identity;
			case LimitAxis.XAxis:
			case LimitAxis.ZAxis:
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		return Quaternion.identity;
	}

	// Token: 0x06000718 RID: 1816 RVA: 0x0002D068 File Offset: 0x0002B268
	[return: TupleElementNames(new string[]
	{
		"grabPointIndex",
		"YRotation",
		"XRotation",
		"ZRotation"
	})]
	public ValueTuple<int, float, float, float> DecodeAdvancedItemState(int encodedValue)
	{
		int item = encodedValue >> 21 & 255;
		float item2 = (float)(encodedValue >> 14 & 127) / 128f * 360f;
		float item3 = (float)(encodedValue >> 7 & 127) / 128f * 360f;
		float item4 = (float)(encodedValue & 127) / 128f * 360f;
		return new ValueTuple<int, float, float, float>(item, item2, item3, item4);
	}

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x06000719 RID: 1817 RVA: 0x0002D0C2 File Offset: 0x0002B2C2
	private float EncodedDeltaRotation
	{
		get
		{
			return this.GetEncodedDeltaRotation();
		}
	}

	// Token: 0x0600071A RID: 1818 RVA: 0x0002D0CA File Offset: 0x0002B2CA
	public float GetEncodedDeltaRotation()
	{
		return Mathf.Abs(Mathf.Atan2(this.angleVectorWhereUpIsStandard.x, this.angleVectorWhereUpIsStandard.y)) / 3.1415927f;
	}

	// Token: 0x0600071B RID: 1819 RVA: 0x0002D0F4 File Offset: 0x0002B2F4
	public void DecodeDeltaRotation(float encodedDelta, bool isFlipped)
	{
		float f = encodedDelta * 3.1415927f;
		if (isFlipped)
		{
			this.angleVectorWhereUpIsStandard = new Vector2(-Mathf.Sin(f), Mathf.Cos(f));
		}
		else
		{
			this.angleVectorWhereUpIsStandard = new Vector2(Mathf.Sin(f), Mathf.Cos(f));
		}
		switch (this.limitAxis)
		{
		case LimitAxis.NoMovement:
		case LimitAxis.XAxis:
		case LimitAxis.ZAxis:
			return;
		case LimitAxis.YAxis:
		{
			Vector3 forward = new Vector3(this.angleVectorWhereUpIsStandard.x, 0f, this.angleVectorWhereUpIsStandard.y);
			Vector3 upwards = this.reverseGrip ? Vector3.down : Vector3.up;
			this.deltaRotation = Quaternion.LookRotation(forward, upwards);
			return;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0600071C RID: 1820 RVA: 0x0002D1A8 File Offset: 0x0002B3A8
	public int EncodeData()
	{
		int num = 0;
		if (this.index >= 32 | this.index < 0)
		{
			throw new ArgumentOutOfRangeException(string.Format("Index is invalid {0}", this.index));
		}
		num |= this.index << 25;
		AdvancedItemState.PointType pointType = this.preData.pointType;
		num |= (int)((int)(pointType & (AdvancedItemState.PointType)7) << 22);
		num |= (int)((int)this.limitAxis << 19);
		num |= (this.reverseGrip ? 1 : 0) << 18;
		bool flag = this.angleVectorWhereUpIsStandard.x < 0f;
		if (pointType != AdvancedItemState.PointType.Standard)
		{
			if (pointType != AdvancedItemState.PointType.DistanceBased)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num2 = (int)(this.GetEncodedDeltaRotation() * 512f) & 511;
			num |= (flag ? 1 : 0) << 17;
			num |= num2 << 9;
			int num3 = (int)(this.preData.distAlongLine * 256f) & 255;
			num |= num3;
		}
		else
		{
			int num4 = (int)(this.GetEncodedDeltaRotation() * 65536f) & 65535;
			num |= (flag ? 1 : 0) << 17;
			num |= num4 << 1;
		}
		return num;
	}

	// Token: 0x0600071D RID: 1821 RVA: 0x0002D2C4 File Offset: 0x0002B4C4
	public AdvancedItemState DecodeData(int encoded)
	{
		AdvancedItemState advancedItemState = new AdvancedItemState();
		advancedItemState.index = (encoded >> 25 & 31);
		advancedItemState.limitAxis = (LimitAxis)(encoded >> 19 & 7);
		advancedItemState.reverseGrip = ((encoded >> 18 & 1) == 1);
		AdvancedItemState.PointType pointType = (AdvancedItemState.PointType)(encoded >> 22 & 7);
		if (pointType != AdvancedItemState.PointType.Standard)
		{
			if (pointType != AdvancedItemState.PointType.DistanceBased)
			{
				throw new ArgumentOutOfRangeException();
			}
			advancedItemState.preData = new AdvancedItemState.PreData
			{
				pointType = pointType,
				distAlongLine = (float)(encoded & 255) / 256f
			};
			this.DecodeDeltaRotation((float)(encoded >> 9 & 511) / 512f, (encoded >> 17 & 1) > 0);
		}
		else
		{
			advancedItemState.preData = new AdvancedItemState.PreData
			{
				pointType = pointType
			};
			this.DecodeDeltaRotation((float)(encoded >> 1 & 65535) / 65536f, (encoded >> 17 & 1) > 0);
		}
		return advancedItemState;
	}

	// Token: 0x040008AE RID: 2222
	private int _encodedValue;

	// Token: 0x040008AF RID: 2223
	public Vector2 angleVectorWhereUpIsStandard;

	// Token: 0x040008B0 RID: 2224
	public Quaternion deltaRotation;

	// Token: 0x040008B1 RID: 2225
	public int index;

	// Token: 0x040008B2 RID: 2226
	public AdvancedItemState.PreData preData;

	// Token: 0x040008B3 RID: 2227
	public LimitAxis limitAxis;

	// Token: 0x040008B4 RID: 2228
	public bool reverseGrip;

	// Token: 0x040008B5 RID: 2229
	public float angle;

	// Token: 0x02000402 RID: 1026
	[Serializable]
	public class PreData
	{
		// Token: 0x04001CAA RID: 7338
		public float distAlongLine;

		// Token: 0x04001CAB RID: 7339
		public AdvancedItemState.PointType pointType;
	}

	// Token: 0x02000403 RID: 1027
	public enum PointType
	{
		// Token: 0x04001CAD RID: 7341
		Standard,
		// Token: 0x04001CAE RID: 7342
		DistanceBased
	}
}

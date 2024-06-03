using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class UberShaderProperty
{
	public string name
	{
		get
		{
			return this._name;
		}
	}

	public int nameID
	{
		get
		{
			return this._nameID;
		}
	}

	public int index
	{
		get
		{
			return this._index;
		}
	}

	public ShaderPropertyType type
	{
		get
		{
			return this._type;
		}
	}

	public Vector2 rangeLimits
	{
		get
		{
			return this._rangeLimits;
		}
	}

	public ShaderPropertyFlags flags
	{
		get
		{
			return this._flags;
		}
	}

	public T GetValue<T>(Material target)
	{
		switch (this._type)
		{
		case ShaderPropertyType.Color:
			return UberShaderProperty.ValueAs<Color, T>(target.GetColor(this._nameID));
		case ShaderPropertyType.Vector:
			return UberShaderProperty.ValueAs<Vector4, T>(target.GetVector(this._nameID));
		case ShaderPropertyType.Float:
		case ShaderPropertyType.Range:
			return UberShaderProperty.ValueAs<float, T>(target.GetFloat(this._nameID));
		case ShaderPropertyType.Texture:
			return UberShaderProperty.ValueAs<Texture, T>(target.GetTexture(this._nameID));
		case ShaderPropertyType.Int:
			return UberShaderProperty.ValueAs<int, T>(target.GetInt(this._nameID));
		default:
			return default(T);
		}
	}

	public void SetValue<T>(Material target, T value)
	{
		switch (this._type)
		{
		case ShaderPropertyType.Color:
			target.SetColor(this._nameID, UberShaderProperty.ValueAs<T, Color>(value));
			return;
		case ShaderPropertyType.Vector:
			target.SetVector(this._nameID, UberShaderProperty.ValueAs<T, Vector4>(value));
			return;
		case ShaderPropertyType.Float:
		case ShaderPropertyType.Range:
			target.SetFloat(this._nameID, UberShaderProperty.ValueAs<T, float>(value));
			return;
		case ShaderPropertyType.Texture:
			target.SetTexture(this._nameID, UberShaderProperty.ValueAs<T, Texture>(value));
			return;
		case ShaderPropertyType.Int:
			target.SetInt(this._nameID, UberShaderProperty.ValueAs<T, int>(value));
			return;
		default:
			return;
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static TOut ValueAs<TIn, TOut>(TIn value)
	{
		return *Unsafe.As<TIn, TOut>(ref value);
	}

	public static UberShaderProperty FromName(string name)
	{
		return UberShaderProperty.FromIndex(UberShader.kReferenceShader.FindPropertyIndex(name));
	}

	public static UberShaderProperty FromIndex(int i)
	{
		Shader kReferenceShader = UberShader.kReferenceShader;
		UberShaderProperty uberShaderProperty = new UberShaderProperty
		{
			_index = i,
			_flags = kReferenceShader.GetPropertyFlags(i),
			_type = kReferenceShader.GetPropertyType(i),
			_nameID = kReferenceShader.GetPropertyNameId(i),
			_name = kReferenceShader.GetPropertyName(i)
		};
		if (uberShaderProperty._type == ShaderPropertyType.Range)
		{
			uberShaderProperty._rangeLimits = kReferenceShader.GetPropertyRangeLimits(uberShaderProperty._index);
		}
		return uberShaderProperty;
	}

	public static UberShaderProperty[] GetAllProperties()
	{
		int propertyCount = UberShader.kReferenceShader.GetPropertyCount();
		UberShaderProperty[] array = new UberShaderProperty[propertyCount];
		for (int i = 0; i < propertyCount; i++)
		{
			array[i] = UberShaderProperty.FromIndex(i);
		}
		return array;
	}

	public UberShaderProperty()
	{
	}

	private int _index;

	private int _nameID;

	private string _name;

	private ShaderPropertyType _type;

	private ShaderPropertyFlags _flags;

	private Vector2 _rangeLimits;
}

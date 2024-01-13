using System;
using System.Collections.Generic;
using System.Linq;
using Oculus.Spatializer.Propagation;
using UnityEngine;

public sealed class ONSPPropagationMaterial : MonoBehaviour
{
	public enum Preset
	{
		Custom,
		AcousticTile,
		Brick,
		BrickPainted,
		Carpet,
		CarpetHeavy,
		CarpetHeavyPadded,
		CeramicTile,
		Concrete,
		ConcreteRough,
		ConcreteBlock,
		ConcreteBlockPainted,
		Curtain,
		Foliage,
		Glass,
		GlassHeavy,
		Grass,
		Gravel,
		GypsumBoard,
		PlasterOnBrick,
		PlasterOnConcreteBlock,
		Soil,
		SoundProof,
		Snow,
		Steel,
		Water,
		WoodThin,
		WoodThick,
		WoodFloor,
		WoodOnConcrete
	}

	[Serializable]
	public sealed class Point
	{
		public float frequency;

		public float data;

		public Point(float frequency = 0f, float data = 0f)
		{
			this.frequency = frequency;
			this.data = data;
		}

		public static implicit operator Point(Vector2 v)
		{
			return new Point(v.x, v.y);
		}

		public static implicit operator Vector2(Point point)
		{
			return new Vector2(point.frequency, point.data);
		}
	}

	[Serializable]
	public sealed class Spectrum
	{
		public int selection = int.MaxValue;

		public List<Point> points = new List<Point>();

		public float this[float f]
		{
			get
			{
				if (points.Count > 0)
				{
					Point point = new Point(float.MinValue);
					Point point2 = new Point(float.MaxValue);
					foreach (Point point3 in points)
					{
						if (point3.frequency < f)
						{
							if (point3.frequency > point.frequency)
							{
								point = point3;
							}
						}
						else if (point3.frequency < point2.frequency)
						{
							point2 = point3;
						}
					}
					if (point.frequency == float.MinValue)
					{
						point.data = points.OrderBy((Point p) => p.frequency).First().data;
					}
					if (point2.frequency == float.MaxValue)
					{
						point2.data = points.OrderBy((Point p) => p.frequency).Last().data;
					}
					return point.data + (f - point.frequency) * (point2.data - point.data) / (point2.frequency - point.frequency);
				}
				return 0f;
			}
		}
	}

	public IntPtr materialHandle = IntPtr.Zero;

	[Tooltip("Absorption")]
	public Spectrum absorption = new Spectrum();

	[Tooltip("Transmission")]
	public Spectrum transmission = new Spectrum();

	[Tooltip("Scattering")]
	public Spectrum scattering = new Spectrum();

	[SerializeField]
	private Preset preset_;

	public Preset preset
	{
		get
		{
			return preset_;
		}
		set
		{
			SetPreset(value);
			preset_ = value;
		}
	}

	private void Start()
	{
		StartInternal();
	}

	public void StartInternal()
	{
		if (!(materialHandle != IntPtr.Zero))
		{
			if (ONSPPropagation.Interface.CreateAudioMaterial(out materialHandle) != ONSPPropagationGeometry.OSPSuccess)
			{
				throw new Exception("Unable to create internal audio material");
			}
			UploadMaterial();
		}
	}

	private void OnDestroy()
	{
		DestroyInternal();
	}

	public void DestroyInternal()
	{
		if (materialHandle != IntPtr.Zero)
		{
			ONSPPropagation.Interface.DestroyAudioMaterial(materialHandle);
			materialHandle = IntPtr.Zero;
		}
	}

	public void UploadMaterial()
	{
		if (materialHandle == IntPtr.Zero)
		{
			return;
		}
		ONSPPropagation.Interface.AudioMaterialReset(materialHandle, MaterialProperty.ABSORPTION);
		foreach (Point point in absorption.points)
		{
			ONSPPropagation.Interface.AudioMaterialSetFrequency(materialHandle, MaterialProperty.ABSORPTION, point.frequency, point.data);
		}
		ONSPPropagation.Interface.AudioMaterialReset(materialHandle, MaterialProperty.TRANSMISSION);
		foreach (Point point2 in transmission.points)
		{
			ONSPPropagation.Interface.AudioMaterialSetFrequency(materialHandle, MaterialProperty.TRANSMISSION, point2.frequency, point2.data);
		}
		ONSPPropagation.Interface.AudioMaterialReset(materialHandle, MaterialProperty.SCATTERING);
		foreach (Point point3 in scattering.points)
		{
			ONSPPropagation.Interface.AudioMaterialSetFrequency(materialHandle, MaterialProperty.SCATTERING, point3.frequency, point3.data);
		}
	}

	public void SetPreset(Preset preset)
	{
		ONSPPropagationMaterial material = this;
		switch (preset)
		{
		case Preset.AcousticTile:
			AcousticTile(ref material);
			break;
		case Preset.Brick:
			Brick(ref material);
			break;
		case Preset.BrickPainted:
			BrickPainted(ref material);
			break;
		case Preset.Carpet:
			Carpet(ref material);
			break;
		case Preset.CarpetHeavy:
			CarpetHeavy(ref material);
			break;
		case Preset.CarpetHeavyPadded:
			CarpetHeavyPadded(ref material);
			break;
		case Preset.CeramicTile:
			CeramicTile(ref material);
			break;
		case Preset.Concrete:
			Concrete(ref material);
			break;
		case Preset.ConcreteRough:
			ConcreteRough(ref material);
			break;
		case Preset.ConcreteBlock:
			ConcreteBlock(ref material);
			break;
		case Preset.ConcreteBlockPainted:
			ConcreteBlockPainted(ref material);
			break;
		case Preset.Curtain:
			Curtain(ref material);
			break;
		case Preset.Foliage:
			Foliage(ref material);
			break;
		case Preset.Glass:
			Glass(ref material);
			break;
		case Preset.GlassHeavy:
			GlassHeavy(ref material);
			break;
		case Preset.Grass:
			Grass(ref material);
			break;
		case Preset.Gravel:
			Gravel(ref material);
			break;
		case Preset.GypsumBoard:
			GypsumBoard(ref material);
			break;
		case Preset.PlasterOnBrick:
			PlasterOnBrick(ref material);
			break;
		case Preset.PlasterOnConcreteBlock:
			PlasterOnConcreteBlock(ref material);
			break;
		case Preset.Soil:
			Soil(ref material);
			break;
		case Preset.SoundProof:
			SoundProof(ref material);
			break;
		case Preset.Snow:
			Snow(ref material);
			break;
		case Preset.Steel:
			Steel(ref material);
			break;
		case Preset.Water:
			Water(ref material);
			break;
		case Preset.WoodThin:
			WoodThin(ref material);
			break;
		case Preset.WoodThick:
			WoodThick(ref material);
			break;
		case Preset.WoodFloor:
			WoodFloor(ref material);
			break;
		case Preset.WoodOnConcrete:
			WoodOnConcrete(ref material);
			break;
		case Preset.Custom:
			break;
		}
	}

	private static void AcousticTile(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.5f),
			new Point(250f, 0.7f),
			new Point(500f, 0.6f),
			new Point(1000f, 0.7f),
			new Point(2000f, 0.7f),
			new Point(4000f, 0.5f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.15f),
			new Point(500f, 0.2f),
			new Point(1000f, 0.2f),
			new Point(2000f, 0.25f),
			new Point(4000f, 0.3f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.05f),
			new Point(250f, 0.04f),
			new Point(500f, 0.03f),
			new Point(1000f, 0.02f),
			new Point(2000f, 0.005f),
			new Point(4000f, 0.002f)
		};
	}

	private static void Brick(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.02f),
			new Point(250f, 0.02f),
			new Point(500f, 0.03f),
			new Point(1000f, 0.04f),
			new Point(2000f, 0.05f),
			new Point(4000f, 0.07f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.2f),
			new Point(250f, 0.25f),
			new Point(500f, 0.3f),
			new Point(1000f, 0.35f),
			new Point(2000f, 0.4f),
			new Point(4000f, 0.45f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.025f),
			new Point(250f, 0.019f),
			new Point(500f, 0.01f),
			new Point(1000f, 0.0045f),
			new Point(2000f, 0.0018f),
			new Point(4000f, 0.00089f)
		};
	}

	private static void BrickPainted(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.01f),
			new Point(250f, 0.01f),
			new Point(500f, 0.02f),
			new Point(1000f, 0.02f),
			new Point(2000f, 0.02f),
			new Point(4000f, 0.03f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.15f),
			new Point(250f, 0.15f),
			new Point(500f, 0.2f),
			new Point(1000f, 0.2f),
			new Point(2000f, 0.2f),
			new Point(4000f, 0.25f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.025f),
			new Point(250f, 0.019f),
			new Point(500f, 0.01f),
			new Point(1000f, 0.0045f),
			new Point(2000f, 0.0018f),
			new Point(4000f, 0.00089f)
		};
	}

	private static void Carpet(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.01f),
			new Point(250f, 0.05f),
			new Point(500f, 0.1f),
			new Point(1000f, 0.2f),
			new Point(2000f, 0.45f),
			new Point(4000f, 0.65f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.1f),
			new Point(500f, 0.15f),
			new Point(1000f, 0.2f),
			new Point(2000f, 0.3f),
			new Point(4000f, 0.45f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.004f),
			new Point(250f, 0.0079f),
			new Point(500f, 0.0056f),
			new Point(1000f, 0.0016f),
			new Point(2000f, 0.0014f),
			new Point(4000f, 0.0005f)
		};
	}

	private static void CarpetHeavy(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.02f),
			new Point(250f, 0.06f),
			new Point(500f, 0.14f),
			new Point(1000f, 0.37f),
			new Point(2000f, 0.48f),
			new Point(4000f, 0.63f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.15f),
			new Point(500f, 0.2f),
			new Point(1000f, 0.25f),
			new Point(2000f, 0.35f),
			new Point(4000f, 0.5f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.004f),
			new Point(250f, 0.0079f),
			new Point(500f, 0.0056f),
			new Point(1000f, 0.0016f),
			new Point(2000f, 0.0014f),
			new Point(4000f, 0.0005f)
		};
	}

	private static void CarpetHeavyPadded(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.08f),
			new Point(250f, 0.24f),
			new Point(500f, 0.57f),
			new Point(1000f, 0.69f),
			new Point(2000f, 0.71f),
			new Point(4000f, 0.73f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.15f),
			new Point(500f, 0.2f),
			new Point(1000f, 0.25f),
			new Point(2000f, 0.35f),
			new Point(4000f, 0.5f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.004f),
			new Point(250f, 0.0079f),
			new Point(500f, 0.0056f),
			new Point(1000f, 0.0016f),
			new Point(2000f, 0.0014f),
			new Point(4000f, 0.0005f)
		};
	}

	private static void CeramicTile(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.01f),
			new Point(250f, 0.01f),
			new Point(500f, 0.01f),
			new Point(1000f, 0.01f),
			new Point(2000f, 0.02f),
			new Point(4000f, 0.02f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.12f),
			new Point(500f, 0.14f),
			new Point(1000f, 0.16f),
			new Point(2000f, 0.18f),
			new Point(4000f, 0.2f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.004f),
			new Point(250f, 0.0079f),
			new Point(500f, 0.0056f),
			new Point(1000f, 0.0016f),
			new Point(2000f, 0.0014f),
			new Point(4000f, 0.0005f)
		};
	}

	private static void Concrete(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.01f),
			new Point(250f, 0.01f),
			new Point(500f, 0.02f),
			new Point(1000f, 0.02f),
			new Point(2000f, 0.02f),
			new Point(4000f, 0.02f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.11f),
			new Point(500f, 0.12f),
			new Point(1000f, 0.13f),
			new Point(2000f, 0.14f),
			new Point(4000f, 0.15f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.004f),
			new Point(250f, 0.0079f),
			new Point(500f, 0.0056f),
			new Point(1000f, 0.0016f),
			new Point(2000f, 0.0014f),
			new Point(4000f, 0.0005f)
		};
	}

	private static void ConcreteRough(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.01f),
			new Point(250f, 0.02f),
			new Point(500f, 0.04f),
			new Point(1000f, 0.06f),
			new Point(2000f, 0.08f),
			new Point(4000f, 0.1f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.12f),
			new Point(500f, 0.15f),
			new Point(1000f, 0.2f),
			new Point(2000f, 0.25f),
			new Point(4000f, 0.3f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.004f),
			new Point(250f, 0.0079f),
			new Point(500f, 0.0056f),
			new Point(1000f, 0.0016f),
			new Point(2000f, 0.0014f),
			new Point(4000f, 0.0005f)
		};
	}

	private static void ConcreteBlock(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.36f),
			new Point(250f, 0.44f),
			new Point(500f, 0.31f),
			new Point(1000f, 0.29f),
			new Point(2000f, 0.39f),
			new Point(4000f, 0.21f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.12f),
			new Point(500f, 0.15f),
			new Point(1000f, 0.2f),
			new Point(2000f, 0.3f),
			new Point(4000f, 0.4f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.02f),
			new Point(250f, 0.01f),
			new Point(500f, 0.0063f),
			new Point(1000f, 0.0035f),
			new Point(2000f, 0.00011f),
			new Point(4000f, 0.00063f)
		};
	}

	private static void ConcreteBlockPainted(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.05f),
			new Point(500f, 0.06f),
			new Point(1000f, 0.07f),
			new Point(2000f, 0.09f),
			new Point(4000f, 0.08f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.11f),
			new Point(500f, 0.13f),
			new Point(1000f, 0.15f),
			new Point(2000f, 0.16f),
			new Point(4000f, 0.2f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.02f),
			new Point(250f, 0.01f),
			new Point(500f, 0.0063f),
			new Point(1000f, 0.0035f),
			new Point(2000f, 0.00011f),
			new Point(4000f, 0.00063f)
		};
	}

	private static void Curtain(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.07f),
			new Point(250f, 0.31f),
			new Point(500f, 0.49f),
			new Point(1000f, 0.75f),
			new Point(2000f, 0.7f),
			new Point(4000f, 0.6f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.15f),
			new Point(500f, 0.2f),
			new Point(1000f, 0.3f),
			new Point(2000f, 0.4f),
			new Point(4000f, 0.5f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.42f),
			new Point(250f, 0.39f),
			new Point(500f, 0.21f),
			new Point(1000f, 0.14f),
			new Point(2000f, 0.079f),
			new Point(4000f, 0.045f)
		};
	}

	private static void Foliage(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.03f),
			new Point(250f, 0.06f),
			new Point(500f, 0.11f),
			new Point(1000f, 0.17f),
			new Point(2000f, 0.27f),
			new Point(4000f, 0.31f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.2f),
			new Point(250f, 0.3f),
			new Point(500f, 0.4f),
			new Point(1000f, 0.5f),
			new Point(2000f, 0.7f),
			new Point(4000f, 0.8f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.9f),
			new Point(250f, 0.9f),
			new Point(500f, 0.9f),
			new Point(1000f, 0.8f),
			new Point(2000f, 0.5f),
			new Point(4000f, 0.3f)
		};
	}

	private static void Glass(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.35f),
			new Point(250f, 0.25f),
			new Point(500f, 0.18f),
			new Point(1000f, 0.12f),
			new Point(2000f, 0.07f),
			new Point(4000f, 0.05f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.05f),
			new Point(250f, 0.05f),
			new Point(500f, 0.05f),
			new Point(1000f, 0.05f),
			new Point(2000f, 0.05f),
			new Point(4000f, 0.05f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.125f),
			new Point(250f, 0.089f),
			new Point(500f, 0.05f),
			new Point(1000f, 0.028f),
			new Point(2000f, 0.022f),
			new Point(4000f, 0.079f)
		};
	}

	private static void GlassHeavy(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.18f),
			new Point(250f, 0.06f),
			new Point(500f, 0.04f),
			new Point(1000f, 0.03f),
			new Point(2000f, 0.02f),
			new Point(4000f, 0.02f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.05f),
			new Point(250f, 0.05f),
			new Point(500f, 0.05f),
			new Point(1000f, 0.05f),
			new Point(2000f, 0.05f),
			new Point(4000f, 0.05f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.056f),
			new Point(250f, 0.039f),
			new Point(500f, 0.028f),
			new Point(1000f, 0.02f),
			new Point(2000f, 0.032f),
			new Point(4000f, 0.014f)
		};
	}

	private static void Grass(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.11f),
			new Point(250f, 0.26f),
			new Point(500f, 0.6f),
			new Point(1000f, 0.69f),
			new Point(2000f, 0.92f),
			new Point(4000f, 0.99f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.3f),
			new Point(250f, 0.3f),
			new Point(500f, 0.4f),
			new Point(1000f, 0.5f),
			new Point(2000f, 0.6f),
			new Point(4000f, 0.7f)
		};
		material.transmission.points = new List<Point>();
	}

	private static void Gravel(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.25f),
			new Point(250f, 0.6f),
			new Point(500f, 0.65f),
			new Point(1000f, 0.7f),
			new Point(2000f, 0.75f),
			new Point(4000f, 0.8f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.2f),
			new Point(250f, 0.3f),
			new Point(500f, 0.4f),
			new Point(1000f, 0.5f),
			new Point(2000f, 0.6f),
			new Point(4000f, 0.7f)
		};
		material.transmission.points = new List<Point>();
	}

	private static void GypsumBoard(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.29f),
			new Point(250f, 0.1f),
			new Point(500f, 0.05f),
			new Point(1000f, 0.04f),
			new Point(2000f, 0.07f),
			new Point(4000f, 0.09f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.11f),
			new Point(500f, 0.12f),
			new Point(1000f, 0.13f),
			new Point(2000f, 0.14f),
			new Point(4000f, 0.15f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.035f),
			new Point(250f, 0.0125f),
			new Point(500f, 0.0056f),
			new Point(1000f, 0.0025f),
			new Point(2000f, 0.0013f),
			new Point(4000f, 0.0032f)
		};
	}

	private static void PlasterOnBrick(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.01f),
			new Point(250f, 0.02f),
			new Point(500f, 0.02f),
			new Point(1000f, 0.03f),
			new Point(2000f, 0.04f),
			new Point(4000f, 0.05f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.2f),
			new Point(250f, 0.25f),
			new Point(500f, 0.3f),
			new Point(1000f, 0.35f),
			new Point(2000f, 0.4f),
			new Point(4000f, 0.45f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.025f),
			new Point(250f, 0.019f),
			new Point(500f, 0.01f),
			new Point(1000f, 0.0045f),
			new Point(2000f, 0.0018f),
			new Point(4000f, 0.00089f)
		};
	}

	private static void PlasterOnConcreteBlock(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.12f),
			new Point(250f, 0.09f),
			new Point(500f, 0.07f),
			new Point(1000f, 0.05f),
			new Point(2000f, 0.05f),
			new Point(4000f, 0.04f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.2f),
			new Point(250f, 0.25f),
			new Point(500f, 0.3f),
			new Point(1000f, 0.35f),
			new Point(2000f, 0.4f),
			new Point(4000f, 0.45f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.02f),
			new Point(250f, 0.01f),
			new Point(500f, 0.0063f),
			new Point(1000f, 0.0035f),
			new Point(2000f, 0.00011f),
			new Point(4000f, 0.00063f)
		};
	}

	private static void Soil(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.15f),
			new Point(250f, 0.25f),
			new Point(500f, 0.4f),
			new Point(1000f, 0.55f),
			new Point(2000f, 0.6f),
			new Point(4000f, 0.6f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.2f),
			new Point(500f, 0.25f),
			new Point(1000f, 0.4f),
			new Point(2000f, 0.55f),
			new Point(4000f, 0.7f)
		};
		material.transmission.points = new List<Point>();
	}

	private static void SoundProof(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(1000f, 1f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(1000f)
		};
		material.transmission.points = new List<Point>();
	}

	private static void Snow(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.45f),
			new Point(250f, 0.75f),
			new Point(500f, 0.9f),
			new Point(1000f, 0.95f),
			new Point(2000f, 0.95f),
			new Point(4000f, 0.95f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.2f),
			new Point(250f, 0.3f),
			new Point(500f, 0.4f),
			new Point(1000f, 0.5f),
			new Point(2000f, 0.6f),
			new Point(4000f, 0.75f)
		};
		material.transmission.points = new List<Point>();
	}

	private static void Steel(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.05f),
			new Point(250f, 0.1f),
			new Point(500f, 0.1f),
			new Point(1000f, 0.1f),
			new Point(2000f, 0.07f),
			new Point(4000f, 0.02f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.1f),
			new Point(500f, 0.1f),
			new Point(1000f, 0.1f),
			new Point(2000f, 0.1f),
			new Point(4000f, 0.1f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.25f),
			new Point(250f, 0.2f),
			new Point(500f, 0.17f),
			new Point(1000f, 0.089f),
			new Point(2000f, 0.089f),
			new Point(4000f, 0.0056f)
		};
	}

	private static void Water(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.01f),
			new Point(250f, 0.01f),
			new Point(500f, 0.01f),
			new Point(1000f, 0.02f),
			new Point(2000f, 0.02f),
			new Point(4000f, 0.03f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.1f),
			new Point(500f, 0.1f),
			new Point(1000f, 0.07f),
			new Point(2000f, 0.05f),
			new Point(4000f, 0.05f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.03f),
			new Point(250f, 0.03f),
			new Point(500f, 0.03f),
			new Point(1000f, 0.02f),
			new Point(2000f, 0.015f),
			new Point(4000f, 0.01f)
		};
	}

	private static void WoodThin(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.42f),
			new Point(250f, 0.21f),
			new Point(500f, 0.1f),
			new Point(1000f, 0.08f),
			new Point(2000f, 0.06f),
			new Point(4000f, 0.06f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.1f),
			new Point(500f, 0.1f),
			new Point(1000f, 0.1f),
			new Point(2000f, 0.1f),
			new Point(4000f, 0.15f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.2f),
			new Point(250f, 0.125f),
			new Point(500f, 0.079f),
			new Point(1000f, 0.1f),
			new Point(2000f, 0.089f),
			new Point(4000f, 0.05f)
		};
	}

	private static void WoodThick(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.19f),
			new Point(250f, 0.14f),
			new Point(500f, 0.09f),
			new Point(1000f, 0.06f),
			new Point(2000f, 0.06f),
			new Point(4000f, 0.05f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.1f),
			new Point(500f, 0.1f),
			new Point(1000f, 0.1f),
			new Point(2000f, 0.1f),
			new Point(4000f, 0.15f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.035f),
			new Point(250f, 0.028f),
			new Point(500f, 0.028f),
			new Point(1000f, 0.028f),
			new Point(2000f, 0.011f),
			new Point(4000f, 0.0071f)
		};
	}

	private static void WoodFloor(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.15f),
			new Point(250f, 0.11f),
			new Point(500f, 0.1f),
			new Point(1000f, 0.07f),
			new Point(2000f, 0.06f),
			new Point(4000f, 0.07f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.1f),
			new Point(500f, 0.1f),
			new Point(1000f, 0.1f),
			new Point(2000f, 0.1f),
			new Point(4000f, 0.15f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.071f),
			new Point(250f, 0.025f),
			new Point(500f, 0.0158f),
			new Point(1000f, 0.0056f),
			new Point(2000f, 0.0035f),
			new Point(4000f, 0.0016f)
		};
	}

	private static void WoodOnConcrete(ref ONSPPropagationMaterial material)
	{
		material.absorption.points = new List<Point>
		{
			new Point(125f, 0.04f),
			new Point(250f, 0.04f),
			new Point(500f, 0.07f),
			new Point(1000f, 0.06f),
			new Point(2000f, 0.06f),
			new Point(4000f, 0.07f)
		};
		material.scattering.points = new List<Point>
		{
			new Point(125f, 0.1f),
			new Point(250f, 0.1f),
			new Point(500f, 0.1f),
			new Point(1000f, 0.1f),
			new Point(2000f, 0.1f),
			new Point(4000f, 0.15f)
		};
		material.transmission.points = new List<Point>
		{
			new Point(125f, 0.004f),
			new Point(250f, 0.0079f),
			new Point(500f, 0.0056f),
			new Point(1000f, 0.0016f),
			new Point(2000f, 0.0014f),
			new Point(4000f, 0.0005f)
		};
	}
}

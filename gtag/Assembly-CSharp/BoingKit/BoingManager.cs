using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000368 RID: 872
	public static class BoingManager
	{
		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06001973 RID: 6515 RVA: 0x0008D280 File Offset: 0x0008B480
		public static IEnumerable<BoingBehavior> Behaviors
		{
			get
			{
				return BoingManager.s_behaviorMap.Values;
			}
		}

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06001974 RID: 6516 RVA: 0x0008D28C File Offset: 0x0008B48C
		public static IEnumerable<BoingReactor> Reactors
		{
			get
			{
				return BoingManager.s_reactorMap.Values;
			}
		}

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x06001975 RID: 6517 RVA: 0x0008D298 File Offset: 0x0008B498
		public static IEnumerable<BoingEffector> Effectors
		{
			get
			{
				return BoingManager.s_effectorMap.Values;
			}
		}

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x06001976 RID: 6518 RVA: 0x0008D2A4 File Offset: 0x0008B4A4
		public static IEnumerable<BoingReactorField> ReactorFields
		{
			get
			{
				return BoingManager.s_fieldMap.Values;
			}
		}

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x06001977 RID: 6519 RVA: 0x0008D2B0 File Offset: 0x0008B4B0
		public static IEnumerable<BoingReactorFieldCPUSampler> ReactorFieldCPUSamlers
		{
			get
			{
				return BoingManager.s_cpuSamplerMap.Values;
			}
		}

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06001978 RID: 6520 RVA: 0x0008D2BC File Offset: 0x0008B4BC
		public static IEnumerable<BoingReactorFieldGPUSampler> ReactorFieldGPUSampler
		{
			get
			{
				return BoingManager.s_gpuSamplerMap.Values;
			}
		}

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06001979 RID: 6521 RVA: 0x0008D2C8 File Offset: 0x0008B4C8
		public static float DeltaTime
		{
			get
			{
				return BoingManager.s_deltaTime;
			}
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x0600197A RID: 6522 RVA: 0x0008D2CF File Offset: 0x0008B4CF
		public static float FixedDeltaTime
		{
			get
			{
				return Time.fixedDeltaTime;
			}
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x0600197B RID: 6523 RVA: 0x0008D2D6 File Offset: 0x0008B4D6
		internal static int NumBehaviors
		{
			get
			{
				return BoingManager.s_behaviorMap.Count;
			}
		}

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x0600197C RID: 6524 RVA: 0x0008D2E2 File Offset: 0x0008B4E2
		internal static int NumEffectors
		{
			get
			{
				return BoingManager.s_effectorMap.Count;
			}
		}

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x0600197D RID: 6525 RVA: 0x0008D2EE File Offset: 0x0008B4EE
		internal static int NumReactors
		{
			get
			{
				return BoingManager.s_reactorMap.Count;
			}
		}

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x0600197E RID: 6526 RVA: 0x0008D2FA File Offset: 0x0008B4FA
		internal static int NumFields
		{
			get
			{
				return BoingManager.s_fieldMap.Count;
			}
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x0600197F RID: 6527 RVA: 0x0008D306 File Offset: 0x0008B506
		internal static int NumCPUFieldSamplers
		{
			get
			{
				return BoingManager.s_cpuSamplerMap.Count;
			}
		}

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x06001980 RID: 6528 RVA: 0x0008D312 File Offset: 0x0008B512
		internal static int NumGPUFieldSamplers
		{
			get
			{
				return BoingManager.s_gpuSamplerMap.Count;
			}
		}

		// Token: 0x06001981 RID: 6529 RVA: 0x0008D320 File Offset: 0x0008B520
		private static void ValidateManager()
		{
			if (BoingManager.s_managerGo != null)
			{
				return;
			}
			BoingManager.s_managerGo = new GameObject("Boing Kit manager (don't delete)");
			BoingManager.s_managerGo.AddComponent<BoingManagerPreUpdatePump>();
			BoingManager.s_managerGo.AddComponent<BoingManagerPostUpdatePump>();
			Object.DontDestroyOnLoad(BoingManager.s_managerGo);
			BoingManager.s_managerGo.AddComponent<SphereCollider>().enabled = false;
		}

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x06001982 RID: 6530 RVA: 0x0008D37A File Offset: 0x0008B57A
		internal static SphereCollider SharedSphereCollider
		{
			get
			{
				if (BoingManager.s_managerGo == null)
				{
					return null;
				}
				return BoingManager.s_managerGo.GetComponent<SphereCollider>();
			}
		}

		// Token: 0x06001983 RID: 6531 RVA: 0x0008D395 File Offset: 0x0008B595
		internal static void Register(BoingBehavior behavior)
		{
			BoingManager.PreRegisterBehavior();
			BoingManager.s_behaviorMap.Add(behavior.GetInstanceID(), behavior);
			if (BoingManager.OnBehaviorRegister != null)
			{
				BoingManager.OnBehaviorRegister(behavior);
			}
		}

		// Token: 0x06001984 RID: 6532 RVA: 0x0008D3BF File Offset: 0x0008B5BF
		internal static void Unregister(BoingBehavior behavior)
		{
			if (BoingManager.OnBehaviorUnregister != null)
			{
				BoingManager.OnBehaviorUnregister(behavior);
			}
			BoingManager.s_behaviorMap.Remove(behavior.GetInstanceID());
			BoingManager.PostUnregisterBehavior();
		}

		// Token: 0x06001985 RID: 6533 RVA: 0x0008D3E9 File Offset: 0x0008B5E9
		internal static void Register(BoingEffector effector)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_effectorMap.Add(effector.GetInstanceID(), effector);
			if (BoingManager.OnEffectorRegister != null)
			{
				BoingManager.OnEffectorRegister(effector);
			}
		}

		// Token: 0x06001986 RID: 6534 RVA: 0x0008D413 File Offset: 0x0008B613
		internal static void Unregister(BoingEffector effector)
		{
			if (BoingManager.OnEffectorUnregister != null)
			{
				BoingManager.OnEffectorUnregister(effector);
			}
			BoingManager.s_effectorMap.Remove(effector.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06001987 RID: 6535 RVA: 0x0008D43D File Offset: 0x0008B63D
		internal static void Register(BoingReactor reactor)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_reactorMap.Add(reactor.GetInstanceID(), reactor);
			if (BoingManager.OnReactorRegister != null)
			{
				BoingManager.OnReactorRegister(reactor);
			}
		}

		// Token: 0x06001988 RID: 6536 RVA: 0x0008D467 File Offset: 0x0008B667
		internal static void Unregister(BoingReactor reactor)
		{
			if (BoingManager.OnReactorUnregister != null)
			{
				BoingManager.OnReactorUnregister(reactor);
			}
			BoingManager.s_reactorMap.Remove(reactor.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x06001989 RID: 6537 RVA: 0x0008D491 File Offset: 0x0008B691
		internal static void Register(BoingReactorField field)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_fieldMap.Add(field.GetInstanceID(), field);
			if (BoingManager.OnReactorFieldRegister != null)
			{
				BoingManager.OnReactorFieldRegister(field);
			}
		}

		// Token: 0x0600198A RID: 6538 RVA: 0x0008D4BB File Offset: 0x0008B6BB
		internal static void Unregister(BoingReactorField field)
		{
			if (BoingManager.OnReactorFieldUnregister != null)
			{
				BoingManager.OnReactorFieldUnregister(field);
			}
			BoingManager.s_fieldMap.Remove(field.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x0600198B RID: 6539 RVA: 0x0008D4E5 File Offset: 0x0008B6E5
		internal static void Register(BoingReactorFieldCPUSampler sampler)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_cpuSamplerMap.Add(sampler.GetInstanceID(), sampler);
			if (BoingManager.OnReactorFieldCPUSamplerRegister != null)
			{
				BoingManager.OnReactorFieldCPUSamplerUnregister(sampler);
			}
		}

		// Token: 0x0600198C RID: 6540 RVA: 0x0008D50F File Offset: 0x0008B70F
		internal static void Unregister(BoingReactorFieldCPUSampler sampler)
		{
			if (BoingManager.OnReactorFieldCPUSamplerUnregister != null)
			{
				BoingManager.OnReactorFieldCPUSamplerUnregister(sampler);
			}
			BoingManager.s_cpuSamplerMap.Remove(sampler.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x0600198D RID: 6541 RVA: 0x0008D539 File Offset: 0x0008B739
		internal static void Register(BoingReactorFieldGPUSampler sampler)
		{
			BoingManager.PreRegisterEffectorReactor();
			BoingManager.s_gpuSamplerMap.Add(sampler.GetInstanceID(), sampler);
			if (BoingManager.OnReactorFieldGPUSamplerRegister != null)
			{
				BoingManager.OnReactorFieldGPUSamplerRegister(sampler);
			}
		}

		// Token: 0x0600198E RID: 6542 RVA: 0x0008D563 File Offset: 0x0008B763
		internal static void Unregister(BoingReactorFieldGPUSampler sampler)
		{
			if (BoingManager.OnFieldGPUSamplerUnregister != null)
			{
				BoingManager.OnFieldGPUSamplerUnregister(sampler);
			}
			BoingManager.s_gpuSamplerMap.Remove(sampler.GetInstanceID());
			BoingManager.PostUnregisterEffectorReactor();
		}

		// Token: 0x0600198F RID: 6543 RVA: 0x0008D58D File Offset: 0x0008B78D
		internal static void Register(BoingBones bones)
		{
			BoingManager.PreRegisterBones();
			BoingManager.s_bonesMap.Add(bones.GetInstanceID(), bones);
			if (BoingManager.OnBonesRegister != null)
			{
				BoingManager.OnBonesRegister(bones);
			}
		}

		// Token: 0x06001990 RID: 6544 RVA: 0x0008D5B7 File Offset: 0x0008B7B7
		internal static void Unregister(BoingBones bones)
		{
			if (BoingManager.OnBonesUnregister != null)
			{
				BoingManager.OnBonesUnregister(bones);
			}
			BoingManager.s_bonesMap.Remove(bones.GetInstanceID());
			BoingManager.PostUnregisterBones();
		}

		// Token: 0x06001991 RID: 6545 RVA: 0x0008D5E1 File Offset: 0x0008B7E1
		private static void PreRegisterBehavior()
		{
			BoingManager.ValidateManager();
		}

		// Token: 0x06001992 RID: 6546 RVA: 0x0008D5E8 File Offset: 0x0008B7E8
		private static void PostUnregisterBehavior()
		{
			if (BoingManager.s_behaviorMap.Count > 0)
			{
				return;
			}
			BoingWorkAsynchronous.PostUnregisterBehaviorCleanUp();
		}

		// Token: 0x06001993 RID: 6547 RVA: 0x0008D600 File Offset: 0x0008B800
		private static void PreRegisterEffectorReactor()
		{
			BoingManager.ValidateManager();
			if (BoingManager.s_effectorParamsBuffer == null)
			{
				BoingManager.s_effectorParamsList = new List<BoingEffector.Params>(BoingManager.kEffectorParamsIncrement);
				BoingManager.s_effectorParamsBuffer = new ComputeBuffer(BoingManager.s_effectorParamsList.Capacity, BoingEffector.Params.Stride);
			}
			if (BoingManager.s_effectorMap.Count >= BoingManager.s_effectorParamsList.Capacity)
			{
				BoingManager.s_effectorParamsList.Capacity += BoingManager.kEffectorParamsIncrement;
				BoingManager.s_effectorParamsBuffer.Dispose();
				BoingManager.s_effectorParamsBuffer = new ComputeBuffer(BoingManager.s_effectorParamsList.Capacity, BoingEffector.Params.Stride);
			}
		}

		// Token: 0x06001994 RID: 6548 RVA: 0x0008D690 File Offset: 0x0008B890
		private static void PostUnregisterEffectorReactor()
		{
			if (BoingManager.s_effectorMap.Count > 0 || BoingManager.s_reactorMap.Count > 0 || BoingManager.s_fieldMap.Count > 0 || BoingManager.s_cpuSamplerMap.Count > 0 || BoingManager.s_gpuSamplerMap.Count > 0)
			{
				return;
			}
			BoingManager.s_effectorParamsList = null;
			BoingManager.s_effectorParamsBuffer.Dispose();
			BoingManager.s_effectorParamsBuffer = null;
			BoingWorkAsynchronous.PostUnregisterEffectorReactorCleanUp();
		}

		// Token: 0x06001995 RID: 6549 RVA: 0x0008D6FA File Offset: 0x0008B8FA
		private static void PreRegisterBones()
		{
			BoingManager.ValidateManager();
		}

		// Token: 0x06001996 RID: 6550 RVA: 0x0008D701 File Offset: 0x0008B901
		private static void PostUnregisterBones()
		{
		}

		// Token: 0x06001997 RID: 6551 RVA: 0x0008D703 File Offset: 0x0008B903
		internal static void Execute(BoingManager.UpdateMode updateMode)
		{
			if (updateMode == BoingManager.UpdateMode.EarlyUpdate)
			{
				BoingManager.s_deltaTime = Time.deltaTime;
			}
			BoingManager.RefreshEffectorParams();
			BoingManager.ExecuteBones(updateMode);
			BoingManager.ExecuteBehaviors(updateMode);
			BoingManager.ExecuteReactors(updateMode);
		}

		// Token: 0x06001998 RID: 6552 RVA: 0x0008D72C File Offset: 0x0008B92C
		internal static void ExecuteBehaviors(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_behaviorMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				BoingBehavior value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteBehaviors(BoingManager.s_behaviorMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteBehaviors(BoingManager.s_behaviorMap, updateMode);
		}

		// Token: 0x06001999 RID: 6553 RVA: 0x0008D7C0 File Offset: 0x0008B9C0
		internal static void PullBehaviorResults(BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				if (keyValuePair.Value.UpdateMode == updateMode)
				{
					keyValuePair.Value.PullResults();
				}
			}
		}

		// Token: 0x0600199A RID: 6554 RVA: 0x0008D828 File Offset: 0x0008BA28
		internal static void RestoreBehaviors()
		{
			foreach (KeyValuePair<int, BoingBehavior> keyValuePair in BoingManager.s_behaviorMap)
			{
				keyValuePair.Value.Restore();
			}
		}

		// Token: 0x0600199B RID: 6555 RVA: 0x0008D880 File Offset: 0x0008BA80
		internal static void RefreshEffectorParams()
		{
			if (BoingManager.s_effectorParamsList == null)
			{
				return;
			}
			BoingManager.s_effectorParamsIndexMap.Clear();
			BoingManager.s_effectorParamsList.Clear();
			foreach (KeyValuePair<int, BoingEffector> keyValuePair in BoingManager.s_effectorMap)
			{
				BoingEffector value = keyValuePair.Value;
				BoingManager.s_effectorParamsIndexMap.Add(value.GetInstanceID(), BoingManager.s_effectorParamsList.Count);
				BoingManager.s_effectorParamsList.Add(new BoingEffector.Params(value));
			}
			if (BoingManager.s_aEffectorParams == null || BoingManager.s_aEffectorParams.Length != BoingManager.s_effectorParamsList.Count)
			{
				BoingManager.s_aEffectorParams = BoingManager.s_effectorParamsList.ToArray();
				return;
			}
			BoingManager.s_effectorParamsList.CopyTo(BoingManager.s_aEffectorParams);
		}

		// Token: 0x0600199C RID: 6556 RVA: 0x0008D954 File Offset: 0x0008BB54
		internal static void ExecuteReactors(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_effectorMap.Count == 0 && BoingManager.s_reactorMap.Count == 0 && BoingManager.s_fieldMap.Count == 0 && BoingManager.s_cpuSamplerMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				BoingReactor value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteReactors(BoingManager.s_effectorMap, BoingManager.s_reactorMap, BoingManager.s_fieldMap, BoingManager.s_cpuSamplerMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteReactors(BoingManager.s_aEffectorParams, BoingManager.s_reactorMap, BoingManager.s_fieldMap, BoingManager.s_cpuSamplerMap, updateMode);
		}

		// Token: 0x0600199D RID: 6557 RVA: 0x0008DA2C File Offset: 0x0008BC2C
		internal static void PullReactorResults(BoingManager.UpdateMode updateMode)
		{
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				if (keyValuePair.Value.UpdateMode == updateMode)
				{
					keyValuePair.Value.PullResults();
				}
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair2 in BoingManager.s_cpuSamplerMap)
			{
				if (keyValuePair2.Value.UpdateMode == updateMode)
				{
					keyValuePair2.Value.SampleFromField();
				}
			}
		}

		// Token: 0x0600199E RID: 6558 RVA: 0x0008DAE8 File Offset: 0x0008BCE8
		internal static void RestoreReactors()
		{
			foreach (KeyValuePair<int, BoingReactor> keyValuePair in BoingManager.s_reactorMap)
			{
				keyValuePair.Value.Restore();
			}
			foreach (KeyValuePair<int, BoingReactorFieldCPUSampler> keyValuePair2 in BoingManager.s_cpuSamplerMap)
			{
				keyValuePair2.Value.Restore();
			}
		}

		// Token: 0x0600199F RID: 6559 RVA: 0x0008DB88 File Offset: 0x0008BD88
		internal static void DispatchReactorFieldCompute()
		{
			if (BoingManager.s_effectorParamsBuffer == null)
			{
				return;
			}
			BoingManager.s_effectorParamsBuffer.SetData(BoingManager.s_aEffectorParams);
			float deltaTime = Time.deltaTime;
			foreach (KeyValuePair<int, BoingReactorField> keyValuePair in BoingManager.s_fieldMap)
			{
				BoingReactorField value = keyValuePair.Value;
				if (value.HardwareMode == BoingReactorField.HardwareModeEnum.GPU)
				{
					value.ExecuteGpu(deltaTime, BoingManager.s_effectorParamsBuffer, BoingManager.s_effectorParamsIndexMap);
				}
			}
		}

		// Token: 0x060019A0 RID: 6560 RVA: 0x0008DC14 File Offset: 0x0008BE14
		internal static void ExecuteBones(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_bonesMap.Count == 0)
			{
				return;
			}
			foreach (KeyValuePair<int, BoingBones> keyValuePair in BoingManager.s_bonesMap)
			{
				BoingBones value = keyValuePair.Value;
				if (!value.InitRebooted)
				{
					value.Reboot();
					value.InitRebooted = true;
				}
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.ExecuteBones(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
				return;
			}
			BoingWorkSynchronous.ExecuteBones(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
		}

		// Token: 0x060019A1 RID: 6561 RVA: 0x0008DCB4 File Offset: 0x0008BEB4
		internal static void PullBonesResults(BoingManager.UpdateMode updateMode)
		{
			if (BoingManager.s_bonesMap.Count == 0)
			{
				return;
			}
			if (BoingManager.UseAsynchronousJobs)
			{
				BoingWorkAsynchronous.PullBonesResults(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
				return;
			}
			BoingWorkSynchronous.PullBonesResults(BoingManager.s_aEffectorParams, BoingManager.s_bonesMap, updateMode);
		}

		// Token: 0x060019A2 RID: 6562 RVA: 0x0008DCEC File Offset: 0x0008BEEC
		internal static void RestoreBones()
		{
			foreach (KeyValuePair<int, BoingBones> keyValuePair in BoingManager.s_bonesMap)
			{
				keyValuePair.Value.Restore();
			}
		}

		// Token: 0x04001A34 RID: 6708
		public static BoingManager.BehaviorRegisterDelegate OnBehaviorRegister;

		// Token: 0x04001A35 RID: 6709
		public static BoingManager.BehaviorUnregisterDelegate OnBehaviorUnregister;

		// Token: 0x04001A36 RID: 6710
		public static BoingManager.EffectorRegisterDelegate OnEffectorRegister;

		// Token: 0x04001A37 RID: 6711
		public static BoingManager.EffectorUnregisterDelegate OnEffectorUnregister;

		// Token: 0x04001A38 RID: 6712
		public static BoingManager.ReactorRegisterDelegate OnReactorRegister;

		// Token: 0x04001A39 RID: 6713
		public static BoingManager.ReactorUnregisterDelegate OnReactorUnregister;

		// Token: 0x04001A3A RID: 6714
		public static BoingManager.ReactorFieldRegisterDelegate OnReactorFieldRegister;

		// Token: 0x04001A3B RID: 6715
		public static BoingManager.ReactorFieldUnregisterDelegate OnReactorFieldUnregister;

		// Token: 0x04001A3C RID: 6716
		public static BoingManager.ReactorFieldCPUSamplerRegisterDelegate OnReactorFieldCPUSamplerRegister;

		// Token: 0x04001A3D RID: 6717
		public static BoingManager.ReactorFieldCPUSamplerUnregisterDelegate OnReactorFieldCPUSamplerUnregister;

		// Token: 0x04001A3E RID: 6718
		public static BoingManager.ReactorFieldGPUSamplerRegisterDelegate OnReactorFieldGPUSamplerRegister;

		// Token: 0x04001A3F RID: 6719
		public static BoingManager.ReactorFieldGPUSamplerUnregisterDelegate OnFieldGPUSamplerUnregister;

		// Token: 0x04001A40 RID: 6720
		public static BoingManager.BonesRegisterDelegate OnBonesRegister;

		// Token: 0x04001A41 RID: 6721
		public static BoingManager.BonesUnregisterDelegate OnBonesUnregister;

		// Token: 0x04001A42 RID: 6722
		private static float s_deltaTime = 0f;

		// Token: 0x04001A43 RID: 6723
		private static Dictionary<int, BoingBehavior> s_behaviorMap = new Dictionary<int, BoingBehavior>();

		// Token: 0x04001A44 RID: 6724
		private static Dictionary<int, BoingEffector> s_effectorMap = new Dictionary<int, BoingEffector>();

		// Token: 0x04001A45 RID: 6725
		private static Dictionary<int, BoingReactor> s_reactorMap = new Dictionary<int, BoingReactor>();

		// Token: 0x04001A46 RID: 6726
		private static Dictionary<int, BoingReactorField> s_fieldMap = new Dictionary<int, BoingReactorField>();

		// Token: 0x04001A47 RID: 6727
		private static Dictionary<int, BoingReactorFieldCPUSampler> s_cpuSamplerMap = new Dictionary<int, BoingReactorFieldCPUSampler>();

		// Token: 0x04001A48 RID: 6728
		private static Dictionary<int, BoingReactorFieldGPUSampler> s_gpuSamplerMap = new Dictionary<int, BoingReactorFieldGPUSampler>();

		// Token: 0x04001A49 RID: 6729
		private static Dictionary<int, BoingBones> s_bonesMap = new Dictionary<int, BoingBones>();

		// Token: 0x04001A4A RID: 6730
		private static readonly int kEffectorParamsIncrement = 16;

		// Token: 0x04001A4B RID: 6731
		private static List<BoingEffector.Params> s_effectorParamsList = new List<BoingEffector.Params>(BoingManager.kEffectorParamsIncrement);

		// Token: 0x04001A4C RID: 6732
		private static BoingEffector.Params[] s_aEffectorParams;

		// Token: 0x04001A4D RID: 6733
		private static ComputeBuffer s_effectorParamsBuffer;

		// Token: 0x04001A4E RID: 6734
		private static Dictionary<int, int> s_effectorParamsIndexMap = new Dictionary<int, int>();

		// Token: 0x04001A4F RID: 6735
		internal static readonly bool UseAsynchronousJobs = true;

		// Token: 0x04001A50 RID: 6736
		internal static GameObject s_managerGo;

		// Token: 0x02000520 RID: 1312
		public enum UpdateMode
		{
			// Token: 0x0400219F RID: 8607
			FixedUpdate,
			// Token: 0x040021A0 RID: 8608
			EarlyUpdate,
			// Token: 0x040021A1 RID: 8609
			LateUpdate
		}

		// Token: 0x02000521 RID: 1313
		public enum TranslationLockSpace
		{
			// Token: 0x040021A3 RID: 8611
			Global,
			// Token: 0x040021A4 RID: 8612
			Local
		}

		// Token: 0x02000522 RID: 1314
		// (Invoke) Token: 0x06001F70 RID: 8048
		public delegate void BehaviorRegisterDelegate(BoingBehavior behavior);

		// Token: 0x02000523 RID: 1315
		// (Invoke) Token: 0x06001F74 RID: 8052
		public delegate void BehaviorUnregisterDelegate(BoingBehavior behavior);

		// Token: 0x02000524 RID: 1316
		// (Invoke) Token: 0x06001F78 RID: 8056
		public delegate void EffectorRegisterDelegate(BoingEffector effector);

		// Token: 0x02000525 RID: 1317
		// (Invoke) Token: 0x06001F7C RID: 8060
		public delegate void EffectorUnregisterDelegate(BoingEffector effector);

		// Token: 0x02000526 RID: 1318
		// (Invoke) Token: 0x06001F80 RID: 8064
		public delegate void ReactorRegisterDelegate(BoingReactor reactor);

		// Token: 0x02000527 RID: 1319
		// (Invoke) Token: 0x06001F84 RID: 8068
		public delegate void ReactorUnregisterDelegate(BoingReactor reactor);

		// Token: 0x02000528 RID: 1320
		// (Invoke) Token: 0x06001F88 RID: 8072
		public delegate void ReactorFieldRegisterDelegate(BoingReactorField field);

		// Token: 0x02000529 RID: 1321
		// (Invoke) Token: 0x06001F8C RID: 8076
		public delegate void ReactorFieldUnregisterDelegate(BoingReactorField field);

		// Token: 0x0200052A RID: 1322
		// (Invoke) Token: 0x06001F90 RID: 8080
		public delegate void ReactorFieldCPUSamplerRegisterDelegate(BoingReactorFieldCPUSampler sampler);

		// Token: 0x0200052B RID: 1323
		// (Invoke) Token: 0x06001F94 RID: 8084
		public delegate void ReactorFieldCPUSamplerUnregisterDelegate(BoingReactorFieldCPUSampler sampler);

		// Token: 0x0200052C RID: 1324
		// (Invoke) Token: 0x06001F98 RID: 8088
		public delegate void ReactorFieldGPUSamplerRegisterDelegate(BoingReactorFieldGPUSampler sampler);

		// Token: 0x0200052D RID: 1325
		// (Invoke) Token: 0x06001F9C RID: 8092
		public delegate void ReactorFieldGPUSamplerUnregisterDelegate(BoingReactorFieldGPUSampler sampler);

		// Token: 0x0200052E RID: 1326
		// (Invoke) Token: 0x06001FA0 RID: 8096
		public delegate void BonesRegisterDelegate(BoingBones bones);

		// Token: 0x0200052F RID: 1327
		// (Invoke) Token: 0x06001FA4 RID: 8100
		public delegate void BonesUnregisterDelegate(BoingBones bones);
	}
}

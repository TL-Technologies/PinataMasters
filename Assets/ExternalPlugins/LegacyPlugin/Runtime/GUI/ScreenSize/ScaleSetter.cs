﻿using UnityEngine;

[AddComponentMenu("Modules/Size/ScaleSetter")]
public class ScaleSetter : SizeSetter {

	public SizeFactor scaleFactorType = SizeFactor.MinFactor;
	
	protected override void UpdateSize() {
		SizeHelper.RecalculateScale(transform, scaleFactorType, roundFloatPreference);
	}
}

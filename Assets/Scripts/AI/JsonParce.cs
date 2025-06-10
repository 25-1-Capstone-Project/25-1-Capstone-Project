using System;
using System.Collections.Generic;

[Serializable]

//Json 파일 파싱용 코드
public class FuzzyRule
{
    public int cluster_id;
    public float[] center;
    public float[] coeffs;
    public float intercept;
    public int n_samples;
}

[Serializable]
public class FuzzyRuleList
{
    public List<FuzzyRule> rules;
}

[Serializable]
public class ScalerData
{
    public float[] min;
    public float[] max;
}

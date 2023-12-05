using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 加权随机函数
/// </summary>
public class MyRandom
{
    public struct Range
    {
        public float min;
        public float max;
        public float probability;

        public Range(float min, float max, float probability)
        {
            this.min = min;
            this.max = max;
            this.probability = probability;
        }
    }
    //填写概率和生成范围
    public float GetWeightedRandom(Range[] ranges)
    {
        // 计算总概率
        float totalProbability = 0;
        foreach (var range in ranges)
        {
            totalProbability += range.probability;
        }

        // 生成一个0到总概率之间的随机数
        float randomValue = UnityEngine.Random.Range(0, totalProbability);

        // 找出随机数落在哪个范围内
        foreach (var range in ranges)
        {
            if (randomValue < range.probability)
            {
                // 返回这个范围内的随机数
                return UnityEngine.Random.Range(range.min, range.max);
            }
            else
            {
                randomValue -= range.probability;
            }
        }

        // 如果程序运行到这里，那说明输入的概率之和不是100%，这是一个错误
        // 在这种情况下，我们只能返回一个默认值
        throw new System.ArgumentException("The sum of probabilities is not equal to 1.");
    }

}

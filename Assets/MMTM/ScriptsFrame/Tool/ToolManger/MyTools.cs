using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;

namespace MMTM
{
    public static class UniTaskTool
    {
        /// <summary>
        /// 物体按高度做抛物线运动
        /// </summary>
        /// <param name="moveThing">运动的物体</param>
        /// <param name="end">终点</param>
        /// <param name="hight">高度</param>
        /// <param name="hightAxis">高度的轴，比如2D游戏的高度轴是Z的话就使用vector3.Forward</param>
        /// <param name="speed">速度倍率</param>
        /// <param name="callback"></param>
        /// <param name="curve"></param>
        /// <param name="onBegin"></param>
        /// <param name="onUpdata"></param>
        /// <param name="onComplete"></param>
        public static async UniTaskVoid DoAimBezierMove(this Transform moveThing,
            Vector3 end,
            float hight,
            Vector3 hightAxis,
            float speed = 1,
            CancellationTokenSource callback = null,
            AnimationCurve curve = null,
            Action onBegin = null,
            Action onUpdata = null,
            Action onComplete = null)
        {
            onBegin?.Invoke();
            callback = new CancellationTokenSource();
            var begin = moveThing.position;
            var mid = MyTools.GetBetweenPoint(begin,end) + hightAxis * hight;
            var progress = 0f;        
            while (progress<1)
            {
                moveThing.transform.position = MyTools.MyMath.GetBezierPoint_MidHight(progress, begin, mid, end);
                progress += Time.deltaTime * speed;
                if(curve!=null) progress = curve.Evaluate(progress);
                onUpdata?.Invoke();
                await UniTask.Delay(TimeSpan.FromTicks(1),cancellationToken:callback.Token);
            }
            moveThing.transform.position = end;
            onComplete?.Invoke();
        }
    }

    public static class MyTools
    {
        public struct Vector2DDouble
        {
            public double x;
            public double y;

            public Vector2DDouble(double x, double y)
            {
                this.x = x;
                this.y = y;
            }

            // ... 其他你需要的操作，比如加法、减法、点乘等
        }
        public static class MyMath
        {
            /// <summary>
            /// 重新映射,包含最后一个值
            /// </summary>
            /// <param name="value"></param>
            /// <param name="from1"></param>
            /// <param name="to1"></param>
            /// <param name="from2"></param>
            /// <param name="to2"></param>
            /// <returns></returns>
            // public static int Remap(int value, int from1, int to1, int from2, int to2) {
            //     return (value - from1) * (to2 - from2) / (to1 - from1) + from2;
            // }
            public static float Remap(float value, float from1, float to1, float from2, float to2)
            {
                if (value < from1) value = from1;
                else if (value > to1) value = to1;
                return (value - from1) * (to2 - from2) / (to1 - from1) + from2;
            }

            public static int RemapInt(float value, float from1, float to1, float from2, float to2)
            {
                if (value < from1) value = from1;
                else if (value > to1) value = to1;
                if ((to1 - from1) == 0) return (int)from2;
                return (int)Mathf.Round((value - from1) * (to2 - from2) / (to1 - from1) + from2);
            }

            /// <summary>
            /// 贝塞尔曲线3点式
            /// </summary>
            /// <param name="t">0~1目标的位置</param>
            /// <param name="start">起始点</param>
            /// <param name="center">中控制点</param>
            /// <param name="end">目标点</param>
            /// <returns></returns>
            public static Vector3 GetBezierPoint_MidControl(float t, Vector3 start, Vector3 center, Vector3 end)
            {
                return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
            }

            /// <summary>
            /// 贝塞尔曲线3点式这个可以让物体通过中间midHight点
            /// </summary>
            /// <param name="t">0~1目标的位置</param>
            /// <param name="start">起始点</param>
            /// <param name="center">中最高点</param>
            /// <param name="end">目标点</param>
            /// <returns></returns>
            public static Vector3 GetBezierPoint_MidHight(float t, Vector3 start, Vector3 midHight, Vector3 end)
            {
                // 将 mid 点作为控制点，并计算修正后的控制点坐标
                Vector3 control = midHight * 2 - (start + end) / 2;

                // 贝塞尔曲线公式
                // P(t) = (1 - t)^2 * start + 2 * (1 - t) * t * control + t^2 * end

                float u = 1 - t;
                float tt = t * t;
                float uu = u * u;

                // 计算点的坐标
                Vector3 point = uu * start + 2 * u * t * control + tt * end;

                return point;
            }


            /// <summary>
            /// 贝塞尔曲线4点式
            /// </summary>
            /// <param name="t"></param>
            /// <param name="p0">起点</param>
            /// <param name="p1">中点1</param>
            /// <param name="p2">中点2</param>
            /// <param name="p3">终点</param>
            /// <returns></returns>
            public static Vector3 GetBezierPoint_4Point(float t, Vector3 start, Vector3 center_1, Vector3 center_2, Vector3 end)
            {
                float u = 1 - t;
                float tt = t * t;
                float uu = u * u;
                float uuu = uu * u;
                float ttt = tt * t;

                Vector3 B = new Vector3();
                B = uuu * start; // (1 - t) ^ 3 * p0
                B += 3 * uu * t * center_1; // 3(1 - t) ^ 2 * t * p1
                B += 3 * u * tt * center_2; // 3(1 - t) * t ^ 2 * p2
                B += ttt * end; // t ^ 3 * p3

                return B;
            }

            /// <summary>
            /// 获取柏林函数
            /// </summary>
            /// <param name="pos">位置</param>
            /// <param name="scale">缩放</param>
            /// <param name="offset">偏移，也可以当种子</param>
            /// <param name="needcount">需要后几位</param>
            /// <returns></returns>
            public static List<int> GetPerlinRandom(Vector2 pos, Vector2 scale, Vector2 offset, int needcount, float multi = 1)
            {
                float perling = Mathf.PerlinNoise((pos.x + offset.x) * scale.x, (pos.y + offset.y) * scale.y);
                perling = Mathf.Pow(perling, multi);
                List<int> get = new List<int>();

                for (var i = 1; i <= needcount; i++)
                {
                    get.Add(Mathf.FloorToInt((perling * Mathf.Pow(10, i)) % 10)); //获取小数后1位
                }

                return get;
            }


            /// <summary>
            /// 通过权重来选择物品但是是浮点
            /// </summary>
            /// <param name="weights">所有权重</param>
            /// <returns></returns>
            public static int GetRandomFromWeights(int Seed, float[] weights)
            {
                // 计算权重总和
                float totalWeight = 0;
                for (int i = 0; i < weights.Length; i++)
                {
                    totalWeight += weights[i];
                }

                Random.InitState(Seed);
                // 生成一个0到totalWeight之间的随机数
                float randomValue = Random.Range(0, totalWeight);

                // 确定随机值落在哪个权重区间内
                float weightSum = 0;
                for (int i = 0; i < weights.Length; i++)
                {
                    weightSum += weights[i];
                    if (randomValue <= weightSum)
                    {
                        return i;
                    }
                }

                // 如果没有返回索引，返回-1表示出错
                return -1;
            }
        }

        //获取到达目标点需要多少力
        public static Vector2 CalculateLaunchForce(Rigidbody2D rb, Vector2 startPosition, Vector2 targetPosition, float apexHeight)
        {
            // Calculate the distance between start and target positions
            float dx = targetPosition.x - startPosition.x;
            float dy = targetPosition.y - startPosition.y;

            // Calculate the initial speed needed to reach the apex height
            float gravity = Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale;
            float initialSpeedY = Mathf.Sqrt(2 * gravity * apexHeight);

            // Calculate the total time for the journey
            float timeToApex = Mathf.Sqrt(2 * apexHeight / gravity);
            float timeToTargetFromApex = Mathf.Sqrt(2 * Mathf.Abs(dy - apexHeight) / gravity);
            float totalFlightTime = timeToApex + timeToTargetFromApex;

            // Calculate the initial speed in x-direction
            float initialSpeedX = dx / totalFlightTime;

            // Calculate the force, which is mass times acceleration
            float forceX = initialSpeedX * rb.mass / Time.fixedDeltaTime;
            float forceY = initialSpeedY * rb.mass / Time.fixedDeltaTime;

            // Return the force as a Vector2
            return new Vector2(forceX, forceY);
        }


        public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier = 0.0F,
            ForceMode2D mode = ForceMode2D.Force)
        {
            var explosionDir = rb.position - explosionPosition;
            var explosionDistance = explosionDir.magnitude;

            // Normalize without computing magnitude again
            if (upwardsModifier == 0)
                explosionDir /= explosionDistance;
            else
            {
                // From Rigidbody.AddExplosionForce doc:
                // If you pass a non-zero value for the upwardsModifier parameter, the direction
                // will be modified by subtracting that value from the Y component of the centre point.
                explosionDir.y += upwardsModifier;
                explosionDir.Normalize();
            }

            rb.AddForce(Mathf.Lerp(0, explosionForce, (1 - explosionDistance)) * explosionDir, mode);
        }

        public static RaycastHit2D CreateOffsetRaycast2D(Vector2 nowpos, Vector2 offset, Vector2 diraction, float length, LayerMask layer, int Hitcolor = ColorInt32.red,
            int NohitColor = ColorInt32.green)
        {
            // 获得玩家当前坐标位置
            //Vector2 playerPosition = transform.position;
            // 生成玩家当前位置水平偏移的射线投射碰撞器

            // 如果于水平地面发生碰撞则显示红色，反之则显示绿色
            //Color rayColor = hit ? Color.red : Color.green;
            // 在Scene中动态打印投射出的光线
            RaycastHit2D hit = Physics2D.Raycast(nowpos + offset, diraction, length, layer);
            Debug.DrawRay(nowpos + offset, diraction * length, hit ? ColorInt32.Int2Color(Hitcolor) : ColorInt32.Int2Color(NohitColor));
            // 返回生成的检测器
            return hit;
        }

        public static RaycastHit CreateOffsetRaycast3D(Vector3 nowpos, Vector3 direction, float length, LayerMask layer, bool ShowLine)
        {
            // 获得玩家当前坐标位置
            //Vector2 playerPosition = transform.position;
            // 生成玩家当前位置水平偏移的射线投射碰撞器


            // 在Scene中动态打印投射出的光线
            RaycastHit hit;
            Ray ray = new Ray(nowpos, direction * length);
            Physics.Raycast(ray, out hit, length, layer);
            if (ShowLine)
                Debug.DrawRay(nowpos, direction * length, Color.yellow);
            // 返回生成的检测器
            return hit;
        }


        public static float GetAbs(float a)
        {
            return a >= 0 ? a : -a;
        }

        public static int GetAbs(int a)
        {
            return a >= 0 ? a : -a;
        }

        //获取面板上的旋转的正确数值
        public static Vector3 GetInspectorRotationValueMethod(Transform transform)
        {
            // 获取原生值
            System.Type transformType = transform.GetType();
            PropertyInfo m_propertyInfo_rotationOrder = transformType.GetProperty("rotationOrder", BindingFlags.Instance | BindingFlags.NonPublic);
            object m_OldRotationOrder = m_propertyInfo_rotationOrder.GetValue(transform, null);
            MethodInfo m_methodInfo_GetLocalEulerAngles = transformType.GetMethod("GetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
            object value = m_methodInfo_GetLocalEulerAngles.Invoke(transform, new object[] { m_OldRotationOrder });
            string temp = value.ToString();
            //将字符串第一个和最后一个去掉
            temp = temp.Remove(0, 1);
            temp = temp.Remove(temp.Length - 1, 1);
            //用‘，’号分割
            string[] tempVector3;
            tempVector3 = temp.Split(',');
            //将分割好的数据传给Vector3
            Vector3 vector3 = new Vector3(float.Parse(tempVector3[0]), float.Parse(tempVector3[1]), float.Parse(tempVector3[2]));
            return vector3;
        }

        public static void SmoothLookAt(Vector3 TargetPos, Transform SelfPos, float Rotate_Speed)
        {
            var targetRotation = Quaternion.LookRotation(TargetPos - SelfPos.position);
            SelfPos.rotation = Quaternion.Slerp(SelfPos.rotation, targetRotation, Rotate_Speed * Time.deltaTime);
        }

        public static bool ContainsPoint(Vector2[] polyPoints, Vector2 p)
        {
            int crossNum = 0;
            int vertexCount = polyPoints.Length;

            for (int i = 0; i < vertexCount; i++)
            {
                Vector2 v1 = polyPoints[i];
                Vector2 v2 = polyPoints[(i + 1) % vertexCount];

                if (((v1.y <= p.y) && (v2.y > p.y))
                    || ((v1.y > p.y) && (v2.y <= p.y)))
                {
                    if (p.x < v1.x + (p.y - v1.y) / (v2.y - v1.y) * (v2.x - v1.x))
                    {
                        crossNum += 1;
                    }
                }
            }

            if (crossNum % 2 == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        ///判断点是否在矩形之内
        public static bool ContainsPoint(Vector3[] polyPoints, Vector2 p)
        {
            int crossNum = 0;
            int vertexCount = polyPoints.Length;

            for (int i = 0; i < vertexCount; i++)
            {
                Vector2 v1 = polyPoints[i];
                Vector2 v2 = polyPoints[(i + 1) % vertexCount];

                if (((v1.y <= p.y) && (v2.y > p.y))
                    || ((v1.y > p.y) && (v2.y <= p.y)))
                {
                    if (p.x < v1.x + (p.y - v1.y) / (v2.y - v1.y) * (v2.x - v1.x))
                    {
                        crossNum += 1;
                    }
                }
            }

            if (crossNum % 2 == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 获取两点之间距离一定百分比的一个点
        /// </summary>
        /// <param name="start">起始点</param>
        /// <param name="end">结束点</param>
        /// <param name="distance">起始点到目标点距离百分比</param>
        /// <returns></returns>
        public static Vector3 GetBetweenPoint(Vector3 start, Vector3 end, float percent = 0.5f)
        {
            Vector3 normal = (end - start).normalized;
            float distance = Vector3.Distance(start, end);
            return normal * (distance * percent) + start;
        }

        /// <summary>
        /// 获取两点之间一定距离的点
        /// </summary>
        /// <param name="start">起始点</param>
        /// <param name="end">结束点</param>
        /// <param name="distance">距离</param>
        /// <returns></returns>
        public static Vector3 GetBetweenPointDistance(Vector3 start, Vector3 end, float distance)
        {
            Vector3 normal = (end - start).normalized;
            return normal * distance + start;
        }

        /// <summary>
        /// 获取所有子transform
        /// </summary>
        /// <param name="parent">要获取的父物体</param>
        /// <param name="maxDepth">深度0是一层，1是两层</param>
        /// <returns></returns>
        public static List<Transform> GetAllChildrenDepth(Transform parent, int maxDepth)
        {
            List<Transform> result = new List<Transform>();
            GetChildrenRecursive(parent, result, 0, maxDepth);
            return result;
        }

        private static void GetChildrenRecursive(Transform current, List<Transform> result, int currentDepth, int maxDepth)
        {
            if (currentDepth > maxDepth) return;

            foreach (Transform child in current)
            {
                // 如果 child 是 RectTransform 类型，则将其转换为 RectTransform 并添加到结果中
                if (child is RectTransform rectTransform)
                {
                    result.Add(rectTransform);
                }
                else
                {
                    result.Add(child);
                }

                // 继续递归处理子物体
                GetChildrenRecursive(child, result, currentDepth + 1, maxDepth);
            }
        }

        public static List<Transform> GetAllChild(Transform transform)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform item in transform)
            {
                // 添加对 RectTransform 的检查
                if (item is RectTransform rectTransform)
                {
                    children.Add(rectTransform);
                }

                List<Transform> son = new List<Transform>();
                if (item.childCount > 0) son = GetAllChild(item);
                if (son.Count > 0)
                {
                    foreach (var obj in son)
                    {
                        children.Add(obj);
                    }
                }

                // 在添加 item 之前检查它是否已经作为 RectTransform 被添加
                if (!(item is RectTransform))
                {
                    children.Add(item);
                }
            }

            return children;
        }
    }

    public class VectorValue
    {
        public static readonly Vector3 Forward = new Vector3(0,0,1);
        public static readonly Vector3 Back = new Vector3(0,0,-1);
        public static readonly Vector3 Up = new Vector3(0,1,0);
    }

    public class ColorInt32
    {
        public const int cyan = 16777215;
        public const int clear = 0;
        public const int grey = 2139062271;
        public const int gray = 2139062271;
        public const int magenta = -16711681;
        public const int red = -16776961;
        public const int yellow = -1374977;
        public const int black = 255;
        public const int white = -1;
        public const int green = 16711935;
        public const int blue = 65535;

        public static Color32 Int2Color(int i)
        {
            byte[] result = new byte[4];
            result[0] = (byte)((i >> 24));
            result[1] = (byte)((i >> 16));
            result[2] = (byte)((i >> 8));
            result[3] = (byte)(i);
            return new Color32(result[0], result[1], result[2], result[3]);
        }

        public static int Color2Int(Color32 color)
        {
            byte[] result = new byte[4];
            result[0] = color.r;
            result[1] = color.g;
            result[2] = color.b;
            result[3] = color.a;
            return (int)(result[0] << 24 | result[1] << 16 | result[2] << 8 | result[3]);
        }
    }
    
    public static class MyArrayExpand
    {
        //清除所有
        public static void Clear<T>(this T[] array) where T : class
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = null;
            }
        }
    }
}
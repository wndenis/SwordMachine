using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class Extrapolations
{
    public class FloatExtrapolation
    {
        private List<double> xList;
        private List<double> yList;
        private int n;
        private List<double> a;
        
        public FloatExtrapolation(List<float> xList, List<float> yList)
        {
            if (xList.Count != yList.Count)
                throw new Exception("Lists must be the same size");
            
            if (xList.Count < 3 || yList.Count < 3)
                throw new Exception("Lists size must be at least 3");

            this.xList = xList.ConvertAll(x => (double)x);
            this.yList = yList.ConvertAll(x => (double)x);
            a = new List<double>();
            n = xList.Count;
            CalculateCoefficients();
        }

        private void CalculateCoefficients()
        {
            for (var i = 0; i < n; i++)
                a.Add(yList[i]);
            // a.InsertRange(0, yList); //todo: check this, may b better (seems not)

            for (var j = 1; j < n; j++)
            for (var i = n - 1; i > j - 1; i--)
                a[i] = (a[i] - a[i - 1]) / (xList[i] - xList[i - j]);
            n -= 1;

        }

        public float Extrapolate(float p)
        {
            var result = a[n];
            for (var i = n - 1; i > -1; i--)
                result = result * (p - xList[i]) + a[i];
            return (float)result;
        }
        
    }

    public class VectorExtrapolation
    {
        private FloatExtrapolation xExtrapolation;
        private FloatExtrapolation yExtrapolation;
        private FloatExtrapolation zExtrapolation;

        public VectorExtrapolation(List<float> timeList, List<Vector3> vectorList)
        {
            var xList = new List<float>();
            var yList = new List<float>();
            var zList = new List<float>();
            foreach (var elem in vectorList)
            {
                xList.Add(elem.x);
                yList.Add(elem.y);
                zList.Add(elem.z);
            }
            xExtrapolation = new FloatExtrapolation(timeList, xList);
            yExtrapolation = new FloatExtrapolation(timeList, yList);
            zExtrapolation = new FloatExtrapolation(timeList, zList);
        }

        public Vector3 Extrapolate(float t)
        {
            var result = new Vector3
            {
                x = xExtrapolation.Extrapolate(t),
                y = yExtrapolation.Extrapolate(t),
                z = zExtrapolation.Extrapolate(t)
            };
            return result;
        }
    }
}
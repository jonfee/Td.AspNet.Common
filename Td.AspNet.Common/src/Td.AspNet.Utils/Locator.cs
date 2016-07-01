using System;

namespace Td.AspNet.Utils
{
    /// <summary>
    /// 定位器
    /// </summary>
    public class Locator
    {
        /// <summary>
        /// 地球半径
        /// </summary>
        private const double EARTH_RADIUS = 6378.137;

        /// <summary>
        /// 根据当前坐标计算半径范围内的坐标
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <param name="raidus">米</param>
        /// <returns></returns>
        public static double[] GetAround(decimal lat, decimal lon, double raidus)
        {
            Double latitude = (double)lat;
            Double longitude = (double)lon;
            Double degree = (24901 * 1609) / 360.0;
            double raidusMile = raidus;
            Double dpmLat = 1 / degree;
            Double radiusLat = dpmLat * raidusMile;
            Double minLat = latitude - radiusLat;
            Double maxLat = latitude + radiusLat;
            Double mpdLng = degree * Math.Cos(latitude * (Math.PI / 180));
            Double dpmLng = 1 / mpdLng;
            Double radiusLng = dpmLng * raidusMile;
            Double minLng = longitude - radiusLng;
            Double maxLng = longitude + radiusLng;
            return new double[] { minLat, minLng, maxLat, maxLng };

        }
        
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double rad(double d)
        {
            return d * Math.PI / 180.0;
        }

        /// <summary>
        /// 根据经纬度计算距离 范围公里
        /// 例: 1.6126 => 1.6公里
        /// </summary>
        /// <param name="lat1">纬度1</param>
        /// <param name="lng1">经度1</param>
        /// <param name="lat2">经度2</param>
        /// <param name="lng2">经度2</param>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

        /// <summary>
        /// 根据经纬度计算距离 范围公里
        /// 例: 1.6126 => 1.6公里
        /// </summary>
        /// <param name="lat1">纬度1</param>
        /// <param name="lng1">经度1</param>
        /// <param name="lat2">经度2</param>
        /// <param name="lng2">经度2</param>
        /// <returns></returns>
        public static double GetDistance(float lat1, float lng1, float lat2, float lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

        /// <summary>
        /// 根据经纬度计算距离 范围公里
        /// 例: 1.6126 => 1.6公里
        /// </summary>
        /// <param name="lat1">纬度1</param>
        /// <param name="lng1">经度1</param>
        /// <param name="lat2">经度2</param>
        /// <param name="lng2">经度2</param>
        /// <returns></returns>
        public static double GetDistance(double lat1, double lng1, float lat2, float lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }

        /// <summary>
        /// 根据经纬度计算距离 范围公里
        /// 例: 1.6126 => 1.6公里
        /// </summary>
        /// <param name="lat1">纬度1</param>
        /// <param name="lng1">经度1</param>
        /// <param name="lat2">经度2</param>
        /// <param name="lng2">经度2</param>
        /// <returns></returns>
        public static double GetDistance(float lat1, float lng1, double lat2, double lng2)
        {
            double radLat1 = rad(lat1);
            double radLat2 = rad(lat2);
            double a = radLat1 - radLat2;
            double b = rad(lng1) - rad(lng2);
            double s = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin(a / 2), 2) +
             Math.Cos(radLat1) * Math.Cos(radLat2) * Math.Pow(Math.Sin(b / 2), 2)));
            s = s * EARTH_RADIUS;
            s = Math.Round(s * 10000) / 10000;
            return s;
        }
    }
}

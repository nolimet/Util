using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace NoUtil.Extensions
{
    public static class JsonExtensions
    {
        public static async Task<bool> ToPathAsync<T>(this T data, string path)
        {
            var file = new FileInfo(path);
            if (file.Directory is { Exists: false })
            {
                file.Directory.Create();
            }

            try
            {
                var json = JsonConvert.SerializeObject(data, Formatting.None);
                using (var writer = file.CreateText())
                {
                   await writer.WriteAsync(json);
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return false;

        }

        public static async Task<T> FromPathAsync<T>(this string path)
        {
            var file = new FileInfo(path);
            if (!file.Exists) return default;
            
            T result;
            try
            {
                string json;
                using (var reader = file.OpenText())
                {
                    json = await reader.ReadToEndAsync();
                }

                result = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                result = default;
                Debug.LogException(e);
            }

            return result;
        }

        public static bool ToPath<T>(this T data, string path)
        {
            var file = new FileInfo(path);
            if (file.Directory is { Exists: false })
            {
                file.Directory.Create();
            }

            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.None);
                using (var writer = file.CreateText())
                {
                    writer.Write(json);
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            return false;
        }
        
        public static bool FromPath<T>(this string path, out T data)
        {
            data = default;
            
            var file = new FileInfo(path);
            if (!file.Exists) return false;
            
            try
            {
                string json;
                using (var reader = file.OpenText())
                {
                    json = reader.ReadToEnd();
                }

                data = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                data = default;
                Debug.LogException(e);
            }

            return data != null;
        }
    }
}
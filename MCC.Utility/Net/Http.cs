﻿using System.Collections;
using System.IO;
using System.Net;
using System.Text;

namespace MCC.Utility.Net
{
    /// <summary>
    /// HTTPコマンドクラス
    /// </summary>
    public static class Http
    {
        /// <summary>
        /// Http:Getコマンドを実行します。
        /// </summary>
        /// <param name="url">GetするURLです。</param>
        /// <returns></returns>
        public static string Get(string url)
        {
            try
            {
                // ウェブリクエストを作成
                var request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";

                // レスポンスの取得
                using (var response = request.GetResponse() as HttpWebResponse)
                using (var stream = response.GetResponseStream())
                {
                    // ストリームの取得
                    if (stream != null)
                    {
                        return new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                    }
                }
            }
            catch
            {

            }
            // なし
            return string.Empty;
        }


        /// <summary>
        /// Http:Postコマンドを実行します。
        /// </summary>
        /// <param name="url">PostするURLです。</param>
        public static string Post(string url)
        {

            try
            {
                // ウェブリクエストを作成
                var request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                // レスポンスの取得
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    // ストリームの取得
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch
            {

            }
            // なし
            return string.Empty;
        }

        /// <summary>
        /// Http:Postコマンドを実行します。
        /// </summary>
        /// <param name="url">PostするURLです。</param>
        public static string Post(string url, Hashtable data)
        {
            try
            {
                var boundary = System.Environment.TickCount.ToString();

                // ウェブリクエストを作成
                var request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "POST";
                request.ContentType = $"multipart/form-data; boundary={boundary}";

                var postData = CreateBoundary(boundary, data);
                var encoded = Encoding.UTF8.GetBytes(postData);

                request.ContentLength = encoded.Length;

                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(encoded, 0, encoded.Length);
                }

                // レスポンスの取得
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    // ストリームの取得
                    using (var stream = response.GetResponseStream())
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch
            {

            }
            return string.Empty;
        }

        private static string CreateBoundary(string boundary, Hashtable data)
        {
            var postData = string.Empty;

            foreach (string value in data.Keys)
            {
                postData += $"--{boundary}\r\n";
                postData += $"Content-Disposition: form-data; name=\"{value}\"\r\n\r\n";
                postData += $"{data[value]}\r\n";
            }

            return postData += $"--{boundary}--";
        }
    }
}

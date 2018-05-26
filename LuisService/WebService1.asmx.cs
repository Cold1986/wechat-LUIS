using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;

namespace LuisService
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public Lusi GetLusiResponse(string message)
        {
            MakeRequest(message);

            return null;
        }

        [WebMethod]
        static async void MakeRequest(string message)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(message);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "b2199b7a91d34ed2a75f27f931aab7e3");

            var uri = "https://api.projectoxford.ai/luis/v1/application?id=9c342701-d6c7-4e6e-85a8-f7a0f03c5e5f&q=" + queryString;

            var response = await client.GetAsync(uri);

            string JSON = await response.Content.ReadAsStringAsync();

            Lusi ro = JsonHelper.Deserialize<Lusi>(JSON);

            return ;

        }
    }

    [DataContract]
    public class Intent
    {
        [DataMember]
        public string intent { get; set; }
        [DataMember]
        public double score { get; set; }
    }

    [DataContract]
    public class Resolution
    {
        [DataMember]
        public string date { get; set; }
    }

    [DataContract]
    public class Entity
    {
        [DataMember]
        public string entity { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public int startIndex { get; set; }
        [DataMember]
        public int endIndex { get; set; }
        [DataMember]
        public double score { get; set; }
        [DataMember]
        public Resolution resolution { get; set; }
    }

    [DataContract]
    public class Lusi
    {
        [DataMember]
        public string query { get; set; }
        [DataMember]
        public List<Intent> intents { get; set; }
        [DataMember]
        public List<Entity> entities { get; set; }
    }

    public class JsonHelper
    {
        /// <summary>
        /// 将JSON字符串反序列化成数据对象
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns>返回数据对象</returns>
        public static T Deserialize<T>(string json)
        {
            var _Bytes = Encoding.Unicode.GetBytes(json);
            using (MemoryStream _Stream = new MemoryStream(_Bytes))
            {
                var _Serializer = new DataContractJsonSerializer(typeof(T));
                return (T)_Serializer.ReadObject(_Stream);
            }
        }

        /// <summary>
        /// 将object序列化成JSON字符串 
        /// </summary>
        /// <param name="instance">被序列化对象</param>
        /// <returns>返回json字符串</returns>
        public static string Serialize(object instance)
        {
            using (MemoryStream _Stream = new MemoryStream())
            {
                var _Serializer = new DataContractJsonSerializer(instance.GetType());
                _Serializer.WriteObject(_Stream, instance);
                _Stream.Position = 0;
                using (StreamReader _Reader = new StreamReader(_Stream))
                { return _Reader.ReadToEnd(); }
            }
        }
    }
}

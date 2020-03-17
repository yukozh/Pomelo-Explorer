using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json;

namespace Pomelo.Explorer
{
    public static class AssemblyHelper
    {
        public static readonly SortedDictionary<string, Assembly> ExtensionAssemblyContainer = new SortedDictionary<string, Assembly>();
        public static ExtensionDescriptor GetExtensionDescriptor(Assembly assembly)
        {
            using (var stream = assembly.GetManifestResourceStream(assembly.GetManifestResourceNames().First(x => x.EndsWith("pomelo.json"))))
            using (var sr = new StreamReader(stream))
            {
                var json = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<ExtensionDescriptor>(json);
            }
        }

        public static void RegisterExtension(Assembly assembly) => ExtensionAssemblyContainer.Add(GetExtensionDescriptor(assembly).Id.ToLower(), assembly);

        public static Assembly GetAssemblyById(string id) => ExtensionAssemblyContainer.ContainsKey(id) ? ExtensionAssemblyContainer[id] : null;

        public static string GetAssemblyIdFromUrl(string url)
        {
            var endpoint = url;
            if (!endpoint.StartsWith("/static"))
            {
                return null;
            }
            endpoint = endpoint.Substring(endpoint.IndexOf('/', 1));
            var id = endpoint.Substring(1, endpoint.IndexOf('/', 1) - 1).ToLower();
            return id;
        }

        public static string ConvertUrlToResourcePath(string url)
        {
            var endpoint = url;
            endpoint = endpoint.Substring(endpoint.IndexOf('/'));
            if (!endpoint.StartsWith("/static"))
            {
                return null;
            }
            endpoint = endpoint.Substring(endpoint.IndexOf('/', 1));
            var id = endpoint.Substring(1, endpoint.IndexOf('/', 1) - 1).ToLower();
            if (!ExtensionAssemblyContainer.ContainsKey(id))
            {
                return null;
            }

            endpoint = endpoint.Substring(endpoint.IndexOf('/', 1));
            var assembly = ExtensionAssemblyContainer[id];
            return assembly.GetName().Name + endpoint.Replace("/", ".");
        }
    }
}

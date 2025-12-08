using k8s;
using YamlDotNet.Serialization;

namespace K8sManager.Api.Infrastructure
{
    public class K8sClientFactory
    {
        private readonly string _kubeconfigPath;
        private string? _context;

        public K8sClientFactory(string kubeconfigPath, string? context = null)
        {
            _kubeconfigPath = Environment.ExpandEnvironmentVariables(kubeconfigPath);
            _context = context;
        }

        public IKubernetes Create()
        {
            var cfg = string.IsNullOrWhiteSpace(_context)
                ? KubernetesClientConfiguration.BuildConfigFromConfigFile(_kubeconfigPath)
                : KubernetesClientConfiguration.BuildConfigFromConfigFile(_kubeconfigPath, _context);
            return new Kubernetes(cfg);
        }

        public void SetContext(string context)
        {
            _context = context;
        }

        public string? GetCurrentContext()
        {
            return _context;
        }

        public List<string> GetAvailableContexts()
        {
            var logFile = Path.Combine(AppContext.BaseDirectory, "k8s-debug.log");
            try
            {
                var kubeConfigPath = Environment.ExpandEnvironmentVariables(_kubeconfigPath);
                File.AppendAllText(logFile, $"[{DateTime.Now}] Kubeconfig path: {kubeConfigPath}\n");

                if (!File.Exists(kubeConfigPath))
                {
                    File.AppendAllText(logFile, $"[{DateTime.Now}] File does not exist!\n");
                    return new List<string>();
                }

                var yaml = File.ReadAllText(kubeConfigPath);
                File.AppendAllText(logFile, $"[{DateTime.Now}] YAML loaded, length: {yaml.Length}\n");

                var deserializer = new DeserializerBuilder().Build();
                var config = deserializer.Deserialize<Dictionary<string, object>>(yaml);
                File.AppendAllText(logFile, $"[{DateTime.Now}] Deserialized, keys: {string.Join(", ", config.Keys)}\n");

                if (config.ContainsKey("contexts") && config["contexts"] is List<object> contexts)
                {
                    File.AppendAllText(logFile, $"[{DateTime.Now}] Found {contexts.Count} contexts\n");
                    var contextNames = contexts
                        .OfType<Dictionary<object, object>>()
                        .Select(c => c.ContainsKey("name") ? c["name"]?.ToString() : null)
                        .Where(name => !string.IsNullOrEmpty(name))
                        .Select(name => name!)
                        .ToList();
                    File.AppendAllText(logFile, $"[{DateTime.Now}] Context names: {string.Join(", ", contextNames)}\n");
                    return contextNames;
                }
                else
                {
                    File.AppendAllText(logFile, $"[{DateTime.Now}] No contexts key found or wrong type\n");
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(logFile, $"[{DateTime.Now}] ERROR: {ex.Message}\n");
                File.AppendAllText(logFile, $"[{DateTime.Now}] Stack: {ex.StackTrace}\n");
            }

            return new List<string>();
        }
    }
}

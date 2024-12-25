using YamlDotNet.Serialization;
using static FiveInARowWeb.DataCore;
using static FiveInARowWeb.FilePath;

namespace FiveInARowWeb {
    public class Config {
        /// <summary>
        /// 将配置数据保存至配置文件中
        /// </summary>
        internal static void SaveData() {
            ISerializer yamlS = new SerializerBuilder()
                    .Build();
            File.WriteAllText(configFile, yamlS.Serialize(config));
        }
        /// <summary>
        /// 读取数据文件并将数据写入实例中
        /// </summary>
        internal static void ReadData() {
            IDeserializer yamlD = new DeserializerBuilder()
                    .Build();
            config = yamlD.Deserialize<Config>(File.ReadAllText(configFile));
        }
        public class SettingC {
            
            private string[] passCode = ["0000", "1111"];
            /// <summary>
            /// 访问代码，可以设置多个，每个访问代码代表一个房间，设置多少个访问代码代表可支持多少个房间
            /// </summary>
            public string[] PassCode {
                set => passCode = value;
                get => passCode;
            }
        }
        private SettingC setting = new();
        public SettingC Setting {
            set => setting = value;
            get => setting;
        }

        public class WebsiteC {
            
            private bool useXFFRequestHeader = false;
            /// <summary>
            /// 是否启用 X-Forwarded-For(XFF)请求标头
            /// </summary>
            public bool UseXFFRequestHeader {
                set => useXFFRequestHeader = value;
                get => useXFFRequestHeader;
            }
            public class UrlC {
                
                private bool useHttps = false;
                /// <summary>
                /// 使用https
                /// </summary>
                public bool UseHttps {
                    set => useHttps = value;
                    get => useHttps;
                }
               
                private string addr = "*";
                /// <summary>
                /// 本地监听地址
                /// </summary>
                public string Addr {
                    set => addr = value;
                    get => addr;
                }
                ///<summary>
                ///urlRoot不能只包含单独的斜杠，这里只是起到占位的作用，到引用的时候单独的斜杠会被去掉。<br/>
                ///在包含内容的时候，urlRoot前面必须包含斜杠，末尾不能含有斜杠
                ///</summary>
                private string urlRoot = "/";
                /// <summary>
                /// 主机地址后的URL地址
                /// </summary>
                public string UrlRoot {
                    set {//格式化
                        string urlRoot_;
                        if (value == "/") urlRoot_ = "";//value不能只包含单独的斜杠
                        else if (value == "") { urlRoot_ = value; }//如果为空则直接输出
                        else {
                            if (value[..1] == "/")
                                urlRoot_ = value;
                            else//如果开头没有斜杠则加上斜杠
                                urlRoot_ = "/" + value;
                            if (urlRoot_.Substring(urlRoot_.Length - 1, 1) == "/")//如果末尾包含斜杠则去掉
                                urlRoot_ = urlRoot_[..(urlRoot_.Length - 1)];
                        }
                        urlRoot = urlRoot_;
                    }
                    get => urlRoot;
                }
                private string port = "80";
                /// <summary>
                /// 监听端口
                /// </summary>
                public string Port {
                    set => port = value;
                    get => port;
                }
                internal string Get() {
                    string head;

                    if (useHttps)
                        head = "https";
                    else
                        head = "http";

                    return $"{head}://{addr}:{port}";
                }
            }
            private UrlC url = new();
            public UrlC Url {
                set => url = value;
                get => url;
            }
        }
        private WebsiteC website = new();
        public WebsiteC Website {
            set => website = value;
            get => website;
        }

        
        private byte debugMode = 0;
        /// <summary>
        /// 调试模式，用于输出调试信息等<br/>
        /// 用等级表示，等级越高，调试输出内容就越详细<br/>
        /// 目前分为1-5级
        /// </summary>
        public byte DebugMode {
            get => debugMode;
            set => debugMode = value;
        }
        private bool updateConfig = true;
        /// <summary>
        /// 设置为true后将在下次启动时更新一次配置文件
        /// </summary>
        public bool UpdateConfig {
            get => updateConfig;
            set => updateConfig = value;
        }
    }
}

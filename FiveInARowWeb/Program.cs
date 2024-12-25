using System.Xml;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using static FiveInARowWeb.FilePath;
using static FiveInARowWeb.DataCore;
using Force.DeepCloner;

namespace FiveInARowWeb {
	public struct About {
        public const string version = "1.3.1.20241210_beta";
        public const string version_addV = $"V{version}";
        public const string copyright = "Copyright (C) 2024 Hgnim, All rights reserved.";
		public const string githubUrl = "https://github.com/Hgnim/FiveInARowWeb";
		public const string githubUrl_addHead = $"Github: {githubUrl}";
    }
	public class Program {
        public static void Main(string[] args) {
			Console.WriteLine(
@$"欢迎使用FiveInARowWeb服务端。
版本: {About.version}
{About.copyright}
{About.githubUrl_addHead}"
                );
			try {
				if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
				if (!File.Exists(configFile)) {
					Config.SaveData();
				}
				else {
					Config.ReadData();
				}
				if (config.UpdateConfig == true) {
					config.UpdateConfig = false;
					Config.SaveData();
					Console.WriteLine("配置文件已更新，已退出服务端");
					return;
				}
			} catch { Console.WriteLine("处理配置文件时出现错误!");return; }

			DataCore.chessGame=new ChessGame[DataCore.config.Setting.PassCode.Length];
			for (int i = 0; i < chessGame.Length; i++) 
				chessGame[i] = new();

            var builder = WebApplication.CreateBuilder(args);
			builder.Services.AddControllersWithViews();

			builder.WebHost.UseUrls(config.Website.Url.Get());
			builder.Services.AddHttpContextAccessor();
			builder.Services.AddSession();

			var app = builder.Build();

			app.UsePathBase(config.Website.Url.UrlRoot);
			app.UseSession();

			/*// Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }*/
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseAuthorization();
			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=StartPage}/{action=Index}"); // /{id?}");
			app.Run();
		}
	}
	
	


}

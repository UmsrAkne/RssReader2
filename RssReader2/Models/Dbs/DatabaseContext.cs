using System;
using System.IO;
using System.Windows;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace RssReader2.Models.Dbs
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Feed> Feeds { get; set; }

        public DbSet<WebSite> WebSites { get; set; }

        public DbSet<WebSiteGroup> WebSiteGroups { get; set; }

        public DbSet<NgWord> NgWords { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var baseDir = AppContext.BaseDirectory;
            var dbFilePath = Path.Combine(baseDir, "db.sqlite");

            if (!File.Exists(dbFilePath))
            {
                #if DEBUG
                Console.WriteLine("[DB] File not found. Creating new database for debug purposes.");
                using var connection = new SqliteConnection($"Data Source={dbFilePath}");
                connection.Open();
                connection.Close();
                #else

                Console.Error.WriteLine("[ERROR] Required database file was not found at:");
                Console.Error.WriteLine($"         {dbFilePath}");
                Console.Error.WriteLine("This application cannot run without an existing database.");
                Console.Error.WriteLine("Please ensure the DB file is present before launching.");

                MessageBox.Show(
                    "起動に必要なデータベースファイルが見つかりませんでした。\n" +
                    "アプリケーションを終了します。",
                    "データベースエラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Environment.Exit(1);
                #endif
            }

            var connectionString = new SqliteConnectionStringBuilder { DataSource = dbFilePath, }.ToString();
            optionsBuilder.UseSqlite(new SqliteConnection(connectionString));

            // フレームワークによって発行された SQL を確認するためのコード
            // 上記の optionsBuilder.UseSqlite をコメントアウトして、代わりにこちらのコメントアウトを外して使います。
            // optionsBuilder.UseSqlite(new SqliteConnection(connectionString))
            //     .LogTo(System.Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
            //     .EnableSensitiveDataLogging();
        }
    }
}
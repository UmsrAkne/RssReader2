using System.IO;
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
            const string dbFileName = "db.sqlite";
            if (!File.Exists(dbFileName))
            {
                using var connection = new SqliteConnection($"Data Source={dbFileName}");
                connection.Open();
                connection.Close();
            }

            var connectionString = new SqliteConnectionStringBuilder { DataSource = dbFileName, }.ToString();

            optionsBuilder.UseSqlite(new SqliteConnection(connectionString));

            // フレームワークによって発行された SQL を確認するためのコード
            // 上記の optionsBuilder.UseSqlite をコメントアウトして、代わりにこちらのコメントアウトを外して使います。
            // optionsBuilder.UseSqlite(new SqliteConnection(connectionString))
            //     .LogTo(System.Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
            //     .EnableSensitiveDataLogging();
        }
    }
}
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using System.IO;
using System.Data;

namespace OrmLiteEx
{
    public class ReliableSqlServerOrmLiteDialectProvider : SqlServerOrmLiteDialectProvider
    {
        public ReliableSqlServerOrmLiteDialectProvider() : base()
        {this.
            RetryPolicy = RetryPolicy.DefaultExponential;
        }

        public RetryPolicy RetryPolicy { get; set; }

        public override IDbConnection CreateConnection(string connectionString, Dictionary<string, string> options)
        {
            var isFullConnectionString = connectionString.Contains(";");

            if (!isFullConnectionString)
            {
                var filePath = connectionString;

                var filePathWithExt = filePath.ToLower().EndsWith(".mdf")
                    ? filePath
                    : filePath + ".mdf";

                var fileName = Path.GetFileName(filePathWithExt);
                var dbName = fileName.Substring(0, fileName.Length - ".mdf".Length);

                connectionString = string.Format(
                @"Data Source=.\SQLEXPRESS;AttachDbFilename={0};Initial Catalog={1};Integrated Security=True;User Instance=True;",
                    filePathWithExt, dbName);
            }

            if (options != null)
            {
                foreach (var option in options)
                {
                    if (option.Key.ToLower() == "read only")
                    {
                        if (option.Value.ToLower() == "true")
                        {
                            connectionString += "Mode = Read Only;";
                        }
                        continue;
                    }
                    connectionString += option.Key + "=" + option.Value + ";";
                }
            }

            return new ReliableSqlConnection(connectionString, RetryPolicy);
        }
    }
}
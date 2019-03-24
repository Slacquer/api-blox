using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using APIBlox.NetCore.Extensions;
using APIBlox.NetCore.Options;

namespace APIBlox.NetCore
{
    internal class SqlDbContext
    {
        private SqlServerOptions _options;
        private List<string> _esTables = new List<string>();

        public SqlDbContext(SqlServerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task SqlCnn<TModel>(Action<SqlConnection> cb, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = GetCnn(typeof(TModel).Name))
            {
                await connection.OpenAsync(cancellationToken);

                cb(connection);
            }
        }

        private SqlConnection GetCnn(string name)
        {
            if (!_esTables.Contains(name))
                EnsureTableExists(name);

            return new SqlConnection(_options.CnnString);
        }

        private void EnsureTableExists(string tableName)
        {
            var myConn = new SqlConnection(_options.CnnString);

            var sqlCreateTable =
$@"IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[{tableName}EventStore]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[{tableName}EventStore](
	[Id] [nvarchar](255) NOT NULL,
	[DocumentType] [int] NOT NULL,
	[StreamId] [nvarchar](255) NOT NULL,
	[Version] [bigint] NOT NULL,
	[TimeStamp] [bigint] NOT NULL,
	[SortOrder] [decimal](18, 2) NOT NULL,
	[DataType] [nvarchar](1024) NOT NULL,
	[Data] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_{tableName}EventStore] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END";

            var sqlCreateIndex = $@"IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[{tableName}EventStore]') AND name = N'IX_{tableName}EventStore_StreamId')
CREATE NONCLUSTERED INDEX [IX_{tableName}EventStore_StreamId] ON [dbo].[{tableName}EventStore]
(
	[StreamId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";

            try
            {
                myConn.Open();

                var createTable = new SqlCommand(sqlCreateTable, myConn);
                createTable.ExecuteNonQuery();

                var createIndex = new SqlCommand(sqlCreateIndex, myConn);
                createIndex.ExecuteNonQuery();

            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                    myConn.Close();
            }
        }
    }
}

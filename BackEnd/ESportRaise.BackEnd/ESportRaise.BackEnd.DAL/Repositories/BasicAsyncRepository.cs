using ESportRaise.BackEnd.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace ESportRaise.BackEnd.DAL.Repositories
{
    public abstract class BasicAsyncRepository<T> : IAsyncRepository<T>, IDisposable
        where T : class
    {
        private static string tableName;

        protected SqlConnection db;

        private bool isDisposed = false;

        static BasicAsyncRepository()
        {
            tableName = typeof(T).Name;
        }

        public BasicAsyncRepository(SqlConnection sqlConnection)
        {
            db = sqlConnection;
            db.Open();
        }

        protected abstract Func<SqlDataReader, T> SelectMapper { get; }

        protected abstract Func<T, object[]> InsertValues { get; }

        protected abstract Func<T, TablePropertyValuePair[]> UpdatePropertiesAndValuesExtractor { get; }

        protected abstract TablePropertyExtractor UpdatePredicatePropertyEqualsValue { get; }

        public async Task DeleteAsync(int id)
        {
            var deleteCommand = db.CreateCommand();
            deleteCommand.CommandText = $"DELETE FROM {tableName} WHERE Id = @id";
            deleteCommand.Parameters.AddWithValue("@id", id);
            await deleteCommand.ExecuteNonQueryAsync();
        }

        public async Task<T> SelectAsync(int id)
        {
            var selectCommand = db.CreateCommand();
            selectCommand.CommandText = $"SELECT * FROM {tableName} WHERE Id = @id";
            selectCommand.Parameters.AddWithValue("@id", id);
            using (var reader = await selectCommand.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return SelectMapper(reader);
                }
                return null;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var selectCommand = db.CreateCommand();
            selectCommand.CommandText = $"SELECT * FROM {tableName}";
            using (var reader = await selectCommand.ExecuteReaderAsync())
            {
                var items = new List<T>();
                while (await reader.ReadAsync())
                {
                    items.Add(SelectMapper(reader));
                }
                return items;
            }
        }

        public async Task CreateAsync(T item)
        {
            var values = InsertValues(item);
            var insertCommand = db.CreateCommand();
            insertCommand.CommandText = GenerateInsertCommandOfValues(values);

            for (int i = 0; i < values.Length; i++)
            {
                insertCommand.Parameters.AddWithValue($"@{i}", values[i]);
            }

            await insertCommand.ExecuteNonQueryAsync();
        }

        private static string GenerateInsertCommandOfValues(object[] values)
        {
            var sb = new StringBuilder($"INSERT INTO {tableName} VALUES(");
            for (int i = 0; i < values.Length; i++)
            {
                if (i != 0)
                {
                    sb.Append(',');
                }
                sb.Append('@');
                sb.Append(i);
            }
            sb.Append(')');
            return sb.ToString();
        }

        public async Task UpdateAsync(T item)
        {
            var fieldsAndValues = UpdatePropertiesAndValuesExtractor(item);
            var updateCommand = db.CreateCommand();
            updateCommand.CommandText = GenerateUpdateCommandOfPropertiesAndValues(fieldsAndValues);

            for (int i = 0; i < fieldsAndValues.Length; i++)
            {
                updateCommand.Parameters.AddWithValue($"@{i}", fieldsAndValues[i].PropertyValue);
            }

            updateCommand.Parameters.AddWithValue($"@id", UpdatePredicatePropertyEqualsValue.PropertyExtractor(item));

            await updateCommand.ExecuteNonQueryAsync();
        }

        private string GenerateUpdateCommandOfPropertiesAndValues(TablePropertyValuePair[] propertiesAndValues)
        {
            var sb = new StringBuilder($"UPDATE {tableName} SET ");
            for (int i = 0; i < propertiesAndValues.Length; i++)
            {
                if (i != 0)
                {
                    sb.Append(',');
                }
                sb.Append(propertiesAndValues[i].PropertyName);
                sb.Append('=');
                sb.Append($"@{i}");
            }
            sb.Append(" WHERE ");
            sb.Append(UpdatePredicatePropertyEqualsValue.PropertyName);
            sb.Append("=@id");
            return sb.ToString();
        }

        protected struct TablePropertyValuePair
        {
            public string PropertyName;

            public object PropertyValue;

            public TablePropertyValuePair(string propertyName, object propertyValue)
            {
                PropertyName = propertyName;
                PropertyValue = propertyValue;
            }
        }

        protected struct TablePropertyExtractor
        {
            public string PropertyName;

            public Func<T, object> PropertyExtractor;

            public TablePropertyExtractor(string propertyName, Func<T, object> propertyExtractor)
            {
                PropertyName = propertyName;
                PropertyExtractor = propertyExtractor;
            }
        }

        #region IDisposable Pattern

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    db.Close();
                }

                isDisposed = true;
            }
        }

        ~BasicAsyncRepository()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

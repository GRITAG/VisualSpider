using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSEngine.Data;

namespace VSEngine.Integration
{
    public abstract class DBAccess
    {
        public abstract void ConnectDB(string path);

        public abstract string StoreNavUnit(NavUnit unit);

        public abstract void StoreResolvedNavUnit(ResolvedNavUnit unit, Config cfg);

        public abstract List<NavUnit> RetriveUnitSet(int max);

        public abstract List<NavUnit> RetriveUnits();

        public abstract int NavUnitCount();

        public abstract int ResolvedNavUnitCount();

        public abstract Dictionary<string, byte[]> GetRawImages();

        //public int NavUnitCount()
        //{
        //    SQLiteCommand countCall = new SQLiteCommand("select * from navunit", Connection);
        //    bool rowsReturned = countCall. ExecuteReader(System.Data.CommandBehavior.Default).HasRows; //ExecuteNonQuery();
        //}

        public abstract void CloseDB();

        public static byte[] GetBytes(SQLiteDataReader reader)
        {
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                while ((bytesRead = reader.GetBytes(0, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bytesRead;
                }
                return stream.ToArray();
            }
        }
    }
}

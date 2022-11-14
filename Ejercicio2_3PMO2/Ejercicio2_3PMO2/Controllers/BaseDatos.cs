using Ejercicio2_3PMO2.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio2_3PMO2.Controllers
{
    public class BaseDatos
    {
        readonly SQLiteAsyncConnection db;
        public BaseDatos(String path)
        {
            db = new SQLiteAsyncConnection(path);
            db.CreateTableAsync<Audio>().Wait();
        }
        public Task<List<Audio>> GetListAudios()
        {
            return db.Table<Audio>().ToListAsync();
        }
        public Task<Audio> GetAudiosporId(int id)
        {
            return db.Table<Audio>()
                .Where(i => i.id == id)
                .FirstOrDefaultAsync();
        }
        public Task<int> guardaAudios(Audio mp3)
        {
            return mp3.id != 0 ? db.UpdateAsync(mp3) : db.InsertAsync(mp3);
        }
        public Task<int> borrarAudios(Audio mp3)
        {
            return db.DeleteAsync(mp3);
        }
    }
}

using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ejercicio2_3PMO2.Models
{
    public class Audio
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public DateTime fecha { get; set; }
        public string path { get; set; }
        [MaxLength(50)]
        public string descripcion { get; set; }
    }
}

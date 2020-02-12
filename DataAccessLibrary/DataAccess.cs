using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using Microsoft.Data.Sqlite;
using Windows.Storage;

namespace DataAccessLibrary
{
    public class DataAccess
    {

        public static void AddVocabulary(string inputText)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "vocabulary.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                insertCommand.CommandText = "INSERT INTO Vocabulary (Word) VALUES (@Entry);";
                insertCommand.Parameters.AddWithValue("@Entry", inputText);
                insertCommand.ExecuteReader();

                db.Close();
            }
        }

        public static void UpdateVocabulary(Vocabulary item)
        {
            //try
            //{
                string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "vocabulary.db");
                using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
                {
                    db.Open();
                    SqliteCommand updateCommand = new SqliteCommand();
                    updateCommand.Connection = db;

                    // Use parameterized query to prevent SQL injection attacks
                    updateCommand.CommandText = "UPDATE Vocabulary SET Type = @Type, Ipa = @Ipa, Translate = @Translate WHERE Id = @Id";
                    
                    updateCommand.Parameters.Add("@Id", SqliteType.Text).Value = item.Id;
                    updateCommand.Parameters.Add("@Type", SqliteType.Text).Value = item.Type;
                    updateCommand.Parameters.Add("@Ipa", SqliteType.Text).Value = item.Ipa;
                    updateCommand.Parameters.Add("@Translate", SqliteType.Text).Value = item.Translate;

                    updateCommand.ExecuteNonQuery();

                    db.Close();
                }
            //} catch (Exception ex)
            //{
            //    Debug.WriteLine(ex.Message);
            //}
        }


        public static void UpdatePlayURL(Vocabulary item)
        {
            //try
            //{
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "vocabulary.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();
                SqliteCommand updateCommand = new SqliteCommand();
                updateCommand.Connection = db;

                // Use parameterized query to prevent SQL injection attacks
                updateCommand.CommandText = "UPDATE Vocabulary SET PlayURL = @PlayURL WHERE Id = @Id";
                updateCommand.Parameters.Add("@Id", SqliteType.Integer).Value = item.Id;
                updateCommand.Parameters.Add("@PlayURL", SqliteType.Text).Value = item.PlayURL;
                updateCommand.ExecuteNonQuery();

                db.Close();
            }
            //} catch (Exception ex)
            //{
            //    Debug.WriteLine(ex.Message);
            //}
        }


        public static Vocabulary GetVocabularyById(int Id)
        {
            List<Vocabulary> entries = new List<Vocabulary>();

            Vocabulary _item = new Vocabulary();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "vocabulary.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT * from Vocabulary WHERE Id = " + Id, db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    _item.Id = int.Parse(query.GetString(0));
                    _item.Word = query.IsDBNull(1) ? "" : query.GetString(1);
                    _item.Type = query.IsDBNull(2) ? "" : query.GetString(2);
                    _item.Ipa = query.IsDBNull(3) ? "" : query.GetString(3);
                    _item.Translate = query.IsDBNull(4) ? "" : query.GetString(4);
                    _item.PlayURL = query.IsDBNull(5) ? "" : query.GetString(5);
                }
                db.Close();
            }

            return _item;
        }



        public static List<Vocabulary> GetListVocabulary()
        {
            List<Vocabulary> entries = new List<Vocabulary>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "vocabulary.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT * from Vocabulary WHERE Translate IS NULL", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    Vocabulary _item = new Vocabulary();

                    _item.Id = int.Parse(query.GetString(0));
                    _item.Word = query.IsDBNull(1) ? "" : query.GetString(1);
                    _item.Type = query.IsDBNull(2) ? "" : query.GetString(2);
                    _item.Ipa = query.IsDBNull(3) ? "" : query.GetString(3);
                    _item.Translate = query.IsDBNull(4) ? "" : query.GetString(4);
                    _item.PlayURL = query.IsDBNull(5) ? "" : query.GetString(5);
                    
                    entries.Add(_item);
                }

                db.Close();
            }

            return entries;
        }


        public static List<Vocabulary> GetListVocabularyToTranslate()
        {
            List<Vocabulary> entries = new List<Vocabulary>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "vocabulary.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT * from Vocabulary WHERE Translate IS NULL OR Translate = ''", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    Vocabulary _item = new Vocabulary();

                    _item.Id = int.Parse(query.GetString(0));
                    _item.Word = query.IsDBNull(1) ? "" : query.GetString(1);
                    _item.Type = query.IsDBNull(2) ? "" : query.GetString(2);
                    _item.Ipa = query.IsDBNull(3) ? "" : query.GetString(3);
                    _item.Translate = query.IsDBNull(4) ? "" : query.GetString(4);
                    _item.PlayURL = query.IsDBNull(5) ? "" : query.GetString(5);

                    entries.Add(_item);
                }

                db.Close();
            }

            return entries;
        }


        public static List<Vocabulary> GetListVocabularyToGetPlayURL()
        {
            List<Vocabulary> entries = new List<Vocabulary>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "vocabulary.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT * from Vocabulary WHERE PlayURL IS NULL OR Translate = ''", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    Vocabulary _item = new Vocabulary();

                    _item.Id = int.Parse(query.GetString(0));
                    _item.Word = query.IsDBNull(1) ? "" : query.GetString(1);
                    _item.Type = query.IsDBNull(2) ? "" : query.GetString(2);
                    _item.Ipa = query.IsDBNull(3) ? "" : query.GetString(3);
                    _item.Translate = query.IsDBNull(4) ? "" : query.GetString(4);
                    _item.PlayURL = query.IsDBNull(5) ? "" : query.GetString(5);

                    entries.Add(_item);
                }

                db.Close();
            }

            return entries;
        }
    }

    

    public class Dictionary
    {
        public int DictionaryId { get; set; }
        public string Name { get; set; }

        public List<Vocabulary> Words { get; } = new List<Vocabulary>();
    }

    public class Vocabulary
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public string Type { get; set; }
        public string Ipa { get; set; }
        public string Translate { get; set; }
        public string PlayURL { get; set; }

        public Dictionary Dictionary { get; set; }
    }
}

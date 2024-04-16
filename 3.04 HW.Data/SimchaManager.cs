using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3._04_HW.Data
{
    public class SimchaManager
    {
        private readonly string _connectionString;

        public SimchaManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Contributor> GetContributors()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributors";
            connection.Open();

            var reader = cmd.ExecuteReader();
            List<Contributor> contributors = new();

            while (reader.Read())
            {
                contributors.Add(new()
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Cell = (decimal)reader["Cell"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"]
                });
            };
            return contributors;
        }

        public Contributor GetContributorById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributors WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();

            return !reader.Read() ? null : new()
            {
                Id = (int)reader["Id"],
                FirstName = (string)reader["FirstName"],
                LastName = (string)reader["LastName"],
                Cell = (decimal)reader["Cell"],
                AlwaysInclude = (bool)reader["AlwaysInclude"]
            };

        }

        public int GetContributorCount()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Count(*) FROM Contributors";
            connection.Open();
            return (int)cmd.ExecuteScalar();
        }

        public void AddContributor(Contributor c, Deposit d)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Contributors (FirstName, LastName, Cell, AlwaysInclude) " +
                              "VALUES (@first, @last, @cell, @include) " +
                              "SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@first", c.FirstName);
            cmd.Parameters.AddWithValue("@last", c.LastName);
            cmd.Parameters.AddWithValue("@cell", c.Cell);
            cmd.Parameters.AddWithValue("@include", c.AlwaysInclude);
            connection.Open();
            var id = (int)(decimal)cmd.ExecuteScalar();
            AddDeposit(new Deposit
            {
                ContributorId = id,
                Date = d.Date,
                Amount = d.Amount,
                Action = "Deposit"
            });
        }

        public int GetContributorCountForSimcha(int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(ContributorsId) FROM Contributions c " +
                              "WHERE c.SimchaId = @simId";
            cmd.Parameters.AddWithValue("@simId", simchaId);
            connection.Open();
            return (int)cmd.ExecuteScalar();
        }

        public decimal GetDepositForContributor(int contribId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT SUM(Amount) FROM Deposit WHERE ContributorId = @contribId";
            cmd.Parameters.AddWithValue("@contribId", contribId);
            connection.Open();
            var deposits = cmd.ExecuteScalar();
            return deposits != DBNull.Value ? (decimal)deposits : 0;
        }

        public decimal GetContributionTotalForContributor(int contribId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT SUM(Amount) FROM Contributions WHERE ContributorsId = @contribId";
            cmd.Parameters.AddWithValue("contribId", contribId);
            connection.Open();
            var contributions = cmd.ExecuteScalar();
            return contributions != DBNull.Value ? (decimal)contributions : 0;
        }

        public decimal GetContributionTotal()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT SUM(Amount) FROM Contributions";
            connection.Open();
            var contributions = cmd.ExecuteScalar();
            return contributions != DBNull.Value ? (decimal)contributions : 0;
        }

        public decimal GetDepositTotal()
        {

            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT SUM(Amount) FROM Deposit";
            connection.Open();
            var deposits = cmd.ExecuteScalar();
            return deposits != DBNull.Value ? (decimal)deposits : 0;
        }

        public List<Transaction> GetTransactionsForContributor(int contribId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT s.SimchaName as 'Action', s.Date, c.Amount FROM Contributions c " +
                              "JOIN Simcha s " +
                              "ON c.SimchaId = s.Id " +
                              "WHERE ContributorsId = @contribId";
            cmd.Parameters.AddWithValue("@contribId", contribId);
            connection.Open();

            var reader = cmd.ExecuteReader();
            List<Transaction> transactions = new();

            while (reader.Read())
            {
                transactions.Add(new()
                {
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"],
                    Description = (string)reader["Action"]
                });
            }
            return transactions;
        }

        public List<Transaction> GetDepositsListForContrib(int contribId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Deposit WHERE ContributorId = @contribId";
            cmd.Parameters.AddWithValue("@contribId", contribId);
            connection.Open();

            var reader = cmd.ExecuteReader();
            List<Transaction> transactions = new();

            while (reader.Read())
            {
                transactions.Add(new()
                {
                    Amount = (decimal)reader["Amount"],
                    Date = (DateTime)reader["Date"],
                    Description = (string)reader["Action"]
                });
            }
            return transactions;
        }

        public void EditContributor(Contributor c)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "UPDATE Contributors SET FirstName = @first, LastName = @last, Cell = @cell, AlwaysInclude = @alwaysInclude " +
                "WHERE id = @contribId";
            cmd.Parameters.AddWithValue("@first", c.FirstName);
            cmd.Parameters.AddWithValue("@last", c.LastName);
            cmd.Parameters.AddWithValue("@cell", c.Cell);
            cmd.Parameters.AddWithValue("@alwaysInclude", c.AlwaysInclude);
            cmd.Parameters.AddWithValue("@contribId", c.Id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public List<Simcha> GetSimchas()
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Simcha";
            connection.Open();
            var reader = cmd.ExecuteReader();

            List<Simcha> simchas = new();

            while (reader.Read())
            {
                simchas.Add(new()
                {
                    Id = (int)reader["Id"],
                    SimchaName = (string)reader["SimchaName"],
                    Date = (DateTime)reader["Date"]
                });
            }
            return simchas;
        }

        public decimal GetTotalForSimcha(int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT SUM(Amount) FROM Contributions c " +
                              "WHERE c.SimchaId = @simId";
            cmd.Parameters.AddWithValue("@simId", simchaId);
            connection.Open();
            var total = cmd.ExecuteScalar();
            return total != DBNull.Value ? (decimal)total : 0;
        }

        public Simcha GetSimchaById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Simcha WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();

            if (!reader.Read())
            {
                return null;
            }

            Simcha simcha = new()
            {
                Id = (int)reader["Id"],
                SimchaName = (string)reader["SimchaName"],
                Date = (DateTime)reader["Date"]
            };

            return simcha;
        }

        public int AddSimcha(Simcha s)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Simcha (SimchaName, Date) " +
                              "VALUES (@name, @date) " +
                              "SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@name", s.SimchaName);
            cmd.Parameters.AddWithValue("@date", s.Date);
            connection.Open();
            var simId = (int)(decimal)cmd.ExecuteScalar();
            return simId;
        }

        public decimal GetBalance(decimal deposits, decimal contributions)
        {
            return deposits - contributions;
        }

        public void AddDeposit(Deposit d)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Deposit (ContributorId, Amount, Date, Action) " +
                              "VALUES (@contribId, @amount, @date, 'Deposit')";
            cmd.Parameters.AddWithValue("@contribId", d.ContributorId);
            cmd.Parameters.AddWithValue("@amount", d.Amount);
            cmd.Parameters.AddWithValue("@date", d.Date);
            connection.Open();
            cmd.ExecuteNonQuery();
            
        }

        public List<Contributions> GetContributionsForSimcha(int simchaId, string simchaName)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM Contributions WHERE SimchaId = @simchaId";
            cmd.Parameters.AddWithValue("@simchaId", simchaId);
            connection.Open();

            List<Contributions> contributions = new();
            var reader = cmd.ExecuteReader();

            while(reader.Read())
            {
                contributions.Add(new()
                {
                    ContributorsId = (int)reader["ContributorsId"],
                    SimchaId = (int)reader["SimchaId"],
                    Action = simchaName,
                    Amount = (decimal)reader["Amount"]
                });
            }
            return contributions;
        }

        public void UpdateContributions(List<Contributor> contributors, int simchaId)
        {
            ClearContributions(simchaId);
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Contributions " +
                "VALUES (@simchaId, @contribId, @amount)";
            connection.Open();

            foreach(Contributor c in contributors)
            {
                if(c.Contributed)
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@simchaId", simchaId);
                    cmd.Parameters.AddWithValue("@amount", c.Amount);
                    cmd.Parameters.AddWithValue("@contribId", c.Id);
                    cmd.ExecuteNonQuery();
                };
            };

            
        }

        private void ClearContributions(int simId)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "DELETE FROM Contributions WHERE SimchaId = @id";
            cmd.Parameters.AddWithValue("@id", simId);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

        public void AddAutoContributions(List<Contributor> automaticContrib, int simchaId)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            foreach (Contributor c in automaticContrib)
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = $@"INSERT INTO Contributions 
                                VALUES ('{simchaId}', @contribId, '5')";
                cmd.Parameters.AddWithValue("@contribId", c.Id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}

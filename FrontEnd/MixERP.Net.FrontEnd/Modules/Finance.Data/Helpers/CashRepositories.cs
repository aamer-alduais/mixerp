﻿using MixERP.Net.Common;
using MixERP.Net.Common.Models.Office;
using MixERP.Net.DatabaseLayer.Helpers;
using MixERP.Net.DatabaseLayer.Office;
using MixERP.Net.DBFactory;
using Npgsql;
using System.Collections.ObjectModel;
using System.Data;

namespace MixERP.Net.Core.Modules.Finance.Data.Helpers
{
    public static class CashRepositories
    {
        public static CashRepository GetCashRepository(int? cashRepositoryId)
        {
            CashRepository cashRepository = new CashRepository();

            if (cashRepositoryId != null && cashRepositoryId != 0)
            {
                const string sql = "SELECT * FROM office.cash_repositories WHERE cash_repository_id=@CashRepositoryId;";
                using (NpgsqlCommand command = new NpgsqlCommand(sql))
                {
                    command.Parameters.AddWithValue("@CashRepositoryId", cashRepositoryId);

                    using (DataTable table = DbOperations.GetDataTable(command))
                    {
                        if (table != null)
                        {
                            if (table.Rows.Count.Equals(1))
                            {
                                cashRepository = GetCashRepository(table.Rows[0]);
                            }
                        }
                    }
                }
            }

            return cashRepository;
        }

        public static Collection<CashRepository> GetCashRepositories()
        {
            const string sql = "SELECT * FROM office.cash_repositories;";
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                return GetCashRepositories(DbOperations.GetDataTable(command));
            }
        }

        public static Collection<CashRepository> GetCashRepositories(int officeId)
        {
            const string sql = "SELECT * FROM office.cash_repositories WHERE office_id=@OfficeId;";
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                command.Parameters.AddWithValue("@OfficeId", officeId);
                return GetCashRepositories(DbOperations.GetDataTable(command));
            }
        }

        private static Collection<CashRepository> GetCashRepositories(DataTable table)
        {
            Collection<CashRepository> cashRepositoryCollection = new Collection<CashRepository>();

            if (table == null || table.Rows.Count.Equals(0))
            {
                return cashRepositoryCollection;
            }

            foreach (DataRow row in table.Rows)
            {
                if (row != null)
                {
                    CashRepository cashRepository = GetCashRepository(row);

                    cashRepositoryCollection.Add(cashRepository);
                }
            }

            return cashRepositoryCollection;
        }

        private static CashRepository GetCashRepository(DataRow row)
        {
            CashRepository cashRepository = new CashRepository();

            cashRepository.CashRepositoryId = Conversion.TryCastInteger(ConversionHelper.GetColumnValue(row, "cash_repository_id"));
            cashRepository.OfficeId = Conversion.TryCastInteger(ConversionHelper.GetColumnValue(row, "office_id"));
            cashRepository.Office = Offices.GetOffice(cashRepository.OfficeId);
            cashRepository.CashRepositoryCode = Conversion.TryCastString(ConversionHelper.GetColumnValue(row, "cash_repository_code"));
            cashRepository.CashRepositoryName = Conversion.TryCastString(ConversionHelper.GetColumnValue(row, "cash_repository_name"));
            cashRepository.ParentCashRepositoryId = Conversion.TryCastInteger(ConversionHelper.GetColumnValue(row, "parent_cash_repository_id"));
            cashRepository.ParentCashRepository = GetCashRepository(cashRepository.ParentCashRepositoryId);
            cashRepository.Description = Conversion.TryCastString(ConversionHelper.GetColumnValue(row, "description"));

            return cashRepository;
        }

        public static decimal GetBalance(int cashRepositoryId, string currencyCode)
        {
            const string sql = "SELECT transactions.get_cash_repository_balance(@CashRepositoryId, @CurrencyCode);";
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                command.Parameters.AddWithValue("@CashRepositoryId", cashRepositoryId);
                command.Parameters.AddWithValue("@CurrencyCode", currencyCode);
                return Conversion.TryCastDecimal(DbOperations.GetScalarValue(command));
            }
        }

        public static decimal GetBalance(int cashRepositoryId)
        {
            const string sql = "SELECT transactions.get_cash_repository_balance(@CashRepositoryId);";
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                command.Parameters.AddWithValue("@CashRepositoryId", cashRepositoryId);
                return Conversion.TryCastDecimal(DbOperations.GetScalarValue(command));
            }
        }

        public static decimal GetBalance(string cashRepositoryCode)
        {
            const string sql = "SELECT transactions.get_cash_repository_balance(office.get_cash_repository_id_by_cash_repository_code(@CashRepositoryCode));";
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                command.Parameters.AddWithValue("@CashRepositoryCode", cashRepositoryCode);
                return Conversion.TryCastDecimal(DbOperations.GetScalarValue(command));
            }
        }

        public static decimal GetBalance(string cashRepositoryCode, string currencyCode)
        {
            const string sql = "SELECT transactions.get_cash_repository_balance(office.get_cash_repository_id_by_cash_repository_code(@CashRepositoryCode), @CurrencyCode);";
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                command.Parameters.AddWithValue("@CashRepositoryCode", cashRepositoryCode);
                command.Parameters.AddWithValue("@CurrencyCode", currencyCode);
                return Conversion.TryCastDecimal(DbOperations.GetScalarValue(command));
            }
        }

        public static bool CashRepositoryCodeExists(string cashRepositoryCode)
        {
            const string sql = "SELECT 1 FROM office.cash_repositories WHERE cash_repository_code=@CashRepositoryCode;";
            using (NpgsqlCommand command = new NpgsqlCommand(sql))
            {
                command.Parameters.AddWithValue("@CashRepositoryCode", cashRepositoryCode);

                return DbOperations.GetDataTable(command).Rows.Count.Equals(1);
            }
        }
    }
}
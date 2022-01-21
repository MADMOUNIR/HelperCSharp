using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using RDLoreal.Framework.Contexts;

namespace IFRA_Batch
{
    /// <summary>
    /// The OracleHelper class is intended to encapsulate high performance, scalable best practices for
    /// common uses of OracleClient
    /// </summary>
    public  class OracleHelper
    {

        #region Fields

        /// <summary>
        /// Flag de control si le context est "dispose"
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Index indiquant le nombre d'ouvertures de connection
        /// </summary>
        private int _nbOpenConnection;

        /// <summary>
        /// Chaine de connexion
        /// </summary>
        private string _connectionString;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Chaine de connexion
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return this._connectionString;
            }
            set
            {
                this._connectionString = value;
                this.Connection = null;
                // création de la connexion
                this.CreateConnection(this._connectionString);
            }
        }

        #endregion Properties

        #region Constructor

        public OracleHelper()
        {
            this.Connection = null;            
            this._nbOpenConnection = 0;
            this.Transaction = null;
        }
        #endregion

        #region IOracleDaoContext Members

        /// <summary>
        /// Connection du context
        /// </summary>
        public OracleConnection Connection { get; private set; }

        /// <summary>
        /// Transaction du context
        /// </summary>
        public IDbTransaction Transaction { get; private set; }

        #endregion IOracleDaoContext Members

        #region Ouverture et fermeture de la connexion
        private OracleConnection CreateConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException("connectionString");

            // Si la connexion n'existe pas
            if (this.Connection == null)
            {
                this.Connection = new OracleConnection(connectionString);
            }


            return this.Connection;
        }

        /// <summary>
        /// Méthode d'ouverture d'une connexion simple
        /// </summary>
        public void OpenConnection(string connectionString)
        {
            // Si la connexion est nulle suite à une fermeture précédante, on en recréé une
            if (this.Connection == null)
            {
                this.CreateConnection(connectionString);
            }

            // On vérifie que la connexion existe, qu'elle est fermée et qu'elle n'est pas multiple
            if (this.Connection.State == ConnectionState.Closed )
            {
                this.Connection.Open();
            }
        }

        /// <summary>
        /// Méthode de fermeture d'une connexion simple
        /// </summary>
        public void CloseConnection()
        {
            // On vérifie que la connexion existe, qu'elle est ouverte et qu'elle n'est pas multiple
            if (this.Connection != null && this.Connection.State == ConnectionState.Open )
            {
                this.Connection.Close();
                this.Connection.Dispose();
                this.Connection = null;
            }
        }

        #endregion

        #region Parameters
        /// <summary>
        /// Méthode de création d'un paramètre PL-SQL Input
        /// </summary>
        /// <param name="type"><see cref="OracleDbType"/> du paramètre</param>
        /// <param name="name">Nom du paramètre</param>
        /// <param name="value">Valeur du paramètre</param>
        /// <returns>Instance du paramètre créé</returns>
        public static OracleParameter CreatePLSqlInParameter(OracleDbType type, string name, object value)
        {
            OracleParameter param = new OracleParameter();
            param.Direction = ParameterDirection.Input;
            param.OracleDbType = type;
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;

            return param;
        }

        /// <summary>
        /// Méthode de création d'un paramètre PL-SQL Input
        /// </summary>
        /// <param name="type"><see cref="OracleDbType"/> du paramètre</param>
        /// <param name="name">Nom du paramètre</param>
        /// <param name="value">Valeur du paramètre</param>
        /// <returns>Instance du paramètre créé</returns>
        public static OracleParameter CreatePLSqlOutParameter(OracleDbType type, string name, object value)
        {
            OracleParameter param = new OracleParameter();
            param.Direction = ParameterDirection.Output;
            param.OracleDbType = type;
            param.ParameterName = name;
            param.Size = 4000;
            param.Value = value ?? DBNull.Value;
            
            return param;
        }

        #endregion
    }
}

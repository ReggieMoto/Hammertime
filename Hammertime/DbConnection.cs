// ==============================================================
//
// class DbConnection
//
// Copyright (c) 2017 David Hammond 
// All Rights Reserved.
// 
// ==============================================================
// NOTICE:  All information contained herein is and remains the
// property of David Hammond. The intellectual and technical
// concepts contained herein are proprietary to David Hammond
// and may be covered by U.S.and Foreign Patents, patents in
// process, and are protected by trade secret or copyright law.
// Dissemination of this information or reproduction of this
// material is strictly forbidden unless prior written permission
// is obtained David Hammond.
// ==============================================================

using System.Collections.Generic;

namespace Hammertime
{
    public abstract class DbConnection
    {
        public enum Server
        {
            None,
            MySql,
            MongoDb
        }

        protected static DbConnection _dbConnection;

        // =====================================================
        public abstract bool Connected();
        // =====================================================

        // =====================================================
        //open connection to database
        public abstract bool OpenConnection();
        // =====================================================

        // =====================================================
        //Close connection
        public abstract bool CloseConnection();
        // =====================================================

        // =====================================================
        // CRUD
        // =====================================================
        //Insert/Create statement
        public abstract bool Insert(HockeyPlayer player);
        // =====================================================

        // =====================================================
        // Read statement
        public abstract HockeyPlayer Read(string cmd);
        public abstract List<HockeyPlayer> Read();
        // =====================================================

        // =====================================================
        //Update statement
        public abstract bool Update(HockeyPlayer player);
        // =====================================================

        // =====================================================
        //Delete statement
        public abstract bool Delete(HockeyPlayer player);
        // =====================================================

        // =====================================================
        //Count statement
        public abstract int Count();
        // =====================================================

        // =====================================================
        //Backup
        public abstract bool Backup();
        // =====================================================

        // =====================================================
        //Restore
        public abstract bool Restore();
        // =====================================================

    }
}

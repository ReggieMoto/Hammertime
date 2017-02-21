// ==============================================================
//
// sealed class HomeTeam
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hammertime
{
    public sealed class HomeTeam : HockeyTeam
    {
        public static HomeTeam Instance
        {
            get
            {
                if (_homeTeam == null)
                    _homeTeam = new HomeTeam();
                return _homeTeam;
            }
        }

        private static HomeTeam _homeTeam;

        private HomeTeam()
            : base(Residence.Home)
        {
            // Base class: Attach to the DB server
            // Base class: Build a roster from the server
            Console.WriteLine("Initializing the home team.");
        }
    }
}
// ==============================================================
//
// sealed class VisitorTeam
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
    public sealed class VisitorTeam : HockeyTeam
    {
        public static VisitorTeam Instance
        {
            get
            {
                if (_visitorTeam == null)
                    _visitorTeam = new VisitorTeam();
                return _visitorTeam;
            }
        }

        private static VisitorTeam _visitorTeam;

        private VisitorTeam()
            : base(Residence.Away)
        {
            // Base class: Attach to the DB server
            // Base class: Build a roster from the server
            Console.WriteLine("Initializing the visiting team.");
        }
    }
}